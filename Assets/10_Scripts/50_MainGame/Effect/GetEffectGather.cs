using UnityEngine;

public class GetEffectGather : MonoBehaviour
{
    private Vector3 velocity;
    private Vector3 position;
    private Vector3 targetPos;
    private float period = 1f; // 収縮時間
    private bool isStart = false;
    private float speed = 50f; // 基本速度

    // 収縮先と速度をセット
    public void SetTarget(Vector3 pos, float spe)
    {
        targetPos = pos;
        period = 50f / spe;
        this.speed = spe;
        AwakeScript();
    }

    private void AwakeScript()
    {
        position = this.transform.position;
        CircleVelocity(); // 弾けさせる
        
        isStart = true; // 収縮スタート
    }

    private void Update()
    {
        if (!isStart) return;
        var acceleration = Vector3.zero;
        Vector3 diff = targetPos - position; // 収縮先へのベクトル
        acceleration += (diff - velocity * period) * 2f / (period * period); // 加速度計算

        period -= Time.deltaTime;
        // 時間が０になったら削除
        if (period < 0f)
        {
            Destroy(this.gameObject);
            return;
        }

        velocity += acceleration.normalized * Time.deltaTime * speed; // 速度
        position += velocity * Time.deltaTime; // ポジション変更
        transform.position = position;
    }

    /// <summary>
    /// 開始した時に弾けさせる
    /// </summary>
    private void CircleVelocity()
    {
        float angle = Random.Range(0f, 360f);
        float radius = Random.Range(10f, 15f);
        velocity = new Vector3(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle), 0f);
        //Debug.Log(_velocity);
    }
}
