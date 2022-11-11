using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeIn : MonoBehaviour
{
    [SerializeField] private Image screen = null;
    private Color c;
    [SerializeField] private int time = 0;

    private void Awake()
    {
        StartCoroutine(Fade());
    }

    private IEnumerator Fade()
    {
        //カラーのセット
        c = screen.color;
        //不透明度初期化
        c.a = 1f;
        screen.color = c;

        //不透明度フェード
        for (int i = 0; i < time; i++)
        {
            c.a -= 1f / (float)time;
            screen.color = c;
            yield return null;
        }
        //完全に透明にする
        c.a = 0f;
        screen.color = c;
    }
}
