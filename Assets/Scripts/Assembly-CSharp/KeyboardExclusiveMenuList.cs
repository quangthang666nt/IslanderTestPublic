using System.Collections.Generic;
using Rewired;
using UnityEngine;

public class KeyboardExclusiveMenuList : MonoBehaviour
{
	[SerializeField]
	private List<BasicNavigationItem> m_KeyboardExclusiveNavigationItems = new List<BasicNavigationItem>();

	[SerializeField]
	private GenericMenuList genericMenuList;

	protected virtual void OnEnable()
	{
		for (int i = 0; i < m_KeyboardExclusiveNavigationItems.Count; i++)
		{
			m_KeyboardExclusiveNavigationItems[i].OnUnselect();
		}
	}

	private void Awake()
	{
		foreach (BasicNavigationItem keyboardExclusiveNavigationItem in m_KeyboardExclusiveNavigationItems)
		{
			if (!genericMenuList.m_NavigationItems.Contains(keyboardExclusiveNavigationItem))
			{
				genericMenuList.m_NavigationItems.Add(keyboardExclusiveNavigationItem);
			}
		}
	}

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
		switch (controller.type)
		{
		case ControllerType.Keyboard:
		{
			foreach (BasicNavigationItem keyboardExclusiveNavigationItem in m_KeyboardExclusiveNavigationItems)
			{
				if (!genericMenuList.m_NavigationItems.Contains(keyboardExclusiveNavigationItem))
				{
					genericMenuList.m_NavigationItems.Add(keyboardExclusiveNavigationItem);
				}
			}
			break;
		}
		case ControllerType.Mouse:
		case ControllerType.Joystick:
		case ControllerType.Custom:
		{
			bool flag = false;
			bool flag2 = false;
			foreach (BasicNavigationItem keyboardExclusiveNavigationItem2 in m_KeyboardExclusiveNavigationItems)
			{
				if (genericMenuList.m_NavigationItems.Contains(keyboardExclusiveNavigationItem2))
				{
					if (keyboardExclusiveNavigationItem2 == genericMenuList.CurrentlySelectedItem)
					{
						flag2 = true;
						flag = controller.type != ControllerType.Mouse;
					}
					genericMenuList.m_NavigationItems.Remove(keyboardExclusiveNavigationItem2);
				}
			}
			if (flag2)
			{
				genericMenuList.Deselect();
			}
			if (flag)
			{
				genericMenuList.SelectFirstAvailable();
			}
			break;
		}
		}
	}
}
