using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHP : MonoBehaviour
{
    GameUI gameUI;
    private void Awake()
    {
        gameUI = GetComponent<GameUI>();
        hp = normalHP;
        gameUI.MaxHP = evoHP;
        gameUI.CurrentHP = hp;
    }

    private int hp = 0; //現在のヒットポイント
    private int minHP = 0; //最小値
    private int maxHP = 0; //現在の最大値
    private int evoHP = 0; //変身後の追加Hp
    //setter
    public int EvoHP{ set { evoHP = value; } }
    private int normalHP = 0;//変身前の最大値
    //setter
    public int NormalHP { set { normalHP = value; } }
    private bool isEvo = false;
    public bool IsEvo {
        set {
            isEvo = value;
            if(isEvo)
            {
                maxHP = normalHP + evoHP;
                hp += evoHP;
            }
            else
            {
                maxHP = normalHP;
                if(hp >= normalHP) hp = normalHP;
            }
        }
    }
    public void Damage(int value)
    {
        hp = Mathf.Max(hp - value, minHP);
        gameUI.CurrentHP = hp;
        if (hp == minHP)
        {
            //dead
            Debug.Log("死にました。");
        }
    }
    public void Heal(int value)
    {
        hp = Mathf.Min(hp + value, maxHP);
        gameUI.CurrentHP = hp;
    }
}
