using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TO_ResolveLookAt : TerrainOperation
{
	public override IEnumerator Execute(TerrainGenerator _terrainGenerator)
	{
		bExecuteDone = false;
		TO_HandlerLookAt[] array = Object.FindObjectsOfType<TO_HandlerLookAt>();
		Dictionary<int, List<TO_HandlerIdentifier>> allHandlerIdentifiers = TO_HandlerIdentifier.GetAllHandlerIdentifiers();
		TO_HandlerLookAt[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].Resolve(allHandlerIdentifiers);
		}
		bExecuteDone = true;
		yield return null;
	}
}
