using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using NTitleController;

public class MediaController : MonoBehaviour
{
    [SerializeField] VideoPlayer videoPlayer;
    [SerializeField] TitileController tC;
    [SerializeField] GameObject screen,video;
    [SerializeField] Fade fade = null;
    [SerializeField] FadeImage fi;
    [SerializeField,Header("ビデオ再生間隔")] float deltaTime;
    bool wait = true;
    bool check = true;
    bool isFlag = false;

    public bool WaitCheck => wait;

    // Start is called before the first frame update
    void Start()
    {
        video.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        Debug.Log(wait);

        if( tC.Timer >= deltaTime && check)
        {
            fade.FadeIn(1f, () => screen.SetActive(false));
            check = false;
            wait = false;
            StartCoroutine(VideoPlay());
        }

        if(!wait)
        {
            if (Input.GetButtonDown("Submit") && tC.Timer >= deltaTime)
            {
                Debug.Log("A");

                fade.FadeIn(1f, () => screen.SetActive(true));
                StartCoroutine(FadeClose());
            }
        }
        
    }

   
    IEnumerator VideoPlay()
    {
        video.SetActive(true);
        yield return new WaitUntil(() => fi.CutoutRange == 1f);
        videoPlayer.Play();
        fade.FadeOut(1f);      
    }

    IEnumerator FadeClose()
    {
        yield return new WaitUntil(() => fi.CutoutRange == 1f);
        Invoke("Await",1f);
    }

    void Await()
    {
       fade.FadeOut(1f);
        check = true;
        tC.Timer = 0f;
        StartCoroutine(Wait());
    }
    IEnumerator Wait()
    {
        yield return new WaitForSeconds(1);
        wait = true;
    }
}
