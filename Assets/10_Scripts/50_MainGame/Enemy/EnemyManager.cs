using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class EnemyManager : MonoBehaviour
{
    EnemySkillList enemySkillList;
    [SerializeField] private GameObject enemyUI = null;
    [SerializeField] private GameObject damageUIPrefab = null;
    [SerializeField] private TextAsset csv = null;
    List<string[]> attackData = new List<string[]>();

    private const int ATTACK_DELAY = 2;
    private int currentAttack = 0;
    private int type;
    private int x;
    private int y;
    private int damage;
    private int cooltime;
    private enum DATA
    {
        TYPE,
        X,
        Y,
        DAMAGE,
        COOLTIME
    }

    private int hp = 10000;
    public IEnumerator Damaged(int value, int freq)
    {
        float y = 450f;
        for(int i = 0; i < freq; i++)
        {
            GameObject damage = Instantiate(damageUIPrefab, enemyUI.transform);
            damage.GetComponent<EnemyDamageUI>().Damaged(value,y);
            hp -= value * freq;
            Destroy(damage, 1f);
            for(int j = 0; j < ATTACK_DELAY; j++)
            {
                yield return null;
            }
            y -= 300f/freq;
        }   
    }

    private void Awake()
    {
        enemySkillList = GetComponent<EnemySkillList>();
        //LoadAttackList();
        //StartCoroutine(AttackLoop());
    }

    public IEnumerator AttackLoop()
    {
        for (int i = currentAttack; i < attackData.Count; i++)
        {
            type = int.Parse(attackData[i][(int)DATA.TYPE]);
            x = int.Parse(attackData[i][(int)DATA.X]);
            y = int.Parse(attackData[i][(int)DATA.Y]);
            damage = int.Parse(attackData[i][(int)DATA.DAMAGE]);
            cooltime = int.Parse(attackData[i][(int)DATA.COOLTIME]);

            yield return StartCoroutine(enemySkillList.Attack(type, x, y, damage));
            currentAttack++;
            for(int j = 0; j < cooltime; j++)
            {
                yield return null;
            }
        }
    }

    public void StopAttack()
    {
        enemySkillList.Stop();
    }

    private void LoadAttackList()
    {
        StringReader reader = new StringReader(csv.text);

        string line = null;
        line = reader.ReadLine(); //見出し行をスキップする
        while (reader.Peek() != -1) // reader.Peekが-1になるまで
        {
            line = reader.ReadLine(); // 一行ずつ読み込み
            attackData.Add(line.Split(',')); // , 区切りでリストに追加
        }
        reader.Close();
    }
}
