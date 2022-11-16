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

    //---------コンストラクタ------------
    private const int OMEN_TIME = 1;
    private const int MARK_LIFETIME = 20;
    private const int DAMAGE_LIFETIME = 5;
    private const int FADE_TIME = 5;
    private const int ULT_TIME = 5;

    private const int SPIKE_DELAY = 10;

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

    public void Stop()
    {
        StopAllCoroutines();
        foreach (Transform child in enemyAttacks.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
    public IEnumerator Attack(int type, int x, int y, int damage)
    {
        switch (type)
        {
            case (int)ATTACK.BOMB:
                yield return StartCoroutine(Bomb(x, y, damage));
                break;
            case (int)ATTACK.LIGHTNING:
                StartCoroutine(AttackEffect(0, y, damage, lightningObj));
                StartCoroutine(AttackEffect(1, y, damage, lightningObj));
                yield return StartCoroutine(AttackEffect(2, y, damage, lightningObj));
                break;
            case (int)ATTACK.BEAM:
                StartCoroutine(AttackEffect(x, 0, damage, beamObj));
                StartCoroutine(AttackEffect(x, 1, damage, beamObj));
                yield return StartCoroutine(AttackEffect(x, 2, damage, beamObj));
                break;
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
            case (int)ATTACK.ALL:
                StartCoroutine(MessageManager.instance.DisplayMessage("敵の大技がくるよ！\n気を付けて！"));
                yield return StartCoroutine(UltEffect(damage));
                break;
            default:
                yield return null;
                break;
        }
    }
    private IEnumerator Bomb(int x, int y, int damage)
    {
        int index = x + y * 3;
        GameObject effect = Instantiate(bombObj, enemyGrid[index].transform.position, enemyGrid[index].transform.rotation, enemyAttacks.transform);
        yield return StartCoroutine(DisplayDangerZone(x, y));
        yield return StartCoroutine(WaitFrame(OMEN_TIME));
        collisionManager.StartCoroutine(collisionManager.DamageGrid(x, y, damage, DAMAGE_LIFETIME));
        yield return StartCoroutine(WaitFrame(DAMAGE_LIFETIME));
        yield return StartCoroutine(WaitFrame(FADE_TIME));
        Destroy(effect.gameObject);
    }
    private IEnumerator AttackEffect(int x, int y, int damage, GameObject effectPrefab)
    {
        int index = x + y * 3;
        yield return StartCoroutine(DisplayDangerZone(x, y));
        GameObject effect = Instantiate(effectPrefab, enemyGrid[index].transform.position, enemyGrid[index].transform.rotation, enemyAttacks.transform);
        yield return StartCoroutine(WaitFrame(OMEN_TIME));
        collisionManager.StartCoroutine(collisionManager.DamageGrid(x, y, damage, DAMAGE_LIFETIME));
        yield return StartCoroutine(WaitFrame(DAMAGE_LIFETIME));
        yield return StartCoroutine(WaitFrame(FADE_TIME));
        Destroy(effect.gameObject);
    }

    [SerializeField] private GameObject ultMark = null;
    [SerializeField] private GameObject ultEffectPrefab = null;
    private IEnumerator UltEffect(int damage)
    {
        List<GameObject> effects = new List<GameObject>();
        yield return StartCoroutine(DisplayUltZone());
        for (int i = 0; i < 9; i++)
        {
            effects.Add(Instantiate(ultEffectPrefab, enemyGrid[i].transform.position, enemyGrid[i].transform.rotation, enemyAttacks.transform));
        }
        yield return StartCoroutine(WaitFrame(OMEN_TIME));
        for(int i = 0; i < 9; i++)
        {
            collisionManager.StartCoroutine(collisionManager.DamageGrid(i % 3, i / 3, damage, DAMAGE_LIFETIME));
        }
        yield return StartCoroutine(WaitFrame(DAMAGE_LIFETIME + FADE_TIME+ULT_TIME));
        foreach(GameObject effect in effects)
        {
            Destroy(effect.gameObject);
        }
        IEnumerator DisplayUltZone()
        {
            int index = 4; // 中心
            GameObject mark = Instantiate(ultMark, enemyGrid[index].transform.position, enemyGrid[index].transform.rotation, enemyAttacks.transform);
            yield return StartCoroutine(WaitFrame(MARK_LIFETIME * 3));
            Destroy(mark.gameObject);
        }
    }

    private IEnumerator DisplayDangerZone(int x, int y)
    {
        int index = x + y * 3;
        GameObject mark = Instantiate(dangerMark, enemyGrid[index].transform.position, enemyGrid[index].transform.rotation, enemyAttacks.transform);
        yield return StartCoroutine(WaitFrame(MARK_LIFETIME));
        Destroy(mark.gameObject);
    }
    private IEnumerator WaitFrame(int frame)
    {
        float oneFrame = 1f / 60f;
        for (int i = 0; i < frame; i++)
        {
            yield return new WaitForSeconds(oneFrame);
        }
    }

    private GameObject tutorialMark;
    public IEnumerator Tutorial(int index)
    {
        tutorialMark = Instantiate(dangerMark, enemyGrid[index].transform.position, enemyGrid[index].transform.rotation, enemyAttacks.transform);
        yield return StartCoroutine(WaitFrame(MARK_LIFETIME));
    }
    public IEnumerator TutorialEnd(int index)
    {
        Destroy(tutorialMark);
        GameObject effect = Instantiate(bombObj, enemyGrid[index].transform.position, enemyGrid[index].transform.rotation, enemyAttacks.transform);
        yield return StartCoroutine(WaitFrame(OMEN_TIME));
        yield return StartCoroutine(WaitFrame(DAMAGE_LIFETIME));
        yield return StartCoroutine(WaitFrame(FADE_TIME));
        Destroy(effect.gameObject);
    }
}
