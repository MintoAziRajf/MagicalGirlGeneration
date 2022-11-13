using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class DialogManager : MonoBehaviour
{
    CharacterSelectManager characterSelectManager;
    private void Awake()
    {
        characterSelectManager = GameObject.Find("CharacterSelectManager").GetComponent<CharacterSelectManager>();
    }
    public void ActivateDialog(int value)
    {
        bool isActivate = Convert.ToBoolean(value);
        Debug.Log(isActivate);
        characterSelectManager.IsDialog = isActivate;
    }
    public void PrologueSelect(bool select)
    {
        characterSelectManager.IsPrologue = select;
    }
    public void TutorialSelect(bool select)
    {
        characterSelectManager.IsTutorial = select;
    }
    public void EndCharacterSelect()
    {
        characterSelectManager.LoadMainGame();
    }
}
