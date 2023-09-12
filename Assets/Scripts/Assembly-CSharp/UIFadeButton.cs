using System.Collections;
using System.Collections.Generic;
using I2.Loc;
using Rewired;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIFadeButton : MonoBehaviour
{
	[SerializeField]
	private float timeToFade = 5f;

	[SerializeField]
	private AnimationCurve fadingOutCurve = new AnimationCurve();

	[SerializeField]
	private AnimationCurve fadingInCurve = new AnimationCurve();

	[SerializeField]
	private Localize fadingLocalizedString;

	[SerializeField]
	private List<Image> fadingImages;

	[SerializeField]
	private List<string> invalidInputActions = new List<string>();

	[SerializeField]
	private bool startHidden;

	private bool fading;

	private bool transition;

	private float currentTime;

	private float currentTransitionTime;

	private float currentStartTime;

	private Material stringMaterial;

	private void Start()
	{
		if (fadingLocalizedString != null)
		{
			fadingLocalizedString.LocalizeEvent.AddListener(LocalizeStringGetMaterial);
			LocalizeStringGetMaterial();
		}
	}

	private void OnEnable()
	{
		if (startHidden)
		{
			fading = true;
			currentTime = 100f;
			currentStartTime = 0f;
		}
	}

	private void LocalizeStringGetMaterial()
	{
		StartCoroutine(DelayedStringGetMaterial());
	}

	private IEnumerator DelayedStringGetMaterial()
	{
		yield return null;
		TextMeshProUGUI component = fadingLocalizedString.GetComponent<TextMeshProUGUI>();
		Debug.Log("LocalizeStringGetMaterial: " + component.fontMaterial.name);
		Material fontMaterial = component.fontMaterial;
		stringMaterial = new Material(fontMaterial);
		component.fontMaterial = stringMaterial;
	}

	private void ProcessAlpha()
	{
		float num = 0f;
		num = ((!fading) ? fadingInCurve.Evaluate(currentTime) : (1f - fadingOutCurve.Evaluate(currentTime)));
		if (stringMaterial != null)
		{
			Color color = stringMaterial.GetColor("_FaceColor");
			color.a = num;
			stringMaterial.SetColor("_FaceColor", color);
		}
		foreach (Image fadingImage in fadingImages)
		{
			if (!(fadingImage == null))
			{
				Color color2 = fadingImage.color;
				color2.a = num;
				fadingImage.color = color2;
			}
		}
	}

	private void Update()
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		if (startHidden && currentStartTime < 1f)
		{
			currentStartTime += Time.deltaTime;
			ProcessAlpha();
			return;
		}
		bool flag = ReInput.players.GetPlayer(0).GetAnyButton() || ReInput.players.GetPlayer(0).GetAnyNegativeButton();
		bool flag2 = false;
		foreach (string invalidInputAction in invalidInputActions)
		{
			if (ReInput.players.GetPlayer(0).GetButton(invalidInputAction))
			{
				flag2 = true;
				break;
			}
		}
		if (flag && !flag2)
		{
			if (fading)
			{
				currentTime = 0f;
			}
			transition = false;
			fading = false;
		}
		else if (!fading && !transition)
		{
			currentTransitionTime = 0f;
			transition = true;
		}
		currentTime += Time.deltaTime;
		if (transition)
		{
			currentTransitionTime += Time.deltaTime;
			if (currentTransitionTime >= timeToFade)
			{
				transition = false;
				fading = true;
				currentTime = 0f;
			}
		}
		ProcessAlpha();
	}
}
