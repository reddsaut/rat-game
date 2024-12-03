using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject MainMenu;
    public GameObject IntroMenu;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToIntroMenu()
    {
        MainMenu.SetActive(false);
        IntroMenu.SetActive(true);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Intro");
    }
}
