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
    [SerializeField] private Outline resumeOutline = null;
    [SerializeField] private Outline exitOutline = null;
    [SerializeField] private GameObject dialogMenu = null;
    [SerializeField] private Outline yesOutline = null;
    [SerializeField] private Outline noOutline = null;

    private void OnEnable()
    {
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
                resumeOutline.enabled = true;
                exitOutline.enabled = false;
            }
            else if (currentSelect == NEGATIVE)
            {
                resumeOutline.enabled = false;
                exitOutline.enabled = true;
            }
        }
        else
        {
            if (currentSelect == POSITIVE)
            {
                yesOutline.enabled = true;
                noOutline.enabled = false;
            }
            else if (currentSelect == NEGATIVE)
            {
                yesOutline.enabled = false;
                noOutline.enabled = true;
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
            if (pauseMenu.activeSelf)
            {
                Time.timeScale = 1f;
                this.gameObject.SetActive(false);
            }
            else
            {
                //title
            }
        }
        else if (cancel)
        {
            Debug.Log("b");
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

        if (unpause)
        {
            Time.timeScale = 1f;
            this.gameObject.SetActive(false);
        }
    }
}
