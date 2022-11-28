using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageManager : MonoBehaviour
{
    public static MessageManager instance;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    [SerializeField] private GameObject messageCanvas = null; // ゲームガイド画面
    [SerializeField] private GameObject messageObj = null; // ゲームガイドオブジェクト
    private bool isTutorial = false; //　チュートリアル中か
    public bool IsTutorial { set { isTutorial = value; } }
    private const int DISPLAY_TIME = 60; // 表示時間
    private Queue<IEnumerator> messageQueue = new Queue<IEnumerator>(); // ゲームガイド表示キュー

    private bool isDisplay = false; // 表示中か

    public IEnumerator DisplayMessage(string msg)
    {
        if (isTutorial) yield break; // チュートリアル中は表示しない
        if (isDisplay)　// 表示中だったらキューに追加
        {
            messageQueue.Enqueue(DisplayMessage(msg));
            yield break;
        }
        isDisplay = true; // 表示スタート
        GameObject message = Instantiate(messageObj, messageCanvas.transform); // ゲームガイドオブジェクトを生成
        //Debug.Log(msg);
        Text displayText = message.transform.Find("Text").gameObject.GetComponent<Text>();
        displayText.text = msg; // テキスト代替
        // 待機
        for(int i = 0; i < DISPLAY_TIME; i++)
        {
            yield return new WaitForSeconds(1f/60f);
        }
        isDisplay = false; // 表示終了
        Destroy(message); // 削除
        if (messageQueue.Count != 0) StartCoroutine(messageQueue.Dequeue()); // キューに登録されているものがあれば表示
    }
}
