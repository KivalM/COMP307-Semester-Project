using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MazeCell : MonoBehaviour
{
    [SerializeField]
    private GameObject _leftWall;

    [SerializeField]
    private GameObject _rightWall;

    [SerializeField]
    private GameObject _frontWall;

    [SerializeField]
    private GameObject _backWall;

    [SerializeField]
    private GameObject _token;

    [SerializeField]
    private GameObject _unvisitedBlock;



    public bool canMoveLeft()
    {
        return !_leftWall.activeSelf;
    }

    public bool canMoveRight()
    {
        return !_rightWall.activeSelf;
    }

    public bool canMoveFront()
    {
        return !_frontWall.activeSelf;
    }

    public bool canMoveBack()
    {
        return !_backWall.activeSelf;
    }

    public bool isTokenActive()
    {
        return _token.activeSelf;
    }

    public bool IsVisited { get; private set; }

    public void Visit()
    {
        IsVisited = true;
        _unvisitedBlock.SetActive(false);
    }



    public void ClearLeftWall()
    {
        _leftWall.SetActive(false);
    }

    public void ClearRightWall()
    {
        _rightWall.SetActive(false);
    }

    public void ClearFrontWall()
    {
        _frontWall.SetActive(false);
    }

    public void ClearBackWall()
    {
        _backWall.SetActive(false);
    }

    public void SetTokenActive(bool active)
    {
        _token.SetActive(active);
    }
}

