using UnityEngine;

public class UIFloating : MonoBehaviour
{
    [SerializeField] private float speed = 0f; // 浮遊ループする速さ
    [SerializeField] private float limit = 0f; // 高さ上限
    Vector2 pos;
    private void Awake() => pos = this.GetComponent<RectTransform>().anchoredPosition;// 現在のポジションを取得
    private void Update()
    {
        // ポジションを計算しセットする
        this.GetComponent<RectTransform>().anchoredPosition = 
            new Vector2(pos.x,pos.y + Mathf.Sin(Time.unscaledTime * speed) * limit);
    }
}
