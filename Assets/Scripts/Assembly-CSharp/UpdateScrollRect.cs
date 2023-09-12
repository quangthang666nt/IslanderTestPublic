using UnityEngine;
using UnityEngine.UI;

public class UpdateScrollRect : MonoBehaviour
{
	private ScrollRect scrollRect;

	private void Awake()
	{
		scrollRect = GetComponent<ScrollRect>();
	}

	private void OnEnable()
	{
		scrollRect.verticalNormalizedPosition = 1f;
	}
}
