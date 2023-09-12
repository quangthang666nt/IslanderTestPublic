using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BoosterPack", menuName = "CUSTOM/BoosterPack", order = 1)]
public class BoosterPack : ScriptableObject
{
	[SerializeField]
	private int iMaxShuffleDistance;

	[SerializeField]
	private List<GameObject> liGoBuildings;

	public List<BoosterPack> liBoosterPackFollowUps;

	private List<GameObject> liGoBuildingsCopy;

	private List<BoosterPack> liBoosterPackFollowUpsCopy;

	[HideInInspector]
	public bool bShuffeled;

	private void OnEnable()
	{
		bShuffeled = false;
	}

	private void Shuffle()
	{
		bShuffeled = true;
		liGoBuildingsCopy = new List<GameObject>();
		foreach (GameObject liGoBuilding in liGoBuildings)
		{
			liGoBuildingsCopy.Add(liGoBuilding);
		}
		for (int i = 0; i < iMaxShuffleDistance; i++)
		{
			for (int j = 0; j < liGoBuildingsCopy.Count - 1; j++)
			{
				if (Random.value < 0.5f)
				{
					GameObject value = liGoBuildingsCopy[j];
					GameObject value2 = liGoBuildingsCopy[j + 1];
					liGoBuildingsCopy[j] = value2;
					liGoBuildingsCopy[j + 1] = value;
					j++;
				}
			}
		}
	}

	public GameObject GoGetBuilding()
	{
		if (!bShuffeled)
		{
			Shuffle();
		}
		if (liGoBuildingsCopy.Count > 0)
		{
			GameObject result = liGoBuildingsCopy[0];
			liGoBuildingsCopy.RemoveAt(0);
			return result;
		}
		return null;
	}

	public BoosterPack BoosterPackGetNext()
	{
		bShuffeled = false;
		return liBoosterPackFollowUps[Random.Range(0, liBoosterPackFollowUps.Count)];
	}

	public bool BHasRemainingBuildings()
	{
		if (!bShuffeled)
		{
			Shuffle();
		}
		return liGoBuildingsCopy.Count > 0;
	}
}
