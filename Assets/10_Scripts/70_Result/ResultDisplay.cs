using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class ResultDisplay : MonoBehaviour
{
    ResultSave resultSave;
    Animator anim;

    //デバッグ用
    public bool isDebug = false; 
    public int debugScore = 0;
    public int debugType = 0;

    [SerializeField] private Text scoreText = null; // スコア表示

    // UIをキャラによって変更させる用
    [System.Serializable]
    private class UIData
    {
        [SerializeField, Header("表示先")]
        public Image displayImage = null;
        [SerializeField, Header("赤、青、黄")]
        public Sprite[] displaySprite = null;
    }
    [SerializeField, Header("キャラに応じてUIを表示")]
    private List<UIData> uiData = new List<UIData>();

    [SerializeField] private Image rankIconImage = null; // ランクアイコン
    [SerializeField] private Sprite[] rankIconSprite = null;
    [SerializeField] private Image rankImage = null; // ランクアイコン(文字)
    [SerializeField] private Sprite[] rankSprite = null;

    private string scoreString; // スコア

    private void Awake()
    {
        scoreText.text = ""; // スコアテキストをリセット
        resultSave = this.GetComponent<ResultSave>();
        anim = this.GetComponent<Animator>();
        SoundManager.instance.PlaySE(SoundManager.SE_Type.DramRoll); // 効果音
        if (isDebug) DisplayResult(debugType, debugScore); // デバッグがtrueだったら表示
    }

    /// <summary>
    /// リザルト表示
    /// </summary>
    /// <param name="type"></param>
    /// <param name="score"></param>
    public void DisplayResult(int type, int score)
    {
        bool isHighscore = score > resultSave.Highscore(type); // ハイスコア更新されたか
        // キャラに応じたUIを変更
        foreach (UIData data in uiData)
        {
            data.displayImage.sprite = data.displaySprite[type];
        }
        int rank = resultSave.RankInt(score); // スコアからランクを取得
        //ランク表示
        rankIconImage.sprite = rankIconSprite[rank]; 
        rankImage.sprite = rankSprite[rank];
        //スコア表示
        scoreString = score.ToString("0000000");

        // 基本アニメーション再生
        anim.SetTrigger("ResultStart");
        // ハイスコアアニメーション
        anim.SetBool("isHighscore", isHighscore); 
        // ハイスコアだったらスコアを保存する
        if(isHighscore) resultSave.SaveScore(type, score);
    }


    /// <summary>
    /// スコア表示(アニメーション)
    /// </summary>
    string numbers = "0123456789";
    private StringBuilder scoreDisplay = new StringBuilder("");
    public IEnumerator DisplayScore()
    {
        string str = scoreString;
        
        for (int i = 0; i < str.Length; i++)
        {
            scoreDisplay.Append(RandomNum(str.Length - i));
            scoreText.text = scoreDisplay.ToString();

            for (int j = 0; j < 5; j++)
            {
                yield return null;
                scoreDisplay.Remove(i, str.Length - i);
                scoreDisplay.Append(RandomNum(str.Length - i));
                scoreText.text = scoreDisplay.ToString();
            }

            scoreDisplay.Remove(i, str.Length - i);
            scoreDisplay.Append(str[i]);
            scoreText.text = scoreDisplay.ToString();
        }
        anim.SetTrigger("DisplayEnd");
    }

    private string RandomNum(int value)
    {
        string str = "";
        for (int i = 0; i < value; i++)
        {
            int randomNum = Random.Range(0, 10);
            str += numbers[randomNum];
        }
        return str;
    }

    /// <summary>
    /// 入力されたらリザルトを終了させる
    /// </summary>
    public IEnumerator WaitInput()
    {
        yield return new WaitUntil(() => Input.GetButtonDown("Submit"));
        SoundManager.instance.PlaySE(SoundManager.SE_Type.Submit); // 効果音
        anim.SetTrigger("ResultEnd"); 
    }

    /// <summary>
    /// リザルトを非アクティブ化
    /// </summary>
    public void InactiveResult()
    {
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// 効果音再生(アニメーションから呼び出し可能に)
    /// </summary>
    public void PlaySE()
    {
        SoundManager.instance.PlaySE(SoundManager.SE_Type.Highscore);
    }
}
