using System;
using UnityEngine;
using UnityEngine.Events;

public class CosmeticHandler : MonoBehaviour
{
	private const string MAT_INSTANCE = " (Instance)";

	[SerializeField]
	private CosmeticID id;

	[SerializeField]
	private GameObject defaultModel;

	private string currentCosmeticId = "";

	private GameObject currentCosmetic;

	private bool initCosmeticLaunched;

	[HideInInspector]
	public Action OnComesticApplied;

	public UnityEvent OnCosmeticExecuted;

	private void Start()
	{
		CosmeticsManager.OnCosmeticsUpdate.AddListener(CheckAndApplyCosmetics);
		if (!initCosmeticLaunched)
		{
			CheckAndApplyCosmetics();
		}
	}

	private void OnDestroy()
	{
		CosmeticsManager.OnCosmeticsUpdate.RemoveListener(CheckAndApplyCosmetics);
		UnloadCosmetic();
	}

	public bool HasCosmeticActivated()
	{
		return !string.IsNullOrEmpty(CosmeticsManager.Cosmetics.GetCosmetic(id));
	}

	public string GetCosmeticId()
	{
		return CosmeticsManager.Cosmetics.GetCosmetic(id);
	}

	public void CheckAndApplyCosmetics()
	{
		initCosmeticLaunched = true;
		string cosmetic = CosmeticsManager.Cosmetics.GetCosmetic(id);
		if (!string.IsNullOrEmpty(cosmetic))
		{
			if (!cosmetic.Equals(currentCosmeticId))
			{
				UnloadCosmetic();
				ShowDefaultModel(show: false);
				CosmeticsManager.singleton.LoadBuildingCosmeticAsync(cosmetic, OnAssetLoaded);
				currentCosmeticId = cosmetic;
			}
			return;
		}
		if (currentCosmetic != null)
		{
			UnloadCosmetic();
			UnityEngine.Object.Destroy(currentCosmetic);
		}
		currentCosmeticId = string.Empty;
		ShowDefaultModel(show: true);
		ExecOnComesticApplied();
	}

	private void OnAssetLoaded(GameObject model)
	{
		if (model == null)
		{
			ShowDefaultModel(show: true);
			ExecOnComesticApplied();
			return;
		}
		if (currentCosmetic != null)
		{
			UnityEngine.Object.Destroy(currentCosmetic);
		}
		currentCosmetic = UnityEngine.Object.Instantiate(model, base.transform);
		ReplaceMaterialRefs(currentCosmetic);
		ExecOnComesticApplied();
		OnCosmeticExecuted?.Invoke();
	}

	private void ReplaceMaterialRefs(GameObject cosmeticObj)
	{
		if (cosmeticObj == null)
		{
			return;
		}
		MeshRenderer[] componentsInChildren = cosmeticObj.GetComponentsInChildren<MeshRenderer>();
		if (componentsInChildren == null)
		{
			return;
		}
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Material[] sharedMaterials = componentsInChildren[i].sharedMaterials;
			if (sharedMaterials == null)
			{
				continue;
			}
			for (int j = 0; j < sharedMaterials.Length; j++)
			{
				Material material = sharedMaterials[j];
				if (material != null && CosmeticsManager.singleton.dicMaterialsRef != null && CosmeticsManager.singleton.dicMaterialsRef.TryGetValue(CleanMaterialName(material.name), out var value))
				{
					sharedMaterials[j] = value;
				}
			}
			componentsInChildren[i].sharedMaterials = sharedMaterials;
		}
	}

	private string CleanMaterialName(string materialName)
	{
		string text = materialName;
		if (text.Contains(" (Instance)"))
		{
			text = text.Replace(" (Instance)", string.Empty);
		}
		return text;
	}

	private void ExecOnComesticApplied()
	{
		if (OnComesticApplied != null)
		{
			OnComesticApplied();
			OnComesticApplied = null;
		}
	}

	private void ShowDefaultModel(bool show)
	{
		if ((bool)defaultModel)
		{
			defaultModel.SetActive(show);
		}
	}

	private void UnloadCosmetic()
	{
		if (!string.IsNullOrEmpty(currentCosmeticId))
		{
			CosmeticsManager.singleton.UnloadBuildingCosmeticIfApply(currentCosmeticId);
		}
	}
}
