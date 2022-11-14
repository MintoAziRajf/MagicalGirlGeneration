using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameUI : PlayerManager
{
    //------------------クールタイム表示------------------

    //回避クールタイム表示
    [SerializeField] private Image avoidDisplay = null;
    private int avoidCurrentTime = 0;
    public int AvoidCurrentTime { set { avoidCurrentTime = value; } }
    private int avoidCooltime = 0;
    public int AvoidCooltime { set { avoidCooltime = value; } }

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
    private enum EMOTE { LOW, MEDIUM, HIGH }
    private int hpColorType = 0;
    public int HPColorType { set { hpColorType = value; } }

    private int type = -1;

    void Update()
    {
        //毎フレームUIを更新
        DisplayUI();
    }
    private void DisplayUI()
    {
        //クールタイムのパーセントを計算、表示
        avoidDisplay.fillAmount = (float)avoidCurrentTime / avoidCooltime;

        //ゲージの計算
        evolutionDisplayGauge = Mathf.MoveTowards(evolutionDisplayGauge, evolutionCurrentGauge, displaySpeed * Time.unscaledDeltaTime);
        hpDisplayGauge = Mathf.MoveTowards(hpDisplayGauge, hpCurrentGauge, displaySpeed * displaySpeed * Time.unscaledDeltaTime);
        //ゲージのパーセントを計算、表示
        hpDisplay.fillAmount = hpDisplayGauge / hpMax;
        evolutionDisplay.fillAmount = evolutionDisplayGauge / evolutionMax;
        DisplayVisual();
    }

    private void DisplayVisual()
    {
        if (type == -1) type = playerController.Type;
        EMOTE emote = CheckEmote(hpColorType);
        
        visual.sprite = visualSprite[type * 3 + (int)emote];
        hpDisplay.color = hpColor[hpColorType];

    }

    private EMOTE CheckEmote(int hpType)
    {
        EMOTE e;
        switch (hpType)
        {
            case (int)EMOTE.MEDIUM:
                e = EMOTE.MEDIUM;
                break;
            case (int)EMOTE.LOW:
                e = EMOTE.LOW;
                break;
            case (int)EMOTE.HIGH:
                e = EMOTE.HIGH;
                break;
            default:
                e = EMOTE.HIGH;
                break;
        }
        return e;
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
