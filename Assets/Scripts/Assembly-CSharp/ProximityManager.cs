using System.Collections.Generic;
using UnityEngine;

public class ProximityManager : MonoBehaviour
{
	public static ProximityManager singleton;

	[Help("This script check which buildings are in proximity of oneanother (for the score calculation).", MessageType.Info)]
	public LayerMask layerMaskStructures;

	private Collider[] m_ArColliderResults = new Collider[1024];

	private List<int> liIIDFind = new List<int>();

	private List<GameObject> liGoReturn2 = new List<GameObject>();

	private List<GameObject> liGoReturn3 = new List<GameObject>();

	public void Awake()
	{
		singleton = this;
	}

	public void LiFindGameObjectsInRadius(Vector3 _v3Position, GameObject _goFind, float _fRadius, List<GameObject> outResult)
	{
		List<GameObject> list = new List<GameObject>();
		list.Add(_goFind);
		LiFindGameObjectsInRadius(_v3Position, list, _fRadius, outResult);
	}

	public void LiFindGameObjectsInRadius(Vector3 _v3Position, List<GameObject> _liGoFind, float _fRadius, List<GameObject> outResult)
	{
		liIIDFind.Clear();
		foreach (GameObject item in _liGoFind)
		{
			if ((bool)item)
			{
				StructureID component = item.GetComponent<StructureID>();
				if ((bool)component)
				{
					liIIDFind.Add(component.iID);
				}
			}
		}
		int num = Physics.OverlapSphereNonAlloc(_v3Position, _fRadius, m_ArColliderResults, layerMaskStructures);
		outResult.Clear();
		for (int i = 0; i < num; i++)
		{
			Collider collider = m_ArColliderResults[i];
			if ((bool)collider.transform.parent)
			{
				GameObject gameObject = collider.transform.parent.gameObject;
				StructureID component2 = gameObject.GetComponent<StructureID>();
				if ((bool)component2 && liIIDFind.Contains(component2.iID) && !outResult.Contains(gameObject))
				{
					outResult.Add(gameObject);
				}
			}
		}
	}

	public List<GameObject> LiFindGameObjectsInRadius(List<Vector3> _liV3Position, List<GameObject> _liGoFind, float _fRadius)
	{
		liGoReturn3.Clear();
		foreach (Vector3 item in _liV3Position)
		{
			liGoReturn2.Clear();
			LiFindGameObjectsInRadius(item, _liGoFind, _fRadius, liGoReturn2);
			foreach (GameObject item2 in liGoReturn2)
			{
				if (!liGoReturn3.Contains(item2))
				{
					liGoReturn3.Add(item2);
				}
			}
		}
		return liGoReturn3;   
	}
}
