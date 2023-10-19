using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class playerHealth : MonoBehaviour
{

    public float maxHealth = 100;
    public float currentHealth;
    public TextMeshProUGUI pauseMenuText;
    
    // gameobject
    public TextMeshProUGUI playerHealthBar;


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

            // get Player object
            GameObject player = GameObject.Find("Player");

            // get component Pause Game
            PauseGame pg = player.GetComponent<PauseGame>();

            pg.Pause();
            pg.HideResume();

            pauseMenuText.SetText("You Died");

        }



        // set the text
        int displayHealth = (int)currentHealth;

        playerHealthBar.SetText(displayHealth  + "%");
    }





}


