using System.Collections;
using UnityEngine;

public class TO_ExecuteRandomChild : TerrainOperation
{
	[SerializeField]
	private int iRepeat = 1;

	public override IEnumerator Execute(TerrainGenerator _terrainGenerator)
	{
		bExecuteDone = false;
		for (int iRep = 0; iRep < iRepeat; iRep++)
		{
			int index = Random.Range(0, base.transform.childCount);
			GameObject gameObject = base.transform.GetChild(index).gameObject;
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
		bExecuteDone = true;
		yield return null;
	}
}
