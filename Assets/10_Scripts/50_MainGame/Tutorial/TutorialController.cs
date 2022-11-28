using System.Collections;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    private const int TUTORIAL_COOLTIME = 30; // チュートリアルのアニメーション時間

    private GameObject player;
    private PlayerController playerController;
    private EnemyManager enemyManager;
    private Animator anim;

    private void Awake()
    {
        anim = this.GetComponent<Animator>();
        player = GameObject.Find("Player").gameObject;
        enemyManager = GameObject.Find("Enemy").GetComponent<EnemyManager>();
        playerController = player.GetComponent<PlayerController>();
    }
    /// <summary>
    /// 決定ボタンでガイドを消す
    /// </summary>
    private IEnumerator WaitGuide()
    {
        yield return new WaitUntil(() => Input.GetButtonDown("Submit"));
        anim.SetTrigger("Next");
        SoundManager.instance.PlaySE(SoundManager.SE_Type.Submit);
        for (int i = 0; i < TUTORIAL_COOLTIME; i++)
        {
            yield return null;
        }
    }
    /// <summary>
    /// 移動ガイド⇒移動
    /// </summary>
    public IEnumerator Move()
    {
        yield return StartCoroutine(WaitGuide());
        yield return StartCoroutine(playerController.TutorialMove());
        anim.SetTrigger("Next");
    }
    /// <summary>
    /// 敵の攻撃ガイド⇒敵の攻撃
    /// </summary>
    public IEnumerator EnemyAttack()
    {
        yield return StartCoroutine(WaitGuide());
        yield return StartCoroutine(enemyManager.TutorialAttack());
        anim.SetTrigger("Next");
    }
    /// <summary>
    /// 攻撃ガイド
    /// </summary>
    public IEnumerator Attack()
    {
        yield return StartCoroutine(WaitGuide());
        yield return StartCoroutine(playerController.TutorialAttack());
        anim.SetTrigger("Next");
    }
    /// <summary>
    /// 変身ガイド
    /// </summary>
    public IEnumerator Evolution()
    {
        yield return StartCoroutine(WaitGuide());
        yield return StartCoroutine(playerController.TutorialEvolution());
        anim.SetTrigger("Next");
    }
    /// <summary>
    /// カウンターガイド
    /// </summary>
    public IEnumerator Counter()
    {
        yield return StartCoroutine(WaitGuide());
        yield return StartCoroutine(playerController.TutorialCounter());
        anim.SetTrigger("Next");
    }
    /// <summary>
    /// ひっさつわざ　ガイド
    /// </summary>
    public IEnumerator Skill()
    {
        yield return StartCoroutine(WaitGuide());
        yield return StartCoroutine(playerController.TutorialSkill());
        anim.SetTrigger("Next");
    }
    /// <summary>
    /// チュートリアル終了
    /// </summary>
    public IEnumerator TutorialEnd()
    {
        yield return StartCoroutine(WaitGuide());
        this.gameObject.SetActive(false);
    }
    /// <summary>
    /// 次のチュートリアルへ
    /// </summary>
    public IEnumerator TutorialNext()
    {
        yield return StartCoroutine(WaitGuide());
        anim.SetTrigger("Next");
    }
}
