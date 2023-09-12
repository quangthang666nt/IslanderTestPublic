using UnityEngine;

public class UIBuildingPackSelectable : MonoBehaviour
{
	[SerializeField]
	private GameObject selectorOutline;

	public void ToggleSelectorOutline(bool value)
	{
		selectorOutline.SetActive(value);
	}
}
