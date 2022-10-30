using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHP : MonoBehaviour
{
    private int hp = 0; //現在のヒットポイント
    private int minHP = 0; //最小値
    private int maxHP = 0; //最大値
    private int evoHP = 0; //変身後の最大値
    //setter
    public int EvoHP
    {
        set { evoHP = value; }
    }
    private int normalHP = 0;//変身前の最大値
    //setter
    public int NormalHP
    {
        set { normalHP = value; }
    }

    public void Damage(int value)
    {
        hp = Mathf.Max(hp - value, minHP);
        if (hp == minHP)
        {
            //dead
            Debug.Log("死にました。");
        }
    }
    public void Heal(int value)
    {
        //hp = Mathf.Min(hp + value, HP_MAX);
    }
}
