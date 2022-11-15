﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutIn : PlayerManager
{
    [SerializeField] GameObject cutInObj = null;
    [SerializeField] GameObject damageEffect = null;
    private Animator cutInAnim;
    private Animator damageAnim;
    private const int ANIM_COOLTIME = 60;
    private const int COUNTER_COOLTIME = 30;
    private const int ATTACK_COOLTIME = 15;
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
        yield return StartCoroutine(WaitAnim(COUNTER_COOLTIME));
        damageAnim.SetTrigger("Counter");
        StartCoroutine(MessageManager.instance.DisplayMessage("ナイスカウンター！"));
    }
    public IEnumerator Skill()
    {
        cutInAnim.SetTrigger("Skill");
        yield return StartCoroutine(WaitAnim(ANIM_COOLTIME));
        damageAnim.SetTrigger("Skill");
        StartCoroutine(MessageManager.instance.DisplayMessage("ソウルバースト！\n決まったね！"));
    }

    public IEnumerator Evolution()
    {
        cutInAnim.SetTrigger("Evolution");
        StartCoroutine(MessageManager.instance.DisplayMessage("さぁ、変身だ！"));
        yield return StartCoroutine(WaitAnim(ANIM_COOLTIME));
    }
    public IEnumerator SolveEvolution()
    {
        Debug.Log("a");
        cutInAnim.SetTrigger("SolveEvolution");
        yield return StartCoroutine(WaitAnim(ANIM_COOLTIME));
    }
}
