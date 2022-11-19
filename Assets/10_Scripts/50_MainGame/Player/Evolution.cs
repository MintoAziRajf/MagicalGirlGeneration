using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evolution : PlayerManager
{
    private void Start()
    {
        gameUI.EvolutionMax = EVO_MAX;
        gameUI.EvolutionCurrentGauge = EVO_MIN;
    }

    private const int EVO_MAX = 100;   // ゲージ最大値
    private const int EVO_MIN = 0;     // 最小値
    private const int EVO_MOVE = 5;　  // 移動時のゲージ上昇量
    private const int EVO_ATTACK = 10; // 攻撃時のゲージ上昇量
    private const float EVO_TICK = 0.25f;  // 何秒ごとに減らすか
    private float evoTime = 0f; // 秒数記録用
    private int evoGauge = 0;   // 現在のゲージ 
    public int EvoGauge { set { evoGauge = value; } } //Debug用
    private bool isEvo = false; // 変身しているかどうか
    private bool isStart = false; //
    public bool IsStart { set { isStart = value; } }

    [SerializeField] private bool debugMode = false;

    
    private void FixedUpdate()
    {
        if(debugMode)
        {
            Increase(100);
            Check();
        }
        gameUI.EvolutionCurrentGauge = evoGauge;
        if (!isEvo || !isStart) return;
        Decrease();
    }

    public void Check()
    {
        //ゲージがMAX　且つ　変身していないとき
        if (evoGauge == EVO_MAX && !isEvo)
        {
            gameUI.AvoidCurrentTime = AVOID_COOLTIME;
            gameManager.AddScore((int)SCORE.EVOLUTION); // スコア追加
            StartCoroutine(WaitAnim(true));
        }
        //ゲージがMIN　且つ　変身しているとき
        if (evoGauge == EVO_MIN && isEvo)
        {
            gameUI.AvoidCurrentTime = 0;
            playerSkill.ResetGrid();
            StartCoroutine(WaitAnim(false));
        }
    }

    /// <summary>
    /// 変身ゲージを溜める
    /// </summary>
    /// <param name="s">増加量</param>
    public void Increase(string s)
    {
        if (isEvo) return;
        int value = 0;
        if (s == "Attack")
        {
            value = EVO_ATTACK;
        }
        else if(s == "Move")
        {
            value = EVO_MOVE;
        }

        //最大値を上回らないように増加させる
        evoGauge = Mathf.Min(evoGauge + value, EVO_MAX);
    }

    /// <summary>
    /// 変身ゲージを溜める(チュートリアル用)
    /// </summary>
    /// <param name="value">増加量</param>
    public void Increase(int value)
    {
        evoGauge = value;
    }


    /// <summary>
    /// 変身後、毎秒ゲージを減少させる
    /// </summary>
    private void Decrease()
    {
        evoTime += Time.deltaTime;
        //一秒を超えたら
        if (evoTime >= EVO_TICK)
        {
            evoTime -= EVO_TICK;
            //最小値を下回らないように減少させる
            evoGauge = Mathf.Max(evoGauge - 1, EVO_MIN);
        }
    }

    /// <summary>
    /// 変身メソッド
    /// </summary>
    /// <param name="b"> (通常⇒変身)かどうか </param>
    private IEnumerator WaitAnim(bool b)
    {
        //変身を解除
        isEvo = b;
        playerController.IsEvo = b;
        playerHP.IsEvo = b;
        playerAttack.IsEvo = b;
        visualAnim.SetBool("Evolution", b);
        playerController.IsAnim = true;       // アニメーション中
        enemyManager.StopAttack();            // エネミーの攻撃を停止

        StartCoroutine(playerController.Invincible((int)PlayerController.INVINCIBLE.CUTIN)); // カットイン中無敵にする
        SoundManager.instance.PlaySE(SoundManager.SE_Type.Evolution); // 効果音

        if (b) yield return StartCoroutine(cutIn.Evolution());     // 通常⇒変身カットイン
        else  yield return StartCoroutine(cutIn.SolveEvolution()); // 変身⇒通常カットイン

        enemyManager.StartAttack();           // エネミーの攻撃を開始
        playerController.IsAnim = false;      // アニメーション終了
    }
}
