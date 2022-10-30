using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySkillList : MonoBehaviour
{
    [SerializeField] private GameObject gameManager = null;
    CollisionManager collisionManager;

    [SerializeField] private Transform[] enemyGrid = null;
    [SerializeField] private GameObject dangerMark = null;
    [SerializeField] private GameObject bombObj = null;

    //---------コンストラクタ------------
    private const int DAMAGE_LIFETIME = 10;
    private const int OMEN_TIME = 1;
    private const int MARK_LIFETIME = 20;
    private const int FADE_TIME = 20;
    //-----------------------------------
    private void Awake()
    {
        collisionManager = gameManager.GetComponent<CollisionManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            StartCoroutine(Bomb(1, 0, 100));
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            StartCoroutine(Bomb(1, 1, 100));
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            StartCoroutine(Bomb(1, 2, 100));
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(Bomb(2, 0, 100));
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            StartCoroutine(Bomb(2, 1, 100));
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            StartCoroutine(Bomb(2, 2, 100));
        }
    }

    public IEnumerator Bomb(int x, int y, int damage)
    {
        yield return StartCoroutine(DisplayDangerZone(x, y));
        int index = x + y * 3;
        GameObject effect = Instantiate(bombObj, enemyGrid[index].position, enemyGrid[index].rotation, enemyGrid[index]);
        for (int i = 0; i < OMEN_TIME; i++)
        {
            yield return null;
        }
        StartCoroutine(collisionManager.DamageGrid(x, y, damage, DAMAGE_LIFETIME));
        for (int i = 0; i < DAMAGE_LIFETIME; i++)
        {
            yield return null;
        }
        for (int i = 0; i < FADE_TIME; i++)
        {
            yield return null;
        }
        Destroy(effect.gameObject);
    }

    private IEnumerator DisplayDangerZone(int x, int y)
    {
        int index = x + y * 3;
        GameObject mark = Instantiate(dangerMark, enemyGrid[index].position, enemyGrid[index].rotation, enemyGrid[index]);
        for (int i = 0; i < MARK_LIFETIME; i++)
        {
            yield return null;
        }
        Destroy(mark.gameObject);
    }
}
