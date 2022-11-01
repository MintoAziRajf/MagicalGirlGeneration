using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutIn : MonoBehaviour
{
    [SerializeField] GameObject cutInObj = null;
    [SerializeField] GameObject damageEffect = null;
    private Animator cutInAnim;
    private Animator damageAnim;
    private const int ANIM_COOLTIME = 120;
    private const int ATTACK_COOLTIME = 20;

    [SerializeField] private int duration = 0;
    [SerializeField] private float magnitude = 0f;
    private void Awake()
    {
        cutInAnim = cutInObj.GetComponent<Animator>();
        damageAnim = damageEffect.GetComponent<Animator>();
        damageAnim.SetInteger("Type", DataStorage.instance.PlayerType);
        cutInAnim.SetInteger("Type", DataStorage.instance.PlayerType);
    }

    public IEnumerator Attack(bool b)
    {
        if (b) damageAnim.SetTrigger("EvoAttack");
        else damageAnim.SetTrigger("Attack");
        StartCoroutine(DamagedAnim());

        yield return StartCoroutine(WaitAnim(ATTACK_COOLTIME));
        StartCoroutine(MessageManager.instance.DisplayMessage("ナイス攻撃！"));
    }

    private IEnumerator DamagedAnim()
    {
        GameObject target = damageEffect.transform.parent.gameObject;
        Vector3 pos = target.transform.localPosition;

        for(int i = 0; i < duration; i++)
        {
            float x = pos.x + Random.Range(-1f, 1f) * magnitude;
            float y = pos.y + Random.Range(-1f, 1f) * magnitude;

            target.transform.localPosition = new Vector3(x, y, pos.z);

            yield return null;
        }
    }

    private IEnumerator WaitAnim(int value)
    {
        for (int i = 0; i < value; i++)
        {
            yield return null;
        }
    }
    public IEnumerator Counter()
    {
        cutInAnim.SetTrigger("Counter");
        yield return StartCoroutine(WaitAnim(ANIM_COOLTIME));
    }
    public IEnumerator Skill()
    {
        cutInAnim.SetTrigger("Skill");
        yield return StartCoroutine(WaitAnim(ANIM_COOLTIME));
        StartCoroutine(MessageManager.instance.DisplayMessage("ソウルバースト！決まったね！"));
    }

    public IEnumerator Evolution()
    {
        cutInAnim.SetTrigger("Evolution");
        StartCoroutine(MessageManager.instance.DisplayMessage("変身できるよ！変身しよう！"));
        yield return StartCoroutine(WaitAnim(ANIM_COOLTIME));
    }
    public IEnumerator SolveEvolution()
    {
        cutInAnim.SetTrigger("SolveEvolution");
        yield return StartCoroutine(WaitAnim(ANIM_COOLTIME));
    }
}
