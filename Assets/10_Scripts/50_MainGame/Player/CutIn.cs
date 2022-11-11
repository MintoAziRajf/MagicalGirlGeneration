using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutIn : PlayerManager
{
    [SerializeField] GameObject cutInObj = null;
    [SerializeField] GameObject damageEffect = null;
    private Animator cutInAnim;
    private Animator damageAnim;
    private const int ANIM_COOLTIME = 120;
    private const int ATTACK_COOLTIME = 20;
    private const float ONE_FRAME = 1f / 60f;

    private void Start()
    {
        cutInAnim = cutInObj.GetComponent<Animator>();
        damageAnim = damageEffect.GetComponent<Animator>();
        damageAnim.SetInteger("Type", gameManager.Type);
        cutInAnim.SetInteger("Type", gameManager.Type);
    }
    

    public IEnumerator Attack(bool b)
    {
        if (b) damageAnim.SetTrigger("EvoAttack");
        else damageAnim.SetTrigger("Attack");

        yield return StartCoroutine(WaitAnim(ATTACK_COOLTIME));
        StartCoroutine(MessageManager.instance.DisplayMessage("ナイス攻撃！"));
    }


    private IEnumerator WaitAnim(int value)
    {
        for (int i = 0; i < value; i++)
        {
            yield return new WaitForSeconds(ONE_FRAME);
        }
    }
    public IEnumerator CounterAttack()
    {
        cutInAnim.SetTrigger("Counter");
        yield return StartCoroutine(WaitAnim(ANIM_COOLTIME));
        damageAnim.SetTrigger("Counter");
        StartCoroutine(MessageManager.instance.DisplayMessage("ナイスカウンター！"));
    }
    public IEnumerator Skill()
    {
        cutInAnim.SetTrigger("Skill");
        yield return StartCoroutine(WaitAnim(ANIM_COOLTIME));
        damageAnim.SetTrigger("Skill");
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
        Debug.Log("a");
        cutInAnim.SetTrigger("SolveEvolution");
        yield return StartCoroutine(WaitAnim(ANIM_COOLTIME));
    }
}
