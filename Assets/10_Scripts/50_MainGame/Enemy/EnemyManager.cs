using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class EnemyManager : MonoBehaviour
{
    public bool isDebug = false;

    EnemySkillList enemySkillList; //攻撃スキルを呼び出す用
    EnemyUI enemyUI; //エネミーに関するUI表示
    GameManager gameManager;
    PlayerController playerController; 

    private IEnumerator attackLoop;  //途中停止用に呼び出すコルーチンの保存先
    private IEnumerator attack;      //

    [SerializeField] private GameObject enemyUIObj = null;　　//UIの表示先(親)
    [SerializeField] private GameObject damageUIPrefab = null;//ダメージ表記のプレファブ
    [SerializeField] private TextAsset[] enemyCSV = null;//エネミーの情報が書いてあるCSV
    List<string[]> enemyData = new List<string[]>();//エネミーの情報

    [SerializeField] private SpriteRenderer enemyVisual = null;
    [SerializeField] private SpriteRenderer defeatEnemy = null;
    [SerializeField] private Image defeatScreen = null;
    [SerializeField] private Sprite[] enemyVisuals = null;

    private int currentEnemy = 0; //現在の敵
    private bool isAlive = false; //生きているかどうか
    private bool isStart = false; //
    public bool IsStart { set { isStart = value; } }

    //元の位置
    Vector3 originPosition;
    private const int DAMAGE_DELAY = 2; //連続ダメージの間にかけるディレイ(フレーム)
    [SerializeField] private float magnitude = 0f;
    [SerializeField] private float duration = 0f;
    [SerializeField] private int interval = 0;

    //攻撃に使う変数
    private int currentAttack = 0;  //現在の攻撃
    private int type = 0; //攻撃の種類
    private int x = 0;　//攻撃の呼び出し先座標 : X
    private int y = 0;　//攻撃の呼び出し先座標 : Y
    private int damage = 0; //攻撃のダメージ
    private int cooltime = 0; //次の攻撃までのクールタイム(フレーム)

    //CSVの列情報
    private enum DATA
    {
        TYPE,
        X,
        Y,
        DAMAGE,
        COOLTIME,
        HP
    }

    //HP
    private int hpMax = 10000; //最大HP
    private int hpMin = 0; //最低HP(死亡するHP)
    private int hpCurrent = 10000; //現在のHP
    public int HPCurrent { set { hpCurrent = Mathf.Min(value, hpMax); enemyUI.HPCurrent = hpCurrent; } } // Debug用

    //弱点
    private int weakPoint = 4;
    public int WeakPoint { get { return weakPoint; } }

    private void Awake()
    {
        originPosition = this.gameObject.transform.localPosition; // 元の位置を取得
        
        enemySkillList = GetComponent<EnemySkillList>();
        enemyUI = this.GetComponent<EnemyUI>();
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

        //エネミー情報をロード
        LoadEnemy();
    }

    /// <summary>
    /// 敵の攻撃パターンとステータスをロード
    /// </summary>
    private void LoadEnemy()
    {
        // 以前のデータをクリア
        enemyData.Clear();
        // 敵のデータをロード
        StringReader reader = new StringReader(enemyCSV[currentEnemy].text); 
        string line = reader.ReadLine(); //見出し行をスキップする
        while (reader.Peek() != -1) // reader.Peekが-1になるまで
        {
            line = reader.ReadLine(); // 一行ずつ読み込み
            enemyData.Add(line.Split(',')); // , 区切りでリストに追加
        }
        reader.Close();

        SetEnemy(); //ロードした情報をセット
    }

    /// <summary>
    /// ロードした情報をセットする
    /// </summary>
    private void SetEnemy()
    {
        //攻撃をリセット
        currentAttack = 0;

        //見た目をセット
        enemyVisual.sprite = enemyVisuals[currentEnemy];
        defeatEnemy.GetComponent<SpriteMask>().sprite = enemyVisuals[currentEnemy];

        isAlive = true;　//生存情報をセット
        playerController.EnemyAlive = true;
        //HPをセット
        hpMax = int.Parse(enemyData[0][(int)DATA.HP]); 
        hpCurrent = hpMax; 
        //UIにHP情報をセット
        enemyUI.HPMax = hpMax; 
        enemyUI.HPCurrent = hpCurrent;
        StartAttack();
        UpdateWeakPoint();
    }

    /// <summary>
    /// ダメージメソッド
    /// UIを揺らす⇒ダメージ×回数分HPを減らす⇒死亡判定⇒弱点をリセット
    /// </summary>
    /// <param name="value">ダメージ</param>
    /// <param name="freq">回数</param>
    /// <param name="isWeak">弱点にヒットしたかどうか</param>
    public IEnumerator Damaged(int value, int freq, bool isWeak)
    {
        StartCoroutine(enemyUI.DamagedEffect());
        StartCoroutine(DamagedShake());
        StartCoroutine(DamagedFlash());
        float y = 350f; //ダメージ表記の高さを設定する
        if (isWeak) SoundManager.instance.PlaySE(SoundManager.SE_Type.Enemy_WeakPoint);
        else SoundManager.instance.PlaySE(SoundManager.SE_Type.Enemy_Damaged);
        for (int i = 0; i < freq; i++)
        {
            GameObject damage = Instantiate(damageUIPrefab, enemyUIObj.transform); //ダメージ表記を生成
            damage.GetComponent<EnemyDamageUI>().Damaged(value, y, isWeak); //生成したダメージ表記にダメージと表示する高さを送る
            hpCurrent -= value;//ダメージ分HPを減らす

            Destroy(damage, 1f); //ダメージ表記を生成してから1秒後に削除
            //連続したダメージ表記にはディレイをかける
            for(int j = 0; j < DAMAGE_DELAY; j++)
            {
                yield return null;
            }
            //ダメージ表記が重ならないように高さを回数に応じて調整
            y -= 250f/freq;
        }
        CheckHP();
        UpdateWeakPoint();
    }

    /// <summary>
    /// 弱点を更新
    /// </summary>
    private void UpdateWeakPoint()
    {
        if (!isStart) return; // ゲームスタートする前は更新しない
        //弱点をセット
        weakPoint = Random.Range(0, 3);
        enemyUI.DisplayWeakIcon(weakPoint); // 弱点を表示
    }

    /// <summary>
    /// ダメージを受けたときに点滅させる
    /// </summary>
    private IEnumerator DamagedFlash()
    {
        //durationフレームの間 intervalフレームの間隔で切り替え
        for(int i = 0; i < duration; i++)
        {
            if(i % interval == 0)
            {
                enemyVisual.enabled = !enemyVisual.enabled;
            }
            yield return null;
        }
        enemyVisual.enabled = true;
    }
    /// <summary>
    /// ダメージを受けたときに振動させる
    /// </summary>
    private IEnumerator DamagedShake()
    {
        for (int i = 0; i < duration; i++)
        {
            float x = originPosition.x + Random.Range(-1f, 1f) * magnitude;
            float y = originPosition.y + Random.Range(-1f, 1f) * magnitude;

            this.gameObject.transform.localPosition = new Vector3(x, y, originPosition.z);
            yield return null;
        }
        this.gameObject.transform.localPosition = originPosition;
    }


    /// <summary>
    /// 生きてるかどうかの判定
    /// </summary>
    private void CheckHP()
    {
        //HPが0以下なら0に調整
        if (hpCurrent < hpMin)
        {
            hpCurrent = hpMin;
        }
        enemyUI.HPCurrent = hpCurrent; //チェック後のHPをUIにセット
        if (!isAlive) return;
        //HPが0なら現在のエネミーを消滅させる
        if (hpCurrent == hpMin)
        {
            DefeatEnemy();
        }
    }

    private IEnumerator AttackLoop()
    {
        yield return new WaitUntil(() => isStart);
        yield return new WaitForSeconds(1f);
        while (isAlive)//生存情報がTRUEの間攻撃
        {
            attack = Attack();
            yield return StartCoroutine(attack); //攻撃の呼び出し,終了次第ループ
        }
        yield break;
    }

    /// <summary>
    /// enemyDataにある攻撃パターンを全て実行
    /// </summary>
    private IEnumerator Attack()
    {
        for (int i = currentAttack; i < enemyData.Count; i++)
        {
            currentAttack++; // 中断された時に次のところから再開するようにカウントを進める
            //攻撃情報をセット
            type = int.Parse(enemyData[i][(int)DATA.TYPE]);
            x = int.Parse(enemyData[i][(int)DATA.X]);
            y = int.Parse(enemyData[i][(int)DATA.Y]);
            damage = int.Parse(enemyData[i][(int)DATA.DAMAGE]);
            cooltime = int.Parse(enemyData[i][(int)DATA.COOLTIME]);

            //攻撃
            yield return StartCoroutine(enemySkillList.Attack(type, x, y, damage));
            //次の攻撃までのクールタイム
            for (int j = 0; j < cooltime; j++)
            {
                yield return null;
            }
        }
        attack = null;
        currentAttack = 0;
    }

    /// <summary>
    /// 攻撃を開始(PlayerControllerとGamaManagerから呼ばれます)
    /// </summary>
    public void StartAttack()
    {
        if (attackLoop != null) return;
        attackLoop = AttackLoop();
        StartCoroutine(attackLoop);
        Debug.Log("敵の攻撃を開始しました");
    }

    /// <summary>
    /// 攻撃を中止(PlayerControllerとGamaManager呼ばれます)
    /// </summary>
    public void StopAttack()
    {
        enemySkillList.Stop();
        if (attackLoop != null) StopCoroutine(attackLoop);
        if (attack != null) StopCoroutine(attack);
        Debug.Log("敵の攻撃を中止しました");
        attackLoop = null;
        attack = null;
    }

    /// <summary>
    /// エネミーが倒された時の処理
    /// </summary>
    private void DefeatEnemy()
    {
        isAlive = false; //生存情報をセット
        playerController.EnemyAlive = false;
        //攻撃ループと現在の攻撃を停止
        StopAttack();
        StartCoroutine(DefeatAnimation());
    }
    
    /// <summary>
    /// 敵の撃破演出(敵の点滅⇒フェードアウト⇒敵のロード⇒フェードイン)
    /// </summary>
    private IEnumerator DefeatAnimation()
    {
        gameManager.StopGame(); // ゲームを停止
        // 1秒間待機
        for (int i = 0; i < 60; i++)
        {
            yield return null;
        }
        Time.timeScale = 0f; // 時間停止
        
        Color c = new Color(0f, 0f, 0f, 0f);
        int flashDuration = 30; // 点滅に要するフレーム数
        int flashTimes = 3; // 敵キャラの点滅回数
        float unitAlpha = 1f / flashDuration; // 一度に変化する不透明度の値
        Animator anim = defeatScreen.GetComponent<Animator>(); // 撃破スクリーンのアニメーターを取得
        // 不透明度の初期化
        defeatEnemy.color = c;
        defeatScreen.color = c;
        // 敵の点滅
        for (int i = 0; i < flashTimes; i++)
        {
            for(int j = 0; j < flashDuration; j++)
            {
                c.a += unitAlpha;
                defeatEnemy.color = c;
                yield return null;
            }
            for (int j = 0; j < flashDuration; j++)
            {
                c.a -= unitAlpha;
                defeatEnemy.color = c;
                yield return null;
            }
        }
        currentEnemy++; // 敵のカウントを進める
        gameManager.AddScore(300000); // スコア追加
        if (currentEnemy == enemyCSV.Length) yield return StartCoroutine(EndGame()); // 最後の敵だった場合　メインゲームを終了させる
        //フェードアウト
        for (int i = 0; i < flashDuration; i++)
        {
            c.a += unitAlpha;
            defeatScreen.color = c;
            yield return null;
        }
        //現在の敵に応じてBGMを変更
        if (currentEnemy == 1) SoundManager.instance.PlayBGM(SoundManager.BGM_Type.MainGame_00);
        else if (currentEnemy == 2) SoundManager.instance.PlayBGM(SoundManager.BGM_Type.MainGame_02);
        else if (currentEnemy == enemyCSV.Length) yield break;
        anim.SetTrigger("Transition"); // 撃破アニメーションを再生
        SoundManager.instance.PlaySE(SoundManager.SE_Type.Warning); // 効果音
        // 撃破アニメーション分待機
        for (int i = 0; i < 255; i++)
        {
            yield return null;
        }
        //次のエネミーをロード
        LoadEnemy();
        //フェードイン
        for (int i = 0; i < flashDuration; i++)
        {
            c.a -= unitAlpha;
            defeatScreen.color = c;
            yield return null;
        }
        //時間を戻す
        Time.timeScale = 1f;
        gameManager.ResumeGame(); // ゲーム再開
    }

    /// <summary>
    /// メインゲームを終了させる
    /// </summary>
    private IEnumerator EndGame()
    {
        yield return StartCoroutine(EnemyExplosion()); // 撃破エフェクト(爆発)
        StartCoroutine(MessageManager.instance.DisplayMessage("よく世界を救ったね！")); // メッセージ表示
        StartCoroutine(gameManager.GameClear()); // ゲームクリア
    }

    [SerializeField] private GameObject explosionPrefab = null; // 爆発エフェクト
    [SerializeField] private Transform[] explosionPosition = null; // エフェクトを再生する場所
    private IEnumerator EnemyExplosion()
    {
        SoundManager.instance.PlaySE(SoundManager.SE_Type.Explosion); // 効果音
        // 指定回数エフェクトを再生
        for (int i = 0; i < explosionPosition.Length; i++)
        {
            Instantiate(explosionPrefab,explosionPosition[i]);
            yield return new WaitForSecondsRealtime(0.1f);
        }
    }

    //------------チュートリアル関連----------------
    public IEnumerator TutorialAttack()
    {
        yield return StartCoroutine(enemySkillList.TutorialAttackStart());
    }
    public IEnumerator TutorialCounter()
    {
        yield return StartCoroutine(enemySkillList.TutorialCounterStart());
    }
    public IEnumerator TutorialCounterEnd()
    {
        yield return StartCoroutine(enemySkillList.TutorialCounterEnd());
    }
}
