using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbFloating : MonoBehaviour
{
    [SerializeField] private float height = 0f;
    [SerializeField] private float speed = 0f;
    [SerializeField] private float angle = 0f;

    private float startHeight = 0f;
    private float currentHeight = 0f;
    private void Awake()
    {
        startHeight = this.transform.position.y;
    }
    private void FixedUpdate()
    {
        this.transform.Rotate(new Vector3(0f, 0f, angle * Time.deltaTime));
        if(Mathf.Abs(currentHeight) >= height)
        {
            speed *= -1f;
        }
        currentHeight += speed * Time.deltaTime;
        this.transform.position = new Vector3(this.transform.position.x, startHeight + currentHeight, this.transform.position.z);
    }
}
