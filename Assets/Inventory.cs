using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private int tokenCount = 0;


    public void AddToken()
    {
        tokenCount++;
        Debug.Log("Token count: " + tokenCount);
    }

}
