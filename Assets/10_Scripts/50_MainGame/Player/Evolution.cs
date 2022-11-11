using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evolution : PlayerManager
{
    private void Start()
    {
        gameUI.EvolutionMax = EVO_MAX;
        gameUI.EvolutionCurrentGauge = EVO_MIN;
    }

    private const int EVO_MAX = 100;  //ゲージ最大値
    private const int EVO_MIN = 0;    //最小値
    private const int EVO_MOVE = 5;　 //移動時のゲージ上昇量
    private const int EVO_ATTACK = 10;//攻撃時のゲージ上昇量
    private const int EVO_TICK = -1;　//毎秒減る値
    private float evoTime = 0f;//秒数記録用
    private int evoGauge = 0;  //現在のゲージ 
    public int EvoGauge { set { evoGauge = value; } } //Debug用
    private bool isEvo = false;//変身しているかどうか
    private bool isStart = false;
    public bool IsStart { set { isStart = value; } }

    
    private void FixedUpdate()
    {
        gameUI.EvolutionCurrentGauge = evoGauge;
        if (!isEvo || !isStart) return;
        Decrease();
    }

    public void Check()
    {
        //ゲージがMAX　且つ　変身していないとき
        if (evoGauge == EVO_MAX && !isEvo)
        {
            //変身
            isEvo = true; 
            //set
            playerController.IsEvo = true;
        }
        //ゲージがMIN　且つ　変身しているとき
        if (evoGauge == EVO_MIN && isEvo)
        {
            //変身を解除
            isEvo = false;
            //set
            playerController.IsEvo = false;
        }
    }
    
    /// <summary>
    /// 変身ゲージを溜める
    /// </summary>
    /// <param name="s">増加量</param>
    public void Increase(string s)
    {
        if (isEvo) return;
        int value = 0;
        if (s == "Attack")
        {
            value = EVO_ATTACK;
        }
        else if(s == "Move")
        {
            value = EVO_MOVE;
        }

        //最大値を上回らないように増加させる
        evoGauge = Mathf.Min(evoGauge + value, EVO_MAX);
    }

    /// <summary>
    /// 変身ゲージを溜める(チュートリアル用)
    /// </summary>
    /// <param name="value">増加量</param>
    public void Increase(int value)
    {
        evoGauge = value;
    }


    /// <summary>
    /// 変身時、毎秒ゲージを減少させる
    /// </summary>
    private void Decrease()
    {
        evoTime += Time.deltaTime;
        //一秒を超えたら
        if (evoTime >= 1.0f)
        {
            evoTime -= 1.0f;
            //最小値を下回らないように減少させる
            evoGauge = Mathf.Max(evoGauge + EVO_TICK, EVO_MIN);
        }
    }
}
