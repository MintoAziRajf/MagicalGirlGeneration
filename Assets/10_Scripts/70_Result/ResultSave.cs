using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class ResultSave : MonoBehaviour
{
    private enum DATA
    {
        SCORE,
        RANK
    }
    private List<string[]> scoreDatas = new List<string[]>();
    private string path;
    private const int RANK_S = 3000000;
    private const int RANK_A = 2000000;
    private const int RANK_B = 1000000;
    public void SaveScore(int type, int score)
    {
        path = Application.dataPath + @"\score.csv";
        using (StreamWriter streamWriter = new StreamWriter(path, false, Encoding.UTF8))
        {
            for (int i = 0; i < scoreDatas.Count; i++)
            {
                if (i == type)
                {
                    string s = Rank(score);
                    streamWriter.WriteLine(score.ToString("0000000") + "," + s);
                }
                else
                {
                    streamWriter.WriteLine(scoreDatas[i][(int)DATA.SCORE] + "," + scoreDatas[i][(int)DATA.RANK]);
                }
            }
            streamWriter.Flush();
            streamWriter.Close();
        }
    }

    /// <summary>
    /// ハイスコアをロードする
    /// スコアデータが存在しない場合のみファイルを作成
    /// </summary>
    private void LoadHighscore()
    {
        path = Application.dataPath + @"\score.csv";
        scoreDatas.Clear();
        if (!File.Exists(path))
        {
            using (File.Create(path))
            {

            }
            using (StreamWriter streamWriter = new StreamWriter(path, false, Encoding.UTF8))
            {
                for (int i = 0; i < 3; i++)
                {
                    streamWriter.Write("0,C");
                    streamWriter.WriteLine();
                }
                streamWriter.Flush();
                streamWriter.Close();
                Debug.Log("プレイヤーデータの初期化に成功しました。");
            }
        }

        StreamReader csv = new StreamReader(path, Encoding.UTF8);
        string line = null;
        while ((line = csv.ReadLine()) != null)
        {
            scoreDatas.Add(line.Split(','));
        }
        csv.Close();
        Debug.Log("スコアデータをロードしました。");
    }

    public int Highscore(int type)
    {
        LoadHighscore();
        return int.Parse(scoreDatas[type][(int)DATA.SCORE]);
    }
    public string Rank(int score)
    {
        string s = null;
        if (score >= RANK_S) s = "S";
        else if (score >= RANK_A) s = "A";
        else if (score >= RANK_B) s = "B";
        else s = "C";
        return s;
    }

    public int RankInt(int score)
    {
        int rank = 0;
        if (score >= RANK_S) rank = 3;
        else if (score >= RANK_A) rank = 2;
        else if (score >= RANK_B) rank = 1;
        else rank = 0;
        return rank;
    }
}
