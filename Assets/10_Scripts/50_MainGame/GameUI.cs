using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] private Image moveDisplay = null;
    [SerializeField] private Image avoidDisplay = null;
    [SerializeField] private Image skillDisplay = null;
    [SerializeField] private Image hpDisplay = null;
    [SerializeField] private Image evolutionDisplay = null;
    private int moveCurrentTime = 0;
    public int MoveCurrentTime {
        set { moveCurrentTime = value;}
    }
    private int avoidCurrentTime = 0;
    public int AvoidCurrentTime {
        set { avoidCurrentTime = value;}
    }
    private int skillCurrentTime = 0;
    public int SkillCurrentTime {
        set { skillCurrentTime = value;}
    }
    private float evolutionCurrentGauge = 0f;
    public float EvolutionCurrentGauge {
        set { evolutionCurrentGauge = value;}
    }
    private float evolutionDisplayGauge = 0f;
    private float displayDelay = 20f;

    private int moveCooltime = 0;
    public int MoveCooltime {
        set { moveCooltime = value;}
    }

    private int avoidCooltime = 0;
    public int AvoidCooltime {
        set { avoidCooltime = value;}
    }

    private int skillCooltime = 0;
    public int SkillCooltime {
        set { skillCooltime = value;}
    }
    
    private float evolutionMax = 0f;
    public float EvolutionMax {
        set { evolutionMax = value;}
    }

    private int maxHP = 0;
    public int MaxHP {
        set { maxHP = value;}
    }

    void Update()
    {
        DisplayUI();
    }
    private void DisplayUI()
    {
        evolutionDisplayGauge = Mathf.MoveTowards(evolutionDisplayGauge, evolutionCurrentGauge, displayDelay * Time.deltaTime);
        moveDisplay.fillAmount = (float)moveCurrentTime / moveCooltime;
        avoidDisplay.fillAmount = (float)avoidCurrentTime / avoidCooltime;
        skillDisplay.fillAmount = (float)skillCurrentTime / skillCooltime;
        evolutionDisplay.fillAmount = evolutionDisplayGauge / evolutionMax;
    }
}
