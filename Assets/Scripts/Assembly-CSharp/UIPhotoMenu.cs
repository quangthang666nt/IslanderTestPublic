using System;
using System.Collections;
using System.Collections.Generic;
using SCS.Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIPhotoMenu : MonoBehaviour
{
	[SerializeField]
	private List<BasicNavigationItem> m_NavigationItems = new List<BasicNavigationItem>();

	[SerializeField]
	private List<GameObject> m_PrintPositionItems = new List<GameObject>();

	[SerializeField]
	private int m_PrintPositionIndex = 4;

	[SerializeField]
	private Color m_ActiveColor = Color.white;

	[SerializeField]
	private Color m_DisabledColor = Color.gray;

	private BasicNavigationItem m_CurrentlySelectedOption;

	private int m_CurrentlySelectedOptionIndex;

	private bool m_JustMoved;

	[SerializeField]
	private RectTransform m_ContentRect;

	[SerializeField]
	private RectTransform m_BackgroundRect;

	[SerializeField]
	private UIElement m_ScreenshotMode;

	[SerializeField]
	private UIElement m_ScreenshotMenu;

	[SerializeField]
	private GameObject m_DayNightSlider;

	[SerializeField]
	private GameObject m_InputEditButton;

	[SerializeField]
	private ChildSequencer m_PrintPositionSequencer;

	[SerializeField]
	private EventObject m_NewFilterEvent;

	[SerializeField]
	private EventObject m_NewFrameEvent;

	[SerializeField]
	private EventObject m_NewPrintPositionEvent;

	[SerializeField]
	private EventObject m_NewPrintPositionNameEvent;

	[SerializeField]
	private EventObject m_NewDateEvent;

	[SerializeField]
	private EventObject m_InputTextEvent;

	private TMP_InputField m_textInputField;

	private TMP_Text m_textInputLabel;

	private TMP_Text m_textLabel;

	private string m_currentInputText = "";

	private string m_previousInputText = "";

	private bool m_isAnyPrintActive;

	private bool m_isPrintInputText;

	private bool m_isEditingInputText;

	private bool m_deactivateEditingInputText;

	public UnityEvent OnScreenshotModeExit;

	public UnityEvent OnScreenshotModeEnter;

	private bool onScreenshotMode;

	private Coroutine delayedClose;

	private bool prevCycleUpdate;

	private bool editConsoleActive;

	private float prevCycleNormTime;

	private const string NAME_TEXT_LABEL = "TextLabel";

	private const string NAME_TEXT_INPUT_FIELD = "TextInputField";

	private const string NAME_TEXT_INPUT_LABEL = "Text";

	private void SelectOption(BasicNavigationItem item)
	{
		if (m_CurrentlySelectedOption != null)
		{
			m_CurrentlySelectedOption.OnUnselect();
		}
		m_CurrentlySelectedOption = item;
		if (m_CurrentlySelectedOption != null)
		{
			m_CurrentlySelectedOption.OnSelect();
		}
	}

	private void Start()
	{
		if (m_ScreenshotMode != null)
		{
			m_ScreenshotMode.internalEventOnDeactivation.AddListener(OnElementDisabled);
		}
		if (m_ScreenshotMenu != null)
		{
			m_ScreenshotMenu.internalEventOnDeactivation.AddListener(OnElementDisabled);
			m_ScreenshotMenu.internalEventOnActivation.AddListener(OnEnableMenu);
		}
		if (m_NewPrintPositionEvent != null)
		{
			EventObject newPrintPositionEvent = m_NewPrintPositionEvent;
			newPrintPositionEvent.objectEvent = (EventObject.ObjectEvent)Delegate.Combine(newPrintPositionEvent.objectEvent, new EventObject.ObjectEvent(NewPrint));
			EventObject newPrintPositionEvent2 = m_NewPrintPositionEvent;
			newPrintPositionEvent2.objectEvent = (EventObject.ObjectEvent)Delegate.Combine(newPrintPositionEvent2.objectEvent, new EventObject.ObjectEvent(UpdateCanvasWrap));
		}
		if (m_NewFilterEvent != null)
		{
			EventObject newFilterEvent = m_NewFilterEvent;
			newFilterEvent.objectEvent = (EventObject.ObjectEvent)Delegate.Combine(newFilterEvent.objectEvent, new EventObject.ObjectEvent(UpdateCanvasWrap));
		}
		if (m_NewFrameEvent != null)
		{
			EventObject newFrameEvent = m_NewFrameEvent;
			newFrameEvent.objectEvent = (EventObject.ObjectEvent)Delegate.Combine(newFrameEvent.objectEvent, new EventObject.ObjectEvent(UpdateCanvasWrap));
		}
	}

	private void OnEnable()
	{
		for (int i = 0; i < m_NavigationItems.Count; i++)
		{
			m_NavigationItems[i].OnUnselect();
		}
		m_CurrentlySelectedOption = m_NavigationItems[0];
		m_CurrentlySelectedOption.OnSelect();
		m_CurrentlySelectedOptionIndex = 0;
	}

	private void Update()
	{
		if (m_isPrintInputText)
		{
			if (InputManager.Singleton.InputDataCurrent.bUIModifyText)
			{
				UpdateInputText();
			}
			if (InputManager.Singleton.imLastUsedInputMethod == InputManager.InputMode.Controller)
			{
				if (InputManager.Singleton.InputDataCurrent.bUIConfirm)
				{
					OnInputTextEndEdit(m_currentInputText);
				}
				else if (InputManager.Singleton.InputDataCurrent.bUICancel)
				{
					OnInputTextEndEdit(m_previousInputText);
				}
			}
		}
		if (!m_isEditingInputText)
		{
			if (InputManager.Singleton.InputDataCurrent.bUICancel || InputManager.Singleton.InputDataCurrent.bToggleMenu)
			{
				ToMainMenu();
			}
			if (InputManager.Singleton.InputDataCurrent.bUIConfirm)
			{
				UiCanvasManager.Singleton.SwitchScreenshotMenu();
			}
		}
	}

	private void LateUpdate()
	{
		if (m_deactivateEditingInputText)
		{
			m_deactivateEditingInputText = false;
			m_isEditingInputText = false;
		}
		else
		{
			if (m_isEditingInputText)
			{
				return;
			}
			Vector2 uIMove = InputManager.Singleton.InputDataCurrent.UIMove;
			Vector2 moveAxis = uIMove;
			uIMove.x = 0f;
			if (!m_JustMoved)
			{
				if (Mathf.Abs(uIMove.y) > 0.3f)
				{
					if (uIMove.y < 0f)
					{
						m_CurrentlySelectedOptionIndex++;
						if (m_CurrentlySelectedOptionIndex >= m_NavigationItems.Count)
						{
							m_CurrentlySelectedOptionIndex--;
						}
					}
					else
					{
						m_CurrentlySelectedOptionIndex--;
						if (m_CurrentlySelectedOptionIndex < 0)
						{
							m_CurrentlySelectedOptionIndex = 0;
						}
					}
					if (!m_isAnyPrintActive && m_CurrentlySelectedOptionIndex == m_PrintPositionIndex && m_NavigationItems.Count > 1)
					{
						if (uIMove.y < 0f)
						{
							m_CurrentlySelectedOptionIndex++;
							if (m_CurrentlySelectedOptionIndex >= m_NavigationItems.Count)
							{
								m_CurrentlySelectedOptionIndex = m_PrintPositionIndex - 1;
							}
						}
						else
						{
							m_CurrentlySelectedOptionIndex--;
							if (m_CurrentlySelectedOptionIndex < 0)
							{
								m_CurrentlySelectedOptionIndex = m_PrintPositionIndex + 1;
							}
						}
					}
					BasicNavigationItem basicNavigationItem = m_NavigationItems[m_CurrentlySelectedOptionIndex];
					if (basicNavigationItem != null && basicNavigationItem != m_CurrentlySelectedOption)
					{
						SelectOption(basicNavigationItem);
						m_JustMoved = true;
					}
				}
			}
			else if (Mathf.Abs(uIMove.y) <= 0.3f)
			{
				m_JustMoved = false;
			}
			if (m_CurrentlySelectedOption != null)
			{
				m_CurrentlySelectedOption.SendInput(moveAxis);
			}
		}
	}

	public void ResetPhotoMode()
	{
		m_isPrintInputText = false;
		SetActivePrintPosition(m_isAnyPrintActive);
		if (m_InputEditButton != null)
		{
			m_InputEditButton.SetActive(value: false);
		}
	}

	public void UpdateCanvas()
	{
		if (m_ContentRect != null)
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(m_ContentRect);
		}
		if (m_BackgroundRect != null)
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(m_BackgroundRect);
		}
	}

	private void UpdateCanvasWrap(object arg)
	{
		UpdateCanvas();
	}

	private void SetActivePrintPosition(bool active)
	{
		m_isAnyPrintActive = active;
		Color color = m_ActiveColor;
		if (!active)
		{
			color = m_DisabledColor;
		}
		foreach (GameObject printPositionItem in m_PrintPositionItems)
		{
			if (printPositionItem.TryGetComponent<TextMeshProUGUI>(out var component))
			{
				component.color = color;
			}
			if (printPositionItem.TryGetComponent<Image>(out var component2))
			{
				component2.color = color;
			}
		}
		if (!active && m_NewPrintPositionNameEvent != null)
		{
			m_NewPrintPositionNameEvent.objectEvent?.Invoke("None");
		}
	}

	public void ToPhotoMode()
	{
		UiCanvasManager.Singleton.SwitchScreenshotMenu();
	}

	public void ToPhotoMenu()
	{
		UiCanvasManager.Singleton.SwitchScreenshotMenu();
	}

	public void ToMainMenu()
	{
		UiCanvasManager.Singleton.ToggleMenu();
		m_PrintPositionSequencer = null;
	}

	private void OnElementDisabled()
	{
		if (onScreenshotMode)
		{
			if (delayedClose != null)
			{
				CoroutineManagerSingleton.Instance.StopCoroutine(delayedClose);
			}
			delayedClose = CoroutineManagerSingleton.Instance.StartCoroutine(DelayedCheckIfClosed());
		}
	}

	private void OnEnableMenu()
	{
		onScreenshotMode = true;
		OnScreenshotModeEnter?.Invoke();
		if (!DayNightCycle.Instance.IsCycleDisabled)
		{
			prevCycleUpdate = DayNightCycle.Instance.CycleActive;
			prevCycleNormTime = DayNightCycle.Instance.NormalizedTime;
			DayNightCycle.Instance.SetCycleActive(isActive: false);
		}
		else
		{
			m_DayNightSlider.SetActive(value: false);
		}
	}

	private IEnumerator DelayedCheckIfClosed()
	{
		yield return null;
		ResetPhotoMode();
		if (m_ScreenshotMenu != null && !m_ScreenshotMenu.active && m_ScreenshotMode != null && !m_ScreenshotMode.active)
		{
			onScreenshotMode = false;
			OnScreenshotModeExit?.Invoke();
			if (!DayNightCycle.Instance.IsCycleDisabled)
			{
				DayNightCycle.Instance.ManualUpdateCycle(prevCycleNormTime);
				DayNightCycle.Instance.SetCycleActive(prevCycleUpdate);
			}
		}
		delayedClose = null;
	}

	private void NewPrint(object printObject)
	{
		SetActivePrintPosition(printObject != null);
		if (printObject == null)
		{
			return;
		}
		GameObject gameObject = printObject as GameObject;
		if (gameObject == null)
		{
			return;
		}
		int childIndex = 0;
		if (m_PrintPositionSequencer != null)
		{
			childIndex = m_PrintPositionSequencer.ChildIndex;
		}
		m_PrintPositionSequencer = gameObject.GetComponent<ChildSequencer>();
		if (m_PrintPositionSequencer != null)
		{
			m_PrintPositionSequencer.SetChildIndex(childIndex);
		}
		if (gameObject.name == "Text" || gameObject.name == "Text2")
		{
			UpdateInputTextReferences();
			m_isPrintInputText = true;
			if (m_InputEditButton != null)
			{
				m_InputEditButton.SetActive(value: true);
			}
		}
		else
		{
			m_isPrintInputText = false;
			if (m_InputEditButton != null)
			{
				m_InputEditButton.SetActive(value: false);
			}
		}
		UpdateDate();
	}

	public void NextPrintPosition()
	{
		if (!(m_PrintPositionSequencer == null))
		{
			m_PrintPositionSequencer.Next();
			if (m_isPrintInputText)
			{
				UpdateInputTextReferences();
			}
			UpdateCanvas();
		}
	}

	public void PrevPrintPosition()
	{
		if (!(m_PrintPositionSequencer == null))
		{
			m_PrintPositionSequencer.Previous();
			if (m_isPrintInputText)
			{
				UpdateInputTextReferences();
			}
			UpdateCanvas();
		}
	}

	public void UpdateDate()
	{
		string currentDateTime = ArchiveManager.singleton.GetCurrentDateTime();
		DateTime dateFromData = ArchiveManager.singleton.GetDateFromData(currentDateTime);
		string arg = PlatformPlayerManagerSystem.Instance.FormatDateWithCultureDatePattern(dateFromData);
		if (m_NewDateEvent != null)
		{
			m_NewDateEvent.objectEvent?.Invoke(arg);
		}
	}

	public void UpdateInputText()
	{
		m_previousInputText = m_currentInputText;
		m_textInputField.gameObject.SetActive(value: true);
		m_textLabel.gameObject.SetActive(value: false);
		m_textInputField.text = m_currentInputText;
		m_textInputField.Select();
		m_textInputField.ActivateInputField();
		m_textInputField.onValueChanged.AddListener(OnInputTextChanged);
		m_textInputField.onEndEdit.AddListener(OnInputTextEndEdit);
		m_isEditingInputText = true;
		m_deactivateEditingInputText = false;
	}

	public void UpdateInputTextReferences()
	{
		if (!(m_PrintPositionSequencer == null))
		{
			int childIndex = m_PrintPositionSequencer.ChildIndex;
			Transform child = m_PrintPositionSequencer.transform.GetChild(childIndex - 1);
			Transform transform = FindInChildsByName(child, "TextLabel");
			if (transform != null)
			{
				m_textLabel = transform.GetComponent<TMP_Text>();
			}
			Transform transform2 = FindInChildsByName(child, "TextInputField");
			if (transform2 != null)
			{
				m_textInputField = transform2.GetComponent<TMP_InputField>();
			}
			Transform transform3 = FindInChildsByName(child, "Text");
			if (transform3 != null)
			{
				m_textInputLabel = transform3.GetComponent<TMP_Text>();
			}
			if (m_InputTextEvent != null)
			{
				m_InputTextEvent.objectEvent?.Invoke(m_currentInputText);
			}
		}
	}

	private Transform FindInChildsByName(Transform parent, string name)
	{
		for (int i = 0; i < parent.childCount; i++)
		{
			Transform child = parent.GetChild(i);
			if (child.name.Equals(name))
			{
				return child;
			}
			if (child.childCount > 0)
			{
				Transform transform = FindInChildsByName(child, name);
				if (transform != null)
				{
					return transform;
				}
			}
		}
		return null;
	}

	private void OnInputTextChanged(string value)
	{
		m_textLabel.enableWordWrapping = true;
		m_textInputLabel.enableWordWrapping = true;
		m_currentInputText = value;
	}

	private void OnInputTextEndEdit(string value)
	{
		m_textInputField.onValueChanged.RemoveAllListeners();
		m_textInputField.onEndEdit.RemoveAllListeners();
		m_textInputField.gameObject.SetActive(value: false);
		m_textLabel.gameObject.SetActive(value: true);
		m_currentInputText = value;
		if (m_InputTextEvent != null)
		{
			m_InputTextEvent.objectEvent?.Invoke(m_currentInputText);
		}
		m_deactivateEditingInputText = true;
	}
}
