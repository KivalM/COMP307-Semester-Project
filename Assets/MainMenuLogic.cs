using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuLogic : MonoBehaviour
{
    private GameObject mainMenu;
    private GameObject loading;
    private GameObject pauseMenu;


    void Start()
    {
        pauseMenu = GameObject.Find("HUD");
        mainMenu = GameObject.Find("MainMenuCanvas");
        loading = GameObject.Find("LoadingCanvas");

        mainMenu.SetActive(true);
        loading.SetActive(false);
        pauseMenu.SetActive(false);
    }

    public void StartButton()
    {
        mainMenu.SetActive(false);
        pauseMenu.SetActive(true);
    
        Invoke("PlayGame", 0.01f);
    }

    public void PlayGame()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        pauseMenu.GetComponent<Canvas>().enabled = true;
        loading.GetComponent<Canvas>().enabled = false;
    }

    public void ExitGameButton()
    {
        Application.Quit();
        Debug.Log("App Has Exited");
    }

    void Update()
    {
        
    }
}
