using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UiManager : MonoBehaviour
{
    public GameObject menu;
    public GameObject controlScreen;
    public GameObject deathScreen;
    private CameraController playerCamera;
    void Start()
    {
        Time.timeScale = 1;
        menu.SetActive(false);
        controlScreen.SetActive(false);
        deathScreen.SetActive(false);
        playerCamera = FindFirstObjectByType<CameraController>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            ToggleMenu();
        }
    }

    public void ToggleMenu()
    {
        menu.SetActive(!menu.activeSelf);
        controlScreen.SetActive(false); // this should always be off unless info button pressed
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }
        else
        {
            Time.timeScale = 0;
        }
    }

    public void QuitToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().ToString());
    }

    public void SetRotationSpeed(float speed)
    {
        playerCamera.rotationSpeed = speed;
    }

    public void ToggleControlsScreen()
    {
        controlScreen.SetActive(!controlScreen.activeSelf);
    }

    public void Death()
    {
        Time.timeScale = 0.01f;
        deathScreen.SetActive(true);
    }
}
