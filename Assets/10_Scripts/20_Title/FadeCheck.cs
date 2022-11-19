using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeCheck : MonoBehaviour
{

    [SerializeField] GameObject obj = null;
    /// <summary>
    /// fadeCanvasの
    /// </summary>
    [SerializeField ] Fade fade = null;
   
    // Start is called before the first frame update
    void Start()
    {
        obj.SetActive(true);
        Invoke("wait", 0.1f);
        fade.FadeIn(0f,() => fade.FadeOut(1.5f));        
    }

    void wait()
    {
        obj.SetActive(false);
    }
}
