using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject introMenu;
    public GameObject controlsScreen;
    public GameObject creditsScreen;
    GameObject currentScreen;

    void Start()
    {
        mainMenu.SetActive(true);
        introMenu.SetActive(false);
        controlsScreen.SetActive(false);
        creditsScreen.SetActive(false);
        currentScreen = mainMenu;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            currentScreen.SetActive(false);
            mainMenu.SetActive(true);
            currentScreen = mainMenu;
        }
    }

    public void ToggleIntroMenu()
    {
        mainMenu.SetActive(!mainMenu.activeSelf);
        introMenu.SetActive(!introMenu.activeSelf);
        if(introMenu.activeSelf)
        {
            currentScreen = introMenu;
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Intro");
    }

    public void ToggleControls()
    {
        mainMenu.SetActive(!mainMenu.activeSelf);
        controlsScreen.SetActive(!controlsScreen.activeSelf);
        if(controlsScreen.activeSelf)
        {
            currentScreen = controlsScreen;
        }
    }

    public void ToggleCredits()
    {
        mainMenu.SetActive(!mainMenu.activeSelf);
        creditsScreen.SetActive(!controlsScreen.activeSelf);
        if(creditsScreen.activeSelf)
        {
            currentScreen = creditsScreen;
        }
    }
}
