using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySkillList : MonoBehaviour
{
    [SerializeField] private GameObject gameManager = null;
    CollisionManager collisionManager;

    [SerializeField] private GameObject[] enemyGrid = null;
    [SerializeField] private GameObject dangerMark = null;
    [SerializeField] private GameObject bombObj = null;
    [SerializeField] private GameObject beamObj = null;
    [SerializeField] private GameObject lightningObj = null;
    [SerializeField] private GameObject stoneObj = null;
    [SerializeField] private GameObject iceObj = null;
    [SerializeField] private GameObject enemyAttacks = null;

    // 定数 ------------
    private const int OMEN_TIME = 1; // 予兆エフェクト時間
    private const int MARK_LIFETIME = 30; // 危険マーク表示時間 
    private const int DAMAGE_LIFETIME = 5; // ダメージ判定時間
    private const int FADE_TIME = 5; // 攻撃エフェクトが消えるまでの時間
    private const int ULT_TIME = 5; // 必殺技エフェクト追加時間

    private const int SPIKE_DELAY = 10; // 段々攻撃の遅延時間
    // 攻撃の種類
    private enum ATTACK
    {
        BOMB,
        LIGHTNING,
        BEAM,
        EARTHSPIKE,
        ICESPIKE,
        ALL
    }
    //-----------------------------------
    private void Awake()
    {
        collisionManager = gameManager.GetComponent<CollisionManager>();
    }
    /// <summary>
    /// 全ての攻撃を停止
    /// </summary>
    public void Stop()
    {
        StopAllCoroutines();
        foreach (Transform child in enemyAttacks.transform)
        {
            GameObject.Destroy(child.gameObject); // エフェクトを全て消去
        }
    }

    /// <summary>
    /// 攻撃
    /// </summary>
    /// <param name="type">種類</param>
    /// <param name="x">攻撃を開始する場所</param>
    /// <param name="y">攻撃を開始する場所</param>
    /// <param name="damage">ダメージ量</param>
    /// <returns></returns>
    public IEnumerator Attack(int type, int x, int y, int damage)
    {
        switch (type)
        {
            // 単マス攻撃
            case (int)ATTACK.BOMB:
                yield return StartCoroutine(Bomb(x, y, damage));
                break;
            // 行攻撃
            case (int)ATTACK.LIGHTNING:
                StartCoroutine(AttackEffect(0, y, damage, lightningObj));
                StartCoroutine(AttackEffect(1, y, damage, lightningObj));
                yield return StartCoroutine(AttackEffect(2, y, damage, lightningObj));
                break;
            // 列攻撃
            case (int)ATTACK.BEAM:
                StartCoroutine(AttackEffect(x, 0, damage, beamObj));
                StartCoroutine(AttackEffect(x, 1, damage, beamObj));
                yield return StartCoroutine(AttackEffect(x, 2, damage, beamObj));
                break;
            // 行段々攻撃
            case (int)ATTACK.EARTHSPIKE:
                if (x == 2)
                {
                    for(int i = x; i >= 0; i--)
                    {
                        StartCoroutine(AttackEffect(i, y, damage, stoneObj));
                        yield return StartCoroutine(WaitFrame(SPIKE_DELAY));
                    }
                }
                else if (x == 0)
                {
                    for (int i = x; i <= 2; i++)
                    {
                        StartCoroutine(AttackEffect(i, y, damage, stoneObj));
                        yield return StartCoroutine(WaitFrame(SPIKE_DELAY));
                    }
                }
                else Debug.Log("Error");
                break;
            // 列段々攻撃
            case (int)ATTACK.ICESPIKE:
                if (y == 2)
                {
                    for (int i = y; i >= 0; i--)
                    {
                        StartCoroutine(AttackEffect(x, i, damage, iceObj));
                        yield return StartCoroutine(WaitFrame(SPIKE_DELAY));
                    }
                }
                else if (y == 0)
                {
                    for (int i = y; i <= 2; i++)
                    {
                        StartCoroutine(AttackEffect(x, i, damage, iceObj));
                        yield return StartCoroutine(WaitFrame(SPIKE_DELAY));
                    }
                }
                else Debug.Log("Error");
                break;
            // 必殺技
            case (int)ATTACK.ALL:
                StartCoroutine(MessageManager.instance.DisplayMessage("敵の大技がくるよ！\n気を付けて！"));
                yield return StartCoroutine(UltEffect(damage));
                break;
            default:
                yield return null;
                break;
        }
    }
    /// <summary>
    /// 単体マス攻撃
    /// </summary>
    private IEnumerator Bomb(int x, int y, int damage)
    {
        int index = x + y * 3; // 位置を配列用に変換
        GameObject effect = Instantiate(bombObj, enemyGrid[index].transform.position, enemyGrid[index].transform.rotation, enemyAttacks.transform); // エフェクトを生成
        yield return StartCoroutine(DisplayDangerZone(x, y)); // 危険マークを表示
        yield return StartCoroutine(WaitFrame(OMEN_TIME)); // 予兆時間待機
        collisionManager.StartCoroutine(collisionManager.DamageGrid(x, y, damage, DAMAGE_LIFETIME)); // 当たり判定を生成
        yield return StartCoroutine(WaitFrame(DAMAGE_LIFETIME)); // 当たり判定時間待機
        yield return StartCoroutine(WaitFrame(FADE_TIME)); // エフェクトが消えるまで待機
        Destroy(effect.gameObject); // エフェクト削除
    }
    /// <summary>
    /// 単体マス、必殺技以外の攻撃
    /// </summary>
    /// <param name="effectPrefab">表示するエフェクト</param>
    private IEnumerator AttackEffect(int x, int y, int damage, GameObject effectPrefab)
    {
        int index = x + y * 3; // 位置を配列用に変換
        yield return StartCoroutine(DisplayDangerZone(x, y)); // 危険マークを表示
        GameObject effect = Instantiate(effectPrefab, enemyGrid[index].transform.position, enemyGrid[index].transform.rotation, enemyAttacks.transform);// エフェクトを生成
        yield return StartCoroutine(WaitFrame(OMEN_TIME)); // 予兆時間待機
        collisionManager.StartCoroutine(collisionManager.DamageGrid(x, y, damage, DAMAGE_LIFETIME)); // 当たり判定を生成
        yield return StartCoroutine(WaitFrame(DAMAGE_LIFETIME)); // 当たり判定時間待機
        yield return StartCoroutine(WaitFrame(FADE_TIME)); // エフェクトが消えるまで待機
        Destroy(effect.gameObject); // エフェクト削除
    }

    [SerializeField] private GameObject ultMark = null; // 必殺技の危険マーク
    [SerializeField] private GameObject ultEffectPrefab = null; // 必殺技のエフェクト
    private IEnumerator UltEffect(int damage)
    {
        List<GameObject> effects = new List<GameObject>(); // エフェクトリスト
        yield return StartCoroutine(DisplayUltZone()); // 危険マークを表示
        // 全てのマスにエフェクトを生成
        for (int i = 0; i < 9; i++)
        {
            effects.Add(Instantiate(ultEffectPrefab, enemyGrid[i].transform.position, enemyGrid[i].transform.rotation, enemyAttacks.transform));
        }
        yield return StartCoroutine(WaitFrame(OMEN_TIME)); // 予兆時間待機
        // 全てのマスに当たり判定を生成
        for(int i = 0; i < 9; i++)
        {
            collisionManager.StartCoroutine(collisionManager.DamageGrid(i % 3, i / 3, damage, DAMAGE_LIFETIME));
        }
        yield return StartCoroutine(WaitFrame(DAMAGE_LIFETIME + FADE_TIME + ULT_TIME)); // 当たり判定,エフェクト消失,必殺技の追加時間待機
        // 全てのエフェクトを消去
        foreach (GameObject effect in effects)
        {
            Destroy(effect.gameObject);
        }
        // 必殺技用危険マーク
        IEnumerator DisplayUltZone()
        {
            int index = 4; // 中心
            GameObject mark = Instantiate(ultMark, enemyGrid[index].transform.position, enemyGrid[index].transform.rotation, enemyAttacks.transform);
            yield return StartCoroutine(WaitFrame(MARK_LIFETIME * 2));
            Destroy(mark.gameObject);
        }
    }

    /// <summary>
    /// 危険マーク
    /// </summary>
    /// <param name="x">位置</param>
    /// <param name="y">位置</param>
    private IEnumerator DisplayDangerZone(int x, int y)
    {
        int index = x + y * 3; // 位置を配列用に変換
        GameObject mark = Instantiate(dangerMark, enemyGrid[index].transform.position, enemyGrid[index].transform.rotation, enemyAttacks.transform); // マークを生成
        yield return StartCoroutine(WaitFrame(MARK_LIFETIME)); // 待機
        Destroy(mark.gameObject); // 削除
    }
    /// <summary>
    /// 指定フレーム数待機
    /// </summary>
    /// <param name="frame">指定フレーム数</param>
    private IEnumerator WaitFrame(int frame)
    {
        float oneFrame = 1f / 60f;
        for (int i = 0; i < frame; i++)
        {
            yield return new WaitForSeconds(oneFrame);
        }
    }

    /// <summary>
    /// チュートリアル関連
    /// </summary>
    private List<GameObject> tutorialMark = new List<GameObject>();
    /// <summary>
    /// プレイヤーがダメージを受けるチュートリアル用
    /// </summary>
    public IEnumerator TutorialAttackStart()
    {
        yield return StartCoroutine(Attack((int)ATTACK.LIGHTNING, 0, 1, 300));
    }
    /// <summary>
    /// プレイヤーがカウンター攻撃をするチュートリアル用(開始)
    /// </summary>
    public IEnumerator TutorialCounterStart()
    {
        yield return StartCoroutine(Attack((int)ATTACK.BEAM, 0, 0, 10));
        yield return StartCoroutine(Attack((int)ATTACK.BEAM, 1, 0, 10));
        tutorialMark.Add(Instantiate(dangerMark, enemyGrid[2].transform.position, enemyGrid[2].transform.rotation, enemyAttacks.transform));
        tutorialMark.Add(Instantiate(dangerMark, enemyGrid[5].transform.position, enemyGrid[5].transform.rotation, enemyAttacks.transform));
        tutorialMark.Add(Instantiate(dangerMark, enemyGrid[8].transform.position, enemyGrid[8].transform.rotation, enemyAttacks.transform));
        yield return StartCoroutine(WaitFrame(MARK_LIFETIME));
    }
    /// <summary>
    /// プレイヤーがカウンター攻撃をするチュートリアル用(終了)
    /// </summary>
    public IEnumerator TutorialCounterEnd()
    {
        foreach(GameObject g in tutorialMark)
        {
            Destroy(g);
        }
        List<GameObject> effect = new List<GameObject>();
        effect.Add(Instantiate(lightningObj, enemyGrid[2].transform.position, enemyGrid[2].transform.rotation, enemyAttacks.transform));
        effect.Add(Instantiate(lightningObj, enemyGrid[5].transform.position, enemyGrid[5].transform.rotation, enemyAttacks.transform));
        effect.Add(Instantiate(lightningObj, enemyGrid[8].transform.position, enemyGrid[8].transform.rotation, enemyAttacks.transform));
        yield return StartCoroutine(WaitFrame(OMEN_TIME));
        collisionManager.StartCoroutine(collisionManager.DamageGrid(2, 0, 10, DAMAGE_LIFETIME));
        collisionManager.StartCoroutine(collisionManager.DamageGrid(2, 1, 10, DAMAGE_LIFETIME));
        collisionManager.StartCoroutine(collisionManager.DamageGrid(2, 2, 10, DAMAGE_LIFETIME));
        yield return StartCoroutine(WaitFrame(DAMAGE_LIFETIME));
        yield return StartCoroutine(WaitFrame(FADE_TIME));
        foreach (GameObject g in effect)
        {
            Destroy(g);
        }
    }
}
