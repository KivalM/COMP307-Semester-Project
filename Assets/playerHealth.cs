using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerHealth : MonoBehaviour
{

    public float maxHealth = 100;
    public float currentHealth;


    void Start()
    {
        currentHealth = maxHealth;
    }

    public void damagePlayer(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            // player is dead
            Debug.Log("Player is dead");
            currentHealth = 0;
        }
    }





}


