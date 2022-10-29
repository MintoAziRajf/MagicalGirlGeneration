using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShake : MonoBehaviour
{
    [SerializeField] private float power = 0f;
    private float previousX, previousY;
    private RectTransform rectTransform;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        previousX = rectTransform.position.x;
        previousY = rectTransform.position.y;
    }
    private void FixedUpdate()
    {
        rectTransform.position = new Vector3(previousX + Random.Range(-power, power), previousY + Random.Range(-power, power), 0f);
    }
}
