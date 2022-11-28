using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DebugManager : MonoBehaviour
{
    private GameManager gameManager;
    private PlayerController playerController;
    private bool canInput = true; //入力可能か

    private void OnEnable()
    {
        if (gameManager == null) gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        if (playerController == null) playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    }

    void Update()
    {
        DebugInput();
        DebugDisplay(); 
    }

    [SerializeField] private RectTransform selectObj = null;
    private void DebugDisplay()
    {
        var pos = selectObj.anchoredPosition;
        var selectPos = buttons[currentSelect].rectTransform.anchoredPosition;
        selectObj.anchoredPosition = new Vector3(pos.x, selectPos.y);
    }

    private string[] invincible = { "OFF", "ON" };
    private string[] evoGauge = { "0%", "99%" };
    private string[] allCooltime = { "0%", "100%" };
    private string[] playerHP = { "1%", "100%" };
    private string[] enemyHP = { "1%", "100%" };

    [SerializeField] private Text[] buttons = null;
    private int currentSelect = 0;
    private enum DEBUG
    {
        INVINCIBLE,
        EVOLUTION,
        COOLTIME,
        PLAYER_HP,
        ENEMY_HP,
        EXIT
    }
    /// <summary>
    /// ボタンの役割
    /// </summary>
    private void DebugInput()
    {
        bool up = Input.GetAxisRaw("Vertical") == 1f && canInput;
        bool down = Input.GetAxisRaw("Vertical") == -1f && canInput;
        bool right = Input.GetAxisRaw("Horizontal") == 1f && canInput;
        bool left = Input.GetAxisRaw("Horizontal") == -1f && canInput;
        bool submit = Input.GetButtonDown("Submit") && canInput;

        if (up)
        {
            currentSelect = Mathf.Clamp(--currentSelect, 0, buttons.Length - 1);
            StartCoroutine(WaitInput());
        }
        if (down)
        {
            currentSelect = Mathf.Clamp(++currentSelect, 0, buttons.Length - 1);
            StartCoroutine(WaitInput());
        }
        if (right || left)
        {
            StartCoroutine(WaitInput());
            bool select = right;
            switch (currentSelect)
            {
                case (int)DEBUG.INVINCIBLE://無敵
                    playerController.SetInvincible(select);
                    buttons[currentSelect].text = invincible[Convert.ToInt32(select)];
                    break;
                case (int)DEBUG.EVOLUTION://進化ゲージ調整
                    playerController.SetEvolution(select);
                    buttons[currentSelect].text = evoGauge[Convert.ToInt32(select)];
                    break;
                case (int)DEBUG.COOLTIME: // クールタイム調整
                    playerController.SetCooltime(select);
                    buttons[currentSelect].text = allCooltime[Convert.ToInt32(select)];
                    break;
                case (int)DEBUG.PLAYER_HP: // プレイヤーHP調整
                    playerController.SetPlayerHP(select);
                    buttons[currentSelect].text = playerHP[Convert.ToInt32(select)];
                    break;
                case (int)DEBUG.ENEMY_HP: // エネミーHP調整
                    playerController.SetEnemyHP(select);
                    buttons[currentSelect].text = enemyHP[Convert.ToInt32(select)];
                    break;
                case (int)DEBUG.EXIT:
                    break;
                default:
                    break;
            }
        }
        if (submit && currentSelect == (int)DEBUG.EXIT)
        {

            Unpause();
        }
    }
    private void Unpause()
    {
        gameManager.StartCoroutine(gameManager.StartCountDown());
        this.gameObject.SetActive(false);
    }

    private IEnumerator WaitInput()
    {
        canInput = false;
        for(int i = 0; i < 10; i++)
        {
            yield return null;
        }
        canInput = true;
    }
}
