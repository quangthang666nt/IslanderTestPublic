using System.Collections;
using UnityEngine;

public class TO_DebugMessage : TerrainOperation
{
	[SerializeField]
	private string strMessage;

	public override IEnumerator Execute(TerrainGenerator _terrainGenerator)
	{
		Debug.Log(strMessage);
		bExecuteDone = true;
		yield return null;
	}
}
