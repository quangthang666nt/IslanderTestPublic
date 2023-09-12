using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class UIBuildingBarBackground : MonoBehaviour, IPointerExitHandler, IEventSystemHandler, IPointerEnterHandler
{
	public static UIBuildingBarBackground singleton;

	public UISmoothLayoutGroup buildingBar;

	public float fPadding = 40f;

	public float smoothingSpeed = 200f;

	private RectTransform rtThis;

	private float targetWidth;

	private float currentWidth;

	private bool bSmoothing;

	private bool bMouseOver;

	public bool BMouseOver => bMouseOver;

	private void Start()
	{
		singleton = this;
		rtThis = GetComponent<RectTransform>();
	}

	private void Update()
	{
		targetWidth = buildingBar.DisplayWidth;
		if (targetWidth > 0f)
		{
			targetWidth += fPadding;
		}
		if (targetWidth != currentWidth)
		{
			currentWidth = Mathf.SmoothStep(currentWidth, targetWidth, smoothingSpeed * Time.deltaTime);
		}
		Vector2 sizeDelta = rtThis.sizeDelta;
		sizeDelta.x = currentWidth / base.transform.localScale.x;
		rtThis.sizeDelta = sizeDelta;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		bMouseOver = true;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		bMouseOver = false;
	}

	private void OnDisable()
	{
		bMouseOver = false;
	}
}
