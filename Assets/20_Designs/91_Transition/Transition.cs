using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Transition : MonoBehaviour
{
    [SerializeField]
    private Material _transitionIn = null;

    [SerializeField]
    private Material _transitionOut = null;

    [SerializeField]
    private UnityEvent OnTransition = null;
    [SerializeField]
    private UnityEvent OnComplete = null;

    public void StartTransition()
    {
        StartCoroutine(BeginTransition());
    }

    void OnEnable()
    {

        GetComponent<Image>().material.SetFloat("_Alpha", 1);
    }

    IEnumerator BeginTransition()
    {
        yield return Animate(_transitionIn, 1);
        if (OnTransition != null) { OnTransition.Invoke(); }
        yield return new WaitForEndOfFrame();
        if (OnComplete != null) { OnComplete.Invoke(); }
        yield return Animate(_transitionOut, 1);
    }

    /// <summary>
    /// time秒かけてトランジションを行う
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
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