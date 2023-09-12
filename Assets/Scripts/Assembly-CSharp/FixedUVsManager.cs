using System.Collections.Generic;
using UnityEngine;

public class FixedUVsManager : MonoBehaviour
{
	public Vector2 UVValue;

	private void Start()
	{
		MeshFilter component = GetComponent<MeshFilter>();
		if (!(component == null))
		{
			Mesh mesh = component.mesh;
			List<Vector2> list = new List<Vector2>();
			for (int i = 0; i < mesh.uv.Length; i++)
			{
				list.Add(UVValue);
			}
			mesh.SetUVs(0, list);
			component.mesh = mesh;
		}
	}

	private void Update()
	{
	}
}
