using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    EnemyManager enemyManager;
    PlayerController playerController;
    Tutorial tutorial;
    Prologue prologue;

    [SerializeField] private GameObject player = null;
    [SerializeField] private GameObject enemy = null;
    [SerializeField] private GameObject pauseCanvas = null;

    [SerializeField] private bool isTutorial = false;
    public bool IsTutorial { set { isTutorial = value; } }
    [SerializeField] private bool isPrologue = false;
    public bool IsPrologue { set { isPrologue = value; } }
    [SerializeField] private int type = 0;
    public int Type
    {
        set { type = value; }
        get { return type; }
    }

    private void Awake()
    {
        playerController = player.GetComponent<PlayerController>();
        enemyManager = enemy.GetComponent<EnemyManager>();
        tutorial = this.GetComponent<Tutorial>();
        prologue = this.GetComponent<Prologue>();
        Time.timeScale = 0f;
        StartCoroutine(GameStart());
    }

    private void FixedUpdate()
    {
        if (Input.GetButtonDown("Option"))
        {
            pauseCanvas.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    private IEnumerator Tutorial()
    {
        if (!isTutorial)
        {
            yield break;
        }
        yield return StartCoroutine(tutorial.Move());
    }

    private IEnumerator Prologue()
    {
        if (!isPrologue)
        {
            yield break;
        }
        yield return StartCoroutine(prologue.StartPrologue(type));
    }

    private IEnumerator GameStart()
    {
        yield return StartCoroutine(Prologue());
        yield return StartCoroutine(Tutorial());
        Time.timeScale = 1f;
        playerController.IsStart = true;
        enemyManager.IsStart = true;
        enemyManager.StartAttack();
    }

    public void GameOver()
    {

    }

    public void GameClear()
    {

    }
}
