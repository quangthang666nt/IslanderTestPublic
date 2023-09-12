using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIElement : MonoBehaviour
{
	[Serializable]
	public class ToggleObjectData
	{
		public GameObject goToToggle;

		public float fInDelay;

		public float fOutDelay;
	}

	[SerializeField]
	private List<ToggleObjectData> ObjectsToToggle = new List<ToggleObjectData>();

	[SerializeField]
	public UnityEvent eventOnActivation = new UnityEvent();

	[HideInInspector]
	public UnityEvent internalEventOnActivation = new UnityEvent();

	[SerializeField]
	private float activationEventDelay;

	[SerializeField]
	public UnityEvent eventOnDeactivation = new UnityEvent();

	[HideInInspector]
	public UnityEvent internalEventOnDeactivation = new UnityEvent();

	[SerializeField]
	private float deactivationEventDelay;

	public bool active;

	public float MaxDelay
	{
		get
		{
			float num = 0f;
			foreach (ToggleObjectData item in ObjectsToToggle)
			{
				num = Mathf.Max(num, item.fOutDelay);
			}
			return num;
		}
	}

	public virtual void EnableElement()
	{
		if (active)
		{
			return;
		}
		active = true;
		StopAllCoroutines();
		foreach (ToggleObjectData item in ObjectsToToggle)
		{
			StartCoroutine(SetActiveAfterTime(item.goToToggle, item.fInDelay, bValue: true));
		}
		StartCoroutine(CallActivationEvent());
	}

	public virtual void DisableElement()
	{
		active = false;
		StopAllCoroutines();
		foreach (ToggleObjectData item in ObjectsToToggle)
		{
			StartCoroutine(SetActiveAfterTime(item.goToToggle, item.fOutDelay, bValue: false));
		}
		StartCoroutine(CallDeactivationEvent());
	}

	public virtual void DisableElementImmediate()
	{
		active = false;
		StopAllCoroutines();
		foreach (ToggleObjectData item in ObjectsToToggle)
		{
			item.goToToggle.SetActive(value: false);
		}
		eventOnDeactivation.Invoke();
		internalEventOnDeactivation.Invoke();
	}

	private IEnumerator CallActivationEvent()
	{
		if (activationEventDelay > 0f)
		{
			yield return new WaitForSeconds(activationEventDelay);
		}
		eventOnActivation.Invoke();
		internalEventOnActivation.Invoke();
	}

	private IEnumerator CallDeactivationEvent()
	{
		if (deactivationEventDelay > 0f)
		{
			yield return new WaitForSeconds(deactivationEventDelay);
		}
		eventOnDeactivation.Invoke();
		internalEventOnDeactivation.Invoke();
	}

	private IEnumerator SetActiveAfterTime(GameObject goTarget, float fDelay, bool bValue)
	{
		yield return new WaitForSeconds(fDelay);
		goTarget.SetActive(bValue);
	}
}
