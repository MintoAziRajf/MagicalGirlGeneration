using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySkillList : MonoBehaviour
{
    [SerializeField] private Transform[] dangerGrid = null;
    [SerializeField] private GameObject dangerMark = null;
    [SerializeField] private GameObject bombObj = null;


    private const int MARK_LIFETIME = 20;

    public IEnumerator Attack(string skillName, int x, int y, int damage)
    {

        yield return StartCoroutine(skillName, (x, y, damage));
    }

    private IEnumerator Bomb(int x, int y, int damage)
    {
        for(int i = 0; i < 30; i++)
        {
            yield return null;
        }
    }

    private IEnumerator DisplayDangerZone(int x, int y) 
    {
        int index = x + y * 3;
        GameObject mark = Instantiate(dangerMark, dangerGrid[index].position, this.transform.rotation);
        for(int i = 0; i < MARK_LIFETIME; i++)
        {
            yield return null;
        }
        Destroy(mark.gameObject);

    }
}
