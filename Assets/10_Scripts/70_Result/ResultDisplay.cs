using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class ResultDisplay : MonoBehaviour
{
    ResultSave resultSave;
    Animator anim;

    public bool isDebug = false;
    public int debugScore = 0;
    public int debugType = 0;

    [SerializeField] private Text scoreText = null;

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

    [SerializeField] private Image rankIconImage = null;
    [SerializeField] private Sprite[] rankIconSprite = null;
    [SerializeField] private Image rankImage = null;
    [SerializeField] private Sprite[] rankSprite = null;

    private string scoreString;

    private void Awake()
    {
        scoreText.text = "";
        resultSave = this.GetComponent<ResultSave>();
        anim = this.GetComponent<Animator>();
        SoundManager.instance.PlaySE(SoundManager.SE_Type.DramRoll);
        if (isDebug) DisplayResult(debugType, debugScore);
    }

    public void DisplayResult(int type, int score)
    {
        bool isHighscore = score > resultSave.Highscore(type);
        foreach (UIData data in uiData)
        {
            data.displayImage.sprite = data.displaySprite[type];
        }
        int rank = resultSave.RankInt(score);
        rankIconImage.sprite = rankIconSprite[rank];
        rankImage.sprite = rankSprite[rank];
        scoreString = score.ToString("0000000");

        anim.SetTrigger("ResultStart");
        anim.SetBool("isHighscore", isHighscore);
        if(isHighscore) resultSave.SaveScore(type, score);
    }

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

    public IEnumerator WaitInput()
    {
        yield return new WaitUntil(() => Input.GetButtonDown("Submit"));
        SoundManager.instance.PlaySE(SoundManager.SE_Type.Submit);
        anim.SetTrigger("ResultEnd");
    }

    public void InactiveResult()
    {
        this.gameObject.SetActive(false);
    }

    public void PlaySE()
    {
        SoundManager.instance.PlaySE(SoundManager.SE_Type.Highscore);
    }
}
