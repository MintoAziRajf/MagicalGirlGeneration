using System.Collections;

public class PlayerDeadAnimation : PlayerManager
{
    /// <summary>
    /// 死亡演出
    /// </summary>
    public IEnumerator StartAnimation()
    {
        visualAnim.SetTrigger("Dead"); // プレイヤーを倒れさせる
        gameManager.GameOver(); // ゲームマネージャーにゲームオーバーを通知
        yield break;
    }
}
