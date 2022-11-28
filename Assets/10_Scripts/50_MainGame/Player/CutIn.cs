using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutIn : PlayerManager
{
    [SerializeField] GameObject cutInObj = null; // カットインオブジェクト
    [SerializeField] GameObject damageEffect = null; // プレイヤーの攻撃エフェクト
    private Animator cutInAnim; // カットインアニメーター
    private Animator damageAnim; // 攻撃エフェクトアニメーター
    private const int ANIM_COOLTIME = 60; // カットインの時間
    private const int COUNTER_COOLTIME = 30; // カウンターエフェクトの時間
    private const int ATTACK_COOLTIME = 15; // 攻撃エフェクトの時間
    private const float ONE_FRAME = 1f / 60f; // 1フレームを1/60秒に定義

    private void Start()
    {
        cutInAnim = cutInObj.GetComponent<Animator>();
        damageAnim = damageEffect.GetComponent<Animator>();
        damageAnim.SetInteger("Type", gameManager.Type);
        cutInAnim.SetInteger("Type", gameManager.Type);
    }
    
    /// <summary>
    /// 通常攻撃エフェクト
    /// </summary>
    /// <param name="b">変身中か</param>
    public IEnumerator Attack(bool b)
    {
        if (b) damageAnim.SetTrigger("EvoAttack");
        else damageAnim.SetTrigger("Attack");

        yield return StartCoroutine(WaitAnim(ATTACK_COOLTIME)); // エフェクトが終了するまで待機
    }

    /// <summary>
    /// アニメーションが終了するまで待機
    /// </summary>
    /// <param name="value">何フレーム待機するか</param>
    private IEnumerator WaitAnim(int value)
    {
        for (int i = 0; i < value; i++)
        {
            yield return new WaitForSeconds(ONE_FRAME);
        }
    }
    /// <summary>
    /// カウンター攻撃
    /// </summary>
    public IEnumerator CounterAttack()
    {
        damageAnim.SetTrigger("Counter"); // アニメーション開始
        yield return StartCoroutine(WaitAnim(COUNTER_COOLTIME)); // 終了待機
        
        StartCoroutine(MessageManager.instance.DisplayMessage("ナイスカウンター！")); // ゲームガイド表示
    }
    /// <summary>
    /// 必殺技
    /// </summary> 
    public IEnumerator Skill()
    {
        cutInAnim.SetTrigger("Skill"); // カットイン
        yield return StartCoroutine(WaitAnim(ANIM_COOLTIME)); // 終了待機
        damageAnim.SetTrigger("Skill"); // ダメージエフェクト
        StartCoroutine(MessageManager.instance.DisplayMessage("ソウルバースト！\n決まったね！")); // ゲームガイド
    }
    /// <summary>
    /// 変身
    /// </summary>
    public IEnumerator Evolution()
    {
        cutInAnim.SetTrigger("Evolution");
        StartCoroutine(MessageManager.instance.DisplayMessage("さぁ、変身だ！"));
        yield return StartCoroutine(WaitAnim(ANIM_COOLTIME));
    }
    /// <summary>
    /// 変身解除
    /// </summary>
    public IEnumerator SolveEvolution()
    {
        cutInAnim.SetTrigger("SolveEvolution");
        yield return StartCoroutine(WaitAnim(ANIM_COOLTIME));
    }
}
