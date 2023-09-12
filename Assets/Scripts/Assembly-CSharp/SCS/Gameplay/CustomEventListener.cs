using UnityEngine;
using UnityEngine.Events;

namespace SCS.Gameplay
{
	public class CustomEventListener : MonoBehaviour
	{
		[SerializeField]
		private CustomEventHandler customEvent;

		[SerializeField]
		[Tooltip("Destroy after event called")]
		private bool doOnce = true;

		[Header("Callback")]
		[SerializeField]
		private UnityEvent OnEventCalled;

		private void OnEnable()
		{
			customEvent?.RegisterEvent(CallEvent);
		}

		private void OnDisable()
		{
			if (!doOnce)
			{
				customEvent.UnRegisterEvent(CallEvent);
			}
		}

		private void CallEvent()
		{
			OnEventCalled?.Invoke();
			if (doOnce)
			{
				customEvent.UnRegisterEvent(CallEvent);
				Object.Destroy(this);
			}
		}
	}
}
