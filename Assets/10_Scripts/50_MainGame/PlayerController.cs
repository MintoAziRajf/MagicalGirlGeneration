using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : PlayerManager
{
    private bool[,] playerGrid = new bool[3, 3];
    private bool[,] skillGrid = new bool[3, 3];
    private bool[,] healGrid = new bool[3, 3];
    
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
    private const int AVOID_COOLTIME = 60;
    private const int AVOID_INPUT_FRAME = 20;
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
    private int damageCounterAttack = 0;
    public int DamageCounterAttack { set { damageCounterAttack = value; } }
    private int counterAttackFreq = 0;
    public int CounterAttackFreq { set { counterAttackFreq = value; } }
    private int damageSkill = 0;
    public int DamageSkill { set { damageSkill = value;} }
    private int skillFreq = 0;
    public int SkillFreq { set { skillFreq = value;} }

    //倍率
    private const float WEAK_MULTIPLIER = 1.5f;
    //------------------------------

    //----------変身関連------------
    private bool isEvo = false;
    public bool IsEvo
    {
        set
        {
            isEvo = value;
            playerHP.IsEvo = value;
            playerSkill.IsEvo = value;
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

    //-----------ゲームのスタートフラグ----------
    private bool isStart = false;
    public bool IsStart { set { isStart = value; } }
    //------------------------------------------

    //----------操作フラグ----------
    private bool isMove = false;
    private bool isAttack = false;
    private bool isSkill = false;
    private bool isCounter = false;
    private bool isAvoid = false;
    private bool isAnim = false;
    private bool canInput = true;
    //------------------------------

    //---------------------タイル表示関連------------------
    [SerializeField] private GameObject[] displayGrid = null;
    [SerializeField] private GameObject attackGridEffectPrefab = null;
    [SerializeField] private GameObject skillOrbPrefab = null;
    [SerializeField] private GameObject healGridEffectPrefab = null;
    //-----------------------------------------------------

    private void Start()
    {
        Initialize();
    }

    private void FixedUpdate()
    {
        if (!isStart) return;//GameManagerからのスタートが送られるまでは何もしない
        evolution.Check();
        //移動
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPos, SPEED * Time.deltaTime);
        canInput = (transform.localPosition == targetPos) && !isMove && !isAttack && !isAnim && !isSkill && !isCounter;
        if (!canInput) return;
        InputDirection();
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
            StartCoroutine(WaitInput());
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
        IEnumerator WaitInput()
        {
            bool isInput = up || down || left || right;
            for (int i = 0; i < AVOID_INPUT_FRAME; i++)
            {
                if (isInput)
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
                    yield break;
                }
                yield return null;
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
        if (playerGrid[currentX,currentY]) StartCoroutine(Attack());
        if (skillGrid[currentX, currentY]) RemoveSkillGrid();
        if (healGrid[currentX, currentY]) RemoveHealGrid();
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

    public IEnumerator AvoidSuccess()
    {
        isCounter = true;
        yield return new WaitUntil(() => !isAttack);
        enemyManager.StopAttack();
        yield return StartCoroutine(cutIn.CounterAttack());
        AttackEnemy(damageCounterAttack / counterAttackFreq, counterAttackFreq);
        isCounter = false;
        enemyManager.StartAttack();
    }
    //------------------------------------------------------------

    //--------------------------攻撃関連--------------------------
    private IEnumerator Attack()
    {
        evolution.Increase("Attack");
        isAttack = true;
        
        //damage
        if(isEvo) AttackEnemy(damageEvolution / attackFreq, attackFreq);
        else AttackEnemy(damageNormal / attackFreq, attackFreq);
        yield return StartCoroutine(cutIn.Attack(isEvo));
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
        if(currentX == enemyManager.WeakPoint)
        {
            damage = (int)(damage * WEAK_MULTIPLIER);
            SetHealGrid();
        }
        StartCoroutine(enemyManager.Damaged((int)damage, freq));
    }
    //------------------------------------------------------------------

    //------------------------回復関連----------------------------------
    private void SetHealGrid()
    {
        Debug.Log("a");
        if (CheckExistHealGrid()) return; 

        //プレイヤーの現在の位置を取得
        int playerPos = currentX + currentY * 3;
        List<int> numbers = new List<int>();
        //生成可能な位置を追加
        for (int i = 0; i < 9; i++)
        {
            //プレイヤーがいる場所以外を追加
            if (playerPos != i) numbers.Add(i);
        }
        //生成可能な位置からランダムに生成
        int index = Random.Range(0, numbers.Count);
        healGrid[numbers[index] % 3, numbers[index] / 3] = true;
        Instantiate(healGridEffectPrefab, displayGrid[numbers[index]].transform);
    }
    private bool CheckExistHealGrid()
    {
        bool a = false;
        foreach (bool b in healGrid)
        {
            a = a || b;
        }
        return a;
    }
    private void RemoveHealGrid()
    {
        Destroy(displayGrid[currentX + currentY * 3].transform.Find("HealGridEffect(Clone)").gameObject);
        healGrid[currentX, currentY] = false;
    }
    //-------------------------------------------------------------------

    //-------------------------カットイン関連----------------------------
    private IEnumerator WaitAnim(string s)
    {
        isAnim = true;
        enemyManager.StopAttack();
        yield return cutIn.StartCoroutine(s);
        isAnim = false;
        enemyManager.StartAttack();
    }
    //--------------------------------------------------------------

    //--------------------HP関連----------------------
    public void Damaged(int value)
    {
        playerHP.Damaged(value);
    }
    private void Healed()
    {
        //playerHP.Healed();
    }
    //------------------------------------------------
    //--------------------------SKILL-------------------------------
    private void ResetSkillGrid()
    {
        for(int i = 0; i < 9; i++)
        {
            if(skillGrid[i % 3, i / 3])
            {
                skillGrid[i % 3, i / 3] = false;
                Destroy(displayGrid[i].transform.Find("SkillOrb(Clone)").gameObject);
            }
        }
    }
    private void RemoveSkillGrid()
    {
        skillGrid[currentX, currentY] = false;
        Destroy(displayGrid[currentX + currentY * 3].transform.Find("SkillOrb(Clone)").gameObject);
        playerSkill.RemoveTile();
    }
    public void SetSkillGrid(int x, int y)
    {
        skillGrid[x, y] = true;
        Instantiate(skillOrbPrefab, displayGrid[x+y*3].transform);
    }
    public IEnumerator Skill()
    {
        yield return new WaitUntil(() => !isAttack && !isCounter);
        isSkill = true;
        enemyManager.StopAttack();
        yield return StartCoroutine(cutIn.Skill());
        AttackEnemy(damageSkill / skillFreq, skillFreq);
        isSkill = false;
        enemyManager.StartAttack();
    }
    //--------------------------------------------------------------

    //------init--------
    private void Initialize()
    {
        InitTiles();
        SetUI();
        SetAttackTile();
    }

    private void InitTiles()
    {
        for (int i = 0; i < 9; i++)
        {
            playerGrid[i%3, i/3] = false;
            skillGrid[i%3, i/3] = false;
        }
        collisionManager.PlayerMoved(currentX, currentY);
        targetPos = transform.localPosition;
    }

    private void SetAttackTile()
    {
        for (int i = 0; i < 3; i++)
        {
            playerGrid[i, ATTACK_LINE[attackType]] = true;
            Instantiate(attackGridEffectPrefab,displayGrid[i + ATTACK_LINE[attackType] * 3].transform); 
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
