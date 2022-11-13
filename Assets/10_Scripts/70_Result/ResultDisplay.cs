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
    [SerializeField] private Text rankText = null;

    [SerializeField] private Image titleImage = null;
    [SerializeField] private Sprite[] titleSprite = null;
    [SerializeField] private Image baseImage = null;
    [SerializeField] private Sprite[] baseSprite = null;
    [SerializeField] private Image characterImage = null;
    [SerializeField] private Sprite[] characterSprite = null;
    [SerializeField] private Image highscoreImage = null;
    [SerializeField] private Sprite[] highscoreSprite = null;
    [SerializeField] private Image boardImage = null;
    [SerializeField] private Sprite[] boardSprite = null;

    private string scoreString;
    private string rankString;

    private void Awake()
    {
        scoreText.text = "";
        rankText.text = "";
        resultSave = this.GetComponent<ResultSave>();
        anim = this.GetComponent<Animator>();
        if (isDebug) DisplayResult(debugType, debugScore);
    }

    public void DisplayResult(int type, int score)
    {
        bool isHighscore = score > resultSave.Highscore(type);
        titleImage.sprite = titleSprite[type];
        baseImage.sprite = baseSprite[type];
        characterImage.sprite = characterSprite[type];
        boardImage.sprite = boardSprite[type];
        highscoreImage.sprite = highscoreSprite[type];
       scoreString = score.ToString("000000000");
        rankString = resultSave.Rank(score);

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
        anim.SetTrigger("ResultEnd");
    }

    public void InactiveResult()
    {
        this.gameObject.SetActive(false);
    }
}
