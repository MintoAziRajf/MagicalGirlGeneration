using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prologue : MonoBehaviour
{
    [SerializeField] private GameObject novelObj = null; // ノベルオブジェクト
    NovelSystem novelSystem;
    [SerializeField] private string[] csvNames = null; // プロローグの名前リスト
    /// <summary>
    /// 指定されたプロローグを再生
    /// </summary>
    /// <param name="type">指定値</param>
    public IEnumerator StartPrologue(int type)
    {
        GameObject novel = Instantiate(novelObj);
        novelSystem = novel.GetComponent<NovelSystem>();
        yield return StartCoroutine(novelSystem.NovelStart(csvNames[type])); // プロローグが終了するまで待機
    }
}
