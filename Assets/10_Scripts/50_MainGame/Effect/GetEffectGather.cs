using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetEffectGather : MonoBehaviour
{
    private Vector3 _velocity;
    private Vector3 _position;
    private Vector3 _targetPos;
    private float _period = 1f;
    private bool _isStart = false;
    private float _speed = 50f;
    public void SetTarget(Vector3 pos, float speed)
    {
        _targetPos = pos;
        _period = 50f / speed;
        _speed = speed;
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
        Vector3 _diff = _targetPos - _position;
        acceleration += (_diff - _velocity * _period) * 2f / (_period * _period);

        _period -= Time.deltaTime;
        if (_period < 0f)
        {
            Destroy(this.gameObject);
            return;
        }

        _velocity += acceleration.normalized * Time.deltaTime * _speed;
        _position += _velocity * Time.deltaTime;
        transform.position = _position;
    }

    private void CircleVelocity()
    {
        float angle = Random.Range(0f, 360f);
        float radius = Random.Range(10f, 15f);
        _velocity = new Vector3(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle), 0f);
        //Debug.Log(_velocity);
    }
}
