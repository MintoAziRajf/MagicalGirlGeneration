using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusSetter : MonoBehaviour
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
        MOVE_COOLTIME = 8,
        ATTACK_LINE = 9
    }
    private void Awake()
    {

    }

    private void SetHP()
    {

    }
}
