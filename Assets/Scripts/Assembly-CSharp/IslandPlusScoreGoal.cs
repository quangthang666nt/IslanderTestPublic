using System;
using UnityEngine;

[Serializable]
public class IslandPlusScoreGoal
{
	[Tooltip("Stores all possible island generator prefabs for this island.")]
	public PrefabListWithProbabilities prefabListWithProb;

	[Tooltip("Points you need to reach to go to the next island.")]
	public int iScoreGoal;
}
