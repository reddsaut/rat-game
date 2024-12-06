using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UiManager : MonoBehaviour
{
    public GameObject menu;
    public GameObject controlScreen;
    private CameraController playerCamera;
    void Start()
    {
        menu.SetActive(false);
        controlScreen.SetActive(false);
        playerCamera = FindFirstObjectByType<CameraController>();
    }

    void Update()
    {
        if(Input.GetButtonDown("Pause"))
        {
            ToggleMenu();
        }
    }

    public void ToggleMenu()
    {
        menu.SetActive(!menu.activeSelf);
        controlScreen.SetActive(false); // this should always be off unless info button pressed
        if(Time.timeScale == 0)
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
        SceneManager.LoadScene("Intro");
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
}
