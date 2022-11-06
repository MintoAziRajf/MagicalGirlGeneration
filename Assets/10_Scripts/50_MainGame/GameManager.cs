using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    EnemyManager enemyManager;
    PlayerController playerController;
    Tutorial tutorial;
    Prologue prologue;

    [SerializeField] private GameObject player = null;
    [SerializeField] private GameObject enemy = null;
    [SerializeField] private GameObject pauseCanvas = null;

    [SerializeField] private bool isTutorial = false;
    public bool IsTutorial { set { isTutorial = value; } }
    [SerializeField] private bool isPrologue = false;
    public bool IsPrologue { set { isPrologue = value; } }
    [SerializeField] private int type = 0;
    public int Type
    {
        set { type = value; }
        get { return type; }
    }

    private void Start()
    {
        playerController = player.GetComponent<PlayerController>();
        playerController.IsTutorial = isTutorial;
        enemyManager = enemy.GetComponent<EnemyManager>();
        tutorial = this.GetComponent<Tutorial>();
        prologue = this.GetComponent<Prologue>();
        Time.timeScale = 0f;
        StartCoroutine(GameStart());
    }

    private void FixedUpdate()
    {
        if (isTutorial || isPrologue) return;
        if (Input.GetButtonDown("Option"))
        {
            pauseCanvas.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void StopGame()
    {
        playerController.IsStart = false;
    }
    public void ResumeGame()
    {
        playerController.IsStart = true;
    }
    public void Result()
    {
        playerController.IsStart = false;
        enemyManager.IsStart = false;
    }

    private IEnumerator Tutorial()
    {
        if (!isTutorial)
        {
            yield break;
        }
        yield return StartCoroutine(tutorial.Flow());
        SceneManager.sceneLoaded += GameSceneLoaded;
        LoadManager.instance.LoadScene("50_MainGame");
    }

    private void GameSceneLoaded(Scene next, LoadSceneMode mode)
    {
        // シーン切り替え後のスクリプトを取得
        GameManager gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();

        // データを渡す処理
        gameManager.Type = type;
        gameManager.IsTutorial = false;
        gameManager.IsPrologue = false;
        // イベントから削除
        SceneManager.sceneLoaded -= GameSceneLoaded;
    }

    private IEnumerator Prologue()
    {
        if (!isPrologue)
        {
            yield break;
        }
        yield return StartCoroutine(prologue.StartPrologue(type));
    }

    private IEnumerator GameStart()
    {
        yield return StartCoroutine(Prologue());
        Time.timeScale = 1f;
        yield return StartCoroutine(Tutorial());
        playerController.IsStart = true;
        enemyManager.IsStart = true;
        enemyManager.StartAttack();
    }

    [SerializeField] private GameObject resultPrefab = null;
    private int score;
    public int Score { set { score = value; } }
    public IEnumerator GameOver()
    {
        GameObject resultObj = Instantiate(resultPrefab);
        resultObj.GetComponent<ResultDisplay>().DisplayResult(type,score);
        while (resultObj.activeSelf)
        {
            yield return null;
        }
        //retry
    }

    public IEnumerator GameClear()
    {
        GameObject resultObj = Instantiate(resultPrefab);
        resultObj.GetComponent<ResultDisplay>().DisplayResult(type, score);
        while (resultObj.activeSelf)
        {
            yield return null;
        }
        //endcard
    }
}
