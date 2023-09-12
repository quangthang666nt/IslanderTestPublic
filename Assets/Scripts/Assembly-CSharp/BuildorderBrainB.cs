using System.Collections.Generic;
using UnityEngine;

public class BuildorderBrainB : MonoBehaviour
{
	public static BuildorderBrainB singleton;

	[Help("This script decides which buildings are dropped into your inventory when opening a new building back (aka plus button). Nothing interesting to see here.", MessageType.Info)]
	public GameObject goWallPlateau;

	public BBPack bbPackWaterPlateau;

	public BBPack bbPackWallPlateau;

	public BBPack bbPackAncientStatue;

	public List<BBPack> liBBPacksAll;

	public List<BBPack> liBBPackUnlockableRemaining = new List<BBPack>();

	public List<GameObject> liGoUnlockedBuildings = new List<GameObject>();

	public List<GameObject> liGoRemaining = new List<GameObject>();

	public List<GameObject> liGoGuaranteedNext = new List<GameObject>();

	public Inventory<GameObject> inventoryReceivedBuildings = new Inventory<GameObject>();

	public Random.State randomStateForNextChoice;

	public bool bShuffled;

	private void Awake()
	{
		singleton = this;
	}

	public void Reset()
	{
		bShuffled = false;
		CheckShuffled();
	}

	private void CheckShuffled()
	{
		if (bShuffled)
		{
			return;
		}
		inventoryReceivedBuildings.Clear();
		bShuffled = true;
		liBBPackUnlockableRemaining.Clear();
		liGoUnlockedBuildings.Clear();
		liGoRemaining.Clear();
		foreach (BBPack item in liBBPacksAll)
		{
			liBBPackUnlockableRemaining.Add(item);
		}
		liBBPackUnlockableRemaining.Add(bbPackWaterPlateau);
		liBBPackUnlockableRemaining.Add(bbPackWallPlateau);
		liBBPackUnlockableRemaining.Add(bbPackAncientStatue);
	}

	public GameObject GoGetBuilding()
	{
		CheckShuffled();
		GameObject gameObject = null;
		if (liGoGuaranteedNext.Count <= 0)
		{
			if (liGoRemaining.Count <= 0)
			{
				ReAddAllUnlockedBuildings();
			}
			int num = Random.Range(0, liGoRemaining.Count);
			for (int i = 0; i < 100; i++)
			{
				num = Random.Range(0, liGoRemaining.Count);
				gameObject = liGoRemaining[num];
				if (BCanGiveOutThisBuilding(gameObject))
				{
					break;
				}
			}
		}
		else
		{
			gameObject = liGoGuaranteedNext[0];
			liGoGuaranteedNext.RemoveAt(0);
		}
		if (liGoRemaining.Contains(gameObject))
		{
			liGoRemaining.Remove(gameObject);
		}
		inventoryReceivedBuildings.Add(gameObject);
		return gameObject;
	}

	private void ReAddAllUnlockedBuildings()
	{
		TerrainGenerator terrainGenerator = TerrainGenerator.singleton;
		foreach (GameObject liGoUnlockedBuilding in liGoUnlockedBuildings)
		{
			Building component = liGoUnlockedBuilding.GetComponent<Building>();
			if (component.bReAddIntoDeck)
			{
				int num = component.iAmount;
				if (terrainGenerator.bWaterHeavy)
				{
					num += component.iAmountWater;
				}
				for (int i = 0; i < num; i++)
				{
					liGoRemaining.Add(liGoUnlockedBuilding);
				}
			}
		}
	}

	public void UnlockBBPack(BBPack _bbPack)
	{
		TerrainGenerator terrainGenerator = TerrainGenerator.singleton;
		if (liBBPackUnlockableRemaining.Contains(_bbPack))
		{
			liBBPackUnlockableRemaining.Remove(_bbPack);
		}
		foreach (GameObject liGoBuilding in _bbPack.liGoBuildings)
		{
			Building component = liGoBuilding.GetComponent<Building>();
			liGoGuaranteedNext.Add(liGoBuilding);
			if (liGoBuilding == goWallPlateau)
			{
				liGoGuaranteedNext.Add(liGoBuilding);
				liGoGuaranteedNext.Add(liGoBuilding);
			}
			if (liGoUnlockedBuildings.Contains(liGoBuilding))
			{
				continue;
			}
			liGoUnlockedBuildings.Add(liGoBuilding);
			if (liGoBuilding != goWallPlateau)
			{
				int num = component.iAmount;
				if (terrainGenerator.bWaterHeavy)
				{
					num += component.iAmountWater;
				}
				for (int i = 0; i < num; i++)
				{
					liGoRemaining.Add(liGoBuilding);
				}
			}
		}
		liGoRemaining.Shuffle();
	}

	private List<BBPack> LiBBPacksGetUnlockable()
	{
		CheckShuffled();
		List<BBPack> list = new List<BBPack>();
		TerrainGenerator terrainGenerator = TerrainGenerator.singleton;
		if (terrainGenerator.bWaterHeavy && liBBPackUnlockableRemaining.Contains(bbPackWaterPlateau))
		{
			list.Add(bbPackWaterPlateau);
		}
		else if (terrainGenerator.bVertical && liBBPackUnlockableRemaining.Contains(bbPackWallPlateau))
		{
			list.Add(bbPackWallPlateau);
		}
		else if (terrainGenerator.bAncient && liBBPackUnlockableRemaining.Contains(bbPackAncientStatue))
		{
			list.Add(bbPackAncientStatue);
		}
		else
		{
			for (int i = 0; i <= 10; i++)
			{
				foreach (BBPack item in liBBPackUnlockableRemaining)
				{
					if (BCanUlockThisPack(item, terrainGenerator, i))
					{
						list.Add(item);
					}
				}
				if (list.Count > 0)
				{
					break;
				}
			}
		}
		return list;
	}

	public List<BBPack> LiBBPacksGetUnlockable(int _iAmount)
	{
		Random.State state = Random.state;
		Random.state = randomStateForNextChoice;
		List<BBPack> list = LiBBPacksGetUnlockable();
		List<BBPack> list2 = new List<BBPack>();
		for (int num = list.Count - 1; num >= 0; num--)
		{
			BBPack bBPack = list[num];
			if (bBPack.bAlwaysShowWhenAvailable)
			{
				list2.Add(bBPack);
				list.RemoveAt(num);
			}
		}
		while (list2.Count < _iAmount && list.Count > 0)
		{
			int index = Random.Range(0, list.Count);
			BBPack item = list[index];
			list2.Add(item);
			list.RemoveAt(index);
		}
		randomStateForNextChoice = Random.state;
		Random.state = state;
		return list2;
	}

	private bool BCanUlockThisPack(BBPack _bbPack, TerrainGenerator _terrainGen, int iTolerance)
	{
		if (_bbPack.bRequiresGold && !_terrainGen.bAllowGoldUnlocks)
		{
			return false;
		}
		if (_bbPack.bRequiresGrass && !_terrainGen.bAllowGrassUnlocks)
		{
			return false;
		}
		if (_bbPack.bRequiresRock && !_terrainGen.bAllowRockUnlocks)
		{
			return false;
		}
		if (_bbPack.bRequiresSand && !_terrainGen.bAllowSandUnlocks)
		{
			return false;
		}
		if (_bbPack.bRequiresNoSand && _terrainGen.bAllowSandUnlocks)
		{
			return false;
		}
		if (_bbPack.bRequiresWood && !_terrainGen.bAllowWoodUnlocks)
		{
			return false;
		}
		if (_bbPack.eAvailableIn != BBPack.EAvailableIn.Both)
		{
			if (_bbPack.eAvailableIn == BBPack.EAvailableIn.Winter && !_terrainGen.bSnow)
			{
				return false;
			}
			if (_bbPack.eAvailableIn == BBPack.EAvailableIn.Summer && _terrainGen.bSnow)
			{
				return false;
			}
		}
		int num = _bbPack.iRequirementJokers + iTolerance;
		foreach (GameObject liGoRequirement in _bbPack.liGoRequirements)
		{
			if (!liGoUnlockedBuildings.Contains(liGoRequirement))
			{
				num--;
				if (num < 0)
				{
					return false;
				}
			}
		}
		if (liGoUnlockedBuildings.Count + iTolerance < _bbPack.iMinimumRequiredTotalUnlocks)
		{
			return false;
		}
		return true;
	}

	private bool BCanGiveOutThisBuilding(GameObject _go)
	{
		if (TerrainGenerator.singleton.bSnow)
		{
			return true;
		}
		Building component = _go.GetComponent<Building>();
		GameObject goRequiredRelation = component.goRequiredRelation;
		float fRequiredRelationAmount = component.fRequiredRelationAmount;
		fRequiredRelationAmount *= 1f + (float)inventoryReceivedBuildings.IGet(_go);
		return (float)inventoryReceivedBuildings.IGet(goRequiredRelation) >= fRequiredRelationAmount;
	}

	public List<int> LiIBBPacksToIndecies(List<BBPack> _liBBPacks)
	{
		List<int> list = new List<int>();
		foreach (BBPack _liBBPack in _liBBPacks)
		{
			if (_liBBPack == bbPackWaterPlateau)
			{
				list.Add(-1);
				continue;
			}
			if (_liBBPack == bbPackWallPlateau)
			{
				list.Add(-2);
				continue;
			}
			if (_liBBPack == bbPackAncientStatue)
			{
				list.Add(-3);
				continue;
			}
			int item = liBBPacksAll.IndexOf(_liBBPack);
			list.Add(item);
		}
		return list;
	}

	public List<BBPack> LiBBPackFromIndecies(List<int> _liI)
	{
		List<BBPack> list = new List<BBPack>();
		foreach (int item in _liI)
		{
			switch (item)
			{
			case -1:
				list.Add(bbPackWaterPlateau);
				break;
			case -2:
				list.Add(bbPackWallPlateau);
				break;
			case -3:
				list.Add(bbPackAncientStatue);
				break;
			default:
				list.Add(liBBPacksAll[item]);
				break;
			}
		}
		return list;
	}

	public void SetNextGuaranteedBuilings()
	{
		if (LiBBPacksGetUnlockable().Count <= 0)
		{
			return;
		}
		TerrainGenerator terrainGenerator = TerrainGenerator.singleton;
		foreach (GameObject liGoUnlockedBuilding in liGoUnlockedBuildings)
		{
			Building component = liGoUnlockedBuilding.GetComponent<Building>();
			float num = component.fAmountPerUnlocks;
			if (terrainGenerator.bWaterHeavy)
			{
				num += component.fAmountPerUnlocksWater;
			}
			if (terrainGenerator.bVertical)
			{
				num += component.fAmountPerUnlocksVertical;
			}
			int num2 = (int)Mathf.Floor(num);
			if (Random.value < num % 1f)
			{
				num2 = (int)Mathf.Ceil(num);
			}
			for (int i = 0; i < num2; i++)
			{
				liGoGuaranteedNext.Add(liGoUnlockedBuilding);
			}
		}
	}
}
