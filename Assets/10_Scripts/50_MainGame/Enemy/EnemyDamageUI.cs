using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyDamageUI : MonoBehaviour
{
    private Text onesText;
    private Text tensText;
    private Text hundredsText;
    private Text thousandsText;
    private GameObject onesObj;
    private GameObject tensObj;
    private GameObject hundredsObj;
    private GameObject thousandsObj;
    [SerializeField] private GameObject weakObj = null;

    private const int FADE_COOLTIME = 5;
    private const float FADE_TIME = 5f;
    private float defaultScale = 1f;
    private void SetObject()
    {
        onesObj = this.transform.Find("Ones").gameObject;
        onesText = onesObj.GetComponent<Text>();
        tensObj = this.transform.Find("Tens").gameObject;
        tensText = tensObj.GetComponent<Text>();
        hundredsObj = this.transform.Find("Hundreds").gameObject;
        hundredsText = hundredsObj.GetComponent<Text>();
        thousandsObj = this.transform.Find("Thousands").gameObject;
        thousandsText = thousandsObj.GetComponent<Text>();
    }

    public void Damaged(int value, float y, bool isWeak)
    {
        SetObject();
        weakObj.SetActive(isWeak);
        int ones = value % 10;
        int tens = (value / 10) % 10;
        int hundreds = (value / 100) % 10;
        int thousands = value / 1000;

        onesText.text = ones.ToString("0");
        tensText.text = tens.ToString("0");
        if (hundreds != 0)
        {
            defaultScale = 1.3f;
            hundredsText.text = hundreds.ToString("0");
        }
        else hundredsText.text = " ";
        if(thousands != 0)
        {
            defaultScale = 1.5f;
            thousandsText.text = thousands.ToString("0");
            hundredsText.text = hundreds.ToString("0");
            y = 300f;
        }
        else thousandsText.text = " "; 
        StartCoroutine(DisplayText(y));
    }

    [SerializeField] private Vector3 onesPos = new Vector3(0f, 0f, 0f);
    [SerializeField] private Vector3 tensPos = new Vector3(0f, 0f, 0f);
    [SerializeField] private Vector3 hundredsPos = new Vector3(0f, 0f, 0f);
    [SerializeField] private Vector3 thousandsPos = new Vector3(0f, 0f, 0f);
    [SerializeField] private Vector3 centerPos = new Vector3(0f, 0f, 0f);
    private Vector3 randomPos = new Vector3(0f, 0f, 0f);
    private IEnumerator DisplayText(float y)
    {
        randomPos.x = Random.Range(-300f, -200f);
        randomPos.y = y;
        onesPos += randomPos;
        tensPos += randomPos;
        hundredsPos += randomPos;
        thousandsPos += randomPos;

        StartCoroutine(TextAnimation(onesObj,onesPos));
        for (int i = 0; i < FADE_COOLTIME; i++)
        {
            yield return null;
        }
        StartCoroutine(TextAnimation(tensObj,tensPos));
        for (int i = 0; i < FADE_COOLTIME; i++)
        {
            yield return null;
        }
        StartCoroutine(TextAnimation(hundredsObj,hundredsPos));
        for (int i = 0; i < FADE_COOLTIME; i++)
        {
            yield return null;
        }
        StartCoroutine(TextAnimation(thousandsObj, thousandsPos));
    }
    private IEnumerator TextAnimation(GameObject target, Vector3 destination)
    {
        RectTransform targetTransform = target.GetComponent<RectTransform>();
        targetTransform.anchoredPosition = centerPos;
        
        float speed = (destination - centerPos).magnitude / FADE_TIME;
        float addScale = defaultScale / FADE_TIME;
        Vector3 scale = targetTransform.localScale;

        for (int i = 0; i < (int)FADE_TIME+3; i++)
        {
            targetTransform.anchoredPosition = Vector3.MoveTowards(targetTransform.anchoredPosition, destination, speed);
            scale.x += addScale;
            scale.y += addScale;
            targetTransform.localScale = scale;
            yield return null;
        }
        for (int i = 0; i < 3; i++)
        {
            targetTransform.anchoredPosition = Vector3.MoveTowards(targetTransform.anchoredPosition, destination, speed);
            scale.x -= addScale;
            scale.y -= addScale;
            targetTransform.localScale = scale;
            yield return null;
        }
    }
}
