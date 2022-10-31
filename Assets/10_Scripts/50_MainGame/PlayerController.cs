using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject GamaManager = null;
    CollisionManager collisionManager;
    GameManager gameManager;
    CutIn cutIn;
    Evolution evolution;
    GameUI gameUI;
    PlayerSkill skill;
    PlayerHP playerHP;

    private int[,] playerGrid = new int[3, 3];
    private int[,] skillGrid = new int[3, 3];
    private const int EMPTY = 0;
    private const int ATTACK = 1;
    private const int SKILL = 2;

    //----------移動関連------------
    private int currentX = 1, currentY = 1;
    public int CurrentX { get { return currentX; } }
    public int CurrentY { get { return currentY; } }
    private const int LIMIT_MIN = 0;
    private const int LIMIT_MAX = 2;
    private Vector3 targetPos;
    private const float SPEED = 20f;
    private int moveCooltime = 30;
    public int MoveCooltime { set { moveCooltime = value; } }
    private enum DIRECTION
    {
        UP = 0,
        RIGHT = 1,
        DOWN = 2,
        LEFT = 3
    }
    private Vector3[] MOVE =
    {
         new Vector3(0f, 3f, 0f),
         new Vector3(3f, 0f, 0f),
         new Vector3(0f, -3f, 0f),
         new Vector3(-3f, 0f, 0f)
    };
    //------------------------------

    //----------回避関連------------
    private const int AVOID_COOLTIME = 600;
    //------------------------------

    //----------攻撃関連------------
    private int attackType = 0;
    public int AttackType { set { attackType = value; } }
    private int[] ATTACK_LINE = { 0, 2 };
    private int[] BACK_LINE = { 2, 0 };
    private Vector3[] ATTACK_MOVE =
    {
        new Vector3(0f, -6f, 0f),
        new Vector3(0f, 6f, 0f)
    };

    private int damageNormal = 0;
    public int DamageNoraml { set { damageNormal = value; } }
    private int damageEvolution = 0;
    public int DamageEvolution { set { damageEvolution = value; } }
    private int attackFreq = 0;
    public int AttackFreq { set { attackFreq = value; } }
    private int damageSkill = 0;
    public int DamageSkill { set { damageSkill = value;} }
    private int skillFreq = 0;
    public int SkillFreq { set { skillFreq = value;} }
    //------------------------------

    //----------変身関連------------
    private bool isEvo = false;
    public bool IsEvo
    {
        set
        {
            isEvo = value;
            playerHP.IsEvo = value;
            skill.IsEvo = value;
            if (isEvo)
            {
                StartCoroutine(WaitAnim("Evolution"));
            }
            else
            {
                StartCoroutine(WaitAnim("SolveEvolution"));
            }
        }
    }
    //------------------------------

    //----------操作フラグ----------
    private bool isMove = false;
    private bool isAttack = false;
    private bool isSkill = false;
    private bool isAvoid = false;
    private bool isAnim = false;
    private bool canInput = true;
    //------------------------------

    [SerializeField] private SpriteRenderer[] displayPlayerGrid = new SpriteRenderer[9];
    [SerializeField] private SpriteRenderer[] displaySkillGrid = new SpriteRenderer[9];
    [Header("EMPTY,ATTACK,SKILL")] [SerializeField] private Color[] gridColor = new Color[3];

    private void Awake()
    {
        Initialize();
    }

    private void Update()
    {
        DisplayGrid();
        
        //移動
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPos, SPEED * Time.deltaTime);
        canInput = (transform.localPosition == targetPos) && !isMove && !isAttack && !isAnim && !isSkill;
        if (!canInput) return;
        evolution.Check();
        InputDirection();
    }
    //-------------------------UI-------------------------------
    private void DisplayGrid()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                int index = i + j * 3;
                displayPlayerGrid[index].color = gridColor[playerGrid[i, j]];
                displaySkillGrid[index].color = gridColor[skillGrid[i, j]];
            }
        }
    }

    //-----------------------------------------------------------

    private void InputDirection()
    {
        bool avoid = Input.GetKey(KeyCode.Space) && !isAvoid;
        bool up = Input.GetKey(KeyCode.W) && currentY > LIMIT_MIN;
        bool down = Input.GetKey(KeyCode.S) && currentY < LIMIT_MAX;
        bool left = Input.GetKey(KeyCode.A) && currentX > LIMIT_MIN;
        bool right = Input.GetKey(KeyCode.D) && currentX < LIMIT_MAX;
        
        if (avoid)
        {
            collisionManager.PlayerAvoided(currentX, currentY, 30);
            StartCoroutine(Avoid());
            if (up)
            {
                currentY--;
                Move((int)DIRECTION.UP);
            }
            else if (down)
            {
                currentY++;
                Move((int)DIRECTION.DOWN);
            }
            else if (left)
            {
                currentX--;
                Move((int)DIRECTION.LEFT);
            }
            else if (right)
            {
                currentX++;
                Move((int)DIRECTION.RIGHT);
            }
            else
            {
                collisionManager.PlayerMoved(currentX, currentY);
                StartCoroutine(MoveDelay());
            }
            return;
        }
        else
        {
            if (up)
            {
                currentY--;
                Move((int)DIRECTION.UP);
            }
            else if (down)
            {
                currentY++;
                Move((int)DIRECTION.DOWN);
            }
            else if (left)
            {
                currentX--;
                Move((int)DIRECTION.LEFT);
            }
            else if (right)
            {
                currentX++;
                Move((int)DIRECTION.RIGHT);
            }
        }
        
    }

    //--------------------------移動関連-------------------------
    private void Move(int direction)
    {
        collisionManager.PlayerMoved(currentX, currentY);
        targetPos = targetPos + MOVE[direction];
        evolution.Increase("Move");
        StartCoroutine(MoveDelay());
        CheckTile();
    }
    private IEnumerator MoveDelay()
    {
        isMove = true;
        gameUI.MoveCurrentTime = 0;
        for (int i = 1; i <= moveCooltime; i++)
        {
            gameUI.MoveCurrentTime = i;
            yield return null;
        }
        isMove = false;
    }
    //------------------------------------------------------------

    private void CheckTile()
    {
        if (playerGrid[currentX,currentY] == ATTACK) StartCoroutine(Attack());
        if (skillGrid[currentX, currentY] == SKILL)
        {
            skillGrid[currentX, currentY] = EMPTY;
            skill.RemoveTile();
        }
            
    }

    //--------------------------回避関連-------------------------
    private IEnumerator Avoid()
    {
        isAvoid = true;
        gameUI.AvoidCurrentTime = 0;
        for (int i = 0; i < AVOID_COOLTIME; i++)
        {
            gameUI.AvoidCurrentTime = i;
            yield return null;
        }
        StartCoroutine(MessageManager.instance.DisplayMessage("スタミナが回復したよ！"));
        isAvoid = false;
    }
    //------------------------------------------------------------

    //--------------------------攻撃関連--------------------------
    private IEnumerator Attack()
    {
        evolution.Increase("Attack");
        isAttack = true;
        yield return StartCoroutine(cutIn.Attack(isEvo));
        //damage
        if(isEvo) AttackEnemy(damageEvolution,attackFreq);
        else AttackEnemy(damageNormal, attackFreq);

        isAttack = false;
        BringBackPlayer();
    }
    private void BringBackPlayer()
    {
        currentY = BACK_LINE[attackType];
        collisionManager.PlayerMoved(currentX, currentY);
        targetPos = targetPos + ATTACK_MOVE[attackType];
        CheckTile();
    }
    private void AttackEnemy(int damage, int freq)
    {
        for(int i = 0; i < freq; i++){
            //hp -= damage;
        }
    }
    //------------------------------------------------------------------
    
    //-------------------------カットイン関連---------------------------------
    private IEnumerator WaitAnim(string s)
    {
        isAnim = true;
        yield return cutIn.StartCoroutine(s);
        isAnim = false;
    }
    //--------------------------------------------------------------


    //--------------------------SKILL-------------------------------
    
    public IEnumerator Skill()
    {
        yield return new WaitUntil(() => !isAttack);
        isSkill = true;
        yield return StartCoroutine(cutIn.Skill());
        //damage
        isSkill = false;
    }
    //--------------------------------------------------------------

    //------init--------
    private void Initialize()
    {
        SetScript();
        InitTiles();
        SetUI();
        SetAttackTile();
    }
    
    
    private void SetScript()
    {
        collisionManager = GamaManager.GetComponent<CollisionManager>();
        gameManager = GamaManager.GetComponent<GameManager>();
        cutIn = GetComponent<CutIn>();
        evolution = GetComponent<Evolution>();
        gameUI = GetComponent<GameUI>();
        skill = GetComponent<PlayerSkill>();
        playerHP = GetComponent<PlayerHP>();
    }
    private void InitTiles()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                playerGrid[i, j] = EMPTY;
                skillGrid[i, j] = EMPTY;
            }
        }
        collisionManager.PlayerMoved(currentX, currentY);
        targetPos = transform.localPosition;
    }

    private void SetAttackTile()
    {
        for (int i = 0; i < 3; i++)
        {
            playerGrid[i, ATTACK_LINE[attackType]] = ATTACK;
        }
    }

    private void SetUI()
    {
        gameUI.MoveCooltime = moveCooltime;
        gameUI.MoveCurrentTime = moveCooltime;
        gameUI.AvoidCooltime = AVOID_COOLTIME;
        StartCoroutine(MessageManager.instance.DisplayMessage("さぁ世界を救いに行こう！"));
    }
}
