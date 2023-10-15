using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class damagePlayer : MonoBehaviour
{

    public float damage = 10;

    public float damageDistance = 2;

    public float damageRate = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        // if the parent object is within 1 unit of the player object
        // then damage the player

        // get the player object
        GameObject player = GameObject.Find("Player");

        // get the distance between the player and the parent object
        float distance = Vector3.Distance(player.transform.position, transform.position);

        // if the distance is less than the damageDistance
            if (distance < damageDistance)
        {
            // damage the player
            playerHealth ph = player.GetComponent<playerHealth>();

            // calculate the dps that should be applied depending on the time
            // between frames
            float dam = damage  / damageRate * (Time.deltaTime / 0.02f);
            ph.damagePlayer(dam);
        }
        
    }
}
