using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageManager : MonoBehaviour
{
    public static MessageManager instance;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    [SerializeField] private GameObject messageCanvas = null;
    [SerializeField] private GameObject messageObj = null;
    private const int DISPLAY_TIME = 60;
    private Queue<IEnumerator> messageQueue = new Queue<IEnumerator>();

    private bool isDisplay = false;

    public IEnumerator DisplayMessage(string msg)
    {
        if (isDisplay)
        {
            messageQueue.Enqueue(DisplayMessage(msg));
            yield break;
        }
        isDisplay = true;
        GameObject message = Instantiate(messageObj, messageCanvas.transform);
        //Debug.Log(msg);
        Text displayText = message.transform.Find("Text").gameObject.GetComponent<Text>();
        displayText.text = msg;
        for(int i = 0; i < DISPLAY_TIME; i++)
        {
            yield return new WaitForSeconds(1f/60f);
        }
        isDisplay = false;
        Destroy(message);
        if(messageQueue.Count != 0) StartCoroutine(messageQueue.Dequeue());
    }
}
