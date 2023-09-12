using System.Collections;
using UnityEngine;

public class TerrainOperation : MonoBehaviour
{
	protected bool bExecuteDone;

	public bool BExecuteDone
	{
		get
		{
			return bExecuteDone;
		}
		set
		{
			bExecuteDone = value;
		}
	}

	public virtual IEnumerator Execute(TerrainGenerator _terrainGenerator)
	{
		bExecuteDone = false;
		bExecuteDone = true;
		yield return null;
	}
}
