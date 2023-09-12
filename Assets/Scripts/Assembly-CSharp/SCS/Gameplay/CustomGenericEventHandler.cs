using System;
using UnityEngine;

namespace SCS.Gameplay
{
	public abstract class CustomGenericEventHandler<T> : ScriptableObject
	{
		private Action<T> eventDispatcher;

		private T lastState;

		private void OnDisable()
		{
			ClearEvent();
		}

		private void OnDestroy()
		{
			ClearEvent();
		}

		public void RegisterEvent(Action<T> eventHandler)
		{
			eventDispatcher = (Action<T>)Delegate.Combine(eventDispatcher, eventHandler);
		}

		public void UnRegisterEvent(Action<T> eventHandler)
		{
			eventDispatcher = (Action<T>)Delegate.Remove(eventDispatcher, eventHandler);
		}

		public void ClearEvent()
		{
			eventDispatcher = null;
		}

		public void Dispatch(T data)
		{
			eventDispatcher?.Invoke(data);
			lastState = data;
		}

		public Delegate[] GetEventArray()
		{
			if (eventDispatcher == null)
			{
				return new Delegate[0];
			}
			return eventDispatcher.GetInvocationList();
		}

		public Type GetHandlerType()
		{
			return typeof(T);
		}

		public T GetParameterLastState()
		{
			return lastState;
		}
	}
}
