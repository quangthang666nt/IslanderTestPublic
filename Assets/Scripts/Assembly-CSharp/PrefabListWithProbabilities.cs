using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PListProp", menuName = "CUSTOM/Prefab List With Probabilities", order = 1)]
public class PrefabListWithProbabilities : ScriptableObject
{
	[Serializable]
	public class ListPrefabWithProbability
	{
		public List<PrefabWithProbability> liPrefabAndPropability = new List<PrefabWithProbability>();
	}

	[Serializable]
	public class PrefabWithProbability
	{
		public GameObject goPrefab;

		[Range(0f, 10f)]
		public float fPropability = 1f;
	}

	public List<ListPrefabWithProbability> liGameVersions = new List<ListPrefabWithProbability>();

	public GameObject GoReturnRandom()
	{
		int iGameVersionCurrentlyPlayingOn = LocalGameManager.singleton.iGameVersionCurrentlyPlayingOn;
		iGameVersionCurrentlyPlayingOn = Mathf.Clamp(iGameVersionCurrentlyPlayingOn, 0, liGameVersions.Count - 1);
		List<PrefabWithProbability> liPrefabAndPropability = liGameVersions[iGameVersionCurrentlyPlayingOn].liPrefabAndPropability;
		float num = 0f;
		foreach (PrefabWithProbability item in liPrefabAndPropability)
		{
			num += item.fPropability;
		}
		GameObject result = null;
		float num2 = UnityEngine.Random.Range(0f, num);
		foreach (PrefabWithProbability item2 in liPrefabAndPropability)
		{
			num2 -= item2.fPropability;
			if (num2 <= 0f)
			{
				result = item2.goPrefab;
				break;
			}
		}
		return result;
	}
}
