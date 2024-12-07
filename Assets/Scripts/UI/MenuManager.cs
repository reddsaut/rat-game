using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject introMenu;
    public GameObject controlsScreen;

    void Start()
    {
        mainMenu.SetActive(true);
        introMenu.SetActive(false);
        controlsScreen.SetActive(false);
    }

    public void ToIntroMenu()
    {
        mainMenu.SetActive(false);
        introMenu.SetActive(true);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Intro");
    }

    public void ToggleControls()
    {
        mainMenu.SetActive(!mainMenu.activeSelf);
        controlsScreen.SetActive(!controlsScreen.activeSelf);
    }
}
