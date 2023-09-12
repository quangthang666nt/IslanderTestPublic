using UnityEngine;

public class LockInputOnEnable : MonoBehaviour
{
	private void OnEnable()
	{
		InputManager.Singleton.AddInputLocker(this);
	}

	private void OnDisable()
	{
		InputManager.Singleton.RemoveInputLocker(this);
	}
}
