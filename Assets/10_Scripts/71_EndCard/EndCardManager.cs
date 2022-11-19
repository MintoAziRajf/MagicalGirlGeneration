using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NUnityGameLib;

public class EndCardManager : UnityGameLib, IUnityGameLib
{
    
    [SerializeField] private GameObject loadPrefab = null;
    [SerializeField] FadeImage fi = null;

    public static LoadManager instance = null;
    private bool isFirst = true;


    // Start is called before the first frame update
    void Start()
    {
        SoundManager.instance.PlayBGM(SoundManager.BGM_Type.EndCard);
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(wait());
    }

    IEnumerator wait()
    {
        yield return new WaitUntil(() => fi.CutoutRange == 0f);

        if (Input.GetButtonDown("Submit") && isFirst)
        {
            SoundManager.instance.PlaySE(SoundManager.SE_Type.Submit);
            isFirst = false;
            LoadManager.instance.LoadScene("20_Title");
        }
    }
}
