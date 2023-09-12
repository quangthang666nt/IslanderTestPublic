using System.Collections;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UIHeightAnimator : MonoBehaviour
{
	public float targetHeight;

	public AnimationCurve animationCurve;

	public float animationLenght = 1.5f;

	public float delay = 0.5f;

	private RectTransform thisRT;

	private void OnEnable()
	{
		StopAllCoroutines();
		thisRT = GetComponent<RectTransform>();
		StartCoroutine(Play());
	}

	private void OnDisable()
	{
		Vector2 sizeDelta = thisRT.sizeDelta;
		sizeDelta.y = 0f;
		thisRT.sizeDelta = sizeDelta;
	}

	private IEnumerator Play()
	{
		yield return new WaitForSeconds(delay);
		float clock = 0f;
		while (clock <= animationLenght)
		{
			clock += Time.deltaTime;
			float time = Mathf.InverseLerp(0f, animationLenght, clock);
			Vector2 sizeDelta = thisRT.sizeDelta;
			sizeDelta.y = Mathf.Lerp(0f, targetHeight, animationCurve.Evaluate(time));
			thisRT.sizeDelta = sizeDelta;
			yield return null;
		}
	}
}
