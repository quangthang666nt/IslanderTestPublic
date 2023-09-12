using System.Collections;
using UnityEngine;

public class TO_ExecuteAllChildren : TerrainOperation
{
	[SerializeField]
	private GameObject goOverride;

	[SerializeField]
	private GameObject goDestroyOnExecute;

	[SerializeField]
	private int iRepeat = 1;

	public override IEnumerator Execute(TerrainGenerator _terrainGenerator)
	{
		bExecuteDone = false;
		TO_HandlerIdentifier.ClearHandlersCache();
		if (goOverride == null)
		{
			goOverride = base.gameObject;
		}
		if (_terrainGenerator.BShouldYield())
		{
			_terrainGenerator.BeforeYield();
			yield return null;
			_terrainGenerator.AfterYield();
		}
		if ((bool)goDestroyOnExecute)
		{
			Object.DestroyImmediate(goDestroyOnExecute);
		}
		if (_terrainGenerator.BShouldYield())
		{
			_terrainGenerator.BeforeYield();
			yield return null;
			_terrainGenerator.AfterYield();
		}
		for (int iRep = 0; iRep < iRepeat; iRep++)
		{
			for (int i = 0; i < goOverride.transform.childCount; i++)
			{
				GameObject gameObject = goOverride.transform.GetChild(i).gameObject;
				TerrainOperation toChild = gameObject.GetComponent<TerrainOperation>();
				if ((bool)toChild && toChild.gameObject.activeInHierarchy)
				{
					IEnumerator ieExecute = toChild.Execute(_terrainGenerator);
					toChild.BExecuteDone = false;
					while (!toChild.BExecuteDone)
					{
						ieExecute.MoveNext();
						yield return null;
					}
				}
				if (_terrainGenerator.BShouldYield())
				{
					_terrainGenerator.BeforeYield();
					yield return null;
					_terrainGenerator.AfterYield();
				}
			}
		}
		bExecuteDone = true;
		yield return null;
	}
}
