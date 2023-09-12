using System;
using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEngine;
using UnityEngine.UI;

public class GenericMenuList : MonoBehaviour
{
	private enum Direction
	{
		UP = 0,
		DOWN = 1
	}

	[Serializable]
	private enum InputBlockedType
	{
		UNTIL_MOVE_IS_NONE = 0,
		UNTIL_TIME = 1
	}

	private const float DEAD_ZONE = 0.3f;

	private const float TIME_INPUT_BLOCKED = 0.15f;

	[Header("Basic Navigation")]
	public List<BasicNavigationItem> m_NavigationItems = new List<BasicNavigationItem>();

	[SerializeField]
	private bool yAxisNav = true;

	[SerializeField]
	private bool xAxisNav;

	[Header("Select")]
	[SerializeField]
	private bool unlockAfterDelay;

	[SerializeField]
	private float unlockDelay = 0.5f;

	[SerializeField]
	private bool selectFirstAvailable = true;

	[Header("Loop")]
	[SerializeField]
	private bool allowAllInputTypesHorizontalLoop;

	[SerializeField]
	private bool allowAllInputTypesVerticalLoop;

	[SerializeField]
	private bool allowKeyboardHorizontalLoop;

	[SerializeField]
	private bool allowKeyboardVerticalLoop;

	[Header("Extra config")]
	[SerializeField]
	private bool sendInputToSelectors;

	[SerializeField]
	private MenuHelper.AxisDirection forceAxis = MenuHelper.AxisDirection.None;

	[SerializeField]
	private bool unselectAllOnMouse;

	[SerializeField]
	private InputBlockedType blockInputAfterMoveType;

	[SerializeField]
	private Image[] imagesToFadeOnLock;

	[SerializeField]
	private float alphaImagesLocked = 0.5f;

	[Header("Vertical scroll")]
	[SerializeField]
	private bool haveVerticalScroll;

	[SerializeField]
	private ScrollRect verticalScrollRect;

	[SerializeField]
	private float fitContentDelay;

	[SerializeField]
	private bool resetScrollOnFit;

	[SerializeField]
	private float joystickVel = 1f;

	[SerializeField]
	[Range(0f, 1f)]
	private float safeZoneOffsetProportion = 0.2f;

	[SerializeField]
	[Range(0f, 1f)]
	private float scrollFixedMovement = 0.1f;

	private bool m_JustMoved;

	private bool m_Locked;

	private bool lockedByMouse;

	private bool lockedByJoystick;

	private bool fittingContent;

	private float timeLastInput;

	public BasicNavigationItem CurrentlySelectedItem { get; private set; }

	protected virtual void OnEnable()
	{
		CurrentlySelectedItem = null;
		for (int i = 0; i < m_NavigationItems.Count; i++)
		{
			m_NavigationItems[i].OnUnselect();
		}
		Unlock();
		if (haveVerticalScroll)
		{
			StartCoroutine(FittingContentRoutine());
		}
		if (PlatformPlayerManagerSystem.Instance != null)
		{
			PlatformPlayerManagerSystem.Instance.OnLastActiveControllerUpdated += OnActiveControllerUpdated;
			OnActiveControllerUpdated(PlatformPlayerManagerSystem.Instance.LastActiveController);
		}
	}

	private IEnumerator FittingContentRoutine()
	{
		fittingContent = true;
		yield return new WaitForSeconds(fitContentDelay);
		if (resetScrollOnFit)
		{
			verticalScrollRect.verticalScrollbar.value = 1f;
		}
		fittingContent = false;
		OnActiveControllerUpdated(PlatformPlayerManagerSystem.Instance.LastActiveController);
	}

	private void OnDisable()
	{
		if (PlatformPlayerManagerSystem.IsReady)
		{
			PlatformPlayerManagerSystem.Instance.OnLastActiveControllerUpdated -= OnActiveControllerUpdated;
		}
	}

	private void OnActiveControllerUpdated(Controller controller)
	{
		if (fittingContent)
		{
			return;
		}
		switch (controller.type)
		{
		case ControllerType.Mouse:
			if (unselectAllOnMouse)
			{
				CurrentlySelectedItem = null;
				for (int i = 0; i < m_NavigationItems.Count; i++)
				{
					m_NavigationItems[i].OnUnselect();
				}
				Lock();
				lockedByMouse = true;
				lockedByJoystick = false;
			}
			break;
		case ControllerType.Keyboard:
		case ControllerType.Joystick:
		case ControllerType.Custom:
			if (unselectAllOnMouse && lockedByMouse)
			{
				Unlock();
			}
			break;
		}
	}

	public void SelectFirstAvailable()
	{
		for (int i = 0; i < m_NavigationItems.Count; i++)
		{
			if (m_NavigationItems[i].IsAvailableForNavigation)
			{
				Select(m_NavigationItems[i]);
				break;
			}
		}
	}

	public void Deselect()
	{
		Select(null);
	}

	public void Lock(bool endCoroutines = false)
	{
		if (endCoroutines)
		{
			StopAllCoroutines();
		}
		m_Locked = true;
		Image[] array = imagesToFadeOnLock;
		foreach (Image obj in array)
		{
			Color color = obj.color;
			color.a = alphaImagesLocked;
			obj.color = color;
		}
	}

	public void Unlock()
	{
		m_Locked = false;
		Image[] array = imagesToFadeOnLock;
		foreach (Image obj in array)
		{
			Color color = obj.color;
			color.a = 1f;
			obj.color = color;
		}
	}

	public void UnlockAfterDelay(float delay)
	{
		if (base.gameObject.activeInHierarchy)
		{
			StartCoroutine(UnlockAfterDelayRoutine(delay));
		}
	}

	private IEnumerator UnlockAfterDelayRoutine(float delay)
	{
		yield return new WaitForSeconds(delay);
		Unlock();
	}

	public void Select(BasicNavigationItem item)
	{
		if (CurrentlySelectedItem != null)
		{
			CurrentlySelectedItem.OnUnselect();
		}
		CurrentlySelectedItem = item;
		if (CurrentlySelectedItem != null)
		{
			CurrentlySelectedItem.OnSelect();
		}
	}

	private void Update()
	{
		if (fittingContent)
		{
			return;
		}
		if (!m_Locked)
		{
			if (selectFirstAvailable && (CurrentlySelectedItem == null || !CurrentlySelectedItem.IsAvailableForNavigation))
			{
				if (haveVerticalScroll)
				{
					SelectFirstAvailableScrollVisible();
				}
				else
				{
					SelectFirstAvailable();
				}
			}
			Vector2 uIMove = InputManager.Singleton.InputDataCurrent.UIMove;
			if (xAxisNav && yAxisNav)
			{
				if (!m_JustMoved)
				{
					if (IsGreaterThanDeadZone(uIMove.x) || IsGreaterThanDeadZone(uIMove.y))
					{
						bool allowLoop;
						if (Mathf.Abs(uIMove.y) >= Mathf.Abs(uIMove.x))
						{
							uIMove.x = 0f;
							allowLoop = allowAllInputTypesVerticalLoop || (allowKeyboardVerticalLoop && PlatformPlayerManagerSystem.Instance.LastActiveController.type == ControllerType.Keyboard);
						}
						else
						{
							uIMove.y = 0f;
							allowLoop = allowAllInputTypesHorizontalLoop || (allowKeyboardHorizontalLoop && PlatformPlayerManagerSystem.Instance.LastActiveController.type == ControllerType.Keyboard);
						}
						if (CurrentlySelectedItem == null && !selectFirstAvailable)
						{
							SelectFirstAvailable();
						}
						FindAndSelect(uIMove, allowLoop);
					}
				}
				else if ((blockInputAfterMoveType == InputBlockedType.UNTIL_MOVE_IS_NONE && IsLessThanDeadZone(uIMove.x) && IsLessThanDeadZone(uIMove.y)) || (blockInputAfterMoveType == InputBlockedType.UNTIL_TIME && Time.time - timeLastInput > 0.15f))
				{
					m_JustMoved = false;
				}
			}
			else if (xAxisNav)
			{
				uIMove.y = 0f;
				if (!m_JustMoved)
				{
					if (IsGreaterThanDeadZone(uIMove.x))
					{
						if (CurrentlySelectedItem == null && !selectFirstAvailable)
						{
							SelectFirstAvailable();
						}
						FindAndSelect(uIMove, allowAllInputTypesHorizontalLoop || (allowKeyboardHorizontalLoop && PlatformPlayerManagerSystem.Instance.LastActiveController.type == ControllerType.Keyboard));
					}
				}
				else if ((blockInputAfterMoveType == InputBlockedType.UNTIL_MOVE_IS_NONE && IsLessThanDeadZone(uIMove.x)) || (blockInputAfterMoveType == InputBlockedType.UNTIL_TIME && Time.time - timeLastInput > 0.15f))
				{
					m_JustMoved = false;
				}
			}
			else if (yAxisNav)
			{
				uIMove.x = 0f;
				if (!m_JustMoved)
				{
					if (IsGreaterThanDeadZone(uIMove.y))
					{
						if (CurrentlySelectedItem == null && !selectFirstAvailable)
						{
							SelectFirstAvailable();
						}
						FindAndSelect(uIMove, allowAllInputTypesVerticalLoop || (allowKeyboardVerticalLoop && PlatformPlayerManagerSystem.Instance.LastActiveController.type == ControllerType.Keyboard));
					}
				}
				else if ((blockInputAfterMoveType == InputBlockedType.UNTIL_MOVE_IS_NONE && IsLessThanDeadZone(uIMove.y)) || (blockInputAfterMoveType == InputBlockedType.UNTIL_TIME && Time.time - timeLastInput > 0.15f))
				{
					m_JustMoved = false;
				}
			}
			if (haveVerticalScroll && IsGreaterThanDeadZone(Mathf.Abs(InputManager.Singleton.InputDataCurrent.fUISlider)))
			{
				CurrentlySelectedItem = null;
				for (int i = 0; i < m_NavigationItems.Count; i++)
				{
					m_NavigationItems[i].OnUnselect();
				}
				Lock();
				lockedByJoystick = true;
			}
			if (!(CurrentlySelectedItem != null))
			{
				return;
			}
			if (InputManager.Singleton.InputDataCurrent.bUIConfirm)
			{
				CurrentlySelectedItem.OnSubmit();
				if (CurrentlySelectedItem.bShouldLock)
				{
					Lock();
					if (unlockAfterDelay)
					{
						UnlockAfterDelay(unlockDelay);
					}
				}
			}
			else if (sendInputToSelectors)
			{
				CurrentlySelectedItem.SendInput(InputManager.Singleton.InputDataCurrent.UIMove);
			}
		}
		else if (haveVerticalScroll && lockedByJoystick)
		{
			float fUISlider = InputManager.Singleton.InputDataCurrent.fUISlider;
			if (IsGreaterThanDeadZone(Mathf.Abs(fUISlider)))
			{
				verticalScrollRect.verticalScrollbar.value = Mathf.Clamp01(verticalScrollRect.verticalScrollbar.value + joystickVel * Time.deltaTime * fUISlider);
			}
			else
			{
				Unlock();
			}
		}
	}

	private void FindAndSelect(Vector2 moveAxis, bool allowLoop)
	{
		BasicNavigationItem basicNavigationItem = MenuHelper.FindNavigationItemInDirection(CurrentlySelectedItem, m_NavigationItems, moveAxis, allowLoop, usePositionOnly: false, forceAxis);
		if (basicNavigationItem != null && basicNavigationItem != CurrentlySelectedItem)
		{
			Select(basicNavigationItem);
			m_JustMoved = true;
			timeLastInput = Time.time;
			if (haveVerticalScroll)
			{
				AdjustScrollRect((!(moveAxis.y >= 0f)) ? Direction.DOWN : Direction.UP);
			}
		}
	}

	private List<BasicNavigationItem> GetNavItemsInsideScrollView()
	{
		List<BasicNavigationItem> list = new List<BasicNavigationItem>(m_NavigationItems);
		list.RemoveAll((BasicNavigationItem t) => !t.transform.IsChildOf(verticalScrollRect.transform));
		return list;
	}

	private void AdjustScrollRect(Direction direction)
	{
		List<BasicNavigationItem> navItemsInsideScrollView = GetNavItemsInsideScrollView();
		int num = navItemsInsideScrollView.IndexOf(CurrentlySelectedItem);
		if (num < 0)
		{
			return;
		}
		Mask componentInChildren = verticalScrollRect.GetComponentInChildren<Mask>();
		if (!componentInChildren || componentInChildren.transform.childCount != 1 || !componentInChildren.rectTransform.GetChild(0).TryGetComponent<RectTransform>(out var component))
		{
			Debug.LogError("Not adjusting Scroll Rect, wrong configuration");
			return;
		}
		float num2 = navItemsInsideScrollView[0].transform.position.y - navItemsInsideScrollView[navItemsInsideScrollView.Count - 1].transform.position.y;
		float num3 = (navItemsInsideScrollView[0].transform.position.y - navItemsInsideScrollView[num].transform.position.y) / num2;
		float num4 = componentInChildren.rectTransform.rect.height / component.rect.height;
		float num5 = (1f - verticalScrollRect.verticalScrollbar.value) * (1f - num4);
		Vector2 vector = new Vector2(num5, num4 + num5);
		float num6 = num4 * safeZoneOffsetProportion;
		if (direction == Direction.UP && num3 <= vector.x + num6)
		{
			verticalScrollRect.verticalScrollbar.value = Mathf.Clamp01(verticalScrollRect.verticalScrollbar.value + scrollFixedMovement);
		}
		else if (direction == Direction.DOWN && num3 >= vector.y - num6)
		{
			verticalScrollRect.verticalScrollbar.value = Mathf.Clamp01(verticalScrollRect.verticalScrollbar.value - scrollFixedMovement);
		}
		float num7 = (1f - verticalScrollRect.verticalScrollbar.value) * (1f - num4);
		Vector2 vector2 = new Vector2(num7, num4 + num7);
		if (num3 < vector2.x || num3 > vector2.y)
		{
			verticalScrollRect.verticalScrollbar.value = 1f - num3;
		}
	}

	private void SelectFirstAvailableScrollVisible()
	{
		List<BasicNavigationItem> navItemsInsideScrollView = GetNavItemsInsideScrollView();
		float num = navItemsInsideScrollView[0].transform.position.y - navItemsInsideScrollView[navItemsInsideScrollView.Count - 1].transform.position.y;
		float num2 = float.PositiveInfinity;
		int num3 = -1;
		for (int i = 0; i < navItemsInsideScrollView.Count; i++)
		{
			float num4 = navItemsInsideScrollView[0].transform.position.y - navItemsInsideScrollView[i].transform.position.y;
			float num5 = 1f - num4 / num;
			if (navItemsInsideScrollView[i].IsAvailableForNavigation && Mathf.Abs(num5 - verticalScrollRect.verticalScrollbar.value) < num2)
			{
				num3 = i;
				num2 = Mathf.Abs(num5 - verticalScrollRect.verticalScrollbar.value);
			}
		}
		if (num3 != -1)
		{
			Select(navItemsInsideScrollView[num3]);
		}
	}

	private bool IsGreaterThanDeadZone(float value)
	{
		return Mathf.Abs(value) > 0.3f;
	}

	private bool IsLessThanDeadZone(float value)
	{
		return Mathf.Abs(value) <= 0.3f;
	}
}
