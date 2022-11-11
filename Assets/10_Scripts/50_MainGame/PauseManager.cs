using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PauseManager : MonoBehaviour
{
    private int currentSelect = 0;
    private const int POSITIVE = 0;
    private const int NEGATIVE = 1;
    private const int ANIM_TIME = 20;
    private int time = 0;

    [SerializeField] private GameObject pauseMenu = null;
    [SerializeField] private GameObject resumeOutline = null;
    [SerializeField] private GameObject exitOutline = null;
    [SerializeField] private GameObject dialogMenu = null;
    [SerializeField] private GameObject yesOutline = null;
    [SerializeField] private GameObject noOutline = null;

    private GameManager gameManager;

    private void OnEnable()
    {
        if(gameManager == null) gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        currentSelect = POSITIVE;
        pauseMenu.SetActive(true);
        dialogMenu.SetActive(false);
        time = 0;
    }

    void Update()
    {
        if(time <= ANIM_TIME)
        {
            time++;
            return;
        }
        PauseInput();
        PauseDisplay();
    }

    private void PauseDisplay()
    {
        if (pauseMenu.activeSelf)
        {
            if (currentSelect == POSITIVE)
            {
                resumeOutline.SetActive(true);
                exitOutline.SetActive(false);
            }
            else if (currentSelect == NEGATIVE)
            {
                resumeOutline.SetActive(false);
                exitOutline.SetActive(true);
            }
        }
        else
        {
            if (currentSelect == POSITIVE)
            {
                yesOutline.SetActive(true);
                noOutline.SetActive(false);
            }
            else if (currentSelect == NEGATIVE)
            {
                yesOutline.SetActive(false);
                noOutline.SetActive(true);
            }
        }
    }

    private void PauseInput()
    {
        float vertical = Input.GetAxisRaw("Vertical");
        bool submit = Input.GetButtonDown("Submit") && currentSelect == POSITIVE;
        bool cancel = Input.GetButtonDown("Submit") && currentSelect == NEGATIVE;
        bool unpause = Input.GetButtonDown("Option");
        if (vertical == 1f)
        {
            currentSelect = POSITIVE;
        }
        else if (vertical == -1f)
        {
            currentSelect = NEGATIVE;
        }

        if (submit)
        {
            if (pauseMenu.activeSelf) Unpause();
            else LoadManager.instance.LoadScene("20_Title");
        }
        else if (cancel)
        {
            if (pauseMenu.activeSelf)
            {
                pauseMenu.SetActive(false);
                dialogMenu.SetActive(true);
            }
            else
            {
                pauseMenu.SetActive(true);
                dialogMenu.SetActive(false);
            }
        }

        if (unpause) Unpause();
    }

    private void Unpause()
    {
        gameManager.StartCoroutine(gameManager.StartCountDown());
        this.gameObject.SetActive(false);
    }
}
