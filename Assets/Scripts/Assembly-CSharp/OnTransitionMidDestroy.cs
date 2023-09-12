using UnityEngine;

public class OnTransitionMidDestroy : MonoBehaviour
{
	private void Start()
	{
		SaveLoadManager.singleton.eventOnTransitionMidEnd.AddListener(SelfDestroy);
	}

	private void SelfDestroy()
	{
		Object.Destroy(base.gameObject);
	}
}
