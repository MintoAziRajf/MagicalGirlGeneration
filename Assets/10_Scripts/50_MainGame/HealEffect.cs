﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealEffect : MonoBehaviour
{
    [SerializeField] private GameObject _star = null;
    [SerializeField] private RectTransform _rTransform = null;
    [SerializeField] private Camera cam = null;
    private GameObject player;

    public void StartEffect()
    {
        List<GameObject> stars = new List<GameObject>();
        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
        }
        this.transform.position = player.transform.position;
        Vector3 target = cam.ScreenToWorldPoint(_rTransform.position);
        for(int i = 0; i < 20; i++)
        {
            stars.Add(Instantiate(_star, this.gameObject.transform));
            stars[i].GetComponent<HealGather>().SetTarget(target, (90f - 18f * i) * Mathf.Deg2Rad, 360f/20f); 
        }
    }
} 
