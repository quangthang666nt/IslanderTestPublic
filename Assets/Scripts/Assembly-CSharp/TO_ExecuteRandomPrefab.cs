using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TO_ExecuteRandomPrefab : TerrainOperation
{
	[Serializable]
	public class PrefabWithProbability
	{
		public GameObject goPrefab;

		[Range(0f, 10f)]
		public float fPropability = 1f;
	}

	public List<PrefabWithProbability> liPrefabAndPropability = new List<PrefabWithProbability>();

	public override IEnumerator Execute(TerrainGenerator _terrainGenerator)
	{
		bExecuteDone = false;
		float num = 0f;
		foreach (PrefabWithProbability item in liPrefabAndPropability)
		{
			num += item.fPropability;
		}
		GameObject goChosenPrefab = null;
		float num2 = UnityEngine.Random.Range(0f, num);
		foreach (PrefabWithProbability item2 in liPrefabAndPropability)
		{
			num2 -= item2.fPropability;
			if (num2 <= 0f)
			{
				goChosenPrefab = item2.goPrefab;
				break;
			}
		}
		if (_terrainGenerator.BShouldYield())
		{
			_terrainGenerator.BeforeYield();
			yield return null;
			_terrainGenerator.AfterYield();
		}
		while (base.transform.childCount > 0)
		{
			UnityEngine.Object.DestroyImmediate(base.transform.GetChild(0).gameObject);
		}
		if (_terrainGenerator.BShouldYield())
		{
			_terrainGenerator.BeforeYield();
			yield return null;
			_terrainGenerator.AfterYield();
		}
		GameObject goChild = UnityEngine.Object.Instantiate(goChosenPrefab, base.gameObject.transform);
		if (_terrainGenerator.BShouldYield())
		{
			_terrainGenerator.BeforeYield();
			yield return null;
			_terrainGenerator.AfterYield();
		}
		TerrainGenerator component = goChild.transform.GetChild(1).gameObject.GetComponent<TerrainGenerator>();
		if ((bool)component)
		{
			_terrainGenerator.FRadius = component.FRadius;
			_terrainGenerator.overrideColorSetups = component.overrideColorSetups;
			_terrainGenerator.bWaterHeavy = component.bWaterHeavy;
			_terrainGenerator.bVertical = component.bVertical;
			_terrainGenerator.bSnow = component.bSnow;
			_terrainGenerator.bAncient = component.bAncient;
			_terrainGenerator.bAllowGrassUnlocks = component.bAllowGrassUnlocks;
			_terrainGenerator.bAllowSandUnlocks = component.bAllowSandUnlocks;
			_terrainGenerator.bAllowRockUnlocks = component.bAllowRockUnlocks;
			_terrainGenerator.bAllowWoodUnlocks = component.bAllowWoodUnlocks;
			_terrainGenerator.bAllowGoldUnlocks = component.bAllowGoldUnlocks;
		}
		if (_terrainGenerator.BShouldYield())
		{
			_terrainGenerator.BeforeYield();
			yield return null;
			_terrainGenerator.AfterYield();
		}
		TerrainOperation toChild = goChild.GetComponent<TerrainOperation>();
		if ((bool)toChild)
		{
			IEnumerator ieExecute = toChild.Execute(_terrainGenerator);
			toChild.BExecuteDone = false;
			while (!toChild.BExecuteDone)
			{
				ieExecute.MoveNext();
				yield return null;
			}
		}
		bExecuteDone = true;
		yield return null;
	}
}
