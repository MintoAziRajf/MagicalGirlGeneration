using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class EnemyManager : MonoBehaviour
{
    public bool isDebug = false;

    EnemySkillList enemySkillList; //攻撃スキルを呼び出す用
    EnemyUI enemyUI; //エネミーに関するUI表示

    private IEnumerator attackLoop;  //途中停止用に呼び出すコルーチンの保存先
    private IEnumerator attack;      //

    [SerializeField] private GameObject enemyUIObj = null;　　//UIの表示先(親)
    [SerializeField] private GameObject damageUIPrefab = null;//ダメージ表記のプレファブ
    [SerializeField] private TextAsset[] csv = null;//エネミーの情報が書いてあるCSV
    List<string[]> enemyData = new List<string[]>();//エネミーの情報

    private int currentEnemy = 0; //現在の敵
    private bool isAlive = false; //生きているかどうか
    private bool isStart = false; //
    public bool IsStart { set { isStart = value; } }

    private const int DAMAGE_DELAY = 2; //連続ダメージの間にかけるディレイ(フレーム)
    [SerializeField] private float magnitude = 0f;
    [SerializeField] private float duration = 0f;

    //攻撃に使う変数
    private int currentAttack = 0;  //現在の攻撃
    private int type = 0; //攻撃の種類
    private int x = 0;　//攻撃の呼び出し先座標 : X
    private int y = 0;　//攻撃の呼び出し先座標 : Y
    private int damage = 0; //攻撃のダメージ
    private int cooltime = 0; //次の攻撃までのクールタイム(フレーム)

    //CSVの列情報
    private enum DATA
    {
        TYPE,
        X,
        Y,
        DAMAGE,
        COOLTIME,
        HP
    }

    //HP
    private int hpMax = 10000; //最大HP
    private int hpMin = 0; //最低HP(死亡するHP)
    private int hpCurrent = 10000; //現在のHP

    //弱点
    private int weakPoint = 0;
    public int WeakPoint { get { return weakPoint; } }

    private void Awake()
    {
        enemySkillList = GetComponent<EnemySkillList>();
        enemyUI = this.GetComponent<EnemyUI>();
        //エネミー情報をロード
        LoadEnemy();
    }
    /// <summary>
    /// 敵の攻撃パターンとステータスをロード
    /// </summary>
    private void LoadEnemy()
    {
        enemyData.Clear();
        StringReader reader = new StringReader(csv[currentEnemy].text);

        string line = null;
        line = reader.ReadLine(); //見出し行をスキップする
        while (reader.Peek() != -1) // reader.Peekが-1になるまで
        {
            line = reader.ReadLine(); // 一行ずつ読み込み
            enemyData.Add(line.Split(',')); // , 区切りでリストに追加
        }
        reader.Close();

        SetEnemy(); //ロードした情報をセット
    }

    /// <summary>
    /// ロードした情報をセットする
    /// </summary>
    private void SetEnemy()
    {
        isAlive = true;　//生存情報をセット
        //HPをセット
        hpMax = int.Parse(enemyData[0][(int)DATA.HP]); 
        hpCurrent = hpMax; 
        //UIにHP情報をセット
        enemyUI.HPMax = hpMax; 
        enemyUI.HPCurrent = hpCurrent;
        //弱点をセット
        weakPoint = Random.Range(0, 3);
        enemyUI.DisplayWeakIcon(weakPoint);
    }

    /// <summary>
    /// ダメージメソッド
    /// </summary>
    /// <param name="value">ダメージ</param>
    /// <param name="freq">回数</param>
    public IEnumerator Damaged(int value, int freq)
    {
        StartCoroutine(enemyUI.DamagedEffect());
        StartCoroutine(DamagedShake());
        float y = 400f; //ダメージ表記の高さを設定する
        for(int i = 0; i < freq; i++)
        {
            GameObject damage = Instantiate(damageUIPrefab, enemyUIObj.transform); //ダメージ表記を生成
            damage.GetComponent<EnemyDamageUI>().Damaged(value,y); //生成したダメージ表記にダメージと表示する高さを送る
            hpCurrent -= value;//ダメージ分HPを減らす

            Destroy(damage, 1f); //ダメージ表記を生成してから1秒後に削除
            //連続したダメージ表記にはディレイをかける
            for(int j = 0; j < DAMAGE_DELAY; j++)
            {
                yield return null;
            }
            //ダメージ表記が重ならないように高さを回数に応じて調整
            y -= 250f/freq;
        }
        CheckHP();
        //弱点をリセット
        weakPoint = Random.Range(0, 3);
        enemyUI.DisplayWeakIcon(weakPoint);
    }

    
    private IEnumerator DamagedShake()
    {
        Transform target = this.gameObject.transform;
        Vector3 pos = target.localPosition;

        for (int i = 0; i < duration; i++)
        {
            float x = pos.x + Random.Range(-1f, 1f) * magnitude;
            float y = pos.y + Random.Range(-1f, 1f) * magnitude;

            target.localPosition = new Vector3(x, y, pos.z);
            yield return null;
        }
    }


    /// <summary>
    /// 生きてるかどうかの判定
    /// </summary>
    private void CheckHP()
    {
        //HPが0以下なら0に調整
        if (hpCurrent < hpMin)
        {
            hpCurrent = hpMin;
        }
        enemyUI.HPCurrent = hpCurrent; //チェック後のHPをUIにセット

        //HPが0なら現在のエネミーを消滅させる
        if (hpCurrent == hpMin)
        {
            DefeatEnemy();
        }
    }

    private IEnumerator AttackLoop()
    {
        yield return new WaitUntil(() => isStart);

        while (isAlive)//生存情報がTRUEの間攻撃
        {
            attack = Attack();
            yield return StartCoroutine(attack); //攻撃の呼び出し,終了次第ループ
        }
        yield break;
    }

    /// <summary>
    /// enemyDataにある攻撃パターンを全て実行
    /// </summary>
    private IEnumerator Attack()
    {
        for (int i = currentAttack; i < enemyData.Count; i++)
        {
            //攻撃情報をセット
            type = int.Parse(enemyData[i][(int)DATA.TYPE]);
            x = int.Parse(enemyData[i][(int)DATA.X]);
            y = int.Parse(enemyData[i][(int)DATA.Y]);
            damage = int.Parse(enemyData[i][(int)DATA.DAMAGE]);
            cooltime = int.Parse(enemyData[i][(int)DATA.COOLTIME]);

            //攻撃
            yield return StartCoroutine(enemySkillList.Attack(type, x, y, damage));
            //前の攻撃が終わったら次の攻撃に
            currentAttack++;
            //次の攻撃までのクールタイム
            for (int j = 0; j < cooltime; j++)
            {
                yield return null;
            }
        }
        currentAttack = 0;
    }

    /// <summary>
    /// 攻撃を開始(PlayerControllerとGamaManagerから呼ばれます)
    /// </summary>
    public void StartAttack()
    {
        attackLoop = AttackLoop();
        StartCoroutine(attackLoop);
    }

    /// <summary>
    /// 攻撃を開始(PlayerControllerrとGamaManager呼ばれます)
    /// </summary>
    public void StopAttack()
    {
        enemySkillList.Stop();
        StopCoroutine(attackLoop);
        StopCoroutine(attack);
    }

    /// <summary>
    /// エネミーが倒された時の処理
    /// </summary>
    private void DefeatEnemy()
    {
        isAlive = false; //生存情報をセット
        //攻撃ループと現在の攻撃を停止
        StopCoroutine(attackLoop);
        StopCoroutine(attack);

        //次のエネミーをロード
        currentEnemy++;
        LoadEnemy();
    }
}
