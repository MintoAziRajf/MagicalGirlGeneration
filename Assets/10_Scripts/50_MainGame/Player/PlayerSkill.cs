using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    PlayerController playerController;
    GameUI gameUI;
    private void Awake()
    {
        playerController = this.GetComponent<PlayerController>();
        gameUI = this.GetComponent<GameUI>();
        gameUI.SkillCooltime = SKILL_COOLTIME;
    }

    private int maxTiles = 4;//スキルタイルの最大枚数
    private int currentTiles = 0;//現在の枚数
    private const int SKILL_COOLTIME = 600;//スキルタイル再生成までの時間(フレーム)
    private int time = 0; //クールタイム計算用
    private bool isEvo = false;
    public bool IsEvo
    {
        set
        {
            isEvo = value;
        }
    }

    private void Update()
    {
        gameUI.SkillCurrentTime = time;
        //変身中、タイルが残っていない
        bool canGenerate = isEvo && currentTiles == 0;
        if (canGenerate)
        {
            GenerateTile();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="x">プレイヤーの位置X</param>
    /// <param name="y">プレイヤーの位置X</param>
    private void GenerateTile()
    {
        //タイルの枚数を設定
        currentTiles = maxTiles;
        //クールタイム
        if (time < SKILL_COOLTIME)
        {
            time++;
            return;
        }
        
        RandomGenerate();
    }

    /// <summary>
    /// タイルをランダムで生成
    /// </summary>
    private void RandomGenerate()
    {
        //プレイヤーの現在の位置を取得
        int playerPos = playerController.CurrentX + playerController.CurrentY * 3;

        List<int> numbers = new List<int>();
        //生成可能な位置を追加
        for (int i = 0; i < 9; i++)
        {
            //プレイヤーがいる場所以外を追加
            if (playerPos != i) numbers.Add(i);
        }
        //生成可能な位置からランダムに生成
        for (int i = 0; i < maxTiles; i++)
        {
            //ランダムな数字を取得
            int index = Random.Range(0, numbers.Count);
            //残ってる位置から生成位置を設定
            int x = numbers[index] / 3;
            int y = numbers[index] % 3;
            
            //
            //playerController.SetSkillGrid(x,y);

            //使った位置を消す
            numbers.RemoveAt(index);
        }
    }

    public void RemoveTile()
    {
        currentTiles--;
        if (currentTiles == 0)
        {
            StartCoroutine(playerController.Skill());
        }
    }
}
