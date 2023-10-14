using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{

    public int CellSize = 3;
    public int TokenCount = 10;

    [SerializeField]
    private MazeCell _mazeCellPrefab;

    [SerializeField]
    private MazeCell _mazeCellPrefabOther;

    [SerializeField]
    private int _mazeWidth;

    [SerializeField]
    private int _mazeDepth;

    [SerializeField]
    private MazeCell[,] _mazeGrid;


    public int getMazeWidth()
    {
        return _mazeWidth;
    }

    public int getMazeDepth()
    {
        return _mazeDepth;
    }

    public MazeCell[,] getMazeGrid()
    {
        return _mazeGrid;
    }



    void Start()
    {
        _mazeGrid = new MazeCell[_mazeWidth, _mazeDepth];

        int randomPrefabIndex;

        MazeCell prefabChoice;

        for (int x = 0; x < _mazeWidth; x++)
        {
            for (int z = 0; z < _mazeDepth; z++)
            {
                randomPrefabIndex = Random.Range(0, 2);
                if (randomPrefabIndex == 0)
                {
                    prefabChoice = _mazeCellPrefab;
                }
                else
                {
                    prefabChoice = _mazeCellPrefabOther;
                }

                _mazeGrid[x, z] = Instantiate(prefabChoice, new Vector3(x*CellSize, 0, z*CellSize), Quaternion.identity);
                // rename the maze cells to the coordinates
                _mazeGrid[x, z].name = x + "," + z;
            }
        }

        GenerateMaze(null, _mazeGrid[0, 0]);
    }

    private void GenerateMaze(MazeCell previousCell, MazeCell currentCell)
    {
        currentCell.Visit();
        ClearWalls(previousCell, currentCell);

        MazeCell nextCell;

        do
        {
            nextCell = GetNextUnvisitedCell(currentCell);

            if (nextCell != null)
            {
                GenerateMaze(currentCell, nextCell);
            }
            else
            {
                // 
            }
        } while (nextCell != null);

        // disable tokens for all cells
        for (int x = 0; x < _mazeWidth; x++)
        {
            for (int z = 0; z < _mazeDepth; z++)
            {
                _mazeGrid[x, z].SetTokenActive(false);
            }
        }

        // scatter tokens in the maze
        int tokenCount = TokenCount;
        
        while (tokenCount > 0)
        {
            Random.InitState( tokenCount * 1000 + (int)System.DateTime.Now.Ticks );
            int x = Random.Range(0, _mazeWidth);
            int z = Random.Range(0, _mazeDepth);


            _mazeGrid[x, z].SetTokenActive(true);
            tokenCount--;
        }




        // remove the center 20% of the maze
        int xStart = (int)(_mazeWidth * 0.4);
        int xEnd = (int)(_mazeWidth * 0.6);

        int zStart = (int)(_mazeDepth * 0.4);
        int zEnd = (int)(_mazeDepth * 0.6);

        for (int x = xStart; x < xEnd; x++)
        {
            for (int z = zStart; z < zEnd; z++)
            {
                _mazeGrid[x, z].ClearBackWall();
                _mazeGrid[x, z].ClearFrontWall();
                _mazeGrid[x, z].ClearLeftWall();
                _mazeGrid[x, z].ClearRightWall();
            }
        }


        // randomly remove 20% of the walls
    /*        int wallCount = 30;

            while (wallCount > 0)
            {
                int x = Random.Range(0, _mazeWidth);
                int z = Random.Range(0, );

                Debug.Log("Removing wall at " + x + "," + z);


                int wall = Random.Range(0, 4);

                MazeCell mazeCell = _mazeGrid[x, z];
                MazeCell cell = null;

                if (wall == 0 && z + 1 < _mazeDepth)
                    cell = _mazeGrid[x, z + 1];
                else if (wall == 1 && z - 1 >= 0)
                    cell = _mazeGrid[x, z - 1];
                else if (wall == 2 && x + 1 < _mazeWidth)
                    cell = _mazeGrid[x + 1, z];
                else if (wall == 3 && x - 1 >= 0)
                    cell = _mazeGrid[x - 1, z];

                ClearWalls(mazeCell, cell);
                wallCount--;
            }*/


    }

    private MazeCell GetNextUnvisitedCell(MazeCell currentCell)
    {
        var unvisitedCells = GetUnvisitedCells(currentCell);

        return unvisitedCells.OrderBy(_ => Random.Range(1, 10)).FirstOrDefault();
    }

    private IEnumerable<MazeCell> GetUnvisitedCells(MazeCell currentCell)
    {
        int x = (int)currentCell.transform.position.x /3;
        int z = (int)currentCell.transform.position.z /3;

        if (x + 1 < _mazeWidth)
        {
            var cellToRight = _mazeGrid[x + 1, z];

            if (cellToRight.IsVisited == false)
            {
                yield return cellToRight;
            }
        }

        if (x - 1 >= 0)
        {
            var cellToLeft = _mazeGrid[x - 1, z];

            if (cellToLeft.IsVisited == false)
            {
                yield return cellToLeft;
            }
        }

        if (z + 1 < _mazeDepth)
        {
            var cellToFront = _mazeGrid[x, z + 1];

            if (cellToFront.IsVisited == false)
            {
                yield return cellToFront;
            }
        }

        if (z - 1 >= 0)
        {
            var cellToBack = _mazeGrid[x, z - 1];

            if (cellToBack.IsVisited == false)
            {
                yield return cellToBack;
            }
        }
    }

    private void ClearWalls(MazeCell previousCell, MazeCell currentCell)
    {
        if (previousCell == null)
        {
            return;
        }

        if (previousCell.transform.position.x < currentCell.transform.position.x)
        {
            previousCell.ClearRightWall();
            currentCell.ClearLeftWall();
            return;
        }

        if (previousCell.transform.position.x > currentCell.transform.position.x)
        {
            previousCell.ClearLeftWall();
            currentCell.ClearRightWall();
            return;
        }

        if (previousCell.transform.position.z < currentCell.transform.position.z)
        {
            previousCell.ClearFrontWall();
            currentCell.ClearBackWall();
            return;
        }

        if (previousCell.transform.position.z > currentCell.transform.position.z)
        {
            previousCell.ClearBackWall();
            currentCell.ClearFrontWall();
            return;
        }
    }

}
