using UnityEngine;

public class UIScroll : MonoBehaviour
{
	[SerializeField] private float speed = 0f; // 速度
	[SerializeField] private float limit = 0f; // 高さ上限
 	private Vector2 defaultPos;
	private RectTransform myTransform;
	private void Awake()
	{
		myTransform = this.GetComponent<RectTransform>();
		defaultPos = this.myTransform.anchoredPosition;
	}
	
	// スクロール処理
	private void Update()
	{
		myTransform.anchoredPosition = new Vector2(defaultPos.x, myTransform.anchoredPosition.y + speed * Time.deltaTime); // スクロール
		if (Mathf.Abs(myTransform.anchoredPosition.y) > Mathf.Abs(limit)) // 上限に到達したら一番下に戻す
		{
			myTransform.anchoredPosition = new Vector2(defaultPos.x, -limit);
		}
	}
}
