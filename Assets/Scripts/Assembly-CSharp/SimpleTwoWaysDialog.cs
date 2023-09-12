using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class SimpleTwoWaysDialog : MonoBehaviour
{
	public UnityEvent m_OnConfirm;

	public UnityEvent m_OnCancel;

	public bool sleepAfterInteract = true;

	private bool m_Ready = true;

	private UIElement UIElement;

	private void Awake()
	{
		UIElement = GetComponent<UIElement>();
	}

	private void OnEnable()
	{
		if ((bool)UIElement)
		{
			StartCoroutine(ReadyRoutine());
		}
		else
		{
			m_Ready = true;
		}
	}

	private IEnumerator ReadyRoutine()
	{
		m_Ready = false;
		yield return new WaitForSeconds(UIElement.MaxDelay);
		m_Ready = true;
	}

	private void Unready()
	{
		m_Ready = false;
	}

	private void Update()
	{
		if (!m_Ready)
		{
			return;
		}
		if (InputManager.Singleton.InputDataCurrent.bUICancel)
		{
			if (m_OnCancel != null)
			{
				m_OnCancel.Invoke();
			}
			m_Ready = false;
		}
		else if (InputManager.Singleton.InputDataCurrent.bUIConfirm && PlatformPlayerManagerSystem.Instance.LastActiveController.type != 0)
		{
			if (m_OnConfirm != null)
			{
				m_OnConfirm.Invoke();
			}
			m_Ready = false;
		}
		if (!sleepAfterInteract && !m_Ready)
		{
			m_Ready = true;
		}
	}
}
