using UnityEngine;
using UnityEngine.UI;

public class UIUndoButton : MonoBehaviour
{
	[SerializeField]
	private GameObject parentObject;

	[SerializeField]
	private Button button;

	private SaveLoadManager saveLoadManager;

	private void Start()
	{
		saveLoadManager = SaveLoadManager.singleton;
	}

	private void Update()
	{
		if (saveLoadManager.bIsUndoAvailable())
		{
			button.interactable = true;
			parentObject.SetActive(value: true);
		}
		else
		{
			button.interactable = false;
			parentObject.SetActive(value: false);
		}
	}
}
