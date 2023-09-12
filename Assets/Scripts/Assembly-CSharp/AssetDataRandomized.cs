using UnityEngine;

public class AssetDataRandomized
{
	public GameObject goAsset;

	public Vector3 v3Scale;

	public Vector3 v3Rotation;

	public Quaternion qRotation => Quaternion.Euler(v3Rotation);
}
