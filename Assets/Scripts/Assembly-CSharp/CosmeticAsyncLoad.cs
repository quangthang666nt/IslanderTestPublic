using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class CosmeticAsyncLoad
{
	public static IEnumerator LoadModel<TObjectModel>(CosmeticID id, Action<TObjectModel> LoadModelEvent, Action FailedLoadEvent)
	{
		while (!CosmeticsManager.Cosmetics.HasEntries())
		{
			yield return null;
		}
		string cosmetic = CosmeticsManager.Cosmetics.GetCosmetic(id);
		if (string.IsNullOrEmpty(cosmetic))
		{
			FailedLoadEvent?.Invoke();
			yield break;
		}
		AsyncOperationHandle<TObjectModel> opHandle = Addressables.LoadAssetAsync<TObjectModel>(cosmetic);
		yield return opHandle;
		if (opHandle.Status == AsyncOperationStatus.Succeeded)
		{
			LoadModelEvent?.Invoke(opHandle.Result);
			yield break;
		}
		Debug.LogError("NO ASSET IS LOADED!");
		FailedLoadEvent?.Invoke();
	}
}
