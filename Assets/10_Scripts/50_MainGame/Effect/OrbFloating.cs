using UnityEngine;

public class OrbFloating : MonoBehaviour
{
    [SerializeField] private float height = 0f; // 浮遊上限
    [SerializeField] private float speed = 0f; // 浮遊速度
    [SerializeField] private float angle = 0f; // 回転速度

    private float startHeight = 0f;
    private float currentHeight = 0f;
    private void Awake()
    {
        startHeight = this.transform.position.y;
    }
    private void FixedUpdate()
    {
        this.transform.Rotate(new Vector3(0f, 0f, angle * Time.deltaTime)); // 回転
        if(Mathf.Abs(currentHeight) >= height) // 上限に達したら反転
        {
            speed *= -1f;
        }
        currentHeight += speed * Time.deltaTime;
        this.transform.position = new Vector3(this.transform.position.x, startHeight + currentHeight, this.transform.position.z);
    }
}
