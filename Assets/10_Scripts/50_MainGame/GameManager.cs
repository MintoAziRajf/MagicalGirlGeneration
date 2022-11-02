using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    EnemyManager enemyManager;
    PlayerController playerController;
    Tutorial tutorial;

    [SerializeField] private GameObject player = null;
    [SerializeField] private GameObject enemy = null;

    private void Awake()
    {
        playerController = player.GetComponent<PlayerController>();
        enemyManager = enemy.GetComponent<EnemyManager>();
        tutorial = GetComponent<Tutorial>();
        GameStart();
    }

    private IEnumerator Tutorial()
    {
        yield return StartCoroutine(tutorial.Move());
        GameStart();
    }

    private void GameStart()
    {
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
