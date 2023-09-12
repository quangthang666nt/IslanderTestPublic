using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIThemedEventsLogo : MonoBehaviour
{
	public Image logoImg;

	public Sprite defaultLogo;

	private void OnEnable()
	{
		if ((bool)CosmeticsManager.singleton && CosmeticsManager.singleton.IsConfigLoaded() && CosmeticsManager.singleton.bDataLoaded)
		{
			CheckLogo();
		}
		else
		{
			StartCoroutine(EnableOnDataLoaded());
		}
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}

	private IEnumerator EnableOnDataLoaded()
	{
		while (!CosmeticsManager.singleton || !CosmeticsManager.singleton.IsConfigLoaded() || !CosmeticsManager.singleton.bDataLoaded)
		{
			yield return null;
		}
		CheckLogo();
	}

	private void CheckLogo()
	{
		if (!string.IsNullOrEmpty(CosmeticsManager.Cosmetics.theme))
		{
			Sprite packageTitleLogo = CosmeticsManager.singleton.CatalogConfig.GetPackageTitleLogo(CosmeticsManager.Cosmetics.theme);
			if (packageTitleLogo != null)
			{
				logoImg.sprite = packageTitleLogo;
				return;
			}
		}
		if (defaultLogo != null)
		{
			logoImg.sprite = defaultLogo;
		}
	}
}
