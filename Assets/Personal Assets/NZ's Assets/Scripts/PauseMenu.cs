using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    void Start()
    {
        GetComponent<Canvas>().enabled = false;
    }

   
    void Update()
    {
        //If you press the escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }

    }
   
    public void Pause()
    {
        if (Time.timeScale == 1)
        {
            //make the pause menu visible
            GetComponent<Canvas>().enabled = true;
            //also, stop the game from playing
            Time.timeScale = 0;
        }
        else
        {
            Resume();
        }
    }
    public void Resume()
    {
        GetComponent<Canvas>().enabled = false;
        //reset the time scale
        Time.timeScale = 1;
    }

    public void Retry()
    {
        Time.timeScale = 1;
        GetComponent<Canvas>().enabled = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        GetComponent<Canvas>().enabled = false;
        Time.timeScale = 1;
        Application.Quit();
    }

}
