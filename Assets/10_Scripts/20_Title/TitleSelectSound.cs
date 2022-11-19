using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleSelectSound : MonoBehaviour
{
    void OnEnable()
    {
        SoundManager.instance.PlaySE(SoundManager.SE_Type.Select); //se
    }
}
