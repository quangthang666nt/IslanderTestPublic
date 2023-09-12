using Rewired;
using UnityEngine;

public class SaveInputDataOnDisable : MonoBehaviour
{
	private void OnDisable()
	{
		if (ReInput.userDataStore != null)
		{
			ReInput.userDataStore.Save();
		}
	}
}
