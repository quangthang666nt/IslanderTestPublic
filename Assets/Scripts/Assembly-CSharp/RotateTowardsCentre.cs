using UnityEngine;

public class RotateTowardsCentre : MonoBehaviour
{
	[Range(0f, 1f)]
	[SerializeField]
	private float fRotateAmount = 1f;

	private Vector3 v3Centre;

	private Transform trThisTransform;

	private void Start()
	{
		trThisTransform = base.transform;
		Rotate();
	}

	private void Rotate()
	{
		if ((bool)IslandManager.singleton.GoCurrentIsland)
		{
			v3Centre = IslandManager.singleton.GoCurrentIsland.transform.position;
		}
		Vector3 b = v3Centre - trThisTransform.position;
		trThisTransform.forward = Vector3.Lerp(trThisTransform.forward, b, fRotateAmount);
	}
}
