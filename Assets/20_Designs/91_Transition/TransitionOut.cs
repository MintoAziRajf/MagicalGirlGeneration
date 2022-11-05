using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransitionOut : MonoBehaviour
{
    [SerializeField]
    private Material _transitionOut = null;

    private void Awake()
    {
        StartCoroutine(Animate(_transitionOut, 1));
    }

    /// <summary>
    /// time秒かけてトランジションする
    /// </summary>
    /// <param name="material"></param>
    /// <param name="time"></param>
    IEnumerator Animate(Material material, float time)
    {
        GetComponent<Image>().material = material;
        float current = 0;
        while (current < time)
        {
            material.SetFloat("_Alpha", current / time);
            yield return new WaitForEndOfFrame();
            current += Time.unscaledDeltaTime;
        }
        material.SetFloat("_Alpha", 1);
    }
}
