using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SXUIGraphicsPack : MonoBehaviour
{
	public Toggle toggle;

	private bool bListenerAdded;

	private void OnEnable()
	{
		if ((bool)CosmeticsManager.singleton && CosmeticsManager.singleton.IsConfigLoaded() && CosmeticsManager.singleton.bDataLoaded)
		{
			SetToggleFromData();
			return;
		}
		toggle.enabled = false;
		StartCoroutine(EnableOnDataLoaded());
	}

	private void OnDisable()
	{
		StopAllCoroutines();
		if (bListenerAdded)
		{
			toggle.onValueChanged.RemoveListener(OnToggleValueChangend);
			bListenerAdded = false;
		}
	}

	private IEnumerator EnableOnDataLoaded()
	{
		while (!CosmeticsManager.singleton || !CosmeticsManager.singleton.IsConfigLoaded() || !CosmeticsManager.singleton.bDataLoaded)
		{
			yield return null;
		}
		SetToggleFromData();
	}

	private void OnToggleValueChangend(bool newValue)
	{
		UiCanvasManager.Singleton.ToUpdateThemeEvent();
	}

	private void SetToggleFromData()
	{
		if (CosmeticsManager.singleton.MainThemeEventAvailable())
		{
			toggle.isOn = CosmeticsManager.singleton.CatalogConfig.mainPackage.Equals(CosmeticsManager.Cosmetics.theme);
			toggle.enabled = true;
			toggle.onValueChanged.AddListener(OnToggleValueChangend);
			bListenerAdded = true;
		}
		else
		{
			toggle.gameObject.SetActive(value: false);
			base.gameObject.SetActive(value: false);
		}
	}
}
