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
    private bool isFirst = true;

    [SerializeField] private GameObject pauseMenu = null; // ポーズ画面
    [SerializeField] private GameObject resumeOutline = null; // 再開
    [SerializeField] private GameObject exitOutline = null; // ゲームをやめる
    [SerializeField] private GameObject dialogMenu = null; // 確認画面
    [SerializeField] private GameObject yesOutline = null; // はい
    [SerializeField] private GameObject noOutline = null; // いいえ

    private GameManager gameManager;

    private void OnEnable()
    {
        if(gameManager == null) gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        SoundManager.instance.PlaySE(SoundManager.SE_Type.Pause);
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

    /// <summary>
    /// 選択中のメニューのアウトラインをアクティブにする
    /// </summary>
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
        // 選択
        if (vertical == 1f)
        {
            currentSelect = POSITIVE;
        }
        else if (vertical == -1f)
        {
            currentSelect = NEGATIVE;
        }

        // 決定
        if (submit)
        {
            SoundManager.instance.PlaySE(SoundManager.SE_Type.Submit);
            if (pauseMenu.activeSelf) Unpause();
            else
            {
                if (!isFirst) return; // 一度目しか選択されないように
                isFirst = false;
                LoadManager.instance.LoadScene("20_Title");
            }
        }
        // 戻る
        else if (cancel)
        {
            SoundManager.instance.PlaySE(SoundManager.SE_Type.Submit);
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

        if (unpause) Unpause(); // ポーズ終了
    }

    private void Unpause()
    {
        SoundManager.instance.PlaySE(SoundManager.SE_Type.Submit);
        gameManager.StartCoroutine(gameManager.StartCountDown());
        this.gameObject.SetActive(false);
    }
}
