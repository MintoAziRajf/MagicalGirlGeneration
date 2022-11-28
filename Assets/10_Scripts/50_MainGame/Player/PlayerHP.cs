using UnityEngine;

public class PlayerHP : PlayerManager
{
    private int hp = 0; //現在のヒットポイント
    private int minHP = 0; //最小値
    private int maxHP = 0; //現在の最大値
    private int evoHP = 0; //変身後の最大Hp
    private int hpColor = 0; // HPの色

    // HPの状態
    private enum HP { LOWEST, LOW, MEDIUM, HIGH } 
    private const int HIGH = 750; 
    private const int MEDIUM = 500;
    private const int LOW = 250;

    //setter
    public int EvoHP{ set { evoHP = value; gameUI.MaxHP = evoHP; } }
    private int normalHP = 0;//変身前の最大値
    //setter
    public int NormalHP { set { normalHP = value; hp = value; maxHP = value; HealedCheckHP(); } }
    private bool isEvo = false; // 変身しているか
    public bool IsEvo {
        set {
            isEvo = value;
            // 変身していたらHPの上限を増やし、回復
            if(isEvo)
            {
                maxHP = evoHP;
                hp += evoHP - normalHP;
                HealedCheckHP();
            }
            // 変身していなかったらHP上限を減らす
            else
            {
                maxHP = normalHP;
                if(hp >= normalHP) hp = normalHP;
                DamagedCheckHP();
            }
        }
    }

    private void Update()
    {
        gameUI.CurrentHP = hp; // UI更新
    }

    /// <summary>
    /// ダメージを受けたときの処理
    /// </summary>
    /// <param name="value">ダメージ値</param>
    public void Damaged(int value)
    {
        // hpを減らしUIを更新
        hp = Mathf.Max(hp - value, minHP); 
        gameUI.CurrentHP = hp;
        StartCoroutine(gameUI.DamagedEffect()); // ダメージエフェクト
        DamagedCheckHP();
        
        // HPが0になったらゲームオーバー
        if (hp == minHP)
        {
            Debug.Log("ゲームオーバー");
            StartCoroutine(MessageManager.instance.DisplayMessage("仕方ない一回撤退だ！")); // ゲームガイド再生
            gameManager.StopGame(); // メインゲームの処理を止める
            SoundManager.instance.PlayBGM(SoundManager.BGM_Type.GameOver); // ゲームオーバーBGMを再生
            StartCoroutine(playerDeadAnimation.StartAnimation()); // 死亡演出を再生
        }
    }

    /// <summary>
    /// ダメージを受けたときに現在のHPから状態を判定
    /// </summary>
    private void DamagedCheckHP()
    {
        if (hp <= HIGH && hpColor == (int)HP.HIGH)
        {
            StartCoroutine(MessageManager.instance.DisplayMessage("大丈夫！？\nまだやれるかい？")); // ゲームガイド
            hpColor = (int)HP.MEDIUM; // HPカラーをセット
        }
        else if (hp <= MEDIUM && hpColor == (int)HP.MEDIUM)
        {
            StartCoroutine(MessageManager.instance.DisplayMessage("怪我してるよ！\n気を付けて！"));
            hpColor = (int)HP.LOW;
        }
        else if (hp <= LOW && hpColor == (int)HP.LOW)
        {
            StartCoroutine(MessageManager.instance.DisplayMessage("これ以上はまずいよ！"));
            hpColor = (int)HP.LOWEST;
        }
        gameUI.HPColorType = hpColor;
    }

    private const int heal = 50; // 回復量
    /// <summary>
    /// 回復処理
    /// </summary>
    public void Heal()
    {
        Debug.Log(hp + heal);
        hp = Mathf.Min(hp + heal, maxHP);
        gameUI.CurrentHP = hp;
        HealedCheckHP();
    }
    /// <summary>
    /// 回復したときに現在のHPから状態を判定
    /// </summary>
    private void HealedCheckHP()
    {
        if (hp >= HIGH) hpColor = (int)HP.HIGH;
        else if (hp >= MEDIUM) hpColor = (int)HP.MEDIUM;
        else if (hp >= LOW) hpColor = (int)HP.LOW;
        else hpColor = (int)HP.LOWEST;
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
