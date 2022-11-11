using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealGather : MonoBehaviour
{
    private Vector3 velocity;
    private Vector3 position;
    Transform target;
    private float period = 1f;
    private bool isStart = false;
    [SerializeField] private float radius = 0f;
    private float rot = 0f;
    public float Rot { set { rot = value; AwakeScript(); } }

    private void AwakeScript()
    {
        position = this.transform.position;
        CircleVelocity();
        target = GameObject.Find("Cube").transform;
    }
    int time = 0;
    private void Update()
    {
        if (target == null) return;
        var acceleration = Vector3.zero;

        var diff = target.position - position;
        acceleration += (diff - velocity * period) * 2f / (period * period);

        period -= Time.deltaTime;
        if (period < 0f)
        {
            Destroy(this.gameObject);
            return;
        }

        velocity += acceleration * Time.deltaTime;
        position += velocity * Time.deltaTime;
        transform.position = position;
    }

    private void CircleVelocity()
    {
        velocity = new Vector3(radius * Mathf.Cos(rot), radius * Mathf.Sin(rot), 0f);
        Debug.Log(velocity);
    }
}
