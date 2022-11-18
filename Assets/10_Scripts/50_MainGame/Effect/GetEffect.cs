using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetEffect : MonoBehaviour
{
    [SerializeField] private GameObject _star = null;
    [SerializeField] private RectTransform _rTransform = null;
    [SerializeField] private Camera cam = null;

    public void StartEffect()
    {
        List<GameObject> stars = new List<GameObject>();
        Vector3 targetPos = _rTransform.anchoredPosition;
        targetPos.z = 10f;
        Vector3 target = cam.ScreenToWorldPoint(targetPos);
        for(int i = 0; i < 20; i++)
        {
            stars.Add(Instantiate(_star, this.gameObject.transform));
            stars[i].GetComponent<GetEffectGather>().SetTarget(target); 
        }
    }
} 
