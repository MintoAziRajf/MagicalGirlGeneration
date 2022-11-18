using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFlash : MonoBehaviour
{
    [SerializeField] private float speed = 0f;
    [SerializeField,Range(0f,1f)] private float platform = 0f;
    private Image myImage;
    private void Awake()
    {
        myImage = this.GetComponent<Image>();
        
    }

    private void FixedUpdate()
    {
        Color myColor = myImage.color;
        myColor.a = platform + Mathf.Sin(Time.time*speed) * (1f - platform);
        myImage.color = myColor;
    }
}
