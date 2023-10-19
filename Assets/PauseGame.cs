using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseGame : MonoBehaviour
{
    public GameObject menu;
    public GameObject resume;
    public GameObject quit;

    public bool on;
    public bool off;





    void Start()
    {
        menu.SetActive(false);
        off = true;
        on = false;
    }

    public void Pause()
    {

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;
            menu.SetActive(true);
            off = false;
            on = true;
 
    }

    public void HideResume()
    {
        resume.SetActive(false);
    }




    void Update()
    {


 





        if (off && Input.GetButtonDown("pause"))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;
            menu.SetActive(true);
            off = false;
            on = true;
        }

        else if (on && Input.GetButtonDown("pause"))
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1;
            menu.SetActive(false);
            off = true;
            on = false;
        }
        
    }

    public void Resume()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;
        menu.SetActive(false);
        off = true;
        on = false;

    }

    public void Exit()
    {
        Application.Quit();
    }
}
