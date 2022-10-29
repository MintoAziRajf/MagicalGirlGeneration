using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    private int[,] collisionGrid = new int[3, 3];
    private const int EMPTY = 0;
    private const int PLAYER = 1;
    private const int AVOID = 2;
    private const int DAMAGE = 3;
    private bool needReplace = false;

    private const int DAMAGE_FRAME = 5;

    [SerializeField] private SpriteRenderer[] gridObj = new SpriteRenderer[9];
    [Header("EMPTY,PLAYER,AVOID,DAMAGE")][SerializeField] private Color[] gridColor = new Color[4];
    private void Awake()
    {
        InitGrid();
    }
 
    public void DamageGrid(int x, int y, int power)
    {
        StartCoroutine(DamageCollision(x, y, power));
    }
    private IEnumerator DamageCollision(int x, int y,int power)
    {
        //生成先にプレイヤーがいたらダメージを与える
        if (collisionGrid[x, y] == PLAYER)
        {
            PlayerDamaged(collisionGrid[x, y]);
            yield break;
        }
        //生成先に回避判定があったら成功判定を送る
        if (collisionGrid[x, y] == AVOID)
        {
            //EvationSuccess;
            yield break;
        }

        //ダメージフィールド生成
        collisionGrid[x, y] = power;
        //持続
        for (int i = 0; i < DAMAGE_FRAME; i++)
        {
            yield return null;
        }
        //ダメージフィールド削除
        collisionGrid[x, y] = EMPTY;
    }

    /// <summary>
    /// プレイヤーが移動したら呼び出されます
    /// </summary>
    /// <param name="x">移動先X</param>
    /// <param name="y">移動先Y</param>
    public void PlayerMoved(int x, int y)
    {
        if (collisionGrid[x, y] == AVOID) needReplace = true;
        ReplaceCollision(EMPTY);
        MoveCheck(x, y);
    }
    /// <summary>
    /// プレイヤーが回避したら呼び出されます
    /// </summary>
    /// <param name="x">回避タイルの生成先X</param>
    /// <param name="y">回避タイルの生成先Y</param>
    /// <param name="frame">移動先Y</param>
    public void PlayerAvoided(int x, int y, int frame)
    {
        ReplaceCollision(AVOID);
        StartCoroutine(AvoidedCollision(x, y, frame));
    }
    private IEnumerator AvoidedCollision(int x, int y, int frame)
    {
        int value;
        //持続
        for (int i = 0; i < frame; i++)
        {
            yield return null;
        }

        if (needReplace)
        {
            value = PLAYER;
            needReplace = false;
        }
        else
        {
            value = EMPTY;
        }

        collisionGrid[x, y] = value;
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
        if (needReplace) return;
        
        collisionGrid[x, y] = PLAYER;
    }

    private void PlayerDamaged(int value)
    {

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
                gridObj[index].color = gridColor[collisionGrid[i, j]];
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
