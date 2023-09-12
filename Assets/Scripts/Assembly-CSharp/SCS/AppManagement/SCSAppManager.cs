using System;
using System.Collections;
using UnityEngine;

namespace SCS.AppManagement
{
	public class SCSAppManager : MonoBehaviour
	{
		public delegate void OnSuspensionHandler();

		public delegate void OnResumingHandler();

		public delegate void OnAppInitializationCompleteHandler();

		public static Action<bool> OnConstraining;

		public bool showDebugSafeArea = true;

		public Region region;

		[Tooltip("-1 = disabled")]
		public int onlineAgeRestriction = -1;

		public bool usesServerPushNotifications;

		[HideInInspector]
		[Tooltip("Not necessary unless the game uses online features (leaderboards, multiplayer)")]
		public bool checkNPAvailabilityOnLogin;

		public static SCSAppManager Instance { get; private set; }

		public bool IsSuspended { get; private set; }

		public bool IsActiveApp { get; private set; }

		public static event OnSuspensionHandler OnSuspending;

		public static event OnResumingHandler OnResuming;

		public static event OnAppInitializationCompleteHandler OnAppInitializationComplete;

		private void Awake()
		{
			IsActiveApp = true;
			if (Instance != null)
			{
				UnityEngine.Object.DestroyImmediate(base.gameObject);
			}
			Instance = this;
			UnityEngine.Object.DontDestroyOnLoad(this);
		}

		private void Start()
		{
			StartCoroutine(DelayInitialization(30));
		}

		private void Update()
		{
		}

		private IEnumerator DelayInitialization(int frames)
		{
			Debug.Log("[SCS-App-Manager] DelayInitialization called");
			for (int i = 0; i < frames; i++)
			{
				yield return null;
			}
			SetInitialized();
		}

		private void SetInitialized()
		{
			Debug.Log("[APP] Initialization complete");
			if (SCSAppManager.OnAppInitializationComplete != null)
			{
				SCSAppManager.OnAppInitializationComplete();
			}
		}

		public void ExecuteSuspension()
		{
			IsSuspended = true;
			if (SCSAppManager.OnSuspending != null)
			{
				SCSAppManager.OnSuspending();
			}
		}

		public void ExecuteResume()
		{
			IsSuspended = false;
			if (SCSAppManager.OnResuming != null)
			{
				SCSAppManager.OnResuming();
			}
		}

		public void OnGUI()
		{
			if (showDebugSafeArea && Debug.isDebugBuild)
			{
				GUI.Box(new Rect((float)Screen.width * 0.95f, (float)Screen.height * 0f, (float)Screen.width * 0.05f, (float)Screen.height * 1f), "");
				GUI.Box(new Rect((float)Screen.width * 0f, (float)Screen.height * 0.95f, (float)Screen.width * 1f, (float)Screen.height * 0.05f), "");
				GUI.Box(new Rect((float)Screen.width * 0f, (float)Screen.height * 0f, (float)Screen.width * 0.05f, (float)Screen.height * 1f), "");
				GUI.Box(new Rect((float)Screen.width * 0f, (float)Screen.height * 0f, (float)Screen.width * 1f, (float)Screen.height * 0.05f), "");
			}
		}
	}
}
