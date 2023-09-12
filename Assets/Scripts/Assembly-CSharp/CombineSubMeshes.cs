using UnityEngine;

public class CombineSubMeshes : MonoBehaviour
{
	[SerializeField]
	private bool combineAtStart;

	public string folderPath = "";

	public string meshName = "";

	private void Start()
	{
		if (combineAtStart)
		{
			ExecuteCombineSubMeshes();
		}
	}

	public void ExecuteCombineSubMeshes()
	{
		MeshFilter component = GetComponent<MeshFilter>();
		if (!(component == null) && !(component.mesh == null))
		{
			component.mesh.subMeshCount = 1;
		}
	}
}
