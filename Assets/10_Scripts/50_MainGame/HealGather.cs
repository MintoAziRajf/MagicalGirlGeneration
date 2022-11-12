using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealGather : MonoBehaviour
{
    private Vector3 _velocity;
    private Vector3 _position;
    private Vector3 _targetPos;
    private float _period = 1f;
    private bool _isStart = false;
    private float _radius = 0f;
    private float _angle = 0f;
    private Vector3 _diff;
    private float speed = 50f;
    public void SetTarget(Vector3 pos, float angle, float radius)
    {
        _radius = radius;
        _targetPos = pos;
        _angle = angle;
        AwakeScript();
    }

    private void AwakeScript()
    {
        _position = this.transform.position;
        CircleVelocity();
        
        _isStart = true;
    }

    private void Update()
    {
        if (!_isStart) return;
        var acceleration = Vector3.zero;
        _diff = _targetPos - _position;
        acceleration += (_diff - _velocity * _period) * 2f / (_period * _period);

        _period -= Time.deltaTime;
        if ((this.transform.position - _targetPos).magnitude <= 0.1f)
        {
            Destroy(this.gameObject);
            return;
        }

        _velocity += acceleration.normalized * Time.deltaTime * speed;
        _position += _velocity * Time.deltaTime;
        transform.position = _position;
    }

    private void CircleVelocity()
    {
        _velocity = new Vector3(_radius * Mathf.Cos(_angle), _radius * Mathf.Sin(_angle), 0f);
        //Debug.Log(_velocity);
    }
}
