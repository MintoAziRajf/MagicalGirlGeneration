using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NUnityGameLib;

public class EndCardManager : UnityGameLib, IUnityGameLib
{
    public static LoadManager instance = null;
    [SerializeField] private GameObject loadPrefab = null;
    [SerializeField] FadeImage fi = null;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(wait());
    }

    IEnumerator wait()
    {
        yield return new WaitUntil(() => fi.CutoutRange == 0f);

        if (Input.GetButtonDown("Submit"))
        {
            LoadManager.instance.LoadScene("20_Title");
        }
    }


}
