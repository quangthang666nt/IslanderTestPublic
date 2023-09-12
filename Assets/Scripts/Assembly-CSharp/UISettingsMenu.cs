using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEngine;
using UnityEngine.UI;

public class UISettingsMenu : MonoBehaviour
{
	public enum SettingsState
	{
		Gameplay = 0,
		Video = 1,
		Audio = 2,
		Controls = 3,
		Credits = 4
	}

	private bool m_InSubMenu;

	[SerializeField]
	private GameObject gameplayContent;

	[SerializeField]
	private GameObject videoContent;

	[SerializeField]
	private GameObject audioContent;

	[SerializeField]
	private GameObject controlsContent;

	[SerializeField]
	private GameObject creditsContent;

	[SerializeField]
	private GameObject inputRevertButton;

	[SerializeField]
	private Button restoreDefaultsButton;

	[SerializeField]
	private Image gameplayTab;

	[SerializeField]
	private Image videoTab;

	[SerializeField]
	private Image audioTab;

	[SerializeField]
	private Image controlsTab;

	[SerializeField]
	private Image creditsTab;

	[SerializeField]
	private Color colSelected = Color.white;

	[SerializeField]
	private Color colUnselected = Color.white;

	[SerializeField]
	private RectTransform rtBackground;

	[SerializeField]
	private Vector2 gameplaySize;

	[SerializeField]
	private Vector2 videoSize;

	[SerializeField]
	private Vector2 audioSize;

	[SerializeField]
	private Vector2 controlsSize;

	[SerializeField]
	private Vector2 creditsSize;

	[SerializeField]
	private UIPageSizeCalculator gameplayPage;

	[SerializeField]
	private UIPageSizeCalculator videoPage;

	[SerializeField]
	private UIPageSizeCalculator audioPage;

	[SerializeField]
	private UIPageSizeCalculator controlsPage;

	[SerializeField]
	private UIPageSizeCalculator creditsPage;

	[SerializeField]
	private float pagePaddingWidth = 40f;

	[SerializeField]
	private float sizeAdjustTime;

	[SerializeField]
	private AnimationCurve acSizeAdjustCuve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[SerializeField]
	private SXUIVideoResolution sxUIVideoResolution;

	[SerializeField]
	private SXUIFullscreen sxUIFullScreen;

	[SerializeField]
	private GenericMenuList keybindList;

	[Header("Credits Scroll")]
	[SerializeField]
	private float creditsScrollbarJoystickVel = 0.5f;

	[SerializeField]
	private float creditsScrollbarAutomaticVel = 0.1f;

	[SerializeField]
	private float creditsScrollbarDelay = 4f;

	[SerializeField]
	private float creditsScrollbarAnimTime = 0.5f;

	public GameObject[] m_PCOptionsButtons;

	public GameObject[] m_ConsoleOptionsButtons;

	public UIPlayButtonSoundOnClick m_SoundEmitter;

	public List<BasicNavigationItem> m_TabButtons = new List<BasicNavigationItem>();

	private BasicNavigationItem m_CurrentlySelectedTab;

	private BasicNavigationItem m_CurrentlySelectedOption;

	private SettingsState state;

	private List<GameObject> contentObjs = new List<GameObject>();

	private List<Image> tabs = new List<Image>();

	private Transform currentContent;

	private Vector2 backgroundTargetSize;

	private Vector2 sizeAdjustOrigin;

	private float sizeAdjustClock;

	private bool m_JustMoved;

	public List<BasicNavigationItem> m_NavigationItems = new List<BasicNavigationItem>();

	private bool m_Opened;

	[Header("Only PS4")]
	[SerializeField]
	private Vector2 audioSizeExtra;

	private float creditsScrollTimer;

	private bool creditsOnAnim;

	private bool creditsValueChangedManually;

	private UIUndraggableScrollRect creditsScrollRect;

	private CanvasGroup creditsScrollbarCanvasGroup;

	private bool m_Focus = true;

	public void SetFocus(bool focus)
	{
		m_Focus = focus;
	}

	public void SetFocusAfterTime(float time)
	{
		StartCoroutine(SetFocusRoutine(time));
	}

	private IEnumerator SetFocusRoutine(float time)
	{
		yield return new WaitForSeconds(time);
		SetFocus(focus: true);
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
		case ControllerType.Mouse:
			controlsTab.gameObject.SetActive(value: true);
			ActivateCreditsScroll(value: true);
			break;
		case ControllerType.Keyboard:
			controlsTab.gameObject.SetActive(value: true);
			if (state == SettingsState.Credits && !m_InSubMenu)
			{
				ActivateCreditsScroll(value: false);
			}
			break;
		case ControllerType.Joystick:
		case ControllerType.Custom:
		{
			controlsTab.gameObject.SetActive(value: false);
			if (state == SettingsState.Controls && gameplayTab.TryGetComponent<BasicNavigationItem>(out var component))
			{
				component.OnSelect();
				if (keybindList.enabled)
				{
					DeselectKeybind();
					m_InSubMenu = false;
					if (m_SoundEmitter != null)
					{
						m_SoundEmitter.PlayButtonClick();
					}
				}
			}
			if (state == SettingsState.Credits && !m_InSubMenu)
			{
				ActivateCreditsScroll(value: false);
			}
			break;
		}
		}
	}

	private void Awake()
	{
		SetupContentObjList();
		SetupTabList();
		SwitchState(SettingsState.Gameplay);
		SetupCreditsScroll();
	}

	public void OnOpen()
	{
		m_CurrentlySelectedTab = null;
		m_CurrentlySelectedOption = null;
		for (int i = 0; i < m_TabButtons.Count; i++)
		{
			m_TabButtons[i].OnUnselect();
		}
		for (int j = 0; j < m_NavigationItems.Count; j++)
		{
			m_NavigationItems[j].OnUnselect();
		}
		m_InSubMenu = false;
		SelectFirstAvailableTab();
		StartCoroutine(OpenRoutine());
	}

	private IEnumerator OpenRoutine()
	{
		yield return new WaitForSeconds(0.1f);
		m_Opened = true;
	}

	public void OnClose()
	{
		m_Opened = false;
	}

	private void SetupContentObjList()
	{
		contentObjs.Clear();
		contentObjs.Add(gameplayContent);
		contentObjs.Add(videoContent);
		contentObjs.Add(audioContent);
		contentObjs.Add(controlsContent);
		contentObjs.Add(inputRevertButton);
		contentObjs.Add(creditsContent);
	}

	private void SetupTabList()
	{
		tabs.Clear();
		for (int i = 0; i < m_PCOptionsButtons.Length; i++)
		{
			m_PCOptionsButtons[i].gameObject.SetActive(value: false);
		}
		for (int j = 0; j < m_ConsoleOptionsButtons.Length; j++)
		{
			m_ConsoleOptionsButtons[j].gameObject.SetActive(value: false);
		}
		tabs.Add(gameplayTab);
		tabs.Add(videoTab);
		tabs.Add(audioTab);
		tabs.Add(creditsTab);
		for (int k = 0; k < m_PCOptionsButtons.Length; k++)
		{
			m_PCOptionsButtons[k].gameObject.SetActive(value: true);
		}
		for (int l = 0; l < m_TabButtons.Count; l++)
		{
			m_TabButtons[l].OnUnselect();
		}
	}

	private void SelectTab(BasicNavigationItem item)
	{
		if (m_CurrentlySelectedTab != null)
		{
			m_CurrentlySelectedTab.OnUnselect();
		}
		m_CurrentlySelectedTab = item;
		if (m_CurrentlySelectedTab != null)
		{
			m_CurrentlySelectedTab.OnSelect();
		}
	}

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

	public void DeselectOption()
	{
		if (m_CurrentlySelectedOption != null)
		{
			m_CurrentlySelectedOption.OnUnselect();
		}
		m_CurrentlySelectedOption = null;
	}

	private void SelectFirstAvailableTab()
	{
		for (int i = 0; i < m_TabButtons.Count; i++)
		{
			if (m_TabButtons[i].IsAvailableForNavigation)
			{
				SelectTab(m_TabButtons[i]);
				break;
			}
		}
	}

	private void SelectFirstAvailableOption()
	{
		for (int i = 0; i < m_NavigationItems.Count; i++)
		{
			if (m_NavigationItems[i].IsAvailableForNavigation)
			{
				SelectOption(m_NavigationItems[i]);
				break;
			}
		}
	}

	private void SelectFirstAvailableKeybind()
	{
		keybindList.enabled = true;
		keybindList.SelectFirstAvailable();
	}

	private void DeselectKeybind()
	{
		keybindList.Deselect();
		keybindList.enabled = false;
	}

	private void Update()
	{
		if (sizeAdjustClock < sizeAdjustTime)
		{
			sizeAdjustClock += Time.deltaTime;
			float t = acSizeAdjustCuve.Evaluate(Mathf.InverseLerp(0f, sizeAdjustTime, sizeAdjustClock));
			rtBackground.sizeDelta = Vector2.Lerp(sizeAdjustOrigin, backgroundTargetSize, t);
		}
		else if (rtBackground.sizeDelta != backgroundTargetSize || currentContent.transform.localScale != Vector3.one)
		{
			rtBackground.sizeDelta = backgroundTargetSize;
		}
	}

	private void LateUpdate()
	{
		if (!m_Focus || !m_Opened)
		{
			return;
		}
		Vector2 uIMove = InputManager.Singleton.InputDataCurrent.UIMove;
		if (!m_InSubMenu && InputManager.Singleton.InputDataCurrent.bUICancel)
		{
			if (m_SoundEmitter != null)
			{
				m_SoundEmitter.PlayButtonClick();
			}
			UiCanvasManager.Singleton.ToPrevious(addToStack: false);
		}
		if (!m_InSubMenu)
		{
			TabNavigation(uIMove);
		}
		else
		{
			OptionsNavigation(uIMove, state);
		}
		if (restoreDefaultsButton.isActiveAndEnabled && InputManager.Singleton.InputDataCurrent.bUIReset)
		{
			if (AudioManager.singleton != null)
			{
				AudioManager.singleton.PlayButtonClick();
			}
			restoreDefaultsButton.onClick.Invoke();
		}
	}

	public void CloseSettings()
	{
		if (m_SoundEmitter != null)
		{
			m_SoundEmitter.PlayButtonClick();
		}
		UiCanvasManager.Singleton.ToPrevious(addToStack: false);
	}

	private void OptionsNavigation(Vector2 moveAxis, SettingsState state)
	{
		Vector2 moveAxis2 = moveAxis;
		if (state == SettingsState.Audio && m_CurrentlySelectedOption != null && !(m_CurrentlySelectedOption is SliderNavigationItem))
		{
			if (!m_JustMoved)
			{
				if (IsGreaterThanDeadZone(moveAxis.x) || IsGreaterThanDeadZone(moveAxis.y))
				{
					if (Mathf.Abs(moveAxis.y) >= Mathf.Abs(moveAxis.x))
					{
						moveAxis.x = 0f;
						FindAndSelect(moveAxis);
					}
					else
					{
						moveAxis.y = 0f;
						FindAndSelect(moveAxis, MenuHelper.AxisDirection.Horizontal);
					}
				}
			}
			else if (IsLessThanDeadZone(moveAxis.x) && IsLessThanDeadZone(moveAxis.y))
			{
				m_JustMoved = false;
			}
		}
		else if (state == SettingsState.Credits)
		{
			CreditsScrollNavigation();
		}
		else
		{
			moveAxis.x = 0f;
			if (!m_JustMoved)
			{
				if (Mathf.Abs(moveAxis.y) > 0.3f)
				{
					FindAndSelect(moveAxis, MenuHelper.AxisDirection.Vertical);
				}
			}
			else if (Mathf.Abs(moveAxis.y) <= 0.3f)
			{
				m_JustMoved = false;
			}
		}
		if (m_CurrentlySelectedOption != null)
		{
			if (InputManager.Singleton.InputDataCurrent.bUIConfirm)
			{
				m_CurrentlySelectedOption.OnSubmit();
			}
			else
			{
				m_CurrentlySelectedOption.SendInput(moveAxis2);
			}
		}
		if (InputManager.Singleton.InputDataCurrent.bUICancel || PlatformPlayerManagerSystem.Instance.LastActiveController.type == ControllerType.Mouse)
		{
			switch (state)
			{
			case SettingsState.Video:
				sxUIVideoResolution.ApplySettings();
				sxUIFullScreen.ApplySettings();
				break;
			case SettingsState.Controls:
				DeselectKeybind();
				break;
			}
			m_InSubMenu = false;
			if (m_SoundEmitter != null)
			{
				m_SoundEmitter.PlayButtonClick();
			}
			m_CurrentlySelectedOption = null;
			for (int i = 0; i < m_NavigationItems.Count; i++)
			{
				m_NavigationItems[i].OnUnselect();
			}
		}
	}

	private void FindAndSelect(Vector2 moveAxis, MenuHelper.AxisDirection forceAxis = MenuHelper.AxisDirection.None)
	{
		BasicNavigationItem basicNavigationItem = MenuHelper.FindNavigationItemInDirection(m_CurrentlySelectedOption, m_NavigationItems, moveAxis, allowLoop: false, usePositionOnly: true, forceAxis);
		if (basicNavigationItem != null && basicNavigationItem != m_CurrentlySelectedOption)
		{
			SelectOption(basicNavigationItem);
			m_JustMoved = true;
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

	private void TabNavigation(Vector2 moveAxis)
	{
		moveAxis.x = 0f;
		if (m_CurrentlySelectedTab == null)
		{
			SelectFirstAvailableTab();
		}
		if (!m_JustMoved)
		{
			if (Mathf.Abs(moveAxis.y) > 0.3f)
			{
				BasicNavigationItem basicNavigationItem = MenuHelper.FindNavigationItemInDirection(m_CurrentlySelectedTab, m_TabButtons, moveAxis);
				if (basicNavigationItem != null && basicNavigationItem != m_CurrentlySelectedTab)
				{
					SelectTab(basicNavigationItem);
					m_JustMoved = true;
					if (state == SettingsState.Credits)
					{
						ActivateCreditsScroll(value: false);
					}
				}
			}
		}
		else if (Mathf.Abs(moveAxis.y) <= 0.3f)
		{
			m_JustMoved = false;
		}
		if (!(m_CurrentlySelectedTab != null))
		{
			return;
		}
		if (InputManager.Singleton.InputDataCurrent.bUIConfirm)
		{
			m_CurrentlySelectedTab.OnSubmit();
			m_InSubMenu = true;
			if (state == SettingsState.Controls)
			{
				SelectFirstAvailableKeybind();
			}
			else if (state == SettingsState.Credits)
			{
				ActivateCreditsScroll(value: true);
			}
			else
			{
				SelectFirstAvailableOption();
			}
		}
		else if (state == SettingsState.Credits)
		{
			UpdateCreditsScroll();
		}
	}

	private void SwitchState(SettingsState _newState)
	{
		if (_newState != state)
		{
			if (m_CurrentlySelectedTab != null)
			{
				m_CurrentlySelectedTab.OnUnselect();
			}
			m_CurrentlySelectedTab = m_TabButtons[(int)_newState];
		}
		List<GameObject> list = new List<GameObject>();
		UIPageSizeCalculator uIPageSizeCalculator = null;
		if (_newState != state)
		{
			sizeAdjustClock = 0f;
			sizeAdjustOrigin = rtBackground.sizeDelta;
		}
		switch (_newState)
		{
		case SettingsState.Gameplay:
			list.Add(gameplayContent);
			currentContent = gameplayContent.transform;
			backgroundTargetSize = gameplaySize;
			if (gameplayPage != null)
			{
				uIPageSizeCalculator = gameplayPage;
			}
			break;
		case SettingsState.Video:
			list.Add(videoContent);
			currentContent = videoContent.transform;
			backgroundTargetSize = videoSize;
			if (videoPage != null)
			{
				uIPageSizeCalculator = videoPage;
			}
			break;
		case SettingsState.Audio:
			list.Add(audioContent);
			currentContent = audioContent.transform;
			backgroundTargetSize = audioSize;
			if (audioPage != null)
			{
				uIPageSizeCalculator = audioPage;
			}
			break;
		case SettingsState.Controls:
			list.Add(controlsContent);
			list.Add(inputRevertButton);
			currentContent = controlsContent.transform;
			backgroundTargetSize = controlsSize;
			if (controlsPage != null)
			{
				uIPageSizeCalculator = controlsPage;
			}
			break;
		case SettingsState.Credits:
			list.Add(creditsContent);
			currentContent = creditsContent.transform;
			backgroundTargetSize = creditsSize;
			if (creditsPage != null)
			{
				uIPageSizeCalculator = creditsPage;
			}
			creditsOnAnim = true;
			creditsScrollTimer = 0f;
			ActivateCreditsScroll(value: true);
			break;
		}
		foreach (GameObject contentObj in contentObjs)
		{
			if (!list.Contains(contentObj) && contentObj.activeSelf)
			{
				contentObj.SetActive(value: false);
			}
		}
		foreach (GameObject item in list)
		{
			if (!item.activeSelf)
			{
				item.SetActive(value: true);
			}
		}
		if (uIPageSizeCalculator != null)
		{
			StartCoroutine(OverridePageWidth(uIPageSizeCalculator));
		}
		state = _newState;
	}

	public void SetSelectedTabColour(Image tab)
	{
		tab.color = colSelected;
	}

	public void SetUnselectedTabColour(Image tab)
	{
		tab.color = colUnselected;
	}

	public void ToGameplay()
	{
		SwitchState(SettingsState.Gameplay);
	}

	public void ToVideo()
	{
		SwitchState(SettingsState.Video);
	}

	public void ToAudio()
	{
		SwitchState(SettingsState.Audio);
	}

	public void ToControls()
	{
		SwitchState(SettingsState.Controls);
	}

	public void ToCredits()
	{
		SwitchState(SettingsState.Credits);
	}

	public void ForceResize()
	{
		sizeAdjustClock = 0f;
		sizeAdjustOrigin = rtBackground.sizeDelta;
	}

	private IEnumerator OverridePageWidth(UIPageSizeCalculator page)
	{
		yield return null;
		yield return null;
		backgroundTargetSize.x = page.GetWidth() + pagePaddingWidth * 2f;
	}

	private void SetupCreditsScroll()
	{
		creditsScrollRect = creditsContent.GetComponentInChildren<UIUndraggableScrollRect>();
		creditsScrollRect.OnScrollEvent.AddListener(delegate
		{
			creditsValueChangedManually = true;
			creditsScrollTimer = 0f;
		});
		creditsScrollRect.verticalScrollbar.GetComponent<ScrollbarWithDragEvent>().OnDragEvent.AddListener(delegate
		{
			creditsValueChangedManually = true;
			creditsScrollTimer = 0f;
		});
		creditsScrollbarCanvasGroup = creditsScrollRect.verticalScrollbar.GetComponent<CanvasGroup>();
		ActivateCreditsScroll(value: false);
	}

	private void ActivateCreditsScroll(bool value)
	{
		creditsScrollbarCanvasGroup.alpha = (value ? 1f : 0f);
	}

	private void UpdateCreditsScroll()
	{
		if (creditsOnAnim)
		{
			creditsScrollTimer += Time.deltaTime;
			creditsScrollRect.verticalScrollbar.value = 1f;
			if (creditsScrollTimer > creditsScrollbarAnimTime)
			{
				creditsOnAnim = false;
				creditsValueChangedManually = false;
			}
			return;
		}
		if (creditsValueChangedManually)
		{
			creditsScrollTimer += Time.deltaTime;
			if (creditsScrollTimer > creditsScrollbarDelay)
			{
				creditsValueChangedManually = false;
			}
			return;
		}
		creditsScrollRect.verticalScrollbar.value = Mathf.Clamp01(creditsScrollRect.verticalScrollbar.value - Time.deltaTime * creditsScrollbarAutomaticVel);
		if (creditsScrollRect.verticalScrollbar.value == 0f)
		{
			creditsScrollTimer += Time.deltaTime;
			if (creditsScrollTimer > creditsScrollbarDelay)
			{
				creditsScrollRect.verticalScrollbar.value = 1f;
			}
		}
		else
		{
			creditsScrollTimer = 0f;
		}
	}

	private void CreditsScrollNavigation()
	{
		float y = InputManager.Singleton.InputDataCurrent.UIMove.y;
		float fUISlider = InputManager.Singleton.InputDataCurrent.fUISlider;
		float num = ((!(Mathf.Abs(y) > 0f)) ? fUISlider : y);
		creditsScrollRect.verticalScrollbar.value = Mathf.Clamp01(creditsScrollRect.verticalScrollbar.value + Time.deltaTime * creditsScrollbarJoystickVel * num);
	}
}
