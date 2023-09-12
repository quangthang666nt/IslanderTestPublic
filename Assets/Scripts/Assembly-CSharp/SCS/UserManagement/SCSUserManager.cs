using System;
using System.Collections.Generic;
using System.Linq;
using SCS.AppManagement;
using UnityEngine;

namespace SCS.UserManagement
{
	public class SCSUserManager : MonoBehaviour
	{
		public delegate void UserManagerInitializationHandler();

		public delegate void UserLoginHandler(UserData user);

		public delegate void UserLogoutHandler(UserData user);

		public static Action OnUserLoging;

		[HideInInspector]
		[Tooltip("Should be enabled for singleplayer games on PS4/PSVita")]
		public bool ps4UserManagement = true;

		[HideInInspector]
		[Tooltip("Should be enabled for singleplayer games on PS5")]
		public bool ps5UserManagement = true;

		[Tooltip("Enable/Disable auto-first login with controller number 0 in editor")]
		public bool editorAutoDoFirstLogin = true;

		[Tooltip("Ignore null user sessions (no user logged) when evaluate OnActiveUserChanged event.")]
		public bool ignoreNullOnActiveUserChangedEvent;

		public bool ignorePS4FirstLogInCommonDialog;

		private const int MAX_LOCAL_USERS = 32;

		private Dictionary<int, UserData> controllerUserMapping = new Dictionary<int, UserData>();

		private Dictionary<string, UserData> userList = new Dictionary<string, UserData>();

		private UserData lastActive;

		public static SCSUserManager Instance { get; private set; }

		public static bool Initialized { get; private set; }

		public static event UserManagerInitializationHandler OnInitializationComplete;

		public static event Action OnChangedUserInitialization;

		public static event UserLoginHandler OnLogin;

		public static event UserLogoutHandler OnLogout;

		public static event UserLoginHandler OnUserPictureGotten;

		public static event UserLoginHandler OnActiveUserChanged;

		public UserData GetOrCreate(string name)
		{
			if (userList.ContainsKey(name))
			{
				return userList[name];
			}
			UserData userData = new UserData(name);
			userList[name] = userData;
			return userData;
		}

		public UserData GetActiveUser()
		{
			for (int i = 0; i < 32; i++)
			{
				UserData userInController = GetUserInController(i);
				if (userInController != null)
				{
					return userInController;
				}
			}
			return null;
		}

		public UserData GetUserInController(int id)
		{
			if (!controllerUserMapping.ContainsKey(id))
			{
				return null;
			}
			return controllerUserMapping[id];
		}

		public IEnumerable<KeyValuePair<int, UserData>> GetAllUsers()
		{
			return controllerUserMapping.ToArray();
		}

		private void Awake()
		{
			if (Instance != null)
			{
				UnityEngine.Object.DestroyImmediate(base.gameObject);
				return;
			}
			Instance = this;
			UnityEngine.Object.DontDestroyOnLoad(this);
			SCSAppManager.OnAppInitializationComplete += Init;
		}

		private void OnDestroy()
		{
			SCSAppManager.OnAppInitializationComplete -= Init;
		}

		private void Init()
		{
			DoFirstLogin();
			if (SCSUserManager.OnInitializationComplete != null)
			{
				SCSUserManager.OnInitializationComplete();
			}
			Initialized = true;
			Debug.Log("<color=green>SCSUserManager initialization complete</color>");
		}

		private void Update()
		{
		}

		private void LateUpdate()
		{
			DoLateUpdate();
		}

		private void DoLateUpdate()
		{
			UserData activeUser = GetActiveUser();
			if (activeUser != lastActive && (!ignoreNullOnActiveUserChangedEvent || (ignoreNullOnActiveUserChangedEvent && activeUser != null)))
			{
				lastActive = activeUser;
				if (SCSUserManager.OnActiveUserChanged != null)
				{
					SCSUserManager.OnActiveUserChanged(activeUser);
				}
			}
		}

		private void ParseLogin(string name, int controller)
		{
			UserData orCreate = GetOrCreate(name);
			if (controllerUserMapping.ContainsKey(controller))
			{
				if (controllerUserMapping[controller] != orCreate)
				{
					ParseLogout(controller);
					orCreate.Controllers.Add(controller);
					controllerUserMapping.Add(controller, orCreate);
					if (SCSUserManager.OnLogin != null)
					{
						SCSUserManager.OnLogin(orCreate);
					}
				}
			}
			else
			{
				orCreate.Controllers.Add(controller);
				controllerUserMapping.Add(controller, orCreate);
				if (SCSUserManager.OnLogin != null)
				{
					SCSUserManager.OnLogin(orCreate);
				}
			}
		}

		private void ParseLogout(int controller)
		{
			if (controllerUserMapping.ContainsKey(controller))
			{
				if (SCSUserManager.OnLogout != null)
				{
					SCSUserManager.OnLogout(GetUserInController(controller));
				}
				GetUserInController(controller).Controllers.Remove(controller);
				controllerUserMapping.Remove(controller);
			}
		}

		private void DoFirstLogin()
		{
			GetActiveUser();
		}

		public void ForceLogout(int controller)
		{
			ParseLogout(controller);
		}

		public void ForceLogin(string name, int controller)
		{
			ForceLogout(controller);
			ParseLogin(name, controller);
		}

		public int CountUsers()
		{
			int num = 0;
			for (int i = 0; i < 32; i++)
			{
				if (controllerUserMapping.ContainsKey(i))
				{
					num++;
				}
			}
			return num;
		}
	}
}
