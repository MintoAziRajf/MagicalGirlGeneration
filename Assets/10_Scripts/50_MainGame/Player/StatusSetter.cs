﻿using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class StatusSetter : PlayerManager
{
    private int PLAYER_TYPE = 0;
    [SerializeField] private TextAsset statusCSV = null;
    List<string[]> statusData = new List<string[]>();
    private enum STATUS
    {
        NOR_HP = 1,
        EVO_HP = 2,
        ATTACK_FREQ = 3,
        NOR_DAMAGE = 4,
        EVO_DAMAGE = 5,
        SKILL_FREQ = 6,
        SKILL_DAMAGE = 7,
        CA_FREQ = 8,
        CA_DAMAGE = 9,
        MOVE_COOLTIME = 10,
        ATTACK_LINE = 11
    }

    private void Start()
    {
        LoadStatus();
    }
    private void LoadStatus()
    {
        StringReader reader = new StringReader(statusCSV.text);

        string line = null;
        line = reader.ReadLine(); //見出し行をスキップする
        while (reader.Peek() != -1) // reader.Peekが-1になるまで
        {
            line = reader.ReadLine(); // 一行ずつ読み込み
            statusData.Add(line.Split(',')); // , 区切りでリストに追加
        }
        reader.Close();
        SetStatus();
    }

    private void SetStatus()
    {
        PLAYER_TYPE = gameManager.Type;
        
        playerHP.NormalHP = ReturnStatus(STATUS.NOR_HP);
        playerHP.EvoHP = ReturnStatus(STATUS.EVO_HP);
        playerController.AttackFreq = ReturnStatus(STATUS.ATTACK_FREQ);
        playerController.DamageNoraml = ReturnStatus(STATUS.NOR_DAMAGE);
        playerController.DamageEvolution = ReturnStatus(STATUS.EVO_DAMAGE);
        playerController.DamageSkill = ReturnStatus(STATUS.SKILL_DAMAGE);
        playerController.SkillFreq = ReturnStatus(STATUS.SKILL_FREQ);
        playerController.DamageCounterAttack = ReturnStatus(STATUS.CA_DAMAGE);
        playerController.CounterAttackFreq = ReturnStatus(STATUS.CA_FREQ);
        playerController.MoveCooltime = ReturnStatus(STATUS.MOVE_COOLTIME);
        playerController.AttackType = ReturnStatus(STATUS.ATTACK_LINE);
    }
    private int ReturnStatus(STATUS s)
    {
        int value = int.Parse(statusData[PLAYER_TYPE][(int)s]);
        return value;
    }

}
