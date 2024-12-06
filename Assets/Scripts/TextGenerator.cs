using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TextGenerator : MonoBehaviour
{
    private string[] messages = { "Welcome to our world...", "Where are we walking?", "Where are you walking?", "Who's the real rat, you or I?" };
    public GameObject textObjectPrefab;
    public static List<GameObject> textObjects;
    public Font thisFont;
    private int messageIndex;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textObjects = new List<GameObject>();
        // should create a bunch of these

        messageIndex = 0;

        for (int i = 0; i < 10; i++)
        {
            if (messageIndex >= messages.Length) messageIndex = 0;

            GameObject textObject = Instantiate(textObjectPrefab, transform.position + (Vector3.right * i * 10) + (Vector3.down * i * 10), transform.rotation);
            Debug.Log(textObject);

            GameObject thisText = new GameObject();
            thisText.transform.parent = textObject.transform;
            Text text = thisText.AddComponent<Text>();
            text.font = thisFont;
            text.text = messages[messageIndex];
            messageIndex++;
            text.fontSize = 4;

            RectTransform thisRectTransform = text.GetComponent<RectTransform>();
            thisRectTransform = text.GetComponent<RectTransform>();
            thisRectTransform.localPosition = new Vector3(0, 0, 0);
            thisRectTransform.sizeDelta = new Vector2(40, 40);

            textObjects.Add(textObject);
        }
        Debug.Log(textObjects);
    }

    // Update is called once per frame
    void Update()
    {
        // if timer runs out, generate next text at random x and y
        // text drift
        foreach (var textObject in textObjects)
        {
            textObject.transform.position = new Vector3(textObject.transform.position.x, transform.position.y, textObject.transform.position.z);
            textObject.transform.position += (Vector3.back * Time.deltaTime * 20);
        }
    }
}
