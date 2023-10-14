using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnCollision : MonoBehaviour
{
    public GameObject targetObject;
    // Start is called before the first frame update
    private void OnCollisionEnter(Collision collision)
    {
        // Check if the collision is with the specified targetObject
        if (collision.gameObject == targetObject)
        {
            foreach (Transform child in transform)
            {
                if (collision.gameObject == child.gameObject)
                {
                    // Destroy the object this script is attached to
                    Destroy(gameObject);
                    return; // Exit the loop if collision is detected with any child
                }
            }
        }
    }
}
