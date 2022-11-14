﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    PlayerController playerController;

    private int[,] collisionGrid = new int[3, 3];
    private const int EMPTY = 0;
    private const int PLAYER = 1;
    private const int AVOID = 2;
    private const int DAMAGE = 3;

    [SerializeField] private SpriteRenderer[] gridObj = new SpriteRenderer[9];
    [Header("EMPTY,PLAYER,AVOID,DAMAGE")] [SerializeField] private Color[] gridColor = new Color[4];
    [SerializeField] private GameObject avoidObj = null;
    private void Awake()
    {
        InitGrid();
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    }

    public IEnumerator DamageGrid(int x, int y, int power, int frame)
    {
        SoundManager.instance.PlaySE(SoundManager.SE_Type.EnemyAttack);
        //生成先にプレイヤーがいたらダメージを与える
        if (collisionGrid[x, y] == PLAYER)
        {
            PlayerDamaged(power);
            Debug.Log("プレイヤーに" + power + "のダメージ");
            yield break;
        }
        //生成先に回避判定があったら成功判定を送る
        if (collisionGrid[x, y] == AVOID)
        {
            //AvoidSuccess;
            StartCoroutine(playerController.AvoidSuccess());
            collisionGrid[x, y] = EMPTY;
            Debug.Log("回避成功!");
            yield break;
        }

        //ダメージフィールド生成
        collisionGrid[x, y] = power;
        //持続
        for (int i = 0; i < frame; i++)
        {
            yield return null;
        }
        //ダメージフィールド削除
        if(collisionGrid[x, y] == power)
        {
            collisionGrid[x, y] = EMPTY;
        }
    }

    /// <summary>
    /// プレイヤーが移動したら呼び出されます
    /// </summary>
    /// <param name="x">移動先X</param>
    /// <param name="y">移動先Y</param>
    public void PlayerMoved(int x, int y)
    {
        ReplaceCollision(EMPTY);
        MoveCheck(x, y);
    }
    /// <summary>
    /// プレイヤーが回避したら呼び出されます
    /// </summary>
    /// <param name="x">回避タイルの生成先X</param>
    /// <param name="y">回避タイルの生成先Y</param>
    /// <param name="frame">持続時間</param>
    public void PlayerAvoided(int x, int y, int frame)
    {
        StartCoroutine(AvoidedCollision(x, y, frame));
    }
    private IEnumerator AvoidedCollision(int x, int y, int frame)
    {
        collisionGrid[x, y] = AVOID;
        GameObject playerAvoid = Instantiate(avoidObj, gridObj[x+y*3].transform);
        //持続
        for (int i = 0; i < frame; i++)
        {
            yield return null;
        }
        Destroy(playerAvoid);
        collisionGrid[x, y] = EMPTY;
    }

    private void ReplaceCollision(int value)
    {
        //Playerがいた場所に指定の値を入れる
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (collisionGrid[i, j] == PLAYER) collisionGrid[i, j] = value; 
            }
        }
    }

    private void MoveCheck(int x, int y)
    {
        //移動先がダメージフィールドならダメージ
        if (collisionGrid[x, y] >= DAMAGE)
        {
            PlayerDamaged(collisionGrid[x, y]);
        }
        //移動先にプレイヤーを置く
        collisionGrid[x, y] = PLAYER;
        
    }

    private void PlayerDamaged(int value)
    {
        playerController.StartCoroutine(playerController.Damaged(value));
    }

    private void Update()
    {
        DisplayGrid();
    }

    private void DisplayGrid()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                int index = i + j * 3;
                int color = collisionGrid[i, j];
                if (color >= DAMAGE) color = 1;
                else color = EMPTY;
                gridObj[index].color = gridColor[color];
            }
        }
    }

    private void InitGrid()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                collisionGrid[i, j] = EMPTY;
            }
        }
    }
}
