using UnityEngine;
using UnityEngine.UI;

public class ScrollController : MonoBehaviour
{
	private ScrollRect scrollRect;

	public float itemHeight = 0.111f;

	private void Awake()
	{
		scrollRect = GetComponent<ScrollRect>();
	}

	private void OnEnable()
	{
		scrollRect.verticalNormalizedPosition = 1f;
	}

	public void SetOnOption(int option)
	{
		scrollRect.verticalNormalizedPosition = 1f - (float)option * itemHeight;
	}
}
