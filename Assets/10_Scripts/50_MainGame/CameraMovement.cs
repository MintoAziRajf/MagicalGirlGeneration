using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private float magnitude = 0f; // 振動の強さ
    [SerializeField] private float speed = 0f; // 速度
    private Vector3 startPos;
    private Vector3 endPos;
    private void Awake()
    {
        startPos = this.transform.position;
    }
    private void FixedUpdate()
    {
        // 移動しきったら再計算
        if(this.transform.position == startPos + endPos) 
        {
            endPos = new Vector3(Random.Range(-magnitude, magnitude), Random.Range(-magnitude, magnitude), Random.Range(-magnitude, magnitude));
        }
        this.transform.position = Vector3.MoveTowards(this.transform.position, startPos + endPos, speed * Time.deltaTime); // 移動
    }
}
