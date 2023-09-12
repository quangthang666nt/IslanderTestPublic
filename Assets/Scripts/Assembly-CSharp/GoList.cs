using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Game Object List", menuName = "CUSTOM/Game Object List", order = 1)]
public class GoList : ScriptableObject
{
	public List<GameObject> liGo = new List<GameObject>();

	public GameObject GoGetRandom()
	{
		return liGo[Random.Range(0, liGo.Count)];
	}
}
