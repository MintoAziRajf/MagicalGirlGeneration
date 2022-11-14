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
    private int hpColor = 0;
    private enum HP_COLOR { LOWEST, LOW, MEDIUM, HIGH }
    private const int HIGH = 750;
    private const int MEDIUM = 500;
    private const int LOW = 250;
    //setter
    public int EvoHP{ set { evoHP = value; gameUI.MaxHP = evoHP; } }
    private int normalHP = 0;//変身前の最大値
    //setter
    public int NormalHP { set { normalHP = value; hp = value; maxHP = value; HealedCheckHP(); } }
    private bool isEvo = false;
    public bool IsEvo {
        set {
            isEvo = value;
            if(isEvo)
            {
                maxHP = evoHP;
                hp += evoHP - normalHP;
                HealedCheckHP();
            }
            else
            {
                maxHP = normalHP;
                if(hp >= normalHP) hp = normalHP;
                DamagedCheckHP();
            }
        }
    }

    public void Damaged(int value)
    {
        hp = Mathf.Max(hp - value, minHP);
        gameUI.CurrentHP = hp;
        StartCoroutine(gameUI.DamagedEffect());
        DamagedCheckHP();
        
        if (hp == minHP)
        {
            //dead
            Debug.Log("死にました。");
            gameManager.StopGame();
            StartCoroutine(playerDeadAnimation.StartAnimation());
        }
    }

    private void DamagedCheckHP()
    {
        if (hp <= HIGH && hpColor == (int)HP_COLOR.HIGH)
        {
            StartCoroutine(MessageManager.instance.DisplayMessage("大丈夫！？まだやれるかい？"));
            hpColor = (int)HP_COLOR.MEDIUM;
        }
        else if (hp <= MEDIUM && hpColor == (int)HP_COLOR.MEDIUM)
        {
            StartCoroutine(MessageManager.instance.DisplayMessage("怪我してるよ！気を付けて！"));
            hpColor = (int)HP_COLOR.LOW;
        }
        else if (hp <= LOW && hpColor == (int)HP_COLOR.LOW)
        {
            StartCoroutine(MessageManager.instance.DisplayMessage("怪我してるよ！気を付けて！"));
            hpColor = (int)HP_COLOR.LOWEST;
        }
        gameUI.HPColorType = hpColor;
    }

    private const int heal = 50;
    public void Heal()
    {
        Debug.Log(hp + heal);
        hp = Mathf.Min(hp + heal, maxHP);
        gameUI.CurrentHP = hp;
        HealedCheckHP();
    }

    private void HealedCheckHP()
    {
        if (hp >= HIGH) hpColor = (int)HP_COLOR.HIGH;
        else if (hp >= MEDIUM) hpColor = (int)HP_COLOR.MEDIUM;
        else if (hp >= LOW) hpColor = (int)HP_COLOR.LOW;
        else hpColor = (int)HP_COLOR.LOWEST;
        gameUI.HPColorType = hpColor;
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
