using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using NTitleController;


interface IMediaController
{

}
public class MediaController : MonoBehaviour
{
    [SerializeField] VideoPlayer videoPlayer;
    [SerializeField] TitileController tC;
    [SerializeField] GameObject screen,video;
    [SerializeField] Fade fade = null;
    [SerializeField] FadeImage fi;
    [SerializeField,Header("ビデオ再生間隔")] float deltaTime;
    bool wait = true;
    bool check = true;//一度しか呼ばれないようにする為の変数
    bool isFlag = false;

    //プロパティのラムダ式getだけ
    public bool WaitCheck => wait;

    // Start is called before the first frame update
    void Start()
    {
        video.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        //時間が経過したらフェードインから動画再生
        if( tC.Timer >= deltaTime && check)
        {
            fade.FadeIn(1f, () => screen.SetActive(false));
            check = false;
            wait = false;
            StartCoroutine(VideoPlay());
        }

        //フェード以外のときtrue
        if(isFlag)
        {
            if (Input.GetButtonDown("Submit") && tC.Timer >= deltaTime)
            {
                Debug.Log("A");

                fade.FadeIn(1f, () => screen.SetActive(true));
                StartCoroutine(FadeClose());
            }
        }
        
    }

 　　//動画の再生
    IEnumerator VideoPlay()
    {
        video.SetActive(true);
        yield return new WaitUntil(() => fi.CutoutRange == 1f);
        SoundManager.instance.MuteBGM();
        videoPlayer.Play();
        fade.FadeOut(1f, () => isFlag = true);      
    }

    //動画の停止
    IEnumerator FadeClose()
    {
        yield return new WaitUntil(() => fi.CutoutRange == 1f);
        videoPlayer.Stop();
        SoundManager.instance.ResumeBGM();
        fade.FadeOut(1f);
        check = true;
        tC.Timer = 0f;

        StartCoroutine(Wait());

        isFlag = false;
    }

    void Await()
    {
    }

    //コントローラーを操作できないようにするための遅延
    IEnumerator Wait()
    {
        yield return new WaitForSeconds(1);
        wait = true;
    }
}
