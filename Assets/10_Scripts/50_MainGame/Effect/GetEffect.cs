using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetEffect : MonoBehaviour
{
    [SerializeField] private GameObject _star = null; // エフェクト
    [SerializeField] private RectTransform _rTransform = null; // エフェクトの収縮先
    [SerializeField] private Camera cam = null; // エフェクトを表示するカメラ
    [SerializeField] private float speed = 0f; // 収縮速度

    /// <summary>
    /// エフェクトを再生
    /// </summary>
    public void StartEffect()
    {
        Vector3 targetPos = _rTransform.anchoredPosition; // 収縮先をセット
        targetPos.z = 10f; // 位置を調整
        Vector3 target = cam.ScreenToWorldPoint(targetPos); // カメラ座標からワールド座標に変換
        for(int i = 0; i < 20; i++)
        {
            GameObject star = Instantiate(_star, this.gameObject.transform);
            star.GetComponent<GetEffectGather>().SetTarget(target, speed); // 収縮先と速度をセット
        }
    }
} 
