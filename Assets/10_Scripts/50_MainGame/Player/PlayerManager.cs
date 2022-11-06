﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    GameObject gameManagerObj;

    [HideInInspector] public GameManager gameManager;
    [HideInInspector] public CollisionManager collisionManager;

    GameObject enemyObj;

    [HideInInspector] public EnemyManager enemyManager;

    [HideInInspector] public PlayerHP playerHP;
    [HideInInspector] public PlayerSkill playerSkill;
    [HideInInspector] public PlayerController playerController;
    [HideInInspector] public Evolution evolution;
    [HideInInspector] public GameUI gameUI;
    [HideInInspector] public CutIn cutIn;

    public void Awake()
    {
        gameManagerObj = GameObject.Find("GameManager");
        gameManager = gameManagerObj.GetComponent<GameManager>();
        collisionManager = gameManagerObj.GetComponent<CollisionManager>();

        enemyObj = GameObject.Find("Enemy");
        enemyManager = enemyObj.GetComponent<EnemyManager>();

        playerHP = this.GetComponent<PlayerHP>();
        playerSkill = this.GetComponent<PlayerSkill>();
        playerController = this.GetComponent<PlayerController>();
        evolution = this.GetComponent<Evolution>();
        gameUI = this.GetComponent<GameUI>();
        cutIn = this.GetComponent<CutIn>();
    }
}