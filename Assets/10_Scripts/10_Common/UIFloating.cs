using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFloating : MonoBehaviour
{
    [SerializeField] private float speed = 0f;
    [SerializeField] private float limit = 0f;
    Vector2 pos;
    private void Awake()
    {
        pos = this.GetComponent<RectTransform>().anchoredPosition;
    }
    private void Update()
    {
        this.GetComponent<RectTransform>().anchoredPosition = new Vector2(pos.x,pos.y + Mathf.Sin(Time.unscaledTime * speed) * limit);
    }
}
