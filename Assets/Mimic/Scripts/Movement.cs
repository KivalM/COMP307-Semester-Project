using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace MimicSpace
{
    public class PathNode
    {
        public Vector2Int coords;
        // pointer to parent node
        public PathNode parent;

        public PathNode(Vector2Int coords, PathNode parent)
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

        public int lineOfSightRange = 5;
        public int hearingRange = 5;
        public int playerSpeedThreshold = 5;


        public Transform target;
        private MazeGenerator generator;

        public int spawnX = 0;
        public int spawnZ = 0;


        public Vector3 lastTargetPos = new Vector3(-1, -1, -1);


        // function to wait for maze to be generated
        IEnumerator Wait()
        {
            yield return new WaitForSeconds(10.0f);
        }

        private void Start()
        {

            // wait for the maze to be generated
            StartCoroutine(Wait());
         

            // get maze
            GameObject maze = GameObject.Find("Maze Generator");
            generator = maze.GetComponent<MazeGenerator>();


            // if the mimic is not in the maze, move it to the maze
            if (transform.position.x < 0 || transform.position.x > generator.getMazeWidth() * generator.CellSize || transform.position.z < 0 || transform.position.z > generator.getMazeDepth() * generator.CellSize)
            {
                transform.position = new Vector3(0, 0, 0);
            }

            spawnX = ((int)transform.position.x + generator.CellSize / 2) / generator.CellSize;
            spawnZ = ((int)transform.position.z + generator.CellSize / 2) / generator.CellSize;

      

        }




        List<PathNode> FindPathInMaze(Vector2Int destination)
        {
            // get mimic coords
            Vector2Int mimicCoords = new Vector2Int((int)transform.position.x / generator.CellSize, (int)transform.position.z / generator.CellSize);


            // get the maze grid
            MazeCell[,] mazeGrid = generator.getMazeGrid();

            // visited array
            bool[,] visited = new bool[generator.getMazeWidth(), generator.getMazeDepth()];

            // bfs from destination to mimic
            Queue<PathNode> queue = new Queue<PathNode>();
            
            queue.Enqueue(new PathNode(destination, null));
            visited[destination.x, destination.y] = true;

            // bfs
            while (queue.Count > 0)
            {
                PathNode pathNode = queue.Dequeue();
                if (pathNode != null)
                {
                    // if we are at the mimic
                    if (mimicCoords.x == pathNode.coords.x && mimicCoords.y == pathNode.coords.y)
                    {
                        // bubble up the path
                        List<PathNode> path = new List<PathNode>();

                        while (pathNode != null)
                        {
                            path.Add(pathNode);
                            pathNode = pathNode.parent;
                        }   
                        return path;
                    }

                    // up
                    if (pathNode.coords.y + 1 < generator.getMazeDepth() && !visited[pathNode.coords.x, pathNode.coords.y + 1] && mazeGrid[pathNode.coords.x, pathNode.coords.y].canMoveFront())
                    {
                        // Debug.Log("Can move up");
                        queue.Enqueue(new PathNode(new Vector2Int(pathNode.coords.x, pathNode.coords.y + 1), pathNode));
                        visited[pathNode.coords.x, pathNode.coords.y + 1] = true;
                    }

                    // down
                    if (pathNode.coords.y - 1 >= 0 && !visited[pathNode.coords.x, pathNode.coords.y - 1] && mazeGrid[pathNode.coords.x, pathNode.coords.y].canMoveBack())
                    {
                        // Debug.Log("Can move down");
                        queue.Enqueue(new PathNode(new Vector2Int(pathNode.coords.x, pathNode.coords.y - 1), pathNode));
                        visited[pathNode.coords.x, pathNode.coords.y - 1] = true;
                    }

                    // left
                    if (pathNode.coords.x - 1 >= 0 && !visited[pathNode.coords.x - 1, pathNode.coords.y] && mazeGrid[pathNode.coords.x, pathNode.coords.y].canMoveLeft())
                    {
                        //  Debug.Log("Can move left");
                        queue.Enqueue(new PathNode(new Vector2Int(pathNode.coords.x - 1, pathNode.coords.y), pathNode));
                        visited[pathNode.coords.x - 1, pathNode.coords.y] = true;
                    }

                    // right
                    if (pathNode.coords.x + 1 < generator.getMazeWidth() && !visited[pathNode.coords.x + 1, pathNode.coords.y] && mazeGrid[pathNode.coords.x, pathNode.coords.y].canMoveRight())
                    {
                        //  Debug.Log("Can move right");
                        queue.Enqueue(new PathNode(new Vector2Int(pathNode.coords.x + 1, pathNode.coords.y), pathNode));
                        visited[pathNode.coords.x + 1, pathNode.coords.y] = true;
                    }
                }
            }
            return new List<PathNode>();
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

            Vector3 targetPos = new Vector3(target.position.x, target.position.y, target.position.z);
            Vector2 targetMazeCoords = new Vector2((targetPos.x + generator.CellSize/2) / generator.CellSize, (targetPos.z + generator.CellSize/2) / generator.CellSize);

            

            Vector3 mimicPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            Vector2 mimicMazeCoords = new Vector2(mimicPos.x / generator.CellSize, mimicPos.z / generator.CellSize);
            
            float distance = Vector3.Distance(targetPos, mimicPos);

            
            // check if player is visible
            if (distance <= lineOfSightRange)
            {
                // fire the ray from slightly above the mimic
                Vector3 rayOrigin = mimicPos + new Vector3(0, 0, 0);

                // cast a ray from mimic 
                if (Physics.Raycast(rayOrigin, targetPos - rayOrigin + new Vector3(0,1,0), out RaycastHit hit, lineOfSightRange))
                {

                    // print the name of the object
                    Debug.DrawLine(rayOrigin, hit.point, Color.green, 0.1f);

                    // if the ray hits the player
                    if (hit.transform.gameObject.CompareTag("Player"))
                    {
                        // only move in x and z
                        Vector3 targetPos2 = new Vector3(targetPos.x, transform.position.y, targetPos.z);
                        
                        // move towards but make it erratic
                        // transform.position = Vector3.MoveTowards(transform.position, targetPos2, speed * Time.deltaTime);

                        transform.position = Vector3.Lerp(transform.position, targetPos2, speed * Time.deltaTime);
                        
                        // rotate towards player
                        transform.LookAt(targetPos2);

                        // set the velocity so the legs move
                        velocity = Vector3.Lerp(velocity, new Vector3(targetPos2.x - transform.position.x, 0, targetPos2.z - transform.position.z).normalized * speed, velocityLerpCoef * Time.deltaTime);

                        lastTargetPos = targetPos2;
                        
                        return;
                    }
                }
            }

           // if the mimic can hear the player
           if (distance <= hearingRange)
            {
                // check the players speed from the character controller
                CharacterController controller = target.GetComponent<CharacterController>();
                Vector3 playerVelocity = controller.velocity;

                // if the player is moving above a certain speed or we are pathing to the last known position
                if (playerVelocity.magnitude >= playerSpeedThreshold)
                {
                    MoveToCoordinate(new Vector2Int((int)targetMazeCoords.x, (int)targetMazeCoords.y));

                    lastTargetPos = target.position;
                    return;
                }
            }

           // if we have a last known position
           if (lastTargetPos != new Vector3(-1, -1, -1))
            {
                Vector2 lastCoords = new Vector2((lastTargetPos.x + 1.5f) / generator.CellSize, (lastTargetPos.z + 1.5f) / generator.CellSize);
                Vector2Int mazeCoords = new Vector2Int((int)lastCoords.x, (int)lastCoords.y);
                MoveToCoordinate(mazeCoords);


                // if we are at the last known position, clear it and return
                if (lastTargetPos.x - transform.position.x < generator.CellSize && lastTargetPos.z - transform.position.z < generator.CellSize)
                {
                    lastTargetPos = new Vector3(-1, -1, -1);
                }

                return;
           }
    

            // if we have no last known position, move towards the center of the maze

            MoveToCoordinate(new Vector2Int(spawnX, spawnZ));
            
        }

        void MoveToCoordinate(Vector2Int coords)
        {
            Vector3 mimicPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);

            // find path to player
            List<PathNode> pathNodes = FindPathInMaze(new Vector2Int((int)coords.x, (int)coords.y));
            PathNode pathNode = pathNodes[0];

            // draw a marker at every node in the path
            foreach (PathNode node in pathNodes)
            {
                Vector3 markerPos = new Vector3(node.coords.x * generator.CellSize, transform.position.y, node.coords.y * generator.CellSize);
                Debug.DrawLine(markerPos, markerPos + new Vector3(0, 1, 0), Color.red, 0.1f);
            }


            if (pathNode != null)
            {
                // check if mimic can see a path to the next node after the next node
                // make sure there is a next node
                if (pathNodes.Count > 1)
                {
                    PathNode nextNode = pathNodes[1];
                    Vector3 rayOrigin = mimicPos;
                    Vector3 rayDestination = new Vector3(nextNode.coords.x * generator.CellSize, transform.position.y, nextNode.coords.y * generator.CellSize);

                    // draw ray
                    Debug.DrawLine(rayOrigin, rayDestination, Color.blue, 0.1f);

                    // cast a ray from mimic
                    if (Physics.Raycast(rayOrigin, rayDestination - rayOrigin, out RaycastHit hit))
                    {
                        pathNodes.RemoveAt(0);
                        pathNode = pathNodes[0];
                    }
                }



                Vector3 targetPos2 = new Vector3(pathNode.coords.x * generator.CellSize, transform.position.y, pathNode.coords.y * generator.CellSize);

                // move towards but make it erratic
                transform.position = Vector3.MoveTowards(transform.position, targetPos2, speed * Time.deltaTime);

                // rotate towards player
                transform.LookAt(targetPos2);

                // set the velocity so the legs move
                velocity = Vector3.Lerp(velocity, new Vector3(targetPos2.x - transform.position.x, 0, targetPos2.z - transform.position.z).normalized * speed, velocityLerpCoef * Time.deltaTime);

          
            }

            return;
        }



    }




}