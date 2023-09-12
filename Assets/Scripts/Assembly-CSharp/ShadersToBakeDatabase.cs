using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShadersToBake")]
public class ShadersToBakeDatabase : ScriptableObject
{
	public List<Shader> shadersToBake;

	[NonSerialized]
	private List<int> hashCodes;

	private void CreateHashCodeList()
	{
		hashCodes = new List<int>();
		foreach (Shader item in shadersToBake)
		{
			int hashCode = item.name.GetHashCode();
			hashCodes.Add(hashCode);
		}
	}

	public bool Contains(Shader shader)
	{
		if (shader == null)
		{
			return false;
		}
		if (hashCodes == null)
		{
			CreateHashCodeList();
		}
		int hashCode = shader.name.GetHashCode();
		for (int i = 0; i < shadersToBake.Count; i++)
		{
			if (hashCode == hashCodes[i])
			{
				return true;
			}
		}
		return false;
	}
}
