using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeadAnimation : PlayerManager
{
    [SerializeField] private SpriteRenderer deadBackground = null;
    [SerializeField] private Camera mainCamera = null;
    Vector3 offset = new Vector3(0f, 4f, 3.5f);

    public IEnumerator StartAnimation()
    {
        //visualAnim.SetTrigger("Dead");
        //StartCoroutine(BackgroundFade());
        //yield return StartCoroutine(CameraZoomIn());
        gameManager.GameOver();
        yield break;
    }
    private IEnumerator BackgroundFade()
    {
        Color c = deadBackground.color;
        c.a = 0f;
        for (int i = 0; i < 30; i++)
        {
            c.a = i + 1 / 30f;
            deadBackground.color = c;
            yield return null;
        }
        yield return new WaitForSecondsRealtime(1f);
    }

    private IEnumerator CameraZoomIn()
    {
        Vector3 targetPos = mainCamera.transform.localPosition + this.transform.localPosition + offset;
        float diff = (mainCamera.transform.localPosition - targetPos).magnitude;
        for (int i = 0; i < 60; i++)
        {
            mainCamera.transform.localPosition = Vector3.MoveTowards(mainCamera.transform.localPosition, targetPos, diff / 60f);
            yield return null;
        }
    }
}
