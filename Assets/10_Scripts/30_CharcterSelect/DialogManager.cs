using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DialogManager : MonoBehaviour
{
    CharacterSelectManager characterSelectManager;
    private void Awake()
    {
        characterSelectManager = GameObject.Find("CharacterSelectManager").GetComponent<CharacterSelectManager>();
    }
    public void ActivateDialog(int value)
    {
        bool isActivate = false;
        if (value == 0) isActivate = false;
        else if (value == 1) isActivate = true;
        else Debug.Log("Error");
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
