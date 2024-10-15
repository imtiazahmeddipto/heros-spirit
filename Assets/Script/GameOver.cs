using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public GameObject GameOverScreen;
    public GameObject GameOverScreenReason2;
    public GameObject WinScreen;
    public GameObject PauseScreen;

    public void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) 
        {
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            PauseScreen.SetActive(true);
        }
    }
    public void Game_Over()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0.1f;
        GameOverScreen.SetActive(true);
    }
    public void Game_OverFor5()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0.1f;
        GameOverScreenReason2.SetActive(true);
    }
    public void Win()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0.1f;
        WinScreen.SetActive(true);
    }
    public void Resume()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        PauseScreen.SetActive(false);
    }

    public void Restart()
    {
        PlayerPrefs.DeleteAll();
        MainShip.PowerDestroyed = 0;
        MainShip.BuildingDestroyed = 0;
        SceneManager.LoadScene("Game");
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        WinScreen.SetActive(false);
        GameOverScreen.SetActive(false);
    }
    public void Quit()
    {
        Application.Quit();
    }
}
