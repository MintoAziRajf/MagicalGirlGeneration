using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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

    //--------スクロール関連--------
    private bool isScroll = false; //スクロール中かどうか

    [SerializeField] private Vector2 centerPos, leftPos, rightPos; //スクロール先(固定値)
    
    private RectTransform currentTrans, beforeTrans; //現在、前のキャラの場所
    private Vector2 targetPos; //スクロール先
    
    private static float speed = 4f; //基本速度
    private float currentDistance, beforeDistance; //速度補間
    //------------------------------

    //--------キャラクター情報-------

    [SerializeField] private Sprite[] characterSprite = new Sprite[3]; //使用するキャラのスプライト
    private Image currentImage, beforeImage; //現在、前の見た目

    [SerializeField] private Text characterName = null, score = null, rank = null; //キャラクターの名前、ハイスコア、ランクの表示先
    [SerializeField] private string[] nameString = new string[3]; //キャラクターの名前
    //------------------------------

    private void Awake()
    {
        //Init
        currentImage = currentObj.GetComponent<Image>();
        currentTrans = currentObj.GetComponent<RectTransform>();
        beforeImage = beforeObj.GetComponent<Image>();
        beforeTrans = beforeObj.GetComponent<RectTransform>();

        DisplayCharacterInfo();
    }

    private void Update()
    {
        isScroll = !(currentTrans.anchoredPosition == centerPos); //スクロール中かどうかの判定
        //スクロール中は操作を受け付けない
        if (isScroll)
        {
            Scroll();
            return;
        }
        Debug.Log("スクロール操作可能");
        //右
        if (Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log("右スクロール");
            SetCharacter(true);
        }
        //左
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("左スクロール");
            SetCharacter(false);
        }
        //選択
        if (Input.GetButtonDown("Submit"))
        {
            SceneManager.sceneLoaded += GameSceneLoaded;
            LoadManager.instance.LoadScene("50_MainGame");
        }
    }

    private void GameSceneLoaded(Scene next, LoadSceneMode mode)
    {
        // シーン切り替え後のスクリプトを取得
        GameManager gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();

        // データを渡す処理
        gameManager.Type = (int)currentCharacter;
        gameManager.IsTutorial = true;
        gameManager.IsPrologue = true;

        // イベントから削除
        SceneManager.sceneLoaded -= GameSceneLoaded;
    }
    
    /// <summary>
    /// キャラクターの切り替え、スクロール先の設定
    /// </summary>
    /// <param name="isRight">右=true,左=false</param>
    private void SetCharacter(bool isRight)
    {
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

        //速度補間用
        currentDistance = Vector2.Distance(currentTrans.anchoredPosition, centerPos);
        beforeDistance = Vector2.Distance(beforeTrans.anchoredPosition, targetPos);
    }

    /// <summary>
    /// スクロール
    /// </summary>
    private void Scroll()
    {
        currentTrans.anchoredPosition = Vector3.MoveTowards(currentTrans.anchoredPosition, centerPos, speed * Time.deltaTime * currentDistance);
        beforeTrans.anchoredPosition = Vector3.MoveTowards(beforeTrans.anchoredPosition, targetPos, speed * Time.deltaTime * beforeDistance);
    }


    /// <summary>
    /// 過去の最高スコアをロード
    /// </summary>
    private void LoadScore()
    {

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
        characterName.text = nameString[(int)currentCharacter];
    }
}
