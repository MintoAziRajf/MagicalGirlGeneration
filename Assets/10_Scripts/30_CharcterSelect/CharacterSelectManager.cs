using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Text;
using System;

public class CharacterSelectManager : MonoBehaviour
{
    private enum CHARACTER
    {
        RED,
        BLUE,
        YELLOW
    }
    private CHARACTER currentCharacter = CHARACTER.RED;//現在選択しているキャラクター

    [SerializeField] private GameObject currentObj = null, beforeObj = null; //現在、前のキャラ

    private bool canOperate = true;
    
    //--------スクロール関連--------
    [SerializeField] private Vector2 centerPos, leftPos, rightPos; // スクロール先(固定値)
    [SerializeField] private Image[] arrow = null; // 矢印
    
    private RectTransform currentTrans, beforeTrans; // 現在、前のキャラの場所
    private Vector2 targetPos; //スクロール先
    
    private const float speed = 100f; // 基本速度
    //------------------------------

    //--------キャラクター情報-------
    [SerializeField] private Sprite[] characterSprite = new Sprite[3]; //使用するキャラのスプライト
    private Image currentImage, beforeImage; //現在、前の見た目

    [SerializeField] private Sprite[] backgroundSprite = null; //使用する背景のスプライト配列
    [SerializeField] private Image background = null; // 表示先

    [SerializeField] private Sprite[] outerSprite = null; // 外側のデザイン
    [SerializeField] private Image outer = null; // 表示先
    [SerializeField] private Sprite[] innerSprite = null; // 内側のデザイン
    [SerializeField] private Image inner = null; // 表示先

    [SerializeField] private Sprite[] infoWindowSprite = null; // キャラクター情報の表示ウィンドウ
    [SerializeField] private Image infoWindow = null; // 表示先

    [SerializeField] private Text characterName = null, score = null; //キャラクターの名前、ハイスコア
    [SerializeField] private string[] nameString = new string[3]; //キャラクターの名前
    [SerializeField] private Text characterType = null; //キャラクターのタイプ名
    [SerializeField] private string[] typeString = new string[3]; //キャラクターの名前

    [SerializeField] private Sprite[] rankIconSprite = null; // ランクの表示 アイコン
    [SerializeField] private Image rankIcon = null; // 表示先
    [SerializeField] private Sprite[] rankTextSprite = null; // ランクの表示 文字
    [SerializeField] private Image rankText = null; // 表示先
    [SerializeField] private Sprite[] weaponIconSprite = null; // 武器アイコンの表示
    [SerializeField] private Image weaponIcon = null; // 表示先

    private List<string[]> scoreDatas = new List<string[]>();
    private enum SCORE
    {
        VALUE = 0,
        RANK = 1
    }
    //------------------------------

    //------------------------------
    Animator anim;
    private bool isDialog = false;
    public bool IsDialog { set { isDialog = value; } }
    [SerializeField] private GameObject dialogs = null;
    private bool isPrologue = true;
    public bool IsPrologue { set { isPrologue = value; } }
    private bool isTutorial = true;
    public bool IsTutorial { set { isTutorial = value; } }
    //------------------------------

    private void Awake()
    {
        //Init
        LoadScore();
        SoundManager.instance.PlayBGM(SoundManager.BGM_Type.CharacterSelect);
        currentImage = currentObj.GetComponent<Image>();
        currentTrans = currentObj.GetComponent<RectTransform>();
        beforeImage = beforeObj.GetComponent<Image>();
        beforeTrans = beforeObj.GetComponent<RectTransform>();
        anim = dialogs.GetComponent<Animator>();
        DisplayCharacterInfo();
    }

    private void FixedUpdate()
    {
        bool isScroll = !(currentTrans.anchoredPosition == centerPos); //スクロール中かどうかの判定
        bool submit = Input.GetButtonDown("Submit") && canOperate;
        bool cancel = Input.GetButtonDown("Cancel") && canOperate;

        bool right = Input.GetAxisRaw("Horizontal") == 1f && !isDialog;
        bool left = Input.GetAxisRaw("Horizontal") == -1f && !isDialog;

        //スクロール中は操作を受け付けない
        if (isScroll)
        {
            Scroll();
            return;
        }
        if (!isDialog) Debug.Log("スクロール操作可能");
        //右
        if (right)
        {
            Debug.Log("右スクロール");
            SoundManager.instance.PlaySE(SoundManager.SE_Type.CS_Scroll);
            SetCharacter(true);
        }
        //左
        if (left)
        {
            Debug.Log("左スクロール");
            SoundManager.instance.PlaySE(SoundManager.SE_Type.CS_Scroll);
            SetCharacter(false);
        }
        //選択
        if (submit)
        {
            anim.SetTrigger("Submit");
            StartCoroutine(ResetTrigger("Submit"));
            SoundManager.instance.PlaySE(SoundManager.SE_Type.CS_Submit);
            StartCoroutine(AllowOperate());
        }
        else if (cancel)
        {
            anim.SetTrigger("Cancel");
            StartCoroutine(ResetTrigger("Cancel"));
            StartCoroutine(AllowOperate());
        }
    }

    private IEnumerator ResetTrigger(string s)
    {
        yield return null;
        anim.ResetTrigger(s);
    }

    private IEnumerator AllowOperate()
    {
        canOperate = false;
        for(int i = 0; i < 10; i++)
        {
            yield return null;
        }
        canOperate = true;
    }

    public void LoadMainGame()
    {
        Debug.Log((int)currentCharacter);
        canOperate = false;
        SceneManager.sceneLoaded += GameSceneLoaded;
        LoadManager.instance.LoadScene("50_MainGame");
    }

    private void GameSceneLoaded(Scene next, LoadSceneMode mode)
    {
        // シーン切り替え後のスクリプトを取得
        GameManager gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();

        // データを渡す処理
        gameManager.Type = (int)currentCharacter;
        gameManager.IsTutorial = this.isTutorial;
        gameManager.IsPrologue = this.isPrologue;

        // イベントから削除
        SceneManager.sceneLoaded -= GameSceneLoaded;
    }
    
    /// <summary>
    /// キャラクターの切り替え、スクロール先の設定
    /// </summary>
    /// <param name="isRight">右=true,左=false</param>
    private void SetCharacter(bool isRight)
    {
        //矢印を点滅させる
        StartCoroutine(ArrowFlash(isRight));
        //選択中のキャラクターに値を変更
        if (!isRight)
        {
            currentCharacter++;
            if ((int)currentCharacter == 3) currentCharacter = CHARACTER.RED;//上限に達したら下限に戻す
        }
        else
        {
            currentCharacter--;
            if ((int)currentCharacter == -1) currentCharacter = CHARACTER.YELLOW;//下限に達したら上限に戻す
        }
        //見た目の入れ替え
        beforeImage.sprite = currentImage.sprite;
        currentImage.sprite = characterSprite[(int)currentCharacter];
        //キャラの情報表示
        DisplayCharacterInfo();

        //場所の入れ替え
        beforeTrans.anchoredPosition = centerPos;
        if (!isRight)
        {
            currentTrans.anchoredPosition = rightPos;
            targetPos = leftPos;
        }
        else
        {
            currentTrans.anchoredPosition = leftPos;
            targetPos = rightPos;
        }
    }

    private IEnumerator ArrowFlash(bool b)
    {
        int index = Convert.ToInt32(b);
        arrow[index].color = Color.gray;
        for (int i = 0; i < 20; i++)
        {
            yield return null;
        }
        arrow[index].color = Color.white;
    }

    /// <summary>
    /// スクロール
    /// </summary>
    private void Scroll()
    {
        currentTrans.anchoredPosition = Vector3.MoveTowards(currentTrans.anchoredPosition, centerPos, speed);
        beforeTrans.anchoredPosition = Vector3.MoveTowards(beforeTrans.anchoredPosition, targetPos, speed);
    }


    /// <summary>
    /// 過去の最高スコアをロード
    /// </summary>
    private void LoadScore()
    {
        string path = Application.dataPath + @"\score.csv";
        scoreDatas.Clear();
        if (!File.Exists(path))
        {
            using (File.Create(path)) { }
            using (StreamWriter streamWriter = new StreamWriter(path, false, Encoding.UTF8))
            {
                for (int i = 0; i < 3; i++)
                {
                    streamWriter.Write("0,C");
                    streamWriter.WriteLine();
                }
                streamWriter.Flush();
                streamWriter.Close();
                Debug.Log("プレイヤーデータを初期化しました。");
            }
        }

        StreamReader csv = new StreamReader(path, Encoding.UTF8);
        string line = null;
        while ((line = csv.ReadLine()) != null)
        {
            scoreDatas.Add(line.Split(','));
        }
        csv.Close();
        Debug.Log("スコアデータをロードしました。");
    }

    /// <summary>
    /// キャラクター説明をロード
    /// </summary>
    private void LoadCharacterInfo()
    {

    }

    /// <summary>
    /// キャラクター情報を表示
    /// </summary>
    private void DisplayCharacterInfo()
    {
        int current = (int)currentCharacter;
        characterName.text = nameString[current];
        characterType.text = typeString[current];
        background.sprite = backgroundSprite[current];
        infoWindow.sprite = infoWindowSprite[current];
        outer.sprite = outerSprite[current];
        inner.sprite = innerSprite[current];
        weaponIcon.sprite = weaponIconSprite[current];

        //
        score.text = int.Parse(scoreDatas[current][(int)SCORE.VALUE]).ToString("0000000");
        int rank = 0;
        switch (scoreDatas[current][(int)SCORE.RANK])
        {
            case "C":
                rank = 0;
                break;
            case "B":
                rank = 1;
                break;
            case "A":
                rank = 2;
                break;
            case "S":
                rank = 3;
                break;
            default:
                Debug.Log("Error");
                break;
        }
        rankIcon.sprite = rankIconSprite[rank];
        rankText.sprite = rankTextSprite[rank];
    }
}
