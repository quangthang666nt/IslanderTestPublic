using System.Collections.Generic;
using UnityEngine;

public class StructureIDTranslator : ScriptableObject
{
	public static StructureIDTranslator singleton;

	[SerializeField]
	private List<GameObject> liGoStructures;

	[SerializeField]
	private List<int> liIStructureIDs;

	public void Activate()
	{
		singleton = this;
	}

	private void OnEnable()
	{
		Activate();
	}

	public static GameObject GoGetOriginalPrefabFromGameObject(GameObject _goTranslate)
	{
		int num = IGetIDFromGameObject(_goTranslate);
		if (num >= 0)
		{
			return GoGetGameObjectFromID(num);
		}
		return null;
	}

	public static GameObject GoGetGameObjectFromID(int _iTranslateID)
	{
		int num = -1;
		for (int i = 0; i < singleton.liIStructureIDs.Count; i++)
		{
			if (singleton.liIStructureIDs[i] == _iTranslateID)
			{
				num = i;
				break;
			}
		}
		if (num >= 0)
		{
			return singleton.liGoStructures[num];
		}
		return null;
	}

	public static int IGetIDFromGameObject(GameObject _goTranslate)
	{
		StructureID component = _goTranslate.GetComponent<StructureID>();
		if (!component)
		{
			return -1;
		}
		return component.iID;
	}
}
