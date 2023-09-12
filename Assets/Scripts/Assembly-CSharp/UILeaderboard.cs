using System.Collections.Generic;
using Rewired;
using UnityEngine;
using UnityEngine.UI;

public class UILeaderboard : MonoBehaviour
{
	private enum Tab
	{
		Global = 0,
		Friends = 1
	}

	private enum ConnectionStatus
	{
		Online = 0,
		Offline = 1,
		Loading = 2
	}

	[Header("Attributes")]
	[SerializeField]
	private float scrollbarSpeed = 10f;

	[Header("References")]
	[SerializeField]
	private Transform scoreEntry;

	[SerializeField]
	private GameObject noServiceNotification;

	[SerializeField]
	private GameObject noRankNotification;

	[SerializeField]
	private GameObject m_Spinner;

	[SerializeField]
	private Button yourScoreButton;

	[SerializeField]
	private Button topScoreButton;

	[SerializeField]
	private GameObject[] deactivateWhileLoading;

	[SerializeField]
	private Scrollbar scrollbar;

	[SerializeField]
	private Image scrollHandleImage;

	[SerializeField]
	private Color colSelected = Color.white;

	[SerializeField]
	private Color colUnselected = Color.white;

	[SerializeField]
	private Color colSelectedScroll = Color.white;

	[SerializeField]
	private Color colUnselectedScroll = Color.white;

	public List<BasicNavigationItem> m_TabButtons = new List<BasicNavigationItem>();

	[SerializeField]
	private UILeaderboardEntry UILeaderboardEntryPrefab;

	[Header("Animation")]
	[SerializeField]
	private UIAnimationGroup parentAnimationEffect;

	private List<UILeaderboardEntry> entryUIs = new List<UILeaderboardEntry>();

	private LeaderboardFilter currentFilter;

	private ConnectionStatus connectionStatus;

	private BasicNavigationItem m_CurrentlySelectedTab;

	private bool m_JustMoved;

	private bool inFriendScroll;

	private void OnEnable()
	{
		UpdateDisplay(LeaderboardFilter.YourScore);
		m_CurrentlySelectedTab = null;
		for (int i = 0; i < m_TabButtons.Count; i++)
		{
			m_TabButtons[i].OnUnselect();
		}
		inFriendScroll = false;
	}

	private void LateUpdate()
	{
		DetectInput();
	}

	private void OnHighScoreUpdated(bool success)
	{
		if (success)
		{
			LeaderboardSystem.GetRangeEntries(connectIfNecessary: true, OnEntriesUpdated);
		}
		else
		{
			ShowErrorNoServiceNotification();
		}
	}

	private void OnEntriesUpdated(bool success, List<LeaderboardEntry> entries)
	{
		if (success)
		{
			ShowUpdatedEntries(success, entries);
		}
		else
		{
			ShowErrorNoServiceNotification();
		}
	}

	public void ToggleLeaderboard()
	{
		UiCanvasManager.Singleton?.ToggleLeaderboard();
	}

	public void ToggleFilter()
	{
		switch (currentFilter)
		{
		case LeaderboardFilter.YourScore:
			UpdateDisplay(LeaderboardFilter.TopScores);
			break;
		case LeaderboardFilter.TopScores:
			UpdateDisplay(LeaderboardFilter.YourScore);
			break;
		}
	}

	public void ToGlobalScore()
	{
		if (currentFilter != 0)
		{
			StopAllCoroutines();
			UpdateDisplay(LeaderboardFilter.YourScore);
			if (m_CurrentlySelectedTab != null)
			{
				m_CurrentlySelectedTab.OnUnselect();
			}
			m_CurrentlySelectedTab = m_TabButtons[0];
		}
	}

	public void ToFriendScore()
	{
		if (currentFilter != LeaderboardFilter.FriendScore)
		{
			StopAllCoroutines();
			UpdateDisplay(LeaderboardFilter.FriendScore);
			if (m_CurrentlySelectedTab != null)
			{
				m_CurrentlySelectedTab.OnUnselect();
			}
			m_CurrentlySelectedTab = m_TabButtons[1];
		}
	}

	public void SetSelectedTabColour(Image tab)
	{
		tab.color = colSelected;
	}

	public void SetUnselectedTabColour(Image tab)
	{
		tab.color = colUnselected;
	}

	public void SetSelectedScrollColour(Image tab)
	{
		tab.color = colSelectedScroll;
	}

	public void SetUnselectedScrollColour(Image tab)
	{
		tab.color = colUnselectedScroll;
	}

	private void UpdateDisplay(LeaderboardFilter filter)
	{
		_ = currentFilter;
		currentFilter = filter;
		connectionStatus = ConnectionStatus.Loading;
		scoreEntry.gameObject.SetActive(value: false);
		noServiceNotification.SetActive(value: false);
		noRankNotification.SetActive(value: false);
		yourScoreButton.gameObject.SetActive(value: false);
		topScoreButton.gameObject.SetActive(value: false);
		for (int i = 0; i < deactivateWhileLoading.Length; i++)
		{
			deactivateWhileLoading[i].gameObject.SetActive(value: false);
		}
		if (PlatformPlayerManagerSystem.Instance == null || !PlatformPlayerManagerSystem.Instance.HasLeaderboardAccess())
		{
			ShowErrorNoServiceNotification();
		}
		else
		{
			PlatformPlayerManagerSystem.Instance.SetLeaderboardFilter(currentFilter);
			RefreshEntryUIList();
			if (StatsManager.BDoStatsSurviveSanityCheck(StatsManager.statsGlobal))
			{
				if (m_Spinner != null)
				{
					m_Spinner.gameObject.SetActive(value: true);
				}
				LeaderboardSystem.SubmitScore(connectIfNecessary: true, StatsManager.statsGlobal.iHighscore, OnHighScoreUpdated);
			}
			else
			{
				OnHighScoreUpdated(success: true);
			}
		}
		parentAnimationEffect?.AnimationIn();
	}

	private void ShowErrorNoServiceNotification()
	{
		connectionStatus = ConnectionStatus.Offline;
		scoreEntry.gameObject.SetActive(value: false);
		noServiceNotification.SetActive(value: true);
		noRankNotification.SetActive(value: false);
		if (m_Spinner != null)
		{
			m_Spinner.gameObject.SetActive(value: false);
		}
		yourScoreButton.gameObject.SetActive(value: false);
		topScoreButton.gameObject.SetActive(value: false);
		for (int i = 0; i < deactivateWhileLoading.Length; i++)
		{
			deactivateWhileLoading[i].gameObject.SetActive(value: false);
		}
	}

	private void RefreshEntryUIList()
	{
		entryUIs.Clear();
		foreach (Transform item in scoreEntry)
		{
			UILeaderboardEntry component = item.GetComponent<UILeaderboardEntry>();
			if (component != null)
			{
				entryUIs.Add(component);
			}
		}
	}

	private void ShowUpdatedEntries(bool success, List<LeaderboardEntry> entries)
	{
		if (m_Spinner != null)
		{
			m_Spinner.gameObject.SetActive(value: false);
		}
		switch (currentFilter)
		{
		case LeaderboardFilter.YourScore:
		{
			LeaderboardEntry leaderboardEntry = default(LeaderboardEntry);
			for (int i = 0; i < entries.Count; i++)
			{
				if (entries[i].IsPlayer)
				{
					leaderboardEntry = entries[i];
					break;
				}
			}
			if (leaderboardEntry.Score > 0)
			{
				scoreEntry.gameObject.SetActive(value: true);
				noRankNotification.SetActive(value: false);
			}
			else
			{
				scoreEntry.gameObject.SetActive(value: false);
				noRankNotification.SetActive(value: true);
			}
			topScoreButton.gameObject.SetActive(value: true);
			break;
		}
		case LeaderboardFilter.TopScores:
			scoreEntry.gameObject.SetActive(value: true);
			yourScoreButton.gameObject.SetActive(value: true);
			break;
		case LeaderboardFilter.FriendScore:
			scoreEntry.gameObject.SetActive(value: true);
			yourScoreButton.gameObject.SetActive(value: false);
			topScoreButton.gameObject.SetActive(value: false);
			break;
		}
		while (entryUIs.Count < entries.Count)
		{
			UILeaderboardEntry uILeaderboardEntry = Object.Instantiate(UILeaderboardEntryPrefab, scoreEntry);
			if (uILeaderboardEntry.TryGetComponent<UIBackgroundFitter>(out var component))
			{
				component.SetTarget((RectTransform)scoreEntry);
			}
			entryUIs.Add(uILeaderboardEntry);
		}
		for (int j = 0; j < entryUIs.Count; j++)
		{
			if (j < entries.Count)
			{
				bool isPlayer = entries[j].IsPlayer;
				entryUIs[j].UpdateDisplay(entries[j].GlobalRank.ToString(), entries[j].Name, entries[j].Score.ToString(), isPlayer);
				entryUIs[j].gameObject.SetActive(value: true);
			}
			else
			{
				entryUIs[j].gameObject.SetActive(value: false);
			}
		}
		connectionStatus = ConnectionStatus.Online;
		for (int k = 0; k < deactivateWhileLoading.Length; k++)
		{
			deactivateWhileLoading[k].gameObject.SetActive(value: true);
		}
	}

	private void DetectInput()
	{
		if (!inFriendScroll)
		{
			if (PlatformPlayerManagerSystem.Instance.LastActiveController.type == ControllerType.Mouse)
			{
				SetSelectedScrollColour(scrollHandleImage);
			}
			else
			{
				SetUnselectedScrollColour(scrollHandleImage);
			}
			TabNavigation();
			if (InputManager.Singleton.InputDataCurrent.bUIToggleFilter && connectionStatus != ConnectionStatus.Loading && currentFilter != LeaderboardFilter.FriendScore)
			{
				ToggleFilter();
				if (AudioManager.singleton != null)
				{
					AudioManager.singleton.PlayMenuClick();
				}
			}
			if (InputManager.Singleton.InputDataCurrent.bUICancel)
			{
				ToggleLeaderboard();
				if (AudioManager.singleton != null)
				{
					AudioManager.singleton.PlayButtonClick();
				}
			}
		}
		else if (InputManager.Singleton.InputDataCurrent.bUICancel || PlatformPlayerManagerSystem.Instance.LastActiveController.type == ControllerType.Mouse)
		{
			ExitFriendScroll();
		}
		else
		{
			float y = InputManager.Singleton.InputDataCurrent.UIMove.y;
			float fUISlider = InputManager.Singleton.InputDataCurrent.fUISlider;
			float num = ((!(Mathf.Abs(y) > 0f)) ? fUISlider : y);
			scrollbar.value = Mathf.Clamp01(scrollbar.value + Time.deltaTime * scrollbarSpeed * num);
		}
	}

	private void TabNavigation()
	{
		if (m_CurrentlySelectedTab == null)
		{
			SelectFirstAvailableTab();
		}
		Vector2 uIMove = InputManager.Singleton.InputDataCurrent.UIMove;
		uIMove.x = 0f;
		if (!m_JustMoved)
		{
			if (Mathf.Abs(uIMove.y) > 0.3f)
			{
				BasicNavigationItem basicNavigationItem = MenuHelper.FindNavigationItemInDirection(m_CurrentlySelectedTab, m_TabButtons, uIMove);
				if (basicNavigationItem != null && basicNavigationItem != m_CurrentlySelectedTab)
				{
					SelectTab(basicNavigationItem);
					m_JustMoved = true;
				}
			}
		}
		else if (Mathf.Abs(uIMove.y) <= 0.3f)
		{
			m_JustMoved = false;
		}
		if (m_CurrentlySelectedTab != null && currentFilter == LeaderboardFilter.FriendScore && InputManager.Singleton.InputDataCurrent.bUIConfirm)
		{
			EnterFriendScore();
		}
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

	private void EnterFriendScore()
	{
		m_CurrentlySelectedTab.OnSubmit();
		inFriendScroll = true;
		SetSelectedScrollColour(scrollHandleImage);
	}

	private void ExitFriendScroll()
	{
		inFriendScroll = false;
		if (AudioManager.singleton != null)
		{
			AudioManager.singleton.PlayMenuClick();
		}
		SetUnselectedScrollColour(scrollHandleImage);
	}
}
