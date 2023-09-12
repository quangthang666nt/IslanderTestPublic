using UnityEngine;

public class UIDeletionButton : MonoBehaviour
{
	[SerializeField]
	private GameObject goTextToUpdate;

	private void Start()
	{
		goTextToUpdate.SetActive(value: false);
	}

	private void Update()
	{
		goTextToUpdate.SetActive(UiBuildingButtonManager.singleton.IsDeleteBuildingButtonSelected());
	}
}
