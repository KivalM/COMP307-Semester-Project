using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private int tokenCount = 0;
    public int RequiredTokens = 5;
    public TextMeshProUGUI pauseMenuText;
    public TextMeshProUGUI inventoryUI;


    public void AddToken()
    {
        tokenCount++;
        Debug.Log("Token count: " + tokenCount);
        inventoryUI.SetText("Tokens: " + tokenCount + " / " + RequiredTokens);


        playerHealth ph = gameObject.GetComponent<playerHealth>();

/*        if (ph.currentHealth >= ph.maxHealth - 50)
        {
            ph.currentHealth = ph.maxHealth;
        }
        else
        {
            ph.currentHealth += 50;
        }*/

        if (tokenCount >= RequiredTokens)
        {
            // win the game
            // get Player object
            GameObject player = GameObject.Find("Player");

            // get component Pause Game
            PauseGame pg = player.GetComponent<PauseGame>();

            pg.Pause();
            pg.HideResume();

            pauseMenuText.SetText("You Win");
        }




    }

}
