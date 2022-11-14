using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{
    [SerializeField] private Image hpGaugeDisplay = null;
    [SerializeField] private Text hpTextDisplay = null;
    private int hpMax = 0;
    public int HPMax
    {
        set { hpMax = value; isFirst = false; }
    }

    private float delay = 10f;
    private float hpDisplayGauge = 0f;
    private int hpCurrent = 0;
    public int HPCurrent
    {
        set { hpCurrent = value; }
    }
    private bool isFirst = true;

    [SerializeField] private Sprite[] hpGaugeSprite = null;
    private const float HIGH = 0.5f;
    private const float LOW = 0.2f;
    private enum GAUGE { HIGH, MEDIUM,LOW }

    //弱点表示用
    [SerializeField] private GameObject[] weakIcon = null;

    private void Update()
    {
        if(Mathf.Abs(hpCurrent - hpDisplayGauge) >= 1000f)
        {
            hpDisplayGauge = hpCurrent;
        }
        hpDisplayGauge = Mathf.MoveTowards(hpDisplayGauge, hpCurrent, delay);
        
        hpGaugeDisplay.fillAmount = hpDisplayGauge / hpMax;

        //hpに応じてゲージのカラーを変更
        if ((hpDisplayGauge / hpMax) > HIGH) hpGaugeDisplay.sprite = hpGaugeSprite[(int)GAUGE.HIGH];
        else if ((hpDisplayGauge / hpMax) > LOW) hpGaugeDisplay.sprite = hpGaugeSprite[(int)GAUGE.MEDIUM];
        else hpGaugeDisplay.sprite = hpGaugeSprite[(int)GAUGE.LOW];

        //エネミーのHPが低くなったらメッセージを送る
        if ((hpDisplayGauge / hpMax) <= LOW && isFirst)
        {
            StartCoroutine(MessageManager.instance.DisplayMessage("あと少しだ頑張れ！"));
            isFirst = false;
        }
        hpTextDisplay.text = hpDisplayGauge.ToString("F0");
    }

    [SerializeField] private float magnitude = 0f;
    [SerializeField] private float duration = 0f;
    [SerializeField] private RectTransform hpBar = null;

    /// <summary>
    /// ダメージを受けたときにUIを揺らすエフェクト
    /// </summary>
    public IEnumerator DamagedEffect()
    {
        Vector3 pos = hpBar.anchoredPosition;

        for (int i = 0; i < duration; i++)
        {
            float x = pos.x + Random.Range(-1f, 1f) * magnitude;
            float y = pos.y + Random.Range(-1f, 1f) * magnitude;

            hpBar.anchoredPosition = new Vector3(x, y, pos.z);
            yield return null;
        }

        hpBar.anchoredPosition = pos;
    }

    /// <summary>
    /// 弱点を表示する
    /// </summary>
    /// <param name="index">左から何番目か</param>
    public void DisplayWeakIcon(int index)
    {
        for(int i = 0; i < weakIcon.Length; i++)
        {
            if(i == index) weakIcon[index].SetActive(true);
            else weakIcon[i].SetActive(false);
        }
    }
}
