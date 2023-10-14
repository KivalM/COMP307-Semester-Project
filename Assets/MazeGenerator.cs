using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{

    public int CellSize = 3;

    [SerializeField]
    private MazeCell _mazeCellPrefab;

    [SerializeField]
    private int _mazeWidth;

    [SerializeField]
    private int _mazeDepth;

    [SerializeField]
    private MazeCell[,] _mazeGrid;


    [SerializeField]
    private Vector2Int PlayerCoords;


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

    public Vector2Int getPlayerCoords()
    {
        return PlayerCoords;
    }

    void Start()
    {
        _mazeGrid = new MazeCell[_mazeWidth, _mazeDepth];

        for (int x = 0; x < _mazeWidth; x++)
        {
            for (int z = 0; z < _mazeDepth; z++)
            {
                _mazeGrid[x, z] = Instantiate(_mazeCellPrefab, new Vector3(x*CellSize, 0, z*CellSize), Quaternion.identity);
                // rename the maze cells to the coordinates
                _mazeGrid[x, z].name = x + "," + z;
            }
        }

        GenerateMaze(null, _mazeGrid[0, 0]);
    }

    private void Update()
    {
        GameObject gameObject = GameObject.Find("Player");

        PlayerCoords.x = (int)gameObject.transform.position.x / CellSize;
        PlayerCoords.y = (int)gameObject.transform.position.z / CellSize;
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

        // place a token at the end cell
        _mazeGrid[_mazeWidth - 1, _mazeDepth - 1].SetTokenActive(true);




/*        // randomly remove walls to create loops
        for (int i = 0; i < _mazeDepth + _mazeWidth; i++)
        {
            // pick a random cell
            int x = Random.Range(0, _mazeWidth);
            int z = Random.Range(0, _mazeDepth);

            var cell = _mazeGrid[x, z];

            // pick a random neighbour 1 - 4
            var neighbour_idx = Random.Range(1, 4);

            if (neighbour_idx == 1)
                x = x + 1;
            else if (neighbour_idx == 2)
                x = x - 1;
            else if (neighbour_idx == 3)
                z = z + 1;
            else if (neighbour_idx == 4)
                z = z - 1;

            // check if the neighbour is within the bounds of the maze
            if (x < 0 || x >= _mazeWidth || z < 0 || z >= _mazeDepth)
                continue;

            var neighbour = _mazeGrid[x, z];



            // remove the wall between the two cells
            ClearWalls(cell, neighbour);

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
