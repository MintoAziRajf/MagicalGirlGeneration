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

    //---------コンストラクタ------------
    private const int DAMAGE_LIFETIME = 10;
    private const int OMEN_TIME = 1;
    private const int MARK_LIFETIME = 20;
    private const int FADE_TIME = 5;

    private enum ATTACK
    {
        BOMB,
        LIGHTNING,
        BEAM
    }
    //-----------------------------------
    private void Awake()
    {
        collisionManager = gameManager.GetComponent<CollisionManager>();
    }

    public void Stop()
    {
        foreach(GameObject parent in enemyGrid)
        {
            foreach (Transform child in parent.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
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
                yield return StartCoroutine(Lightning(x, y, damage));
                break;
            default:
                yield return null;
                break;
        }
    }

    private IEnumerator Bomb(int x, int y, int damage)
    {
        int index = x + y * 3;

        yield return StartCoroutine(DisplayDangerZone(x, y));
        GameObject effect = Instantiate(bombObj, enemyGrid[index].transform.position, enemyGrid[index].transform.rotation, enemyGrid[index].transform);
        yield return StartCoroutine(WaitFrame(OMEN_TIME));
        StartCoroutine(collisionManager.DamageGrid(x, y, damage, DAMAGE_LIFETIME));
        yield return StartCoroutine(WaitFrame(DAMAGE_LIFETIME));
        yield return StartCoroutine(WaitFrame(FADE_TIME));
        Destroy(effect.gameObject);
    }
    private IEnumerator Lightning(int x, int y, int damage)
    {
        int index = x + y * 3;
        yield return StartCoroutine(DisplayDangerZone(x, y));
        GameObject effect = Instantiate(bombObj, enemyGrid[index].transform.position, enemyGrid[index].transform.rotation, enemyGrid[index].transform);
        yield return StartCoroutine(WaitFrame(OMEN_TIME));
        StartCoroutine(collisionManager.DamageGrid(x, y, damage, DAMAGE_LIFETIME));
        yield return StartCoroutine(WaitFrame(DAMAGE_LIFETIME));
        yield return StartCoroutine(WaitFrame(FADE_TIME));
        Destroy(effect.gameObject);
    }


    private IEnumerator DisplayDangerZone(int x, int y)
    {
        int index = x + y * 3;
        GameObject mark = Instantiate(dangerMark, enemyGrid[index].transform.position, enemyGrid[index].transform.rotation, enemyGrid[index].transform);
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
        tutorialMark = Instantiate(dangerMark, enemyGrid[index].transform.position, enemyGrid[index].transform.rotation, enemyGrid[index].transform);
        yield return StartCoroutine(WaitFrame(MARK_LIFETIME));
    }
    public IEnumerator TutorialEnd(int index)
    {
        Destroy(tutorialMark);
        GameObject effect = Instantiate(bombObj, enemyGrid[index].transform.position, enemyGrid[index].transform.rotation, enemyGrid[index].transform);
        yield return StartCoroutine(WaitFrame(OMEN_TIME));
        yield return StartCoroutine(WaitFrame(DAMAGE_LIFETIME));
        yield return StartCoroutine(WaitFrame(FADE_TIME));
        Destroy(effect.gameObject);
    }
}
