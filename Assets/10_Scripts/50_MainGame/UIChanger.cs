using System.Collections;
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

    [SerializeField, Header("キャラに応じてUIを表示")]
    private List<UIData> uiData = new List<UIData>();
    [SerializeField, Header("キャラに応じて2DSpriteObjを表示")]
    private List<SpriteData> spriteData = new List<SpriteData>();

    private void Awake()
    {
        SetUI(GameObject.FindWithTag("GameManager").GetComponent<GameManager>().Type);
    }

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
    }
}
