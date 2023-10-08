using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace MimicSpace
{
    /// <summary>
    /// This is a very basic movement script, if you want to replace it
    /// Just don't forget to update the Mimic's velocity vector with a Vector3(x, 0, z)
    /// </summary>
    /// 



    public class BFSNode
    {
        public Vector2Int coords;
        public BFSNode parent;

        public BFSNode(Vector2Int coords, BFSNode parent)
        {
            this.coords = coords;
            this.parent = parent;
        }
    }




    public class Movement : MonoBehaviour
    {
        [Header("Controls")]
        [Tooltip("Body Height from ground")]
        [Range(0.5f, 5f)]
        public float height = 0.8f;
        public float speed = 60f;
        Vector3 velocity = Vector3.zero;
        public float velocityLerpCoef = 4f;
        Mimic myMimic;

        Queue<Vector2Int> path = new Queue<Vector2Int>();


        private MazeGenerator generator;


        // function to wait for maze to be generated
        IEnumerator Wait()
        {
            yield return new WaitForSeconds(1.0f);
        }

        private void Start()
        {

            // wait for the maze to be generated
            StartCoroutine(Wait());


            myMimic = GetComponent<Mimic>();

            // get maze
            GameObject maze = GameObject.Find("Maze Generator");
            generator = maze.GetComponent<MazeGenerator>();


            // if the mimic is not in the maze, move it to the maze
            if (transform.position.x < 0 || transform.position.x > generator.getMazeWidth() * 3 || transform.position.z < 0 || transform.position.z > generator.getMazeDepth() * 3)
            {
                transform.position = new Vector3(generator.getMazeWidth() * 3 / 2, 0, generator.getMazeDepth() * 3 / 2);
            }

            // find path every 0.5 seconds
           
            InvokeRepeating("findPath", 1.0f, 5.0f);
            


            
        }


        void findPath()
        {
            path.Clear();

            // get player coords
            Vector2Int playerCoords = generator.getPlayerCoords();
            Debug.Log("Player coords: " + playerCoords);
            
            // get mimic coords
            Vector2Int mimicCoords = new Vector2Int((int)transform.position.x / 3, (int)transform.position.z / 3);
            Debug.Log("Mimic coords: " + mimicCoords);


            // get the maze grid
            MazeCell[,] mazeGrid = generator.getMazeGrid();

            // bfs from mimic to player
            Queue<BFSNode> queue = new Queue<BFSNode>();
            queue.Enqueue(new BFSNode(mimicCoords, null));

            // visited array
            bool[,] visited = new bool[generator.getMazeWidth(), generator.getMazeDepth()];

            visited[mimicCoords.x, mimicCoords.y] = true;

            // bfs
            while (queue.Count > 0)
            {
                    
                BFSNode current = queue.Dequeue();
                

                // if we found the player
                if (current.coords.x == playerCoords.x && current.coords.y == playerCoords.y)
                {
                    Debug.Log("Found player");
                    // reconstruct path in reverse order
                    Stack<Vector2Int> path2 = new Stack<Vector2Int>();




                    while (current.parent != null)
                    {
                        path2.Push(current.coords);
                        current = current.parent;
                    }

                    // reverse path
                    while (path2.Count > 0)
                    {
                        path.Enqueue(path2.Pop());
                    }



                    break;
                }

   
             

                // up
                if (current.coords.y + 1 < generator.getMazeDepth() && !visited[current.coords.x, current.coords.y + 1] && mazeGrid[current.coords.x, current.coords.y].canMoveFront())
                {
                    // Debug.Log("Can move up");
                    Debug.Log("Can Move to : " + current.coords.x + " " + (current.coords.y + 1));
                    queue.Enqueue(new BFSNode(new Vector2Int(current.coords.x, current.coords.y + 1), current));
                    visited[current.coords.x, current.coords.y + 1] = true;
                }

                // down
                if (current.coords.y - 1 >= 0 && !visited[current.coords.x, current.coords.y - 1] && mazeGrid[current.coords.x, current.coords.y].canMoveBack())
                {
                   // Debug.Log("Can move down");
                   Debug.Log("Can Move to : " + current.coords.x + " " + (current.coords.y - 1));
                    queue.Enqueue(new BFSNode(new Vector2Int(current.coords.x, current.coords.y - 1), current));
                    visited[current.coords.x, current.coords.y - 1] = true;
                }

                // left
                if (current.coords.x - 1 >= 0 && !visited[current.coords.x - 1, current.coords.y] && mazeGrid[current.coords.x, current.coords.y].canMoveLeft())
                {
                  //  Debug.Log("Can move left");
                  Debug.Log("Can Move to : " + (current.coords.x - 1) + " " + current.coords.y);
                    queue.Enqueue(new BFSNode(new Vector2Int(current.coords.x - 1, current.coords.y), current));
                    visited[current.coords.x - 1, current.coords.y] = true;
                }

                // right
                if (current.coords.x + 1 < generator.getMazeWidth() && !visited[current.coords.x + 1, current.coords.y] && mazeGrid[current.coords.x, current.coords.y].canMoveRight())
                {
                  //  Debug.Log("Can move right");
                  Debug.Log("Can Move to : " + (current.coords.x + 1) + " " + current.coords.y);
                    queue.Enqueue(new BFSNode(new Vector2Int(current.coords.x + 1, current.coords.y), current));
                    visited[current.coords.x + 1, current.coords.y] = true;
                }
            }


            

            Debug.Log("Path length: " + path.Count);
        }

        void Update()
        {
            //velocity = Vector3.Lerp(velocity, new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized * speed, velocityLerpCoef * Time.deltaTime);

            // Assigning velocity to the mimic to assure great leg placement
            //myMimic.velocity = velocity;

            //transform.position = transform.position + velocity * Time.deltaTime;
            //RaycastHit hit;
            //Vector3 destHeight = transform.position;
            //if (Physics.Raycast(transform.position + Vector3.up * 5f, -Vector3.up, out hit))
            //    destHeight = new Vector3(transform.position.x, hit.point.y + height, transform.position.z);
            //transform.position = Vector3.Lerp(transform.position, destHeight, velocityLerpCoef * Time.deltaTime);


            // path to player slowly if there is a path by telling the mimic to move in the direction of the next node in the path
            // when it reaches the node, we then remove it from the path and move to the next node

            if (path.Count > 0)
            {
                Vector2Int nextNode = path.Peek();
                Debug.Log("Next node: " + nextNode);

                // if we are close enough to the next node, remove it from the path
                if ( Mathf.Abs(transform.position.x - nextNode.x * 3) < 0.1f && Mathf.Abs(transform.position.z - nextNode.y * 3) < 0.1f )
                {
                    path.Dequeue();
                }
                else
                {
                    // rotate towards the direction of the next node
                    Vector3 nextNodePos = new Vector3(nextNode.x * 3, 0, nextNode.y * 3);
                    Vector3 direction = (nextNodePos - transform.position).normalized;
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 0.1f);

                    // move towards the node center 
                    myMimic.velocity = direction * speed;

                    // update position
                    transform.position = transform.position + myMimic.velocity * Time.deltaTime;

                    // adjust height
                    RaycastHit hit;
                    Vector3 destHeight = transform.position;
                    if (Physics.Raycast(transform.position + Vector3.up * 5f, -Vector3.up, out hit))
                        destHeight = new Vector3(transform.position.x, hit.point.y + height, transform.position.z);
                    transform.position = Vector3.Lerp(transform.position, destHeight, velocityLerpCoef * Time.deltaTime);


                    
                }
            }
            else
            {
                // if there is no path, just stand still
                myMimic.velocity = Vector3.zero;
            }   

        }
    }

}