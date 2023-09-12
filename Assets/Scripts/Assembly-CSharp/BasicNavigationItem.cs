using UnityEngine;
using UnityEngine.Events;

public class BasicNavigationItem : MonoBehaviour, INavigationItem
{
	public UnityEvent m_OnSelect;

	public UnityEvent m_OnUnselect;

	public UnityEvent m_OnSubmit;

	public bool bShouldLock = true;

	public bool IsAvailableForNavigation => base.isActiveAndEnabled;

	public virtual RectTransform RectTransform => base.transform as RectTransform;

	public bool TryCallOnClickFunction()
	{
		return false;
	}

	public virtual void OnUnselect()
	{
		if (m_OnUnselect != null)
		{
			m_OnUnselect.Invoke();
		}
	}

	public virtual void OnSelect()
	{
		if (m_OnSelect != null)
		{
			m_OnSelect.Invoke();
		}
	}

	public virtual void OnSubmit()
	{
		if (m_OnSubmit != null)
		{
			m_OnSubmit.Invoke();
		}
	}

	public virtual void SendInput(Vector2 moveAxis)
	{
	}
}
