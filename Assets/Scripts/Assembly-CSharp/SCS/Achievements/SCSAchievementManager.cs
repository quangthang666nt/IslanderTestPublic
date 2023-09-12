using System.Collections.Generic;
using SCS.UserManagement;
using UnityEngine;

namespace SCS.Achievements
{
	public class SCSAchievementManager : MonoBehaviour
	{
		public enum AchievementsDisplayPosition
		{
			TOPLEFT = 4,
			TOP = 3,
			TOPRIGHT = 5,
			LEFT = 4,
			CENTER = 3,
			RIGHT = 5,
			BOTTOMLEFT = 1,
			BOTTOM = 0,
			BOTTOMRIGHT = 2
		}

		[HideInInspector]
		public TextAsset XBoxEventManifest;

		public AchievementsDisplayPosition displayPosition;

		public bool forceDebugAchievements;

		private string lastAchievementText;

		private float lastAchievementTime;

		private Rect lastAchievementRect = new Rect(0f, (float)Screen.height * 0.25f, (float)Screen.width * 0.1f, (float)Screen.height * 0.1f);

		private Dictionary<UserData, HashSet<object>> alreadyUnlockedAchievements = new Dictionary<UserData, HashSet<object>>();

		public static SCSAchievementManager Instance { get; private set; }

		private void Awake()
		{
			if (!Debug.isDebugBuild)
			{
				forceDebugAchievements = false;
			}
			if (Instance != null)
			{
				Object.DestroyImmediate(base.gameObject);
				return;
			}
			Instance = this;
			Object.DontDestroyOnLoad(this);
			SCSUserManager.OnLogin += SCSUserManager_OnLogin;
			SCSUserManager.OnInitializationComplete += Initialize;
		}

		private void SCSUserManager_OnLogin(UserData user)
		{
		}

		private void Initialize()
		{
			Debug.Log("Achievement manager initialized");
		}

		public bool GetUnlocked(UserData user, int achievement)
		{
			return false;
		}

		private void OnGUI()
		{
			if ((Application.isEditor || forceDebugAchievements) && lastAchievementText != null)
			{
				GUI.Box(lastAchievementRect, lastAchievementText);
				if (lastAchievementTime < 0f)
				{
					lastAchievementText = null;
				}
			}
		}

		private void Update()
		{
			if ((Application.isEditor || forceDebugAchievements) && lastAchievementText != null)
			{
				lastAchievementTime -= Time.deltaTime;
			}
		}

		public void ShowAchievements(UserData user)
		{
		}

		public void UnlockAchievement(UserData user, int achievement)
		{
			if (user != null)
			{
				if (!alreadyUnlockedAchievements.ContainsKey(user))
				{
					alreadyUnlockedAchievements.Add(user, new HashSet<object>());
				}
				if (alreadyUnlockedAchievements[user].Contains(achievement))
				{
					return;
				}
				alreadyUnlockedAchievements[user].Add(achievement);
			}
			_ = 0;
		}

		public void UnlockAchievement(UserData userData, string achievement, uint percentage = 100u)
		{
			if (!alreadyUnlockedAchievements.ContainsKey(userData))
			{
				alreadyUnlockedAchievements.Add(userData, new HashSet<object>());
			}
			if (!alreadyUnlockedAchievements[userData].Contains(achievement))
			{
				if (percentage >= 100)
				{
					alreadyUnlockedAchievements[userData].Add(achievement);
				}
				if (Application.isEditor || forceDebugAchievements)
				{
					lastAchievementText = "Achievement " + achievement + " unlocked!";
					lastAchievementTime = 3f;
					_ = Application.isEditor;
				}
			}
		}
	}
}
