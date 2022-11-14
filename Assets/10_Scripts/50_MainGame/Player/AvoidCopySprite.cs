using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidCopySprite : MonoBehaviour
{
    private SpriteRenderer playerVisual;
    private SpriteRenderer myVisual;
    private void FixedUpdate()
    {
        if (playerVisual == null) playerVisual = GameObject.FindWithTag("Player").transform.Find("PlayerVisual").GetComponent<SpriteRenderer>();
        if (myVisual == null) myVisual = this.GetComponent<SpriteRenderer>();
        myVisual.sprite = playerVisual.sprite;
    }
}
