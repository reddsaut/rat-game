using UnityEngine;

public class ManGenerator : MonoBehaviour
{
    public const float interManDistance = 30;

    public GameObject man;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        {
            for (int i = -40; i < 40; i++)
            {
                Instantiate(man, transform.localPosition + (-transform.forward * interManDistance * i), transform.localRotation);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
