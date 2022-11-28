using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIChanger : MonoBehaviour
{
    [System.Serializable]
    private class UIData
    {
        [SerializeField, Header("表示先")] 
        public Image displayImage = null;
        [SerializeField, Header("赤、青、黄")]
        public Sprite[] displaySprite = null;
    }
    [System.Serializable]
    private class SpriteData
    {
        [SerializeField, Header("表示先")]
        public SpriteRenderer displayObj = null;
        [SerializeField, Header("赤、青、黄")]
        public Sprite[] displaySprite = null;
    }
    [System.Serializable]
    private class TextData
    {
        [SerializeField, Header("表示先")]
        public Text displayText = null;
        [SerializeField, Header("赤、青、黄"), Multiline]
        public string[] displayString = null;
    }

    [SerializeField, Header("キャラに応じてUIを表示")]
    private List<UIData> uiData = new List<UIData>();
    [SerializeField, Header("キャラに応じて2DSpriteObjを表示")]
    private List<SpriteData> spriteData = new List<SpriteData>();
    [SerializeField, Header("キャラに応じてテキストを表示")]
    private List<TextData> textData = new List<TextData>();

    private void Start()
    {
        SetUI(GameObject.FindWithTag("GameManager").GetComponent<GameManager>().Type);
    }

    /// <summary>
    /// キャラクターに応じてＵＩを変更
    /// </summary>
    /// <param name="type">キャラの種類</param>
    private void SetUI(int type)
    {
        foreach(UIData data in uiData)
        {
            data.displayImage.sprite = data.displaySprite[type]; 
        }
        foreach (SpriteData data in spriteData)
        {
            data.displayObj.sprite = data.displaySprite[type];
        }
        foreach (TextData data in textData)
        {
            data.displayText.text = data.displayString[type];
        }
    }
}
