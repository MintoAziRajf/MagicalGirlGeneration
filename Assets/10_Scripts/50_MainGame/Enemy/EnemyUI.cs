using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{
    [SerializeField] private Image hpGaugeDisplay = null;
    [SerializeField] private Image hpGaugeDisplayDelay = null;
    [SerializeField] private Text hpTextDisplay = null;
    [SerializeField] private Text hpTextDisplayShadow = null;
    private int hpMax = 0;
    public int HPMax
    {
        set { hpMax = value; }
    }

    private float delay = 200f;
    private float hpDisplayGauge = 0f;
    private int hpCurrent = 0;
    public int HPCurrent
    {
        set { hpCurrent = value; }
    }

    private void FixedUpdate()
    {
        if(Mathf.Abs(hpCurrent - hpDisplayGauge) >= 1000f)
        {
            hpDisplayGauge = hpCurrent;
        }
        hpDisplayGauge = Mathf.MoveTowards(hpDisplayGauge, hpCurrent, delay * Time.deltaTime);
        
        hpGaugeDisplayDelay.fillAmount = hpDisplayGauge / hpMax;
        hpGaugeDisplay.fillAmount = (float)hpCurrent / hpMax;
        hpTextDisplay.text = hpDisplayGauge.ToString();
        hpTextDisplayShadow.text = hpDisplayGauge.ToString();
    }

    [SerializeField] private float magnitude = 0f;
    [SerializeField] private float duration = 0f;
    [SerializeField] private RectTransform hpBar = null;
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
    }
}
