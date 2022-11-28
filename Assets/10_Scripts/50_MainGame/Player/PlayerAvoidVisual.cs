using UnityEngine;

public class PlayerAvoidVisual : MonoBehaviour
{
    [SerializeField] private SpriteRenderer playerVisual = null;
    private SpriteRenderer myVisual = null;

    /// <summary>
    /// 回避の見た目をメインキャラに合わせる
    /// </summary>
    private void Update()
    {
        if (myVisual == null) myVisual = this.GetComponent<SpriteRenderer>();
        myVisual.sprite = playerVisual.sprite;
    }
}
