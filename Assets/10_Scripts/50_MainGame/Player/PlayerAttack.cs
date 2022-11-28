using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : PlayerManager
{
    //----------攻撃関連------------
    private int damageNormal = 0; // 基礎攻撃ダメージ
    public int DamageNormal { set { damageNormal = value; } }
    private int damageEvolution = 0; // 変身後基礎攻撃ダメージ
    public int DamageEvolution { set { damageEvolution = value; } }
    private int attackFreq = 0; // 攻撃回数
    public int AttackFreq { set { attackFreq = value; } }
    private int damageCounterAttack = 0; // カウンターダメージ
    public int DamageCounterAttack { set { damageCounterAttack = value; } }
    private int counterAttackFreq = 0; // カウンター攻撃回数
    public int CounterAttackFreq { set { counterAttackFreq = value; } }
    private int damageSkill = 0; // 必殺技ダメージ
    public int DamageSkill { set { damageSkill = value; } }
    private int skillFreq = 0; // 必殺技攻撃回数
    public int SkillFreq { set { skillFreq = value; } }

    //弱点倍率
    private const float WEAK_MULTIPLIER = 1.5f;

    // 変身中か
    private bool isEvo = false;
    public bool IsEvo { set { isEvo = value; } }

    //攻撃エフェクトを再生するかどうか
    [SerializeField] private bool isEffect = false;

    //------------------------------
    /// <summary>
    /// 攻撃メソッド
    /// </summary>
    public IEnumerator NormalAttack()
    {
        evolution.Increase("Attack"); // 変身ゲージを増やす
        playerController.IsAttack = true; // 通常攻撃中
        StartCoroutine(playerController.Invincible((int)PlayerController.INVINCIBLE.ATTACK));
        AttackSE(); // 効果音
        // 攻撃メソッド
        if (isEvo) AttackEnemy("Evolution");
        else AttackEnemy("Normal");
        yield return StartCoroutine(cutIn.Attack(isEvo)); // 攻撃エフェクト(変身してるかどうか)
        StartCoroutine(playerController.BringBackPlayer()); // 攻撃が終わったら反対のタイルに移動
    }

    /// <summary>
    /// キャラに応じた効果音
    /// </summary>
    private void AttackSE()
    {
        switch (playerController.Type)
        {
            case (int)CHARACTER.RED:
                SoundManager.instance.PlaySE(SoundManager.SE_Type.Attack_Red);
                break;
            case (int)CHARACTER.BLUE:
                SoundManager.instance.PlaySE(SoundManager.SE_Type.Attack_Blue);
                break;
            case (int)CHARACTER.YELLOW:
                SoundManager.instance.PlaySE(SoundManager.SE_Type.Attack_Yellow);
                break;
            default:
                Debug.Log("Error");
                break;
        }
    }

    /// <summary>
    /// 攻撃
    /// </summary>
    /// <param name="s">攻撃の種類</param>
    public void AttackEnemy(string s)
    {
        switch (s)
        {
            case "Normal":
                AttackEnemy(damageNormal / attackFreq, attackFreq); // 通常時
                break;
            case "Evolution":
                AttackEnemy(damageEvolution / attackFreq, attackFreq);// 変身後
                break;
            case "Skill": 
                AttackEnemy(damageSkill / skillFreq, skillFreq); // 必殺技
                break;
            case "Counter":
                AttackEnemy(damageCounterAttack / counterAttackFreq, counterAttackFreq); // カウンター
                break;
        }
    }

    /// <summary>
    /// 攻撃メソッド
    /// </summary>
    /// <param name="damage">ダメージ量</param>
    /// <param name="freq">攻撃回数</param>
    private void AttackEnemy(int damage, int freq)
    {
        if(isEffect) playerController.AttackEffect();
        gameManager.AddScore(damage * freq * (int)SCORE.ATTACK); // スコア追加

        if (playerController.CurrentX == enemyManager.WeakPoint) //弱点かどうか判定
        {
            damage = (int)(damage * WEAK_MULTIPLIER); // ダメージを計算
            playerController.SetHealGrid(); // 回復タイルをセット
            StartCoroutine(enemyManager.Damaged((int)damage, freq, true));
        }
        else StartCoroutine(enemyManager.Damaged((int)damage, freq, false)); // エネミーにダメージを送信
    }
}
