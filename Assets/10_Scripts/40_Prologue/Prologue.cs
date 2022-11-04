using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prologue : MonoBehaviour
{
    [SerializeField] private GameObject novelObj = null;
    NovelSystem novelSystem;
    [SerializeField] private string[] csvNames = null;
    private void Awake()
    {
        novelSystem = novelObj.GetComponent<NovelSystem>();
    }
    public IEnumerator StartPrologue(int type)
    {
        yield return StartCoroutine(novelSystem.NovelStart(csvNames[type],type));
    }
}
