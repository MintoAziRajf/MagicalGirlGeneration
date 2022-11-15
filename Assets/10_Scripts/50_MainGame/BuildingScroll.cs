using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingScroll : MonoBehaviour
{
	[SerializeField] private float speed = 0f;
	[SerializeField] private float limit = 0f;
	Vector3 defaultPos;
	private void Awake()
    {
		defaultPos = this.transform.localPosition;
    }
	private void FixedUpdate()
	{
		transform.Translate(speed, 0, 0);
		if (Mathf.Abs(transform.localPosition.x) > Mathf.Abs(limit))
		{
			transform.localPosition = new Vector3(-limit, defaultPos.y, defaultPos.z);
		}
	}
}
