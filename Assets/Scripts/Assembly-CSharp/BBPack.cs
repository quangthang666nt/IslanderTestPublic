using System.Collections.Generic;
using I2.Loc;
using UnityEngine;

[CreateAssetMenu(fileName = "BBPack", menuName = "CUSTOM/BBPack", order = 1)]
public class BBPack : ScriptableObject
{
	public enum EAvailableIn
	{
		Winter = 0,
		Summer = 1,
		Both = 2
	}

	public bool bAlwaysShowWhenAvailable;

	public LocalizedString strPackName;

	public LocalizedString strPackDescriptionTemplate = "Tooltips/Building Pack Description Template 01";

	public string strPackLookupName;

	public string strPackLookupDescription;

	public GameObject goPackIcon;

	public List<GameObject> liGoBuildings;

	public List<GameObject> liGoRequirements;

	public int iRequirementJokers;

	public int iMinimumRequiredTotalUnlocks;

	[Space]
	public bool bRequiresGrass;

	public bool bRequiresSand;

	public bool bRequiresNoSand;

	public bool bRequiresRock;

	public bool bRequiresWood;

	public bool bRequiresGold;

	public EAvailableIn eAvailableIn = EAvailableIn.Both;

	public string StrPackName => strPackName;

	public string StrPackDescription => GenerateTooltip();

	private string GenerateTooltip()
	{
		string text = strPackDescriptionTemplate;
		string text2 = "";
		for (int i = 0; i < liGoBuildings.Count; i++)
		{
			Building component = liGoBuildings[i].GetComponent<Building>();
			if ((bool)component)
			{
				if (i > 0 && i < liGoBuildings.Count - 1)
				{
					text2 += ", ";
				}
				if (i == liGoBuildings.Count - 1)
				{
					text2 = text2 + "</style> " + LocalizationManager.GetTranslation("Tooltips/BuildingPacks/'And'") + " <style=GET>";
				}
				text2 += component.strBuildingName;
			}
		}
		if (text2.Length > 0)
		{
			text = text.Replace("{building_names}", text2);
		}
		return text;
	}
}
