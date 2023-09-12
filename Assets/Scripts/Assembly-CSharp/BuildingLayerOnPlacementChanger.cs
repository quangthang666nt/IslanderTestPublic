using UnityEngine;

public class BuildingLayerOnPlacementChanger : MonoBehaviour
{
	[SerializeField]
	private int iTargetLayer;

	public void SwitchLayer()
	{
		base.gameObject.layer = iTargetLayer;
	}
}
