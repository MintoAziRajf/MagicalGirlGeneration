using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverCanvas : MonoBehaviour
{
    GameManager gameManager;
    Animator anim;
    private void Awake()
    {
        anim = this.GetComponent<Animator>();
    }
    public void End()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        StartCoroutine(gameManager.GameOverResult());
    }

    public void DisplayRetry()
    {
        anim.SetTrigger("Retry");
    }


    [SerializeField] private Text[] buttons = null;
    [SerializeField] private Color selectColor = Color.blue;
    [SerializeField] private Color unselectColor = Color.white;

    private int currentSelect = 0;
    private enum MENU
    {
        RETRY,
        TITLE
    }
    private IEnumerator RetryMenu()
    {
        bool isSelected = false;
        while (!isSelected)
        {
            float v = Input.GetAxisRaw("Vertical");
            if (v == 1f)
            {
                currentSelect = (int)MENU.RETRY;
            }
            else if (v == -1f)
            {
                currentSelect = (int)MENU.TITLE;
            }
            if (Input.GetButtonDown("Submit"))
            {
                isSelected = true;
            }
            for(int i = 0; i < buttons.Length; i++)
            {
                if (i == currentSelect) buttons[i].color = selectColor;
                else buttons[i].color = unselectColor;
            }
            yield return null;
        }

        switch (currentSelect)
        {
            case (int)MENU.RETRY:
                gameManager.Retry();
                break;
            case (int)MENU.TITLE:
                LoadManager.instance.LoadScene("20_Title");
                break;
            default:
                Debug.Log("error");
                break;
        }
    }
}
