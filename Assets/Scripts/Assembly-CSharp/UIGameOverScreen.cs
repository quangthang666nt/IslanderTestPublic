using System.Collections;
using TMPro;
using UnityEngine;

public class UIGameOverScreen : MonoBehaviour
{
	[SerializeField]
	private float fCurrentScoreCountTime = 1f;

	[SerializeField]
	private TextMeshProUGUI tmpScoreDisplay;

	[SerializeField]
	private TextMeshProUGUI tmpBestDisplay;

	[SerializeField]
	private TextMeshProUGUI tmpRankDisplay;

	private int iCurrentScore;

	public GameObject m_SubmitHighScoreGroup;

	public GameObject m_RankDisplayGroup;

	public GameObject m_LeaderboardButton;

	public UIPlayButtonSoundOnClick m_ClickSound;

	public void Activate()
	{
		StartCoroutine(CountScoreUpOnActivate());
	}

	private void Awake()
	{
		if (m_RankDisplayGroup != null && m_SubmitHighScoreGroup != null)
		{
			m_RankDisplayGroup.transform.localPosition = new Vector3(m_RankDisplayGroup.transform.localPosition.x, m_SubmitHighScoreGroup.transform.localPosition.y, m_RankDisplayGroup.transform.localPosition.z);
		}
		else
		{
			Debug.LogError("References Error");
		}
	}

	private void OnEnable()
	{
		if (!(LocalGameManager.singleton == null))
		{
			Activate();
		}
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}

	private IEnumerator CountScoreUpOnActivate()
	{
		float fTimer = 0f;
		while (fTimer < fCurrentScoreCountTime)
		{
			fTimer += Time.deltaTime;
			float t = Mathf.InverseLerp(0f, fCurrentScoreCountTime, fTimer);
			iCurrentScore = Mathf.RoundToInt(Mathf.SmoothStep(0f, LocalGameManager.singleton.IScore, t));
			SetScoreDisplay(iCurrentScore, GetHighscore(), setRank: false);
			yield return null;
		}
		SetScoreDisplay(LocalGameManager.singleton.IScore, GetHighscore(), setRank: true);
	}

	private int GetHighscore()
	{
		return StatsManager.statsGlobal.iHighscore;
	}

	private void SetScoreDisplay(int _score, int _bestScore, bool setRank)
	{
		string text = LocalGameManager.singleton.IScore.ToString();
		string text2 = _score.ToString();
		if (text.Length > text2.Length)
		{
			_ = text.Length;
			_ = text2.Length;
		}
		string text3 = text2;
		tmpScoreDisplay.text = text3;
		string text4 = _bestScore.ToString();
		tmpBestDisplay.text = text4;
		tmpRankDisplay.text = "---";
		if (m_RankDisplayGroup != null)
		{
			m_RankDisplayGroup.gameObject.SetActive(value: true);
		}
		if (m_SubmitHighScoreGroup != null)
		{
			m_SubmitHighScoreGroup.gameObject.SetActive(value: false);
		}
		if (m_LeaderboardButton != null)
		{
			m_LeaderboardButton.gameObject.SetActive(value: true);
		}
		if (PlatformPlayerManagerSystem.Instance.HasLeaderboardAccess() && setRank)
		{
			LeaderboardSystem.GetPlayerEntry(connectIfNecessary: false, OnPlayerEntryUpdated);
		}
	}

	private void OnPlayerEntryUpdated(bool success, LeaderboardEntry entry)
	{
		if (success && entry.GlobalRank > 0)
		{
			tmpRankDisplay.text = "#" + entry.GlobalRank;
		}
	}

	private void Update()
	{
		if (InputManager.Singleton.InputDataCurrent.bOpenScreenshotMode)
		{
			UiCanvasManager.Singleton.ToScreenshotSettings();
			if (m_ClickSound != null)
			{
				m_ClickSound.PlayButtonClick();
			}
		}
		else if (InputManager.Singleton.InputDataCurrent.bUIConfirm && PlatformPlayerManagerSystem.Instance.LastActiveController.type != 0)
		{
			UiCanvasManager.Singleton.ToGameModeSelection();
			if (m_ClickSound != null)
			{
				m_ClickSound.PlayButtonClick();
			}
		}
		else if (InputManager.Singleton.InputDataCurrent.bToggleLeaderboard)
		{
			UiCanvasManager.Singleton.ToLeaderboard();
			if (m_ClickSound != null)
			{
				m_ClickSound.PlayButtonClick();
			}
		}
	}
}
