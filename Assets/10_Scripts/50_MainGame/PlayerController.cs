using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private TextAsset statusCSV = null;
    List<string[]> statusData = new List<string[]>();
    private enum STATUS
    {
        NOR_HP = 1,
        EVO_HP = 2,
        ATTACK_FREQ = 3,
        NOR_DAMAGE = 4,
        EVO_DAMAGE = 5,
        SKILL_FREQ = 6,
        SKILL_DAMAGE = 7,
        MOVE_COOLTIME = 8,
        ATTACK_LINE = 9
    }
    private int type = 0;

    [SerializeField] private GameObject GamaManager = null;
    CollisionManager collisionManager;
    GameManager gameManager;
    CutIn cutIn;

    private int[,] playerGrid = new int[3, 3];
    private int[,] skillGrid = new int[3, 3];
    private const int EMPTY = 0;
    private const int ATTACK = 1;
    private const int SKILL = 2;

    
    //----------UI------------
    [SerializeField] private Image moveDisplay = null;
    [SerializeField] private Image avoidDisplay = null;
    [SerializeField] private Image skillDisplay = null;
    private int moveCurrentTime = 0;
    private int avoidCurrentTime = 0;
    private int skillCurrentTime = 0;
    [SerializeField] private Image hpDisplay = null;
    [SerializeField] private Image evolutionDisplay = null;
    private float evolutionCurrent = 0f;
    private float displayDelay = 20f;
    //----------HP--------------
    private int hp = 100;
    private int HP_NOR_MAX = 100;
    private int HP_EVO_MAX = 100;
    private const int HP_MIN = 0;
    //----------移動関連------------
    private int currentX = 1, currentY = 1;
    private const int LIMIT_MIN = 0;
    private const int LIMIT_MAX = 2;
    private Vector3 targetPos;
    private const float SPEED = 20f;
    private int MOVE_COOLTIME = 30;
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
    private const int AVOID_COOLTIME = 60;
    //------------------------------

    //----------攻撃関連------------
    private int attackType = 0;
    private int[] ATTACK_LINE = { 0, 2 };
    private int[] BACK_LINE = { 2, 0 };
    private const int ATTACK_COOLTIME = 20;
    private Vector3[] ATTACK_MOVE =
    {
        new Vector3(0f, -6f, 0f),
        new Vector3(0f, 6f, 0f)
    };

    private int ATTACK_DAMAGE_NOR = 0;
    private int ATTACK_DAMAGE_EVO = 0;
    private int ATTACK_FREQ = 0;
    private int SKILL_DAMAGE = 0;
    private int SKILL_FREQ = 0;
    //------------------------------

    //----------スキル関連----------
    private int skillTiles = 4;
    private int currentSkillTiles = 0;
    private const int SKILL_COOLTIME = 600;
    //------------------------------

    //----------変身関連------------
    private const int EVO_MAX = 100;
    private const int EVO_MIN = 0;
    private const int EVO_MOVE = 5;
    private const int EVO_ATTACK = 10;
    private const int EVO_TICK = -1;
    private float evoTime = 0f;
    private int evoGauge = 0;
    private bool isEvo = false;
    public bool IsEvo
    {
        set
        {
            isEvo = value;
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
        DisplayUI();
        WhenEvolving();
        
        //移動
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPos, SPEED * Time.deltaTime);
        canInput = (transform.localPosition == targetPos) && !isMove && !isAttack && !isAnim && !isSkill;
        if (!canInput) return;
        CheckEvolution();
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

    private void DisplayUI()
    {
        evolutionCurrent = Mathf.MoveTowards(evolutionCurrent, evoGauge, displayDelay * Time.deltaTime);
        moveDisplay.fillAmount = (float)moveCurrentTime / MOVE_COOLTIME;
        avoidDisplay.fillAmount = (float)avoidCurrentTime / AVOID_COOLTIME;
        skillDisplay.fillAmount = (float)skillCurrentTime / SKILL_COOLTIME;
        evolutionDisplay.fillAmount = evolutionCurrent / EVO_MAX;
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
                Debug.Log("a");
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
        IncreaseEvolution(EVO_MOVE);
        StartCoroutine(MoveDelay());
        CheckTile();
    }
    private IEnumerator MoveDelay()
    {
        isMove = true;
        moveCurrentTime = 0;
        for (int i = 0; i < MOVE_COOLTIME; i++)
        {
            moveCurrentTime++;
            yield return null;
        }
        isMove = false;
    }
    //------------------------------------------------------------

    private void CheckTile()
    {
        if (playerGrid[currentX,currentY] == ATTACK) StartCoroutine(Attack());
        if (skillGrid[currentX, currentY] == SKILL) RemoveSkillTile();
    }

    //--------------------------回避関連-------------------------
    private IEnumerator Avoid()
    {
        isAvoid = true;
        avoidCurrentTime = 0;
        for (int i = 0; i < AVOID_COOLTIME; i++)
        {
            avoidCurrentTime++;
            yield return null;
        }
        StartCoroutine(MessageManager.instance.DisplayMessage("スタミナが回復したよ！"));
        isAvoid = false;
    }
    //------------------------------------------------------------

    //--------------------------攻撃関連--------------------------
    private IEnumerator Attack()
    {
        IncreaseEvolution(EVO_ATTACK);
        isAttack = true;
        yield return StartCoroutine(cutIn.Attack(isEvo));
        //damage
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
    
    //--------------------------変身関連-------------------------------
    private void WhenEvolving()
    {
        if (!isEvo) return;
        DecreaseEvolution();
        StartCoroutine(SetSkillTile());
    }
    private void CheckEvolution()
    {
        if (evoGauge == EVO_MAX && !isEvo)
        {
            isEvo = true;
            StartCoroutine(WaitAnim("Evolution"));
        }
        if (evoGauge == EVO_MIN && isEvo)
        {
            isEvo = false;
            StartCoroutine(WaitAnim("SolveEvolution"));
            ResetSkillTile();
        }
    }
    private void IncreaseEvolution(int value)
    {
        if (isEvo) return;
        evoGauge = Mathf.Min(evoGauge + value, EVO_MAX);
    }
    private void DecreaseEvolution()
    {
        evoTime += Time.deltaTime;
        if (evoTime >= 1.0f)
        {
            evoTime = 0f;
            evoGauge = Mathf.Max(evoGauge + EVO_TICK, EVO_MIN);
        }
    }
    private IEnumerator WaitAnim(string s)
    {
        isAnim = true;
        yield return cutIn.StartCoroutine(s);
        isAnim = false;
    }
    //--------------------------------------------------------------


    //--------------------------SKILL-------------------------------
    private IEnumerator SetSkillTile()
    {
        if (currentSkillTiles > 0) yield break;
        currentSkillTiles = skillTiles;
        skillCurrentTime = 0;
        for (int i = 0; i < SKILL_COOLTIME; i++)
        {
            skillCurrentTime++;
            yield return null;
        }
        if (!isEvo) yield break;
        List<int> numbers = new List<int>();
        for (int i = 0; i < 9; i++)
        {
            numbers.Add(i);
            if (currentX == (i / 3) && currentY == (i % 3)) numbers.RemoveAt(i);
        }
        for (int i = 0; i < skillTiles; i++)
        {
            int index = Random.Range(0, numbers.Count);
            int num = numbers[index];
            numbers.RemoveAt(index);
            skillGrid[num / 3, num % 3] = SKILL;
        }
    }
    private void ResetSkillTile()
    {
        for(int i = 0; i < 9; i++)
        {
            if(skillGrid[i/3,i%3] == SKILL)
            {
                skillGrid[i / 3, i % 3] = EMPTY;
                currentSkillTiles--;
            }
        }
    }
    private void RemoveSkillTile()
    {
        skillGrid[currentX, currentY] = EMPTY;
        currentSkillTiles--;
        CheckSkill();
    }
    private void CheckSkill()
    {
        if(currentSkillTiles == 0)
        {
            StartCoroutine(UseSkill());
        }
    }
    private IEnumerator UseSkill()
    {
        yield return new WaitUntil(() => !isAttack);
        isSkill = true;
        yield return StartCoroutine(cutIn.Skill());
        //damage
        isSkill = false;
    }
    //--------------------------------------------------------------

    //-----------HP---------
    public void Damage(int value)
    {
        hp = Mathf.Max(hp - value, HP_MIN);
        if (hp == HP_MIN)
        {
            //dead
            Debug.Log("死にました。");
        }
    }
    public void Heal(int value)
    {
        //hp = Mathf.Min(hp + value, HP_MAX);
    }
    //------init--------
    private void Initialize()
    {
        LoadStatus();
        SetScript();
        InitTiles();
        SetCharacter();
        SetUI();
        SetAttackTile();
    }
    
    private void LoadStatus()
    {
        StringReader reader = new StringReader(statusCSV.text);

        string line = null;
        line = reader.ReadLine(); //見出し行をスキップする
        while (reader.Peek() != -1) // reader.Peekが-1になるまで
        {
            line = reader.ReadLine(); // 一行ずつ読み込み
            statusData.Add(line.Split(',')); // , 区切りでリストに追加
        }
        reader.Close();
    }
    private void SetScript()
    {
        collisionManager = GamaManager.GetComponent<CollisionManager>();
        gameManager = GamaManager.GetComponent<GameManager>();
        cutIn = GetComponent<CutIn>();
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
    }
    private void SetCharacter()
    {
        type = DataStorage.instance.PlayerType;
        targetPos = transform.localPosition;
        collisionManager.PlayerMoved(currentX, currentY);
        HP_NOR_MAX = ReturnStatus(STATUS.NOR_HP);
        HP_EVO_MAX = ReturnStatus(STATUS.EVO_HP);
        ATTACK_FREQ = ReturnStatus(STATUS.ATTACK_FREQ);
        ATTACK_DAMAGE_NOR = ReturnStatus(STATUS.NOR_DAMAGE);
        ATTACK_DAMAGE_EVO = ReturnStatus(STATUS.EVO_DAMAGE);
        SKILL_DAMAGE = ReturnStatus(STATUS.SKILL_DAMAGE);
        SKILL_FREQ = ReturnStatus(STATUS.SKILL_FREQ);
        MOVE_COOLTIME = ReturnStatus(STATUS.MOVE_COOLTIME);
        attackType = ReturnStatus(STATUS.ATTACK_LINE);
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
        moveCurrentTime = MOVE_COOLTIME;
        avoidCurrentTime = AVOID_COOLTIME;
        skillCurrentTime = SKILL_COOLTIME;
        StartCoroutine(MessageManager.instance.DisplayMessage("さぁ世界を救いに行こう！"));
    }

    private int ReturnStatus(STATUS s)
    {
        int value = int.Parse(statusData[type][(int)s]);
        return value;
    }
}
