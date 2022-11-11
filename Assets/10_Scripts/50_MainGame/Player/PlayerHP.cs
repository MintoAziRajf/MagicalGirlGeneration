using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHP : PlayerManager
{   
    private void Update()
    {
        gameUI.CurrentHP = hp;
    }

    private int hp = 0; //現在のヒットポイント
    private int minHP = 0; //最小値
    private int maxHP = 0; //現在の最大値
    private int evoHP = 0; //変身後の追加Hp
    //setter
    public int EvoHP{ set { evoHP = value; gameUI.MaxHP = evoHP; } }
    private int normalHP = 0;//変身前の最大値
    //setter
    public int NormalHP { set { normalHP = value; hp = value; maxHP = value; } }
    private bool isEvo = false;
    public bool IsEvo {
        set {
            isEvo = value;
            if(isEvo)
            {
                maxHP = evoHP;
                hp += evoHP - normalHP;
            }
            else
            {
                maxHP = normalHP;
                if(hp >= normalHP) hp = normalHP;
            }
        }
    }

    public void Damaged(int value)
    {
        hp = Mathf.Max(hp - value, minHP);
        gameUI.CurrentHP = hp;
        StartCoroutine(gameUI.DamagedEffect());
        if (hp == minHP)
        {
            //dead
            Debug.Log("死にました。");
            gameManager.StopGame();
            StartCoroutine(playerDeadAnimation.StartAnimation());
        }
    }

    private const int heal = 50;
    public void Heal()
    {
        Debug.Log(hp + heal);
        hp = Mathf.Min(hp + heal, maxHP);
        gameUI.CurrentHP = hp;
    }

    /// <summary>
    /// Debug用
    /// </summary>
    public void SetHP(int debugHp)
    {
        hp = Mathf.Min(debugHp, maxHP);
        gameUI.CurrentHP = hp;
    }
}
