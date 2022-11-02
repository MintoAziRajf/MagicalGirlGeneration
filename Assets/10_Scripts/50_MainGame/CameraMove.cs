using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] private float magnitude = 0f;
    [SerializeField] private float speed = 0f;
    private Vector3 startPos;
    private Vector3 endPos;
    private void Awake()
    {
        startPos = this.transform.position;
    }
    private void FixedUpdate()
    {
        if(this.transform.position == startPos + endPos)
        {
            endPos = new Vector3(Random.Range(-magnitude, magnitude), Random.Range(-magnitude, magnitude), Random.Range(-magnitude, magnitude));
        }
        this.transform.position = Vector3.MoveTowards(this.transform.position, startPos + endPos, speed * Time.deltaTime);
    }
}
