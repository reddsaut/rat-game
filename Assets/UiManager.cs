using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UiManager : MonoBehaviour
{
    public GameObject menu;
    private CameraController playerCamera;
    void Start()
    {
        menu.SetActive(false);
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
        playerCamera.isEnabled = !playerCamera.isEnabled;
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
}
