using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    private int score; //スコア
    //getter
    public int Score { get { return score; } } 

    [SerializeField] private Text scoreText = null; // 合計スコア表示
    [SerializeField] private Text addScoreTextPrefab = null; // 追加スコア表示用Prefab
    [SerializeField] private GameObject addScoreParent = null; // 追加スコアの生成先

    /// <summary>
    /// スコアを追加(GameManagerから呼ばれます)
    /// </summary>
    /// <param name="value"></param>
    public void AddScore(int value)
    {
        score += value; // スコアを追加
        UpdateScoreUI();// スコア表示を更新
        AddScoreDisplay(value);// 追加スコアを表示 
        if (score <= 0) score = 0; // スコアがマイナスにならないように
    }

    private const float OFFSET_POS = -55f; // 追加スコアの位置変更用オフセット
    private const float OFFSET_ALPHA = 0.1f;// 追加スコアの不透明度変更用オフセット
    Queue<Text> queueAddScore = new Queue<Text>(); // 追加した順番を記録するキュー
    private void AddScoreDisplay(int value)
    {
        if (queueAddScore.Count == 6) // 追加スコア表示が6件溜まったら一番古いものから消す
        {
            Text last = queueAddScore.Dequeue();
            Destroy(last.gameObject);
        }
        else
        {
            StartCoroutine(DestroyAddText()); // スコア追加1秒後に削除するメソッド
        }
        Text add = Instantiate(addScoreTextPrefab, addScoreParent.transform); // 追加スコアを生成
        add.text = "+" + value.ToString(); // 追加スコアのテキスト更新
        queueAddScore.Enqueue(add); // キューに追加

        Text[] array = queueAddScore.ToArray(); // キューを配列にコピー
        // カラーをリセット
        Color c = array[0].color;
        for (int i = 0; i < queueAddScore.Count; i++)
        {
            c.a = 1f; // カラーをリセット
            int index = queueAddScore.Count - i - 1; // 順番を反転させる
            array[index].rectTransform.anchoredPosition = new Vector3(0f, OFFSET_POS * i, 0f); // 位置を調整
            c.a -= OFFSET_ALPHA * i; // 不透明度調整
            array[index].color = c; // カラーをセット
        }
    }

    /// <summary>
    /// 追加スコア表示を削除
    /// </summary>
    private IEnumerator DestroyAddText()
    {
        yield return new WaitForSeconds(1f); //一秒待機
        Text last = queueAddScore.Dequeue(); //キューから削除
        Destroy(last.gameObject); //ゲームから削除
    }

    /// <summary>
    /// スコアUIを更新(9桁まで)
    /// </summary>
    private void UpdateScoreUI()
    {
        scoreText.text = score.ToString("000000000");
    }
}
