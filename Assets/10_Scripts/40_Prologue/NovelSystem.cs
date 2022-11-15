using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using System;
using System.IO;

public class NovelSystem : MonoBehaviour
{
    //Animator
    Animator anim;

    //ゲーム内表示場所
    [SerializeField] private Image visualImage = null; 　//キャラの見た目
    [SerializeField] private Text mainName = null; 　　　//キャラの名前
    [SerializeField] private Text mainText = null; 　　　//本文
    [SerializeField] private GameObject nextIcon = null; //ページ送りのアイコン表示

    [SerializeField] private int textSpeed = 0;  //テキストの表示速度

    private int current = 0;//現在のテキスト

    private int visualID= 0;//キャラ見た目
    private string serifName = null; //キャラ名前
    private bool isSlide = false;//キャラクター表示時にスライドさせるか
    private string mainString = null; //本文

    //const
    private const int START_ANIMATION = 30;
    private const int END_ANIMATION = 30;

    //
    private bool isLoad = false;
    private bool isStart = false;

    //csvData
    private string csvName = null;  //使用するCSVの名前
    List<string[]> novelDatas = new List<string[]>();//ノベルデータ格納場所

    //スキップする用に呼び出したコルーチンを保存する
    private IEnumerator sentence;

    private enum DATA
    {
        NUMBER,
        VISUAL,
        NAME,
        IS_SLIDE,
        SENTENCE
    }

    [SerializeField] private Sprite[] visual = new Sprite[10];

    public IEnumerator NovelStart(string s)
    {
        csvName = s;
        //CSV読み込み
        LoadCSV();
        //初期表示
        InitializeDisplay();
        //CSVロード後
        yield return new WaitUntil(() => !isLoad);
        anim = this.GetComponent<Animator>();
        yield return new WaitForSecondsRealtime(0.5f);
        anim.SetTrigger("Start");
        for(int i = 0; i < START_ANIMATION; i++)
        {
            yield return null;
        }
        StartCoroutine(DisplaySentence());
        isStart = true;
        //全文表示するまで待つ
        yield return new WaitUntil(() => current >= novelDatas.Count);
        anim.SetTrigger("End");
        for(int i = 0; i < END_ANIMATION; i++)
        {
            yield return null;
        }
        Destroy(this.gameObject);
    }

    private void Update()
    {
        if (!isStart) return;

        //ノベルシーンをスキップする
        if (Input.GetButtonDown("Cancel"))
        {
            StopCoroutine(sentence);
            current = novelDatas.Count;
        }

        //Aボタン
        if (Input.GetButtonDown("Submit"))
        {
            //テキスト全文表示後の場合、次のテキストを表示
            if (nextIcon.activeSelf)
            {
                current++;
                //現在のテキスト
                StartCoroutine(DisplaySentence());
                nextIcon.SetActive(false);
            }
            //テキスト進行中の場合、全文表示
            else
            {
                StopCoroutine(sentence);
                mainText.text = mainString;
                nextIcon.SetActive(true); //次のテキスト表示を可能にする
            }
        }
    }

    //文を表示
    private IEnumerator DisplaySentence()
    {
        if(current >= novelDatas.Count)
        {
            isStart = false;
            yield break;
        }
        ImportSentenceDatas();
        DisplayVisual();

        sentence = DisplayMainText();
        yield return StartCoroutine(sentence);
    }
   

    //データの更新
    void ImportSentenceDatas()
    {
        if(current >=  novelDatas.Count) return;
        nextIcon.SetActive(false);
        visualID = int.Parse(novelDatas[current][(int)DATA.VISUAL]); //表示キャラ
        serifName = novelDatas[current][(int)DATA.NAME];     //名前
        isSlide = bool.Parse(novelDatas[current][(int)DATA.IS_SLIDE]); //スライドするかどうか
        mainString = ConvertText(novelDatas[current][(int)DATA.SENTENCE]); //本文

        //改行処理
        string ConvertText(string s)
        {
            string t = null;
            foreach (char c in s)
            {
                if (c == '/') t += "\n";
                else t += c;
            }
            return t;
        }
    }

    //キャラクター表示
    void DisplayVisual()
    {
        if (isSlide) anim.SetTrigger("Slide");
        visualImage.sprite = visual[visualID];
        mainName.text = serifName;
    }

    //本文表示
    IEnumerator DisplayMainText()
    {
        int messageCount = 0; //現在表示中の文字数
        mainText.text = "";

        while (mainString.Length > messageCount)
        {
            mainText.text += mainString[messageCount]; //一文字追加
            messageCount++; //現在の文字数
            for (int i = 0; i < textSpeed; i++)
            {
                yield return null;
            }
        }
        nextIcon.SetActive(true); //次のテキスト表示を可能にする
    }

    //-------------初期化----------------
    private void InitializeDisplay()
    {
        visualImage.sprite = visual[0];
        mainName.text = "";
        mainText.text = "";
    }

    private void LoadCSV()
    {
        //ロード開始
        isLoad = true;
        string path = Path.Combine("Assets/91_CSV/" + csvName + ".csv");
        Addressables.LoadAssetAsync<TextAsset>(path).Completed += csv =>
        {
            StringReader reader = new StringReader(csv.Result.text);

            string line = null;
            line = reader.ReadLine(); //見出し行をスキップする
            while (reader.Peek() != -1) // reader.Peekが-1になるまで
            {
                line = reader.ReadLine(); // 一行ずつ読み込み
                novelDatas.Add(line.Split(',')); // , 区切りでリストに追加
            }
            reader.Close();
            //ロード完了
            isLoad = false;
        };
    }
}
