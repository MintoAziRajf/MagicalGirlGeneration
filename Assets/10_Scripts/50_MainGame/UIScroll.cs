using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScroll : MonoBehaviour
{
	[SerializeField] private float speed = 0f;
	[SerializeField] private float limit = 0f;
	private Vector2 defaultPos;
	private RectTransform myTransform;
	private void Awake()
	{
		myTransform = this.GetComponent<RectTransform>();
		defaultPos = this.myTransform.anchoredPosition;
	}
	private void Update()
	{
		myTransform.anchoredPosition = new Vector2(defaultPos.x, myTransform.anchoredPosition.y + speed * Time.deltaTime);
		if (Mathf.Abs(myTransform.anchoredPosition.y) > Mathf.Abs(limit))
		{
			myTransform.anchoredPosition = new Vector2(defaultPos.x, -limit);
		}
	}
}
