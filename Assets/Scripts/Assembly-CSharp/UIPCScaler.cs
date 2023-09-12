using System.Collections.Generic;
using UnityEngine;

public class UIPCScaler : MonoBehaviour
{
	[SerializeField]
	private List<RectTransform> UIToScale;

	private void Start()
	{
		foreach (RectTransform item in UIToScale)
		{
			item.localScale = Vector3.one;
		}
		Object.Destroy(this);
	}
}
