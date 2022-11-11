using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    [SerializeField] private GameObject[] buttonsOutline = null;

    private const int INPUT_DELAY = 20;
    private int currentSelect = 0;
    private enum MENU
    {
        RETRY,
        CHARACTER_SELECT,
        TITLE
    }
    private IEnumerator RetryMenu()
    {
        bool isSelected = false;
        bool canInput = true;
        while (!isSelected)
        {
            if (!canInput)
            {
                for (int i = 0; i < INPUT_DELAY; i++)
                {
                    yield return null;
                }
                canInput = true;
            }
            float v = Input.GetAxisRaw("Vertical");
            if (v == 1f)
            {
                currentSelect = Mathf.Clamp(currentSelect - 1, (int)MENU.RETRY, (int)MENU.TITLE);
                canInput = false;
            }
            else if (v == -1f)
            {
                currentSelect = Mathf.Clamp(currentSelect + 1, (int)MENU.RETRY, (int)MENU.TITLE);
                canInput = false;
            }
            if (Input.GetButtonDown("Submit"))
            {
                isSelected = true;
            }
            for(int i = 0; i < buttonsOutline.Length; i++)
            {
                if (i == currentSelect) buttonsOutline[i].SetActive(true);
                else buttonsOutline[i].SetActive(false);
            }
            yield return null;
        }

        switch (currentSelect)
        {
            case (int)MENU.RETRY:
                gameManager.Retry();
                break;
            case (int)MENU.CHARACTER_SELECT:
                LoadManager.instance.LoadScene("30_CharacterSelect");
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
