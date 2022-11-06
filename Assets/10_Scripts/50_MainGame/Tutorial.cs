using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private GameObject tutorialCanvas = null;

    public IEnumerator Flow()
    {
        tutorialCanvas.SetActive(true);
        while (tutorialCanvas.activeSelf)
        {
            yield return null;
        }
    }
}
