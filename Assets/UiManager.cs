using UnityEngine;

public class UiManager : MonoBehaviour
{
    void Start()
    {
        gameObject.SetActive(false);
    }

    void Update()
    {
        if(Input.GetButtonDown("Pause"))
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }
    }
}
