using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UVScroll : MonoBehaviour
{
    [SerializeField]
    private Material _targetMaterial;

    [SerializeField]
    private float _scrollX;
    [SerializeField]
    private float _scrollY;

    private Vector2 offset;
    private void Awake()
    {
        offset = _targetMaterial.mainTextureOffset;
    }

    private void Update()
    {
        offset.y += _scrollY * Time.unscaledDeltaTime;
        offset.x += _scrollX * Time.unscaledDeltaTime;
        if (offset.y >= 10f) offset.y = 0f;
        if (offset.x >= 10f) offset.x = 0f;
        _targetMaterial.mainTextureOffset = offset;
    }
}