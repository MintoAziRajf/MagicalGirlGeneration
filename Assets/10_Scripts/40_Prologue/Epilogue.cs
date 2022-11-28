using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Epilogue : MonoBehaviour
{
    [SerializeField] private GameObject novelObj = null; // ノベルオブジェクト
    NovelSystem novelSystem;
    [SerializeField] private string[] csvNames = null; // エピローグの名前リスト

    /// <summary>
    /// 指定されたエピローグを再生
    /// </summary>
    /// <param name="type">指定値</param>
    public IEnumerator StartEpilogue(int type) 
    {
        GameObject novel = Instantiate(novelObj);
        novelSystem = novel.GetComponent<NovelSystem>();
        yield return StartCoroutine(novelSystem.NovelStart(csvNames[type])); // エピローグが終了されるまで待機
    }
}
