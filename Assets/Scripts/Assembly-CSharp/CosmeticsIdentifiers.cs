using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Islanders/CosmeticsIdentifiers")]
public class CosmeticsIdentifiers : ScriptableObject
{
	public List<string> ids;

	public string ExportIds()
	{
		string text = "";
		if (ids == null || ids.Count == 0)
		{
			return text;
		}
		for (int i = 0; i < ids.Count; i++)
		{
			text = text + ids[i] + "\n";
		}
		return text;
	}
}
