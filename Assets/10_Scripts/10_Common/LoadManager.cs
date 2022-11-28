using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadManager : SingletonMonoBehaviour<LoadManager>
{
    public static LoadManager instance; // どこからでも呼び出せるように
    [SerializeField] private GameObject loadPrefab = null;
    
    /// <summary>
    /// シーン遷移
    /// </summary>
    /// <param name="scene">ロードするシーン名</param>
    public void LoadScene(string scene)
    {
        GameObject SceneLoader = Instantiate(loadPrefab); // ローディング画面を生成
        Loading loading = SceneLoader.GetComponent<Loading>();
        loading.StartCoroutine("LoadScene", scene); // ロード
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
