using UnityEngine;

public class ManGenerator : MonoBehaviour
{
    public float interManDistance = 30;
    public float manScale = 20;

    public GameObject man;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        {
            for (int i = -40; i < 40; i++)
            {
                man = Instantiate(man, transform.localPosition + (-transform.forward * interManDistance * i), transform.localRotation);
                man.transform.localScale = new Vector3(manScale, manScale, manScale);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
