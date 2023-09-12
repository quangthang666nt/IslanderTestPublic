using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIBuildingChoiceButton : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerDownHandler
{
	[SerializeField]
	private AnimationCurve acScaleAnimationCurve;

	[SerializeField]
	private float fScaleDefault = 1f;

	[SerializeField]
	private float fScaleFactorOnHover;

	[SerializeField]
	private float fAnimationTime = 0.25f;

	[HideInInspector]
	public BBPack bbPack;

	[HideInInspector]
	public UIBuildingChoice uiBuildingChoice;

	[HideInInspector]
	public int iHotkey = -1;

	[HideInInspector]
	public UIBuildingPackSelectable selector;

	private Coroutine crCurrentFadeInAnimation;

	private Coroutine crCurrentFadeOutAnimation;

	private float fAnimationTimer;

	private float fHotkeyLockedTimer = -1f;

	private void OnEnable()
	{
		fHotkeyLockedTimer = 0.1f;
	}

	public void OnPointerEnter(PointerEventData e)
	{
		uiBuildingChoice.Select(this);
	}

	public void OnPointerExit(PointerEventData e)
	{
		Deselect();
	}

	public void OnPointerDown(PointerEventData e = null)
	{
		if (e == null || e.button == PointerEventData.InputButton.Left)
		{
			Apply();
		}
	}

	public void Select()
	{
		if (bbPack.liGoBuildings.Count < 2)
		{
			Building component = bbPack.liGoBuildings[0].GetComponent<Building>();
			UITooltip.Singleton.Enable(component.strBuildingName, component.strToolTip);
			UITooltip.Singleton.EnableImmediate(component.strBuildingName, component.strToolTip, InputManager.InputMode.Controller);
		}
		else
		{
			UITooltip.Singleton.Enable(bbPack.StrPackName, bbPack.StrPackDescription);
			UITooltip.Singleton.EnableImmediate(bbPack.StrPackName, bbPack.StrPackDescription, InputManager.InputMode.Controller);
		}
		if (crCurrentFadeInAnimation != null)
		{
			StopCoroutine(crCurrentFadeInAnimation);
		}
		if (crCurrentFadeOutAnimation != null)
		{
			StopCoroutine(crCurrentFadeOutAnimation);
			crCurrentFadeOutAnimation = null;
		}
		crCurrentFadeInAnimation = StartCoroutine(AnimationOnEnter());
		if (selector != null)
		{
			selector.ToggleSelectorOutline(value: true);
		}
	}

	public void Deselect()
	{
		UITooltip.Singleton.Disable();
		if (crCurrentFadeOutAnimation != null)
		{
			StopCoroutine(crCurrentFadeOutAnimation);
		}
		crCurrentFadeOutAnimation = StartCoroutine(AnimationOnExit());
		if (selector != null)
		{
			selector.ToggleSelectorOutline(value: false);
		}
	}

	public void Apply()
	{
		UITooltip.Singleton.Disable();
		uiBuildingChoice.OnButtonPress(bbPack);
	}

	private IEnumerator AnimationOnEnter()
	{
		while (fAnimationTimer < fAnimationTime)
		{
			fAnimationTimer += Time.deltaTime;
			float t = acScaleAnimationCurve.Evaluate(Mathf.InverseLerp(0f, fAnimationTime, fAnimationTimer));
			base.transform.localScale = Vector3.one * Mathf.Lerp(fScaleDefault, fScaleFactorOnHover, t);
			yield return null;
		}
		fAnimationTimer = fAnimationTime;
		crCurrentFadeInAnimation = null;
	}

	private IEnumerator AnimationOnExit()
	{
		while (crCurrentFadeInAnimation != null)
		{
			yield return null;
		}
		while (fAnimationTimer > 0f)
		{
			fAnimationTimer -= Time.deltaTime;
			float t = acScaleAnimationCurve.Evaluate(1f - Mathf.InverseLerp(0f, fAnimationTime, fAnimationTimer));
			base.transform.localScale = Vector3.one * Mathf.Lerp(fScaleFactorOnHover, fScaleDefault, t);
			yield return null;
		}
		fAnimationTimer = 0f;
		crCurrentFadeOutAnimation = null;
	}

	private void Update()
	{
		fHotkeyLockedTimer -= Time.deltaTime;
		if (fHotkeyLockedTimer <= 0f && InputManager.Singleton.InputDataCurrent.bQuickSelect[iHotkey])
		{
			OnPointerDown();
		}
	}
}
