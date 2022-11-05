using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prologue : MonoBehaviour
{
    [SerializeField] private GameObject novelObj = null;
    NovelSystem novelSystem;
    [SerializeField] private string[] csvNames = null;
    public IEnumerator StartPrologue(int type)
    {
        GameObject novel = Instantiate(novelObj);
        novelSystem = novel.GetComponent<NovelSystem>();
        yield return StartCoroutine(novelSystem.NovelStart(csvNames[type]));
    }
}
