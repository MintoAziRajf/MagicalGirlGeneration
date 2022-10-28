using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutIn : MonoBehaviour
{
    [SerializeField] GameObject cutInObj = null;
    private Animator anim;
    private const int ANIM_COOLTIME = 120;
    private void Awake()
    {
        anim = cutInObj.GetComponent<Animator>();
        anim.SetInteger("Type", DataStorage.instance.PlayerType);
    }
    public IEnumerator Attack(bool b)
    {
        if (b) anim.SetTrigger("EvoAttack");
        else anim.SetTrigger("Attack");

        yield return StartCoroutine(WaitAnim());
        StartCoroutine(MessageManager.instance.DisplayMessage("ナイス攻撃！"));
    }


    private IEnumerator WaitAnim()
    {
        for (int i = 0; i < ANIM_COOLTIME; i++)
        {
            yield return null;
        }
    }
    public IEnumerator Counter()
    {
        anim.SetTrigger("Counter");
        yield return StartCoroutine(WaitAnim());
    }
    public IEnumerator Skill()
    {
        anim.SetTrigger("Skill");
        yield return StartCoroutine(WaitAnim());
        StartCoroutine(MessageManager.instance.DisplayMessage("ソウルバースト！決まったね！"));
    }

    public IEnumerator Evolution()
    {
        anim.SetTrigger("Evolution");
        StartCoroutine(MessageManager.instance.DisplayMessage("変身できるよ！変身しよう！"));
        yield return StartCoroutine(WaitAnim());
    }
    public IEnumerator SolveEvolution()
    {
        anim.SetTrigger("SolveEvolution");
        yield return StartCoroutine(WaitAnim());
    }
}
