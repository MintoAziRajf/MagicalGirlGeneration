using UnityEngine;

public class UIRotate : MonoBehaviour
{
    [SerializeField] private float speed = 0f; // 回転する速さ

    private void FixedUpdate() => this.transform.Rotate(0f, 0f, speed); // 毎フレーム回転させる
}
