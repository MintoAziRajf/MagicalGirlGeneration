using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using System;
using System.IO;

public class NovelSystem : MonoBehaviour
{
    //ゲーム内表示場所
    [SerializeField] private Image visualImage = null; 　//キャラの見た目
    [SerializeField] private Text mainName = null; 　　　//キャラの名前
    [SerializeField] private Text mainText = null; 　　　//本文
    [SerializeField] private GameObject nextIcon = null; //ページ送りのアイコン表示
    [SerializeField] private Image backgroundImage = null;//背景

    [SerializeField] private int textSpeed = 0;  //テキストの表示速度

    //次のシーンの名前
    [SerializeField] private string nextScene = null;

    private int typeID = 0; //選択されたキャラクター
    private int current = 0;//現在のテキスト

    private int visualID= 0;//キャラ見た目
    private int nameID = 0; //キャラ名前
    private bool isSlide = false;//キャラクター表示時にスライドさせるか
    private string mainString = null; //本文

    //
    private bool isLoad = false;

    //csvData
    private string csvName = null;  //使用するCSVの名前
    List<string[]> novelDatas = new List<string[]>();//ノベルデータ格納場所

    //スキップする用に呼び出したコルーチンを保存する
    private IEnumerator novel;
    private IEnumerator sentence;

    private enum DATA
    {
        Number,
        Visual,
        Name,
        IsSlide,
        Text
    }

    [SerializeField] private Sprite[] visual = new Sprite[10];
    [SerializeField] private Color[] backgroundColor = new Color[3];
    [SerializeField] private string[] characterName = new string[6];


    public IEnumerator NovelStart(string s, int type)
    {
        csvName = s;
        typeID = type;
        //CSV読み込み
        LoadCSV();
        //初期表示
        InitializeDisplay();
        //CSVロード後
        yield return new WaitUntil(() => !isLoad);
        novel = Novel();
        yield return StartCoroutine(novel);
    }

    private void Update()
    {
        //ノベルシーンをスキップする
        if (Input.GetButtonDown("Cancel"))
        {
            StopCoroutine(novel);
        }
        //Aボタン
        if (Input.GetButtonDown("Submit"))
        {
            //テキスト進行中の場合、全文表示
            StopCoroutine(sentence);
            mainText.text = mainString;
            nextIcon.SetActive(true); //次のテキスト表示を可能にする
        }
    }

    private IEnumerator Novel()
    {
        //全文表示するまで繰り返す
        while(current < novelDatas.Count)
        {
            //現在のテキスト
            yield return StartCoroutine(DisplaySentence());
            //テキスト全文表示後、Submitボタンで次のテキストを表示
            yield return new WaitUntil(() => !Input.GetButtonDown("Submit"));
            nextIcon.SetActive(false);
            current++;
        }
    }

    //文を表示
    private IEnumerator DisplaySentence()
    {
        ImportSentenceDatas();
        DisplayVisual();

        sentence = DisplayMainText();
        yield return StartCoroutine(sentence);
    }
   

    //データの更新
    void ImportSentenceDatas()
    {
        nextIcon.SetActive(false);
        visualID = int.Parse(novelDatas[current][(int)DATA.Visual]); //表示キャラ
        nameID = int.Parse(novelDatas[current][(int)DATA.Name]);     //名前
        isSlide = bool.Parse(novelDatas[current][(int)DATA.IsSlide]); //スライドするかどうか
        mainString = ConvertText(novelDatas[current][(int)DATA.Text]); //本文

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
        visualImage.sprite = visual[visualID];
        mainName.text = characterName[nameID];
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
        backgroundImage.color = backgroundColor[typeID];
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
