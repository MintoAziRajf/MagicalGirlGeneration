using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyDamageUI : MonoBehaviour
{
    private Text onesText; // 1桁目
    private Text tensText; // 2桁目
    private Text hundredsText; // 3桁目
    private Text thousandsText;// 4桁目
    private GameObject onesObj;
    private GameObject tensObj;
    private GameObject hundredsObj;
    private GameObject thousandsObj;
    [SerializeField] private GameObject weakObj = null; // 弱点表記

    private const int FADE_COOLTIME = 5; // 表記遅延
    private const float FADE_TIME = 5f; // 表記するまでの時間
    private float scale = 1f; // 大きさ
    /// <summary>
    /// オブジェクトを取得
    /// </summary>
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

    /// <summary>
    /// ダメージ表記
    /// </summary>
    /// <param name="value">ダメージ値</param>
    /// <param name="y">表記する高さ</param>
    /// <param name="isWeak">弱点にヒットしたか</param>
    public void Damaged(int value, float y, bool isWeak)
    {
        SetObject();
        weakObj.SetActive(isWeak); // 弱点にヒットしていた場合弱点表記
        // 各桁に変換
        int ones = value % 10; 
        int tens = (value / 10) % 10;
        int hundreds = (value / 100) % 10;
        int thousands = value / 1000;

        onesText.text = ones.ToString("0");
        tensText.text = tens.ToString("0");
        // 100以上だったら大きさ変更
        if (hundreds != 0)
        {
            scale = 1.3f;
            hundredsText.text = hundreds.ToString("0");
        }
        else hundredsText.text = " ";
        // 1000以上だったら大きさ変更
        if (thousands != 0)
        {
            scale = 1.5f;
            thousandsText.text = thousands.ToString("0");
            hundredsText.text = hundreds.ToString("0");
            y = 300f;
        }
        else thousandsText.text = " "; 
        StartCoroutine(DisplayText(y));
    }

    // 各桁の表示場所
    [SerializeField] private Vector3 onesPos = new Vector3(0f, 0f, 0f);
    [SerializeField] private Vector3 tensPos = new Vector3(0f, 0f, 0f);
    [SerializeField] private Vector3 hundredsPos = new Vector3(0f, 0f, 0f);
    [SerializeField] private Vector3 thousandsPos = new Vector3(0f, 0f, 0f);
    [SerializeField] private Vector3 startPos = new Vector3(0f, 0f, 0f);
    private Vector3 randomPos = new Vector3(0f, 0f, 0f); // 表示場所をランダムにする用
    private IEnumerator DisplayText(float y)
    {
        // 表示場所に乱数を加える
        randomPos.x = Random.Range(150f, 350f);
        randomPos.y = y;
        onesPos += randomPos;
        tensPos += randomPos;
        hundredsPos += randomPos;
        thousandsPos += randomPos;

        // アニメーション
        StartCoroutine(TextAnimation(onesObj,onesPos));
        //待機
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
    /// <summary>
    /// テキスト表示アニメーション
    /// </summary>
    /// <param name="target">移動させるオブジェクト</param>
    /// <param name="destination">目的地</param>
    /// <returns></returns>
    private IEnumerator TextAnimation(GameObject target, Vector3 destination)
    {
        RectTransform targetTransform = target.GetComponent<RectTransform>();
        targetTransform.anchoredPosition = startPos;
        
        float speed = (destination - startPos).magnitude / FADE_TIME; // 速度計算
        float addScale = this.scale / FADE_TIME; // スケール計算
        Vector3 scale = targetTransform.localScale;

        // 徐々に変化させる
        // 指定値より少し大きく表示させてその後小さくさせる
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
