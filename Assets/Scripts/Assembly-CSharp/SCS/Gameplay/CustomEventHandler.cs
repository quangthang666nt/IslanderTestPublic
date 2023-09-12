using System;
using UnityEngine;

namespace SCS.Gameplay
{
	[CreateAssetMenu(menuName = "CUSTOM/Events/Create New Event")]
	public class CustomEventHandler : ScriptableObject
	{
		private Action eventDispatcher;

		private void OnDisable()
		{
			ClearEvent();
		}

		private void OnDestroy()
		{
			ClearEvent();
		}

		public void RegisterEvent(Action eventHandler)
		{
			eventDispatcher = (Action)Delegate.Combine(eventDispatcher, eventHandler);
		}

		public void UnRegisterEvent(Action eventHandler)
		{
			eventDispatcher = (Action)Delegate.Remove(eventDispatcher, eventHandler);
		}

		public void ClearEvent()
		{
			eventDispatcher = null;
		}

		public void Dispatch()
		{
			eventDispatcher?.Invoke();
		}

		public Delegate[] GetEventArray()
		{
			if (eventDispatcher == null)
			{
				return new Delegate[0];
			}
			return eventDispatcher.GetInvocationList();
		}
	}
}
