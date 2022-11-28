using UnityEngine;
using UnityEngine.UI;

public class UIFlash : MonoBehaviour
{
    [SerializeField] private float speed = 0f; // 点滅する速さ
    [SerializeField,Range(0f,1f)] private float platform = 0f; // 最低値
    private Image myImage;
    private void Awake() => myImage = this.GetComponent<Image>(); // イメージコンポーネントを取得

    private void FixedUpdate()
    {
        Color myColor = myImage.color; // カラー取得
        myColor.a = platform + Mathf.Sin(Time.time*speed) * (1f - platform); // アルファ値を計算
        myImage.color = myColor; // カラーをセット
    }
}
