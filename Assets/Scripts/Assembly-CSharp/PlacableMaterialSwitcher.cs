using UnityEngine;

public class PlacableMaterialSwitcher : MonoBehaviour
{
	public Material matPlacable;

	public Material matNotPlacable;

	private UiBuildingButtonManager uibutman;

	private MeshRenderer meshRen;

	private void Start()
	{
		uibutman = UiBuildingButtonManager.singleton;
		meshRen = GetComponent<MeshRenderer>();
	}

	private void Update()
	{
		if (uibutman.BBuildingPlacable)
		{
			meshRen.material = matPlacable;
		}
		else
		{
			meshRen.material = matNotPlacable;
		}
	}
}
