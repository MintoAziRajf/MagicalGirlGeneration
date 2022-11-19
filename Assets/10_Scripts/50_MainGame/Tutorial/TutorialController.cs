using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    private const int TUTORIAL_COOLTIME = 30;

    private GameObject player;
    private PlayerController playerController;
    private EnemyManager enemyManager;
    private Animator anim;

    private void Awake()
    {
        anim = this.GetComponent<Animator>();
        player = GameObject.Find("Player").gameObject;
        playerController = player.GetComponent<PlayerController>();
    }
    private IEnumerator WaitGuide()
    {
        yield return new WaitUntil(() => Input.GetButtonDown("Submit"));
        anim.SetTrigger("Next");
        for (int i = 0; i < TUTORIAL_COOLTIME; i++)
        {
            yield return null;
        }
    }
    public IEnumerator Move()
    {
        yield return StartCoroutine(WaitGuide());
        yield return StartCoroutine(playerController.TutorialMove());
        anim.SetTrigger("Next");
    }

    public IEnumerator EnemyAttack()
    {
        yield return StartCoroutine(WaitGuide());
        yield return StartCoroutine(enemyManager.TutorialAttack());
        anim.SetTrigger("Next");
    }

    public IEnumerator Attack()
    {
        yield return StartCoroutine(WaitGuide());
        yield return StartCoroutine(playerController.TutorialAttack());
        anim.SetTrigger("Next");
    }
    public IEnumerator Evolution()
    {
        yield return StartCoroutine(WaitGuide());
        //evo
        yield return StartCoroutine(playerController.TutorialEvolution());
        anim.SetTrigger("Next");
    }

    public IEnumerator Evolution2()
    {
        yield return StartCoroutine(WaitGuide());
        anim.SetTrigger("Next");
    }

    public IEnumerator Counter()
    {
        yield return StartCoroutine(WaitGuide());
        yield return StartCoroutine(playerController.TutorialCounter());
        anim.SetTrigger("Next");
    }

    public IEnumerator Skill()
    {
        yield return StartCoroutine(WaitGuide());
        yield return StartCoroutine(playerController.TutorialSkill());
        anim.SetTrigger("Next");
    }

    public IEnumerator TutorialEnd()
    {
        yield return StartCoroutine(WaitGuide());
        this.gameObject.SetActive(false);
    }
}
