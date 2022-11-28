using System.Collections;
using UnityEngine;
public class GameOverCanvas : MonoBehaviour
{
    GameManager gameManager;
    Animator anim;
    private void Awake()
    {
        anim = this.GetComponent<Animator>();
    }
    // ゲームオーバー画面終了
    public void End()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        StartCoroutine(gameManager.GameOverResult());
    }
    // リトライ画面表示
    public void DisplayRetry()
    {
        anim.SetTrigger("Retry");
    }

    [SerializeField] private GameObject[] buttonsOutline = null; // 各ボタンのアウトライン

    private const int INPUT_DELAY = 10; // 入力待機時間
    private int currentSelect = 0; // 現在選択しているメニュー
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
        // 選択されるまで繰り返す
        while (!isSelected)
        {
            // 連続で入力できないように
            if (!canInput)
            {
                for (int i = 0; i < INPUT_DELAY; i++)
                {
                    yield return null;
                }
                canInput = true;
            }

            // 選択
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

            // 決定
            if (Input.GetButtonDown("Submit"))
            {
                SoundManager.instance.PlaySE(SoundManager.SE_Type.Submit);
                isSelected = true;
            }

            // 選択中のメニューにアウトラインを付ける
            for(int i = 0; i < buttonsOutline.Length; i++)
            {
                if (i == currentSelect) buttonsOutline[i].SetActive(true);
                else buttonsOutline[i].SetActive(false);
            }
            yield return null;
        }

        // 選択されたメニューに応じて処理
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
