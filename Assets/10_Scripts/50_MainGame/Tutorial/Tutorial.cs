using System.Collections;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private GameObject tutorialCanvas = null;

    /// <summary>
    /// チュートリアルフローチャート開始
    /// </summary>
    public IEnumerator Flow()
    {
        // チュートリアルスタート
        tutorialCanvas.SetActive(true);
        // チュートリアルが終了するまで待機
        while (tutorialCanvas.activeSelf)
        {
            yield return null;
        }
    }
}
