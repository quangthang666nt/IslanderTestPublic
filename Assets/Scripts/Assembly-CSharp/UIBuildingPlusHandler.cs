using UnityEngine;

public class UIBuildingPlusHandler : MonoBehaviour
{
	public GameObject target;

	private bool previous;

	private void OnEnable()
	{
		previous = target.activeSelf;
		target.SetActive(value: false);
	}

	private void OnDisable()
	{
		target.SetActive(previous);
	}
}
