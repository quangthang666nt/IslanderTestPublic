using Rewired;
using UnityEngine;

public class ActiveOnControllerType : MonoBehaviour
{
	[SerializeField]
	private ControllerType controllerType;

	[SerializeField]
	private bool allowInMouseController;

	private void Start()
	{
		if (PlatformPlayerManagerSystem.Instance != null)
		{
			PlatformPlayerManagerSystem.Instance.OnLastActiveControllerUpdated += OnActiveControllerUpdated;
			OnActiveControllerUpdated(PlatformPlayerManagerSystem.Instance.LastActiveController);
		}
	}

	private void OnDestroy()
	{
		if (PlatformPlayerManagerSystem.IsReady)
		{
			PlatformPlayerManagerSystem.Instance.OnLastActiveControllerUpdated -= OnActiveControllerUpdated;
		}
	}

	private void OnActiveControllerUpdated(Controller controller)
	{
		base.gameObject.SetActive(controllerType == controller.type || (allowInMouseController && controller.type == ControllerType.Mouse));
	}
}
