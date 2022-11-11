using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealEffect : MonoBehaviour
{
    [SerializeField] private GameObject star = null;
    List<GameObject> stars = new List<GameObject>();

    private void Start()
    {
        for(int i = 0; i < 10; i++)
        {
            stars.Add(Instantiate(star, this.gameObject.transform));
            stars[i].GetComponent<HealGather>().Rot = (90f - 36f * i) * Mathf.Deg2Rad;
        }
    }
} 
