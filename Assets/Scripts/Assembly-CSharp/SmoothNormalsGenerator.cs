using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SmoothNormalsGenerator : MonoBehaviour
{
	[SerializeField]
	private bool calculateSmoothNormalAtStart = true;

	private void Start()
	{
		if (calculateSmoothNormalAtStart)
		{
			SetSmoothNormals();
		}
	}

	public void SetSmoothNormals()
	{
		MeshFilter component = GetComponent<MeshFilter>();
		if (!(component == null) && !(component.mesh == null))
		{
			List<Vector3> uvs = CalculateSmoothNormals(component.mesh);
			component.mesh.SetUVs(3, uvs);
		}
	}

	private List<Vector3> CalculateSmoothNormals(Mesh mesh)
	{
		List<Vector3> list = new List<Vector3>(mesh.normals);
		foreach (IGrouping<Vector3, KeyValuePair<Vector3, int>> item in from pair in mesh.vertices.Select((Vector3 vertex, int index) => new KeyValuePair<Vector3, int>(vertex, index))
			group pair by pair.Key)
		{
			Vector3 zero = Vector3.zero;
			foreach (KeyValuePair<Vector3, int> item2 in item)
			{
				zero += list[item2.Value];
			}
			zero.Normalize();
			foreach (KeyValuePair<Vector3, int> item3 in item)
			{
				list[item3.Value] = zero;
			}
		}
		return list;
	}
}
