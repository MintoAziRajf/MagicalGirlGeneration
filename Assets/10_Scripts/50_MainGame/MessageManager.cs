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
        messageObj.SetActive(false);
    }

    [SerializeField] private Text displayText = null;
    [SerializeField] private GameObject messageObj = null;
    private const int DISPLAY_TIME = 180;
    public IEnumerator DisplayMessage(string msg)
    {
        Debug.Log(msg);
        messageObj.SetActive(true);
        displayText.text = msg;
        for(int i = 0; i < DISPLAY_TIME; i++)
        {
            yield return new WaitForSeconds(1f/60f);
        }
        messageObj.SetActive(false);
    }
}
