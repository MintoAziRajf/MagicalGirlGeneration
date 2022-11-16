using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAvoidVisual : MonoBehaviour
{
    [SerializeField] private SpriteRenderer playerVisual = null;
    private SpriteRenderer myVisual = null;

    private void Update()
    {
        if (myVisual == null) myVisual = this.GetComponent<SpriteRenderer>();
        myVisual.sprite = playerVisual.sprite;
    }
}
