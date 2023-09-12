using System.Collections.Generic;
using UnityEngine;

public class AssetFolder : MonoBehaviour
{
	public static List<AssetFolder> liAssetFolderAll = new List<AssetFolder>();

	public static void UnpackAll()
	{
		for (int num = liAssetFolderAll.Count - 1; num >= 0; num--)
		{
			liAssetFolderAll[num].Unpack();
		}
	}

	private void OnEnable()
	{
		liAssetFolderAll.Add(this);
	}

	private void OnDisable()
	{
		liAssetFolderAll.Remove(this);
	}

	public void Unpack()
	{
		Transform transform = base.transform;
		Transform parent = transform.parent;
		while (transform.childCount > 0)
		{
			transform.GetChild(0).SetParent(parent);
		}
		Object.Destroy(base.gameObject);
	}
}
