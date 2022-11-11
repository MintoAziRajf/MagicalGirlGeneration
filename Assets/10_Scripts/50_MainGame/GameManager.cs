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
    Epilogue epilogue;
    ScoreManager scoreManager;

    [SerializeField] private GameObject player = null; 
    [SerializeField] private GameObject enemy = null;
    [SerializeField] private GameObject pauseCanvas = null;
    [SerializeField] private GameObject debugCanvas = null;

    [SerializeField] private bool isTutorial = false;
    public bool IsTutorial { set { isTutorial = value; } }
    [SerializeField] private bool isPrologue = false;
    public bool IsPrologue { set { isPrologue = value; } }
    private bool isStart = false;
    [SerializeField] private int type = 0;
    public int Type
    {
        set { type = value; }
        get { return type; }
    }

    [SerializeField] private GameObject gameOverCanvas = null;
    [SerializeField] private GameObject resultPrefab = null;

    private void Start()
    {
        playerController = player.GetComponent<PlayerController>();
        playerController.IsTutorial = isTutorial;
        enemyManager = enemy.GetComponent<EnemyManager>();
        tutorial = this.GetComponent<Tutorial>();
        prologue = this.GetComponent<Prologue>();
        epilogue = this.GetComponent<Epilogue>();
        scoreManager = this.GetComponent<ScoreManager>();
        StartCoroutine(GameStart());
    }

    private void FixedUpdate()
    {
        if (isTutorial || isPrologue) return; //チュートリアルとプロローグ中は何もしない
        if (Input.GetButtonDown("Option") && isStart)
        {
            isStart = false;
            pauseCanvas.SetActive(true);
            Time.timeScale = 0f;
        }
        if (Input.GetKeyDown("joystick button 6") && isStart || Input.GetKeyDown(KeyCode.R) && isStart)
        {
            isStart = false;
            debugCanvas.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    /// <summary>
    /// スコアマネージャーにスコア追加　(PlayerControllerから呼ばれます)
    /// </summary>
    /// <param name="scoreType"></param>
    public void AddScore(int value)
    {
        scoreManager.AddScore(value);
    }

    /// <summary>
    /// PlayerHPとEnemyManagerから呼ばれる
    /// </summary>
    public void StopGame()
    {
        Debug.Log("プレイヤーと敵の動きを停止します。");
        this.isStart = false;
        playerController.IsStart = false;
        enemyManager.IsStart = false;
        enemyManager.StopAttack();
    }

    /// <summary>
    /// EnemyManagerから呼ばれる
    /// </summary>
    public void ResumeGame()
    {
        Debug.Log("プレイヤーと敵の動きを開始します。");
        this.isStart = true;
        enemyManager.IsStart = true;
        playerController.IsStart = true;
        enemyManager.StartAttack();
    }

    [SerializeField] private GameObject countDownObj = null;
    private const int COUNTDOWN_TIME = 180; //3秒
    public IEnumerator StartCountDown()
    {
        Time.timeScale = 0f;
        countDownObj.SetActive(true);
        for (int i = 0; i < COUNTDOWN_TIME; i++) yield return null;
        countDownObj.SetActive(false);
        Time.timeScale = 1f;
        isStart = true;
        yield break;
    }

    /// <summary>
    /// ゲームを
    /// </summary>
    private IEnumerator GameStart()
    {
        if (isPrologue) yield return StartCoroutine(Prologue()); // isPrologueがTrueならプロローグを開始する
        if (isTutorial)
        {
            yield return StartCoroutine(Tutorial()); // isTutorialがTrueならチュートリアルを開始する
            yield break;
        }
        yield return StartCoroutine(StartCountDown()); //カウントダウンを始める
        playerController.IsStart = true; // playerとenemyを動かす
        enemyManager.IsStart = true;
        enemyManager.StartAttack();
    }

    /// <summary>
    /// プロローグを開始する
    /// </summary>
    private IEnumerator Prologue()
    {
        Time.timeScale = 0f;
        yield return StartCoroutine(prologue.StartPrologue(type));
        Time.timeScale = 1f;
    }

    /// <summary>
    /// チュートリアルを開始する
    /// </summary>
    private IEnumerator Tutorial()
    {
        yield return StartCoroutine(tutorial.Flow());
        Retry();
    }

    /// <summary>
    /// ゲームオーバー時に呼ばれるメソッド
    /// playerDead⇒GameOver()⇒gameOverCanvas
    /// </summary>
    public void GameOver()
    {
        gameOverCanvas.SetActive(true);
    }

    /// <summary>
    /// ゲームオーバー時にゲームオーバーキャンバスから呼ばれるメソッド
    /// gameOverCanvas.End()⇒GameOverResult()⇒gameOverCanvas.DisplayRetry()
    /// </summary>
    public IEnumerator GameOverResult()
    {
        StopGame();

        yield return StartCoroutine(Result());
        //retry
        gameOverCanvas.GetComponent<GameOverCanvas>().DisplayRetry();
    }
    /// <summary>
    /// ゲームをクリアした時に呼ばれるメソッド
    /// EnemyManager.EndGame() ⇒ GameClear() ⇒ Epilogue ⇒ Result ⇒ Endcard
    /// </summary>
    public IEnumerator GameClear()
    {
        StopGame();
        yield return StartCoroutine(epilogue.StartEpilogue(type));
        yield return StartCoroutine(Result());
        //endcard
        LoadManager.instance.LoadScene("71_EndCard");
    }

    /// <summary>
    /// リザルトを表示
    /// </summary>
    private IEnumerator Result()
    {
        GameObject resultObj = Instantiate(resultPrefab); //リザルトプレハブを生成
        resultObj.GetComponent<ResultDisplay>().DisplayResult(type, scoreManager.Score); // 現在のキャラとscoreを受け渡す
        while (resultObj.activeSelf) // リザルト表示終了まで待機
        {
            yield return null;
        }
    }

    /// <summary>
    /// ゲームをリスタートする
    /// </summary>
    public void Retry()
    {
        SceneManager.sceneLoaded += GameSceneLoaded;
        LoadManager.instance.LoadScene("50_MainGame");
    }

    /// <summary>
    /// ゲームをリスタートするとき呼ばれる　
    /// </summary>
    private void GameSceneLoaded(Scene next, LoadSceneMode mode)
    {
        // シーン切り替え後のスクリプトを取得
        GameManager gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();

        // データを渡す処理
        gameManager.Type = type;
        gameManager.IsTutorial = false; // リスタート時は強制OFF
        gameManager.IsPrologue = false; // リスタート時は強制OFF
        // イベントから削除
        SceneManager.sceneLoaded -= GameSceneLoaded;
    }
}
