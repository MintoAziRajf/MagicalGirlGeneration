using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameUI : PlayerManager
{
    //------------------クールタイム表示------------------
    //移動クールタイム表示
    [SerializeField] private Image moveDisplay = null; //表示先
    private int moveCurrentTime = 0;　//現在の時間
    public int MoveCurrentTime { set { moveCurrentTime = value; } }　//現在の時間のセッター
    private int moveCooltime = 0;　//クールタイム
    public int MoveCooltime { set { moveCooltime = value; } }　//クールタイムのセッター

    //回避クールタイム表示
    [SerializeField] private Image avoidDisplay = null;
    private int avoidCurrentTime = 0;
    public int AvoidCurrentTime { set { avoidCurrentTime = value; } }
    private int avoidCooltime = 0;
    public int AvoidCooltime { set { avoidCooltime = value; } }

    //スキルクールタイム表示
    [SerializeField] private Image skillDisplay = null;
    private int skillCurrentTime = 0;
    public int SkillCurrentTime { set { skillCurrentTime = value; } }
    private int skillCooltime = 0;
    public int SkillCooltime { set { skillCooltime = value; } }

    //------------------ゲージ表示------------------
    private float displaySpeed = 80f;　//変化のスピード

    //変身ゲージ表示(徐々に)
    [SerializeField] private Image evolutionDisplay = null; //表示先
    private float evolutionCurrentGauge = 0f;　//現在のゲージ
    public float EvolutionCurrentGauge { set { evolutionCurrentGauge = value; } }//現在のゲージのセッター
    private float evolutionDisplayGauge = 0f;　//値を徐々に変化させるための値
    private int evolutionMax = 0;　//ゲージの上限
    public int EvolutionMax { set { evolutionMax = value; } } //上限のセッター

    //HPゲージ表示(一気に)
    [SerializeField] private Image hpDisplay = null;
    private int hpCurrentGauge = 0;
    public int CurrentHP { set { hpCurrentGauge = value; } }
    private float hpDisplayGauge = 0f;　//値を徐々に変化させるための値
    private int hpMax = 0;
    public int MaxHP { set { hpMax = value;} }

    //
    [SerializeField] private Image visual = null; // キャラ見た目表示先
    [SerializeField] private Sprite[] visualSprite = null; // キャラのスプライト
    [SerializeField] private Color[] hpColor = null; // HPのカラー
    //HPの基準
    private const float HIGH = 0.75f;   // 高い
    private const float DEFAULT = 0.5f; // 普通
    private const float LOW = 0.25f;    // 低い
    private enum EMOTE { HIGH, DEFAULT, LOW }

    private int type = -1;

    void Update()
    {
        //毎フレームUIを更新
        DisplayUI();
    }
    private void DisplayUI()
    {
        //クールタイムのパーセントを計算、表示
        moveDisplay.fillAmount = (float)moveCurrentTime / moveCooltime;
        avoidDisplay.fillAmount = (float)avoidCurrentTime / avoidCooltime;
        skillDisplay.fillAmount = (float)skillCurrentTime / skillCooltime;

        //ゲージの計算
        evolutionDisplayGauge = Mathf.MoveTowards(evolutionDisplayGauge, evolutionCurrentGauge, displaySpeed * Time.unscaledDeltaTime);
        hpDisplayGauge = Mathf.MoveTowards(hpDisplayGauge, hpCurrentGauge, displaySpeed * displaySpeed * Time.unscaledDeltaTime);
        //ゲージのパーセントを計算、表示
        hpDisplay.fillAmount = hpDisplayGauge / hpMax;
        evolutionDisplay.fillAmount = evolutionDisplayGauge / evolutionMax;
        DisplayVisual(hpDisplayGauge / hpMax);
    }

    private void DisplayVisual(float per)
    {
        if (type == -1) type = playerController.Type;
        int index = 0;
        if (per > HIGH) index = (int)EMOTE.HIGH;
        else if (per > DEFAULT) index = (int)EMOTE.DEFAULT;
        else if (per < LOW) index = (int)EMOTE.LOW;

        visual.sprite = visualSprite[type * 3 + index];
        hpDisplay.color = hpColor[index];

    }

    [SerializeField] private float magnitude = 0f;
    [SerializeField] private float duration = 0f;
    [SerializeField] private Image damageScreen = null;
    [SerializeField] private Camera targetCamera;

    public IEnumerator DamagedEffect()
    {
        Vector3 pos = targetCamera.transform.position;
        StartCoroutine(DamagedScreen());
        for (int i = 0; i < duration; i++)
        {
            float x = pos.x + Random.Range(-1f, 1f) * magnitude;
            float y = pos.y + Random.Range(-1f, 1f) * magnitude;
            targetCamera.transform.position = new Vector3(x, y, pos.z);
            yield return null;
        }
        targetCamera.transform.position = pos;
    }

    private IEnumerator DamagedScreen()
    {
        Color c = damageScreen.color;
        
        for (int i = 0; i < duration; i++)
        {
            c.a = Mathf.Sin((i+1)* 3f / duration) / 4f;
            damageScreen.color = c;
            yield return null;
        }
        c.a = 0f;
        damageScreen.color = c;
    }
}
