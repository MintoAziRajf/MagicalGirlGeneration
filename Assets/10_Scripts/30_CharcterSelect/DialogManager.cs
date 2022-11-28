using UnityEngine;
using System;

public class DialogManager : MonoBehaviour
{
    CharacterSelectManager characterSelectManager;
    private void Awake()
    {
        characterSelectManager = GameObject.Find("CharacterSelectManager").GetComponent<CharacterSelectManager>();
    }
    /// <summary>
    /// ダイアログを表示/非表示
    /// </summary>
    public void ActivateDialog(int value)
    {
        bool isActivate = Convert.ToBoolean(value);
        Debug.Log(isActivate);
        characterSelectManager.IsDialog = isActivate;
    }
    /// <summary>
    /// プロローグ選択
    /// </summary>
    public void PrologueSelect(bool select)
    {
        characterSelectManager.IsPrologue = select;
    }
    /// <summary>
    /// チュートリアル選択
    /// </summary>
    public void TutorialSelect(bool select)
    {
        characterSelectManager.IsTutorial = select;
    }
    /// <summary>
    /// キャラクター選択画面終了
    /// </summary>
    public void EndCharacterSelect()
    {
        characterSelectManager.LoadMainGame();
    }
}
