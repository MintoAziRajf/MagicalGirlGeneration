using UnityEngine;
/// <summary>
/// プレイヤー関連の各スクリプトから各スクリプトを取得する作業を省くためのスクリプトです
/// </summary>
public class PlayerManager : MonoBehaviour
{
    GameObject gameManagerObj;

    [HideInInspector] public GameManager gameManager;
    [HideInInspector] public CollisionManager collisionManager;

    GameObject enemyObj;

    [HideInInspector] public EnemyManager enemyManager;

    [HideInInspector] public PlayerHP playerHP;
    [HideInInspector] public PlayerDeadAnimation playerDeadAnimation;
    [HideInInspector] public PlayerSkill playerSkill;
    [HideInInspector] public PlayerAttack playerAttack;
    [HideInInspector] public PlayerController playerController;
    [HideInInspector] public Evolution evolution;
    [HideInInspector] public GameUI gameUI;
    [HideInInspector] public CutIn cutIn;
    [HideInInspector] public AudienceAnimation audienceAnimation;
    [HideInInspector] public Animator visualAnim;

    public enum CHARACTER { RED, BLUE, YELLOW } // キャラの種類
    public enum DIRECTION { UP = 0, RIGHT = 1, DOWN = 2, LEFT = 3 } // 方向
    /// <summary>
    /// スコア情報
    /// </summary>
    public enum SCORE
    {
        MOVE = 50,
        SKILL_ORB = 100,
        COUNTER = 30000,
        SKILL = 10000,
        HEAL = 500,
        ATTACK = 77,
        EVOLUTION = 2000
    }
    public const int AVOID_COOLTIME = 300; // 回避クールタイム
    public const int AVOID_INPUT_FRAME = 20; // 回避入力猶予フレーム
    public const int AVOID_TIME = 30; // 回避持続フレーム

    public void Awake()
    {
        gameManagerObj = GameObject.FindWithTag("GameManager");
        gameManager = gameManagerObj.GetComponent<GameManager>();
        collisionManager = gameManagerObj.GetComponent<CollisionManager>();

        enemyObj = GameObject.Find("Enemy");
        enemyManager = enemyObj.GetComponent<EnemyManager>();

        playerHP = this.GetComponent<PlayerHP>();
        playerDeadAnimation = this.GetComponent<PlayerDeadAnimation>();
        playerSkill = this.GetComponent<PlayerSkill>();
        playerAttack = this.GetComponent<PlayerAttack>();
        playerController = this.GetComponent<PlayerController>();
        evolution = this.GetComponent<Evolution>();
        gameUI = this.GetComponent<GameUI>();
        cutIn = this.GetComponent<CutIn>();
        audienceAnimation = this.GetComponent<AudienceAnimation>();

        visualAnim = this.GetComponent<Animator>();
    }

    public int ConvertPosition(int x, int y) => x + y* 3;
    public int ConvertPositionX(int pos) => pos / 3;
    public int ConvertPositionY(int pos) => pos % 3;
}
