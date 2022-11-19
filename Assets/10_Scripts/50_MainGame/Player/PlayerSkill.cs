using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkill : PlayerManager
{
    private int maxTiles = 4;//スキルタイルの最大枚数
    public int MaxTiles { set { maxTiles = value; } }
    private int currentTiles = 0;//現在の枚数
    private const int SKILL_COOLTIME = 600;//スキルタイル再生成までの時間(フレーム)
    private int time = 0; //クールタイム計算用
    public int SetTime { set { time = value; } } // Debug用
    private bool isEvo = false;
    public bool IsEvo
    {
        set
        {
            isEvo = value;
            if (!isEvo) currentTiles = 0;
        }
    }

    private void FixedUpdate()
    {
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
        //クールタイム
        if (time < SKILL_COOLTIME)
        {
            time++;
            return;
        }
        //タイルの枚数を設定
        time = 0;
        currentTiles = maxTiles;
        RandomGenerate();
    }

    /// <summary>
    /// タイルをランダムで生成
    /// </summary>
    private void RandomGenerate()
    {
        //プレイヤーの現在の位置を取得
        int playerPos = playerController.CurrentX + playerController.CurrentY * 3;
        int healPos = playerController.HealPos;
        List<int> numbers = new List<int>();
        //生成可能な位置を追加
        for (int i = 0; i < 9; i++)
        {
            //プレイヤーがいる場所以外を追加
            if (playerPos != i && healPos != i) numbers.Add(i);
        }
        //生成可能な位置からランダムに生成
        for (int i = 0; i < maxTiles; i++)
        {
            //ランダムな数字を取得
            int index = Random.Range(0, numbers.Count);
            //残ってる位置から生成位置を設定
            int x = numbers[index] % 3;
            int y = numbers[index] / 3;
            
            //
            playerController.InstanceSkillOrb(x,y);
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
    public void ResetGrid()
    {
        for (int i = 0; i < 9; i++)
        {
            if (playerController.SkillGrid[i % 3, i / 3])
            {
                playerController.SkillGrid[i % 3, i / 3] = false;
                playerController.RemoveSkillOrb(i);
            }
        }
    }

    public void RemoveGrid()
    {
        SoundManager.instance.PlaySE(SoundManager.SE_Type.SkillOrb); // 効果音
        audienceAnimation.StartAnimation(); // 観客のアニメーションを変更する
        gameManager.AddScore((int)SCORE.SKILL_ORB); // スコア追加
        playerController.SkillGrid[playerController.CurrentX, playerController.CurrentY] = false; // 該当のスキルタイルを削除
        playerController.RemoveSkillOrb(playerController.CurrentPos);
        playerSkill.RemoveTile(); // スキルメソッドにスキルタイルを削除したことを伝える
    }
    public void SkillSE()
    {
        switch (playerController.Type)
        {
            case (int)CHARACTER.RED:
                SoundManager.instance.PlaySE(SoundManager.SE_Type.Skill_Red);
                break;
            case (int)CHARACTER.BLUE:
                SoundManager.instance.PlaySE(SoundManager.SE_Type.Skill_Blue);
                break;
            case (int)CHARACTER.YELLOW:
                SoundManager.instance.PlaySE(SoundManager.SE_Type.Skill_Yellow);
                break;
            default:
                Debug.Log("Error");
                break;
        }
    }
}
