using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameUI : PlayerManager
{
    // クールタイム表示------------------------------------

    //回避クールタイム表示
    [SerializeField] private Image avoidDisplaySprite = null;
    private int avoidCurrentTime = 0;
    public int AvoidCurrentTime { set { avoidCurrentTime = value; } }
    private int avoidCooltime = 300;
    public int AvoidCooltime { set { avoidCooltime = value; } }

    // EPゲージ表示用--------------------------------------
    [SerializeField] private Image evolutionDisplay = null; // 表示先
    private float evolutionCurrentGauge = 0f;　//現在のゲージ
    public float EvolutionCurrentGauge //現在のゲージのセッター
    { 
        set 
        { 
            if (evolutionCurrentGauge < value) StartCoroutine(EvolutionEffect()); // 数値が上昇していた場合エフェクトを再生
            evolutionCurrentGauge = value; // set
        } 
    }
    private float evolutionMax = 0;　//ゲージの上限
    public float EvolutionMax { set { evolutionMax = value; } } //上限のセッター



    // HPゲージ表示用-------------------------------------
    [SerializeField] private Image hpDisplay = null; // 表示先
    private int hpCurrentGauge = -1; // 現在のHP
    public int CurrentHP 
    { 
        set 
        {
            if (hpCurrentGauge == -1)
            {
                hpCurrentGauge = value;
                hpDelayDisplay.fillAmount = (float)hpCurrentGauge / hpMax;
                hpDisplay.fillAmount = (float)hpCurrentGauge / hpMax;
            }
            else if (hpCurrentGauge > value) StartCoroutine(DamageEffect(hpCurrentGauge, value)); // 数値が減少していた場合ダメージエフェクトを再生
            else if (hpCurrentGauge < value) StartCoroutine(HealEffect(value)); // 数値が上昇していた場合ヒールエフェクトを再生
            hpCurrentGauge = value; // set
        } 
    }
    private int hpMax = 0; // HPの上限
    public int MaxHP { set { hpMax = value;} } // set

    // キャラの表情とHPの色を変更する用-------------------
    private enum HP { LOW, MEDIUM, HIGH } // 低い時、高い時
    private int hpColorType = 0; // hpのカラー指定
    public int HPColorType { set { hpColorType = value; } }　//セッター
    private int type = -1; // キャラクターの種類
    [SerializeField] private Image visual = null; // キャラ見た目表示先
    [SerializeField] private Sprite[] visualSprite = null; // キャラのスプライト
    [SerializeField] private Color[] hpColor = null; // HPのカラー
    [SerializeField] private SpriteRenderer gridImage = null; // マス目の2DSprite
    [SerializeField] private Image hpFrame = null; // 画面枠のHP表示

    // エフェクト関連--------------------------------------
    [SerializeField] private float magnitude = 0f; // 揺れの強さ
    [SerializeField] private float duration = 0f; // 効果時間(フレーム)
    [SerializeField] private Image damageScreen = null; // 画面を赤く点滅させるImage
    [SerializeField] private Camera targetCamera; // 揺らすカメラ
    [SerializeField] private Image evolutionEffect = null;  // 表示エフェクト(白発光)
    [SerializeField] private float effectSpeed = 0f; // エフェクトの速さ
    [SerializeField] private Image hpDelayDisplay = null;

    void Update()
    {
        //毎フレームUIを更新
        DisplayUI();
    }

    /// <summary>
    /// プレイヤー関連のUIを表示する
    /// </summary>
    private void DisplayUI()
    {
        //クールタイムのパーセントを計算、表示
        avoidDisplaySprite.fillAmount = (float)avoidCurrentTime / avoidCooltime;
        //ゲージのパーセントを計算、表示
        evolutionDisplay.fillAmount = evolutionCurrentGauge / evolutionMax;
        //キャラの表情と色を変更
        DisplayVisual();
    }

    /// <summary>
    /// キャラの表情と色を変更
    /// </summary>
    private void DisplayVisual()
    {
        if (type == -1) type = playerController.Type; // タイプが初期値の場合タイプをセットする
        HP emote = CheckEmote(hpColorType); // HPの色からHPの状態を判定

        visual.sprite = visualSprite[type * 3 + (int)emote]; // 表情スプライトを変える
        hpDisplay.color = hpColor[hpColorType]; // HPゲージの色を変更
        gridImage.color = hpColor[hpColorType]; // マス目の色を変更(削除するかも)
        hpFrame.color = new Color(hpColor[hpColorType].r, hpColor[hpColorType].g, hpColor[hpColorType].b, hpFrame.color.a); // 画面枠の色を変更
        //Debug.Log(emote); // 現在の状態をログ表示
    }

    /// <summary>
    /// HPの色からHPの状態を判定しHP型として返す
    /// </summary>
    /// <param name="hpType"> HPの色 </param>
    private HP CheckEmote(int hpType)
    {
        HP e;
        hpFrame.enabled = true;
        switch (hpType)
        {
            case (int)HP.LOW:
                e = HP.LOW;
                break;
            case (int)HP.MEDIUM:
                e = HP.MEDIUM;
                break;
            case (int)HP.HIGH:
                e = HP.HIGH;
                hpFrame.enabled = false;
                break;
            default:
                e = HP.HIGH;
                hpFrame.enabled = false;
                break;
        }
        return e;
    }

    /// <summary>
    /// ダメージを受けたときのエフェクト
    /// </summary>
    public IEnumerator DamagedEffect()
    {
        Vector3 pos = targetCamera.transform.position; // 揺らすカメラの元の位置を記録
        StartCoroutine(DamagedScreen()); // 画面を点滅させる
        //揺らす
        for (int i = 0; i < duration; i++)
        {
            float x = pos.x + Random.Range(-1f, 1f) * magnitude; 
            float y = pos.y + Random.Range(-1f, 1f) * magnitude;
            targetCamera.transform.position = new Vector3(x, y, pos.z);
            yield return null;
        }
        //元に戻す
        targetCamera.transform.position = pos;
    }
    /// <summary>
    /// 画面を点滅させるメソッド
    /// </summary>
    private IEnumerator DamagedScreen()
    {
        Color c = damageScreen.color; // 元の色を記録
        // 点滅
        for (int i = 0; i < duration; i++)
        {
            c.a = Mathf.Sin((i+1)* 3f / duration) / 4f; // 透明度を計算
            damageScreen.color = c;
            yield return null;
        }
        // エフェクトが終わったら完全に透明にする
        c.a = 0f;
        damageScreen.color = c;
    }

    /// <summary>
    /// EPが上がった時のエフェクト
    /// </summary>
    private IEnumerator EvolutionEffect()
    {
        Color effectColor = evolutionEffect.color; // 元の色をセット
        // 不透明度が0.5fになるまで上げる
        while (effectColor.a <= 0.5f)
        {
            effectColor.a += effectSpeed * Time.deltaTime;
            evolutionEffect.color = effectColor;
            yield return null;
        }
        effectColor.a = 0.5f;
        evolutionEffect.color = effectColor;

        // 不透明度が0fになるまで下げる
        while (effectColor.a >= 0f)
        {
            effectColor.a -= effectSpeed * Time.deltaTime;
            evolutionEffect.color = effectColor;
            yield return null;
        }
        effectColor.a = 0f;
        evolutionEffect.color = effectColor;
    }

    /// <summary>
    /// HPが減った時のエフェクト
    /// </summary>
    private IEnumerator DamageEffect(int old, int current)
    {
        float hpDelayGauge = (float)old; //値を徐々に変化させるための値
        float displaySpeed = 100f;　//変化のスピード
        hpDelayDisplay.fillAmount = (float)old / hpMax;
        hpDisplay.fillAmount = (float)current / hpMax;
        yield return new WaitForSeconds(5f / 60f); // 5フレーム待機
        while (hpDelayGauge >= (float)current)
        {
            //ゲージの計算
            hpDelayGauge = Mathf.MoveTowards(hpDelayGauge, (float)current, displaySpeed * Time.deltaTime);
            hpDelayDisplay.fillAmount = hpDelayGauge / hpMax;
            yield return null;
        }
        hpDelayGauge = (float)current;
        hpDelayDisplay.fillAmount = hpDelayGauge / hpMax;
    }

    /// <summary>
    /// HPが減った時のエフェクト
    /// </summary>
    private IEnumerator HealEffect(int current)
    {
        yield return new WaitForSeconds(40f / 60f); // 5フレーム待機
        hpDisplay.fillAmount = (float)current / hpMax;
    }
}
