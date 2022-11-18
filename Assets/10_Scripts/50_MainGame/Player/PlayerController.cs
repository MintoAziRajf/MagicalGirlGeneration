using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : PlayerManager
{
    private bool[,] attackGrid = new bool[3, 3];
    private bool[,] skillGrid = new bool[3, 3];
    public bool[,] SkillGrid { set { skillGrid = value; } get { return skillGrid; } }
    private bool[,] healGrid = new bool[3, 3];

    // キャラクターの種類---------------------
    private int type = 0;
    public int Type { set { type = value; visualAnim.SetInteger("Type", type); } get { return type; } }
    private int attackType = 0;
    public int AttackType { set { attackType = value; } }

    //----------移動関連------------
    private int currentX = 1, currentY = 1; // 現在の位置
    public int CurrentPos { get { return ConvertPosition(currentX,currentY); } }
    public int CurrentX { get { return currentX; } }
    public int CurrentY { get { return currentY; } }
    private const int LIMIT_MIN = 0; // 移動制限　
    private const int LIMIT_MAX = 2; // 移動制限　
    private Vector3 targetPos; // 移動先
    private float speed = 200f; // 移動の速さ
    private int moveCooltime = 0; // 移動クールタイム　
    public int MoveCooltime { set { moveCooltime = value; speed = speed / value; } }
    // 移動距離
    private Vector3[] MOVE =
    {
         new Vector3(0f, 3f, 0f), //Up
         new Vector3(3f, 0f, 0f), //Right
         new Vector3(0f, -3f, 0f),//Down
         new Vector3(-3f, 0f, 0f) //Left
    };
    //------------------------------

    // 回復関連 --------------------
    private int healPos = -1; // ヒールオブジェクトの場所 無い場合は-1
    public int HealPos { get { return healPos; } }
    [SerializeField] private GameObject healEffectObj = null; // ヒールオブジェクトを取った時のエフェクト
    private GetEffect healEffect; // ヒールエフェクトスクリプト
    // -----------------------------

    // スキル関連 ------------------
    [SerializeField] private GameObject skillEffectObj = null; // スキルオブジェクトを取った時のエフェクト
    private GetEffect skillEffect; // スキルエフェクトスクリプト

    //----------攻撃関連------------
    private int[] ATTACK_LINE = { 0, 2 };
    private int[] BACK_LINE = { 2, 0 };
    private Vector3[] ATTACK_MOVE =
    {
        new Vector3(0f, -6f, 0f),
        new Vector3(0f, 6f, 0f)
    };
    //------------------------------

    //----------変身関連------------
    private bool isEvo = false;
    public bool IsEvo
    {
        set
        {
            isEvo = value;
            canAvoid = value;
            if (!isTutorial) playerSkill.IsEvo = value;
        }
    }
    //------------------------------

    // 無敵関連-----------------------------------
    private bool isInvincible = false;
    public enum INVINCIBLE { ATTACK = 15, CUTIN = 60, COUNTER = 30, DAMAGE = 20 }
    //-----------ゲームのスタートフラグ----------
    private bool isStart = false;
    public bool IsStart { set { isStart = value; evolution.IsStart = value; } }
    private bool isTutorial = false;
    public bool IsTutorial { set { isTutorial = value; } get { return isTutorial; } }
    //------------------------------------------

    //----------操作フラグ----------
    private bool isMove = false;
    private bool isAttack = false;
    public bool IsAttack { set { isAttack = value; } }
    private bool isBringBack = false;
    private bool isSkill = false;
    private bool isCounter = false;
    private bool isAvoid = false;
    private bool canAvoid = true;
    private bool isAnim = false;
    public bool IsAnim { set { isAnim = value; } }
    private bool canInput = true;
    private bool enemyAlive = false; //エネミーが存在しているかどうか
    public bool EnemyAlive { set { enemyAlive = value; } }
    //------------------------------


    [SerializeField] private GameObject[] displayGrid = null; // エフェクトの表示先
    [SerializeField] private GameObject attackGridEffectPrefab = null; // 攻撃タイルエフェクト
    [SerializeField] private GameObject healOrbPrefab = null; // 回復エフェクト
    [SerializeField] private GameObject skillOrbPrefab = null;

    private void Start()
    {
        Initialize();
        StartCoroutine(StartSerif());
    }

    private void FixedUpdate()
    {
        //移動
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPos, speed * Time.deltaTime);
        evolution.Check();

        canInput = !isAttack && !isAnim && !isSkill && !isCounter && isStart;
        if (!canInput) return;
        InputAvoid();
        if (transform.localPosition != targetPos || isMove || isBringBack) return; // 自分のいる場所が目的地じゃなかったら操作を受け付けない
        InputDirection();
    }
    // 無敵関連--------------------------------------------------
    /// <summary>
    /// 無敵メソッド
    /// </summary>
    /// <param name="time">何フレーム持続させるか</param>
    public IEnumerator Invincible(int time)
    {
        for(int i = 0; i < time; i++)
        {
            isInvincible = true;
            yield return new WaitForSeconds(1f / 60f);
        }
        isInvincible = false;
    }
    //-----------------------------------------------------------

    /// <summary>
    /// プレイヤーの操作
    /// </summary>
    private void InputDirection()
    {
        bool up = Input.GetAxis("Vertical") >= 0.5f && currentY > LIMIT_MIN;
        bool down = Input.GetAxis("Vertical") <= -0.5f && currentY < LIMIT_MAX;
        bool left = Input.GetAxis("Horizontal") <= -0.5f && currentX > LIMIT_MIN;
        bool right = Input.GetAxis("Horizontal") >= 0.5f && currentX < LIMIT_MAX;

        if (up) MoveUp();
        else if (down) MoveDown();
        else if (left) MoveLeft();
        else if (right) MoveRight();
    }
    private void InputAvoid()
    {
        bool avoid = Input.GetButtonDown("Submit") && canAvoid && isEvo;
        if (avoid) StartCoroutine(Avoid());
    }
    //--------------------------移動関連-------------------------
    private void MoveUp()
    {
        currentY--;
        Move((int)DIRECTION.UP);
    }
    private void MoveDown()
    {
        currentY++;
        Move((int)DIRECTION.DOWN);
    }
    private void MoveLeft()
    {
        currentX--;
        Move((int)DIRECTION.LEFT);
    }
    private void MoveRight()
    {
        currentX++;
        Move((int)DIRECTION.RIGHT);
    }

    /// <summary>
    /// 移動メソッド
    /// </summary>
    /// <param name="direction">方向</param>
    private void Move(int direction)
    {
        SoundManager.instance.PlaySE(SoundManager.SE_Type.Move); // SE
        gameManager.AddScore((int)SCORE.MOVE); //スコア追加
        targetPos = targetPos + MOVE[direction]; // 移動先を設定
        evolution.Increase("Move"); // 変身ゲージを増やす
        StartCoroutine(MoveDelay());// 移動クールタイム
        CheckTile(); // 移動先のタイルを判定
    }

    /// <summary>
    /// 移動クールタイム
    /// </summary>
    private IEnumerator MoveDelay()
    {
        isMove = true; //移動中
        for (int i = 0; i < moveCooltime; i++) //クールタイム分待機
        {
            if(i == moveCooltime / 2) collisionManager.PlayerMoved(currentX, currentY); // コリジョンマネージャーに移動したことを伝える
            yield return new WaitForSeconds(1f / 60f); // １フレーム待機
        }
        isMove = false;//移動終了
    }

    /// <summary>
    /// 対応したタイルによってメソッドを呼ぶ
    /// </summary>
    private void CheckTile()
    {
        if (attackGrid[currentX,currentY]) StartCoroutine(playerAttack.NormalAttack()); 
        if (skillGrid[currentX, currentY]) playerSkill.RemoveGrid();       
        if (healGrid[currentX, currentY]) RemoveHealGrid();          
    }

    // 回避関連-----------------------------------------------------------
    /// <summary>
    /// 回避メソッド
    /// </summary>
    private IEnumerator Avoid()
    {
        canAvoid = false; // 回避できなくする
        isAvoid = true;
        visualAnim.SetTrigger("Avoid");
        for (int i = AVOID_TIME; i > 0; i--)
        {
            gameUI.AvoidCurrentTime = i * (AVOID_COOLTIME / AVOID_TIME);
            yield return new WaitForSeconds(1f / 60f);
        }
        isAvoid = false;
        
        gameUI.AvoidCurrentTime = 0; // 回避クールタイムを最低値に
        for (int i = 0; i < AVOID_COOLTIME; i++) //クールタイム分待機
        {
            if (!isEvo)
            {
                gameUI.AvoidCurrentTime = 0;
                yield break;
            }
            gameUI.AvoidCurrentTime = i; // クールタイム
            yield return new WaitForSeconds(1f / 60f);
        }
        gameUI.AvoidCurrentTime = AVOID_COOLTIME; // 回避クールタイムを最大値に
        StartCoroutine(MessageManager.instance.DisplayMessage("スタミナが回復したよ！")); // メッセージ表示
        canAvoid = true; // 回避できるようにする
    }

    /// <summary>
    /// 回避成功
    /// </summary>
    public IEnumerator AvoidSuccess()
    {
        isCounter = true; // カウンター攻撃中
        yield return new WaitUntil(() => !isAttack && isStart); // 通常攻撃を待機
        // 敵が途中で倒されたらメソッド終了
        if (!enemyAlive)
        {
            isCounter = false;
            yield break;
        }
        gameManager.AddScore((int)SCORE.COUNTER); // スコア追加
        Healed(); // 回復
        StartCoroutine(Invincible((int)INVINCIBLE.COUNTER)); // カットイン中無敵にする
        //enemyManager.StopAttack(); // 敵の攻撃を止める
        SoundManager.instance.PlaySE(SoundManager.SE_Type.Counter); // 効果音
        Time.timeScale = 0.5f;
        visualAnim.SetTrigger("Counter");
        yield return StartCoroutine(cutIn.CounterAttack()); // カウンター攻撃のカットイン
        //if (!isSkill) enemyManager.StartAttack(); // 敵の攻撃を開始
        Time.timeScale = 1f;
        playerAttack.AttackEnemy("Counter"); // 攻撃メソッド
        isCounter = false; // カウンター攻撃終了
    }

    // 攻撃関連----------------------------------------------------------------------------------------
    /// <summary>
    /// 攻撃が終わったら反対のタイルに移動
    /// </summary>
    public IEnumerator BringBackPlayer()
    {
        isAttack = false;
        isBringBack = true;
        yield return new WaitUntil(() => !isCounter && !isSkill);
        //反対のタイルに移動させる
        currentY = BACK_LINE[attackType];
        StartCoroutine(BringBackCollision());
        targetPos = targetPos + ATTACK_MOVE[attackType];
        SoundManager.instance.PlaySE(SoundManager.SE_Type.Move);
        // 移動先のタイルを判定
        CheckTile();
    }
    private IEnumerator BringBackCollision()
    {
        yield return new WaitForSeconds((float)moveCooltime / 60f);
        collisionManager.PlayerMoved(currentX, currentY);
        isBringBack = false;
    }
    //------------------------------------------------------------------

    // 回復関連--------------------------------------------------------------------
    public void SetHealGrid()
    {
        if (CheckExistHealGrid()) return; // 回復タイルがある場合は何もしない

        //プレイヤーの現在の位置を取得
        int playerPos = currentX + currentY * 3;
        List<int> numbers = new List<int>();
        //生成可能な位置を追加
        for (int i = 0; i < 9; i++)
        {
            //プレイヤーがいる場所とスキルオーブがある場所以外を追加
            if (playerPos != i && skillGrid[i%3,i/3] == false) numbers.Add(i);
        }
        //生成可能な位置からランダムに生成
        int index = Random.Range(0, numbers.Count);
        healPos = numbers[index];
        healGrid[numbers[index] % 3, numbers[index] / 3] = true;
        Instantiate(healOrbPrefab, displayGrid[numbers[index]].transform);
    }

    /// <summary>
    ///  回復タイルがあるかチェック
    /// </summary>
    private bool CheckExistHealGrid()
    {
        bool a = false;
        foreach (bool b in healGrid)
        {
            a = a || b;
        }
        return a;
    }

    /// <summary>
    /// 回復タイルが踏まれたら回復タイルを削除し、プレイヤーを回復
    /// </summary>
    private void RemoveHealGrid()
    {
        Destroy(displayGrid[currentX + currentY * 3].transform.Find("HealOrb(Clone)").gameObject); // 回復タイルの見た目を削除
        healGrid[currentX, currentY] = false; // 回復タイルのタイルを削除
        healPos = -1;
        Healed(); //回復メソッド
    }
    //-------------------------------------------------------------------

    //--------------------HP関連-----------------------------------
    /// <summary>
    /// ダメージメソッド
    /// </summary>
    /// <param name="value">ダメージ量</param>
    public IEnumerator Damaged(int value)
    {
        if (isAvoid)
        {
            StartCoroutine(AvoidSuccess());
            isAvoid = false;
            yield break;
        }
        yield return new WaitForSeconds(1f / 60f);
        if (isInvincible) yield break;
        playerHP.Damaged(value);
        SoundManager.instance.PlaySE(SoundManager.SE_Type.Enemy_Damaged);
        StartCoroutine(Invincible((int)INVINCIBLE.DAMAGE));
    }
    /// <summary>
    /// 回復メソッド
    /// </summary>
    private void Healed()
    {
        playerHP.Heal();
        healEffectObj.transform.position = displayGrid[currentX + currentY * 3].transform.position;
        healEffect.StartEffect();
        SoundManager.instance.PlaySE(SoundManager.SE_Type.HealOrb);
        gameManager.AddScore((int)SCORE.HEAL);
    }
    //--------------------------------------------------------------

    //--------------------------SKILL-------------------------------
    public void RemoveSkillOrb(int index)
    {
        Destroy(displayGrid[index].transform.Find("SkillOrb(Clone)").gameObject); // スキルオーブのエフェクトを削除
        skillEffectObj.transform.position = displayGrid[playerController.CurrentPos].transform.position;
        skillEffect.StartEffect(); // スキルオーブの取得エフェクトを再生
    }
    public void InstanceSkillOrb(int x, int y)
    {
        SkillGrid[x, y] = true;
        Instantiate(skillOrbPrefab, displayGrid[ConvertPosition(x,y)].transform);
    }
    public IEnumerator Skill()
    {
        isSkill = true; // スキル開始
       
        yield return new WaitUntil(() => !isAttack && !isCounter); // 通常攻撃とカウンターが終わるのを待つ
        // 敵が途中で倒されたらメソッド終了
        if (!enemyAlive)
        {
            isSkill = false;
            yield break; 
        }
        gameManager.AddScore((int)SCORE.SKILL); // スコア追加
        StartCoroutine(Invincible((int)INVINCIBLE.CUTIN)); // カットイン中無敵にする
        enemyManager.StopAttack(); // 敵の攻撃を止める
        yield return StartCoroutine(cutIn.Skill()); // スキルのカットインが終わるのを待つ
        enemyManager.StartAttack(); // 敵の攻撃を再開
        playerSkill.SkillSE(); // 効果音
        playerAttack.AttackEnemy("Skill"); // ダメージを与える
        isSkill = false; //スキル終了
    }
    //--------------------------------------------------------------

    //------init--------
    private void Initialize()
    {
        InitTiles();
        skillEffect = skillEffectObj.GetComponent<GetEffect>();
        healEffect = healEffectObj.GetComponent<GetEffect>();
        if (isTutorial) return;
        SetUI();
        SetAttackTile();
    }

    private void InitTiles()
    {
        for (int i = 0; i < 9; i++)
        {
            attackGrid[i%3, i/3] = false;
            skillGrid[i%3, i/3] = false;
        }
        collisionManager.PlayerMoved(currentX, currentY);
        targetPos = transform.localPosition;
    }

    private void SetAttackTile()
    {
        for (int i = 0; i < 3; i++)
        {
            attackGrid[i, ATTACK_LINE[attackType]] = true;
            Instantiate(attackGridEffectPrefab,displayGrid[i + ATTACK_LINE[attackType] * 3].transform); 
        }
    }

    private void SetUI()
    {
        gameUI.AvoidCooltime = AVOID_COOLTIME;
    }
    private IEnumerator StartSerif()
    {
        while (!this.isStart) yield return null;
        StartCoroutine(MessageManager.instance.DisplayMessage("さぁ世界を救いに行こう！"));
    }
    //--------------------------------------------------------------------------------

    // チュートリアル関連 ------------------------------------------------------------
    private GameObject tutorialAttack;
    public IEnumerator TutorialMove()
    {
        yield return new WaitUntil(() => (Input.GetAxisRaw("Horizontal") == 1f));
        MoveRight();
        yield return StartCoroutine(MoveDelay());

        //次のチュートリアルの準備
        attackGrid[2, ATTACK_LINE[attackType]] = true;
        tutorialAttack = Instantiate(attackGridEffectPrefab, displayGrid[2 + ATTACK_LINE[attackType] * 3].transform);
    }
    public IEnumerator TutorialAttack()
    {
        yield return new WaitUntil(() => (Input.GetAxisRaw("Vertical") == ((float)BACK_LINE[attackType] - 1f)));
        if(attackType == 0) MoveUp();
        else MoveDown();
        attackGrid[2, ATTACK_LINE[attackType]] = false;
        Destroy(tutorialAttack);
        evolution.Increase(99);
    }
    public IEnumerator TutorialEvolution()
    {
        evolution.Increase(100);
        isAnim = true;
        while (isAnim)
        {
            yield return null;
        }
    }
    public IEnumerator TutorialCounter()
    {
        yield return StartCoroutine(enemyManager.Tutorial(currentX, currentY));
        bool tutorialAvoid = false;
        while (!tutorialAvoid)
        {
            yield return new WaitUntil(() => (Input.GetButtonDown("Submit")));
            for(int i = 0; i < AVOID_INPUT_FRAME; i++)
            {
                if (Input.GetAxisRaw("Horizontal") == -1f) tutorialAvoid = true;
                yield return null;
            }
        }
        enemyManager.TutorialEnd(currentX, currentY);
        MoveLeft();
        yield return StartCoroutine(cutIn.CounterAttack());
        playerAttack.AttackEnemy("Counter");
    }
    public IEnumerator TutorialSkill()
    {
        playerSkill.IsEvo = true;
        playerSkill.SetTime = 999;
        while (!isSkill)
        {
            if(!isMove) InputDirection();
            yield return null;
        }
        for(int i = 0; i < 180; i++)
        {
            yield return null;
        }
    }
    //--------------------------------------------------------------------------

    // Debug -------------------------------------------------------------------
    bool debugInvincible = false;
    bool debugCooltime = false;
    public void SetInvincible(bool b) => debugInvincible = b;
    public void SetCooltime(bool b) => debugCooltime = b;
    public void SetEvolution(bool b)
    {
        if (b) evolution.EvoGauge = 99;
        else evolution.EvoGauge = 0;
    }

    public void SetPlayerHP(bool b)
    {
        if (b) playerHP.SetHP(9999);
        else playerHP.SetHP(1);
    }

    public void SetEnemyHP(bool b)
    {
        if (b) enemyManager.HPCurrent = 99999;
        else enemyManager.HPCurrent = 1;
    }
    private void Update()
    {
        if (debugInvincible) isInvincible = true;
        if (debugCooltime)
        {
            canAvoid = true;
            playerSkill.SetTime = 999;
        }
    }
}
