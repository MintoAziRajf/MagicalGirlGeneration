using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    //------------ローディング背景用-----------------
    [SerializeField] private GameObject loadingScreen = null;
    private int type;
    private int isEvo;
    //背景
    [SerializeField] private Image background = null;
    [SerializeField] private Sprite[] backgroundSprite = new Sprite[3];
    //フレーム
    [SerializeField] private Image frame = null;
    [SerializeField] private Sprite[] frameSprite = new Sprite[3];
    //見た目
    [SerializeField] private Image visual = null;
    [SerializeField] private Sprite[] visualType = new Sprite[6];
    //名前
    [SerializeField] private Text nameText = null;
    [SerializeField] private Color[] nameColor = new Color[3];
    [SerializeField] private string[] characterName = new string[3];
    //変身前かどうか
    [SerializeField] private Text evoText = null;
    [SerializeField] private string[] evoString = new string[2];
    //----------------------------------------------

    //------------フェード用---------------------
    [SerializeField] private Image fadeScreen = null; //スクリーン
    float alpha = 0f;//アルファ
    Color c = Color.black; //カラー
    //-------------------------------------------

    void Awake()
    {
        SetVisual();
    }

    //表示するキャラクターをランダムで表示
    private void SetVisual()
    {
        type = Random.Range(0, 3); //ランダム

        background.sprite = backgroundSprite[type]; //背景
        frame.sprite = frameSprite[type]; //フレーム
        nameText.text = characterName[type]; //表示中のキャラの名前
        nameText.GetComponent<Outline>().effectColor = nameColor[type];


        isEvo = Random.Range(0, 2); //変身前か変身後か　ランダム
        evoText.text = evoString[isEvo]; //変身有無のテキスト表示
        evoText.GetComponent<Outline>().effectColor = nameColor[type];

        //変身後なら変身後のスプライトを適用
        if (isEvo == 1) type = type + 3; 
        visual.sprite = visualType[type];
    }
    public IEnumerator LoadScene(string sceneName)
    {
        Time.timeScale = 0f; //
        yield return StartCoroutine(FadeScreen(false));
        loadingScreen.SetActive(true);
        yield return StartCoroutine(FadeScreen(true));
        // 非同期でロードを行う
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // ロードが完了していても，シーンのアクティブ化は許可しない
        asyncLoad.allowSceneActivation = false;

        while (asyncLoad.progress < 0.9f) //0.9で止まる
        {
            yield return 0;
        }
        yield return new WaitForSecondsRealtime(2.0f);
        yield return StartCoroutine(FadeScreen(false));
        
        // ロードが完了したときにシーンのアクティブ化を許可する
        asyncLoad.allowSceneActivation = true;

        Time.timeScale = 1f; //
        // ロードが完了するまで待つ
        yield return asyncLoad;
    }

    /// <summary>
    /// 画面をフェードさせる
    /// </summary>
    /// <param name="b">フェードイン:True フェードアウト:False<param>
    private IEnumerator FadeScreen(bool b)
    {
        float start, end;
        if (b)
        {
            start = 1f;
            end = 0f;
        }
        else
        {
            start = 0f;
            end = 1f;
        }
        alpha = start;
        for (int i = 0; i < 30; i++)
        {
            yield return null;
            alpha += (end - start) / 30;
            c.a = alpha;
            fadeScreen.color = c;
        }
    }
}
