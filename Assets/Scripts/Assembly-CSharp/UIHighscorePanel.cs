using I2.Loc;
using TMPro;
using UnityEngine;

public class UIHighscorePanel : MonoBehaviour
{
	public TextMeshProUGUI textMesh;

	public LocalizedString highscoreIdentifier = "HIGH SCORE";

	public LocalizedString onlineRankIdentifier = "ONLINE RANK";

	public LocalizedString leaderboardRankIdentifier = "LEADERBOARD";

	public string noRankIdentifier = "---";

	public LocalizedString fontTagLight = "Fonts/Font Tag Light";

	public LocalizedString fontTagBold = "Fonts/Font Tag Bold";

	public GameObject m_LeaderboardButton;

	public GameObject m_LeaderboardInputDisplay;

	private const string LIGHT_FONT = "<font=OpenSans-Light SDF>";

	private const string BOLD_FONT = "<font=OpenSans-Bold SDF>";

	private const string SPACE = "<space=1em>";

	private const string P_DEFAULT = "<size=22>";

	private const string P_LARGE = "<size=35>";

	private const string P_CLOSE = "</size>";

	private int m_CachedHighScore;

	private int m_CachedRank;

	private string m_LocalisedFontTagLight;

	private string m_LocalisedHighScoreIdentifier;

	private string m_LocalisedOnlineRankIdentifier;

	private string m_LocalisedLeaderboardIdentifier;

	private bool m_LocalisationIsDirty = true;

	private void Start()
	{
		m_LocalisationIsDirty = true;
		I2Manager.OnLanguageChange += OnLanguageChange;
		UpdateData();
	}

	private void OnLanguageChange(string language)
	{
		m_LocalisationIsDirty = true;
	}

	private void OnDestroy()
	{
		I2Manager.OnLanguageChange -= OnLanguageChange;
	}

	private void OnEnable()
	{
		UpdateData();
	}

	private void UpdateData()
	{
		if (!(PlatformPlayerManagerSystem.Instance == null))
		{
			m_CachedHighScore = StatsManager.statsGlobal.iHighscore;
			if (StatsManager.statsMatch != null && StatsManager.statsMatch.iHighscore > m_CachedHighScore)
			{
				m_CachedHighScore = StatsManager.statsMatch.iHighscore;
			}
			if (m_CachedHighScore > StatsManager.statsGlobal.iHighscore)
			{
				StatsManager.statsGlobal.iHighscore = m_CachedHighScore;
			}
			m_CachedRank = 0;
			LeaderboardSystem.GetPlayerEntry(connectIfNecessary: false, OnPlayerLeaderboardEntryReady);
			UpdateUI();
		}
	}

	private void OnPlayerLeaderboardEntryReady(bool success, LeaderboardEntry leaderboardEntry)
	{
		if (!success || leaderboardEntry.GlobalRank == 0 || leaderboardEntry.Score == 0)
		{
			return;
		}
		if (m_CachedHighScore > leaderboardEntry.Score)
		{
			LeaderboardSystem.SubmitScore(connectIfNecessary: false, m_CachedHighScore, null);
		}
		else
		{
			m_CachedHighScore = leaderboardEntry.Score;
			if (m_CachedHighScore > StatsManager.statsGlobal.iHighscore)
			{
				StatsManager.statsGlobal.iHighscore = m_CachedHighScore;
			}
		}
		m_CachedRank = leaderboardEntry.GlobalRank;
		UpdateUI();
	}

	private void UpdateUI()
	{
		string rank = string.Empty;
		if (m_CachedRank > 0)
		{
			int cachedRank = m_CachedRank;
			if (cachedRank > 0)
			{
				rank = "#" + cachedRank;
			}
		}
		SetText(m_CachedHighScore.ToString(), rank);
	}

	public void SetText(string highscore, string rank)
	{
		if (m_LocalisationIsDirty)
		{
			m_LocalisedFontTagLight = fontTagLight;
			m_LocalisedHighScoreIdentifier = highscoreIdentifier;
			m_LocalisedOnlineRankIdentifier = onlineRankIdentifier;
			m_LocalisedLeaderboardIdentifier = leaderboardRankIdentifier;
			m_LocalisationIsDirty = false;
		}
		string text = "";
		text = ((rank.Length != 0) ? (m_LocalisedFontTagLight + m_LocalisedHighScoreIdentifier + ": <font=OpenSans-Bold SDF>" + highscore + m_LocalisedFontTagLight + "<size=35><space=1em>|<space=1em></size>" + m_LocalisedOnlineRankIdentifier + ": <font=OpenSans-Bold SDF>" + rank) : (m_LocalisedFontTagLight + m_LocalisedHighScoreIdentifier + ": <font=OpenSans-Bold SDF>" + highscore + m_LocalisedFontTagLight + "<size=35><space=1em>|<space=1em></size>" + m_LocalisedLeaderboardIdentifier));
		textMesh.text = text;
	}
}
