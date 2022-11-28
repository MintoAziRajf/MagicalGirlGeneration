using UnityEngine;

public class UIShake : MonoBehaviour
{
    [SerializeField] private float magnitude = 0f; // 振動する強さ
    private float defaultX, defaultY; // 元の位置
    private RectTransform rectTransform; 
    private void Awake()
    {
        // 位置を取得
        rectTransform = GetComponent<RectTransform>();
        defaultX = rectTransform.position.x;
        defaultY = rectTransform.position.y;
    }
    private void FixedUpdate()
    {
        // 振動
        rectTransform.position = new Vector3(defaultX + Random.Range(-magnitude, magnitude), defaultY + Random.Range(-magnitude, magnitude), 0f);
    }
}
