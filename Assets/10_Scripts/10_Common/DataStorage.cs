using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataStorage : SingletonMonoBehaviour<DataStorage>
{
    public static DataStorage instance;

    private int playerType;//選択されたキャラクター
    public int PlayerType
    {
        get { return playerType; }
        set { playerType = value; }
    }
    [SerializeField] private string[] csvNames = new string[6];
    private string csvName;
    public string CSVName
    {
        get { return csvName; }
    }

    void Awake()
    {
        //Singleton
        CheckSingleton();

        //Debug
        SetNovel(true);
    }

    /// <summary>
    /// 使用するCSVを設定する
    /// </summary>
    /// <param name="b">プロローグ:True エピローグ:False</param>
    public void SetNovel(bool b)
    {
        if (b) csvName = csvNames[playerType];
        else csvName = csvNames[2 + playerType];
    }

    // シングルトンかつ、シーン遷移しても破棄されないようにする
    public override void CheckSingleton()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
