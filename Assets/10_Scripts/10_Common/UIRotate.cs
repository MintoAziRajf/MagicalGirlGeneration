using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRotate : MonoBehaviour
{
    [SerializeField] private float speed = 0f;

    private void FixedUpdate()
    {
        this.transform.Rotate(0f, 0f, speed);
    }
}
