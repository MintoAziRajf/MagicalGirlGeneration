using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadManager : SingletonMonoBehaviour<LoadManager>
{
    public static LoadManager instance;
    [SerializeField] private GameObject loadPrefab = null;
    

    public void LoadScene(string scene)
    {
        GameObject SceneLoader = Instantiate(loadPrefab);
        Loading loading = SceneLoader.GetComponent<Loading>();
        loading.StartCoroutine("LoadScene", scene);
    }

    // シングルトンかつ、シーン遷移しても破棄されないようにする
    public override void CheckSingleton()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
