using System.Collections.Generic;
using UnityEngine;

public class TO_HandlerLookAt : MonoBehaviour
{
	public string identifier;

	public float offsetEuler;

	public float variationEuler;

	public void Resolve(Dictionary<int, List<TO_HandlerIdentifier>> identifiers)
	{
		int hashCode = identifier.GetHashCode();
		if (!identifiers.ContainsKey(hashCode))
		{
			Debug.LogWarning("[TerrainGeneration - HandlerLookAt] Didn't find identifier " + identifier);
			return;
		}
		List<TO_HandlerIdentifier> list = identifiers[hashCode];
		int num = Random.Range(0, list.Count);
		Vector3 position = list[num].transform.position;
		position.y = base.transform.position.y;
		if (position == base.transform.position)
		{
			if (list.Count > num + 1)
			{
				position = list[num + 1].transform.position;
				position.y = base.transform.position.y;
			}
			else if (num > 0)
			{
				position = list[num - 1].transform.position;
				position.y = base.transform.position.y;
			}
		}
		base.transform.LookAt(position);
		base.transform.Rotate(Vector3.up * offsetEuler);
		float num2 = Random.Range(0f, variationEuler) - variationEuler * 0.5f;
		base.transform.Rotate(Vector3.up * num2);
	}
}
