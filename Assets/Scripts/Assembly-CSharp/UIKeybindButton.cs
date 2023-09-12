using Rewired.UI.ControlMapper;
using UnityEngine;

public class UIKeybindButton : MonoBehaviour
{
	private const float UNLOCK_DELAY = 0.1f;

	private static GenericMenuList keybindButtonList;

	private static ControlMapper controlMapper;

	private static BasicNavigationItem currentSelected;

	private InputFieldInfo inputFieldInfo;

	private BasicNavigationItem item;

	private void Start()
	{
		if (controlMapper == null)
		{
			controlMapper = GetComponentInParent<ControlMapper>();
			controlMapper.onPopupWindowOpened += ControlMapper_onPopupWindowOpened;
			controlMapper.onPopupWindowClosed += ControlMapper_onPopupWindowClosed;
		}
		if (keybindButtonList == null)
		{
			keybindButtonList = controlMapper.GetComponent<GenericMenuList>();
		}
		inputFieldInfo = GetComponent<InputFieldInfo>();
		item = GetComponent<BasicNavigationItem>();
		keybindButtonList.m_NavigationItems.Add(item);
		item.m_OnSubmit.AddListener(OnSubmit);
	}

	private void OnDestroy()
	{
		keybindButtonList.m_NavigationItems.Remove(item);
		if ((bool)item)
		{
			item.m_OnSubmit.RemoveListener(OnSubmit);
		}
		if ((bool)controlMapper)
		{
			controlMapper.onPopupWindowOpened -= ControlMapper_onPopupWindowOpened;
			controlMapper.onPopupWindowClosed -= ControlMapper_onPopupWindowClosed;
			controlMapper = null;
		}
	}

	private void OnSubmit()
	{
		controlMapper.OnInputFieldActivated(inputFieldInfo);
	}

	private static void ControlMapper_onPopupWindowOpened()
	{
		if (keybindButtonList.enabled)
		{
			if (currentSelected == null)
			{
				currentSelected = keybindButtonList.CurrentlySelectedItem;
			}
			keybindButtonList.Deselect();
			keybindButtonList.Lock(endCoroutines: true);
		}
	}

	private static void ControlMapper_onPopupWindowClosed()
	{
		if (keybindButtonList.enabled)
		{
			keybindButtonList.Select(currentSelected);
			keybindButtonList.UnlockAfterDelay(0.1f);
			currentSelected = null;
		}
	}
}
