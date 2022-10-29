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

    private bool isStart = false;
    private bool isLoad = false;

    //csvData
    private string csvName = null;  //使用するCSVの名前
    List<string[]> novelDatas = new List<string[]>();//ノベルデータ格納場所

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


    void Awake()
    {
        LoadData();
        LoadCSV();
        InitializeDisplay();
        StartCoroutine(AwakeWaitTime());
    }

    void Update()
    {
        if (!isStart)
        {
            return;
        }
        //ノベルシーンをスキップしてメインゲームに遷移する
        if (Input.GetButtonDown("Cancel"))
        {
            SceneChange();
            return;
        }

        //Aボタン
        if (Input.GetButtonDown("Submit"))
        {
            //テキスト進行中の場合、全文表示
            if (!nextIcon.activeSelf)
            {
                StopAllCoroutines();
                mainText.text = mainString;
                nextIcon.SetActive(true); //次のテキスト表示を可能にする
                return;
            }
            //全て表示した後メインゲームに遷移する
            if (novelDatas.Count == current + 1)
            {
                isStart = false;
                SceneChange();
                return;
            }
            //テキスト全文表示後の場合、次のテキストを表示
            nextIcon.SetActive(false);
            current++;
            ImportNovelDatas(current);
        }
    }

    private void LoadData()
    {
        typeID = DataStorage.instance.PlayerType;
        csvName = DataStorage.instance.CSVName;
    }

    private void InitializeDisplay()
    {
        backgroundImage.color = backgroundColor[typeID];
        visualImage.sprite = visual[0];
        mainName.text = "";
        mainText.text = "";
    }

    //データの更新
    void ImportNovelDatas(int index)
    {
        nextIcon.SetActive(false);
        visualID = int.Parse(novelDatas[current][(int)DATA.Visual]); //表示キャラ
        nameID = int.Parse(novelDatas[current][(int)DATA.Name]);     //名前
        isSlide = bool.Parse(novelDatas[current][(int)DATA.IsSlide]); //スライドするかどうか
        mainString = ConvertText(novelDatas[current][(int)DATA.Text]); //本文
        DisplayNovel();
    }

    //表示
    void DisplayNovel()
    {
        visualImage.sprite = visual[visualID];
        mainName.text = characterName[nameID];
        
        StartCoroutine(DisplayMainText(mainString));
    }

    private string ConvertText(string s)
    {
        string t = null;
        foreach(char c in s)
        {
            if(c == '/') t += "\n";
            else t += c;
            
        }
        return t;
    }

    IEnumerator DisplayMainText(string mText)
    {
        int messageCount = 0; //現在表示中の文字数
        mainText.text = "";

        while (mText.Length > messageCount)
        {
            mainText.text += mText[messageCount]; //一文字追加
            messageCount++; //現在の文字数
            for (int i = 0; i < textSpeed; i++)
            {
                yield return null;
            }
        }
        nextIcon.SetActive(true); //次のテキスト表示を可能にする
    }

    IEnumerator AwakeWaitTime()
    {
        yield return new WaitUntil(() => !isLoad);
        yield return new WaitForSeconds(1f);
        isStart = true;
        ImportNovelDatas(0);
    }

    
    private void SceneChange()
    {
        LoadManager.instance.LoadScene(nextScene);
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
