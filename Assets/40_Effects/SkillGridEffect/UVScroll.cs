using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UVScroll : MonoBehaviour
{
    [SerializeField]
    private Material _targetMaterial;

    [SerializeField]
    private float _scrollX;

    private Vector2 offset;
    private void Awake()
    {
        offset = _targetMaterial.mainTextureOffset;
    }

    private void Update()
    {
        offset.y -= _scrollX * Time.deltaTime;
        if(offset.y <= -100f)
        {
            offset.y = 0f;
        }
        _targetMaterial.mainTextureOffset = offset;
    }
}