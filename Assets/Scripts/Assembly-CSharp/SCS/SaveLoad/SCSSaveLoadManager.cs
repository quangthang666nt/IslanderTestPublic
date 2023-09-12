using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SCS.UserManagement;
using UnityEngine;

namespace SCS.SaveLoad
{
	public class SCSSaveLoadManager : SCSSaveLoadManagerBase
	{
		public delegate void InitializationCompleteHandler();

		private struct SaveDataParams
		{
			public bool isSaveOperation;

			public bool isLoadOperation;

			public SCSSaveData saveData;

			public SaveLocation saveLocation;

			public UserData user;

			public Action saveCallback;

			public Action<SaveLoadErrorResult> errorCallback;

			public Action<SCSSaveData> loadCallback;

			public SaveDataParams(bool isSave, bool isLoad, SCSSaveData saveData, SaveLocation location, UserData user, Action scs, Action<SaveLoadErrorResult> fil, Action<SCSSaveData> loadCallback)
			{
				isSaveOperation = isSave;
				isLoadOperation = isLoad;
				this.saveData = saveData;
				saveLocation = location;
				this.user = user;
				saveCallback = scs;
				errorCallback = fil;
				this.loadCallback = loadCallback;
			}
		}

		public string basePath;

		public int slotCount = 1;

		public bool enableNotSpaceDialogWarning;

		private int saveOperations;

		private int loadOperations;

		private LinkedList<SaveDataParams> queuedOperations = new LinkedList<SaveDataParams>();

		public static bool Initialized { get; private set; }

		public static SCSSaveLoadManager Instance { get; private set; }

		public static bool BlockThreadWhileSaving { get; set; }

		public bool IsSaving => saveOperations > 0;

		public bool IsLoading => loadOperations > 0;

		public static event InitializationCompleteHandler OnInitializationComplete;

		private void Awake()
		{
			if (Instance != null)
			{
				UnityEngine.Object.DestroyImmediate(base.gameObject);
				return;
			}
			Instance = this;
			UnityEngine.Object.DontDestroyOnLoad(this);
			SCSUserManager.OnInitializationComplete += Init;
		}

		private void SetInitialized()
		{
			Debug.Log("[SAVELOAD] " + SaveLocation.ApplicationProductName + " Initialization complete");
			Initialized = true;
			if (SCSSaveLoadManager.OnInitializationComplete != null)
			{
				SCSSaveLoadManager.OnInitializationComplete();
			}
		}

		private static void CreateDirectoryRecursively(string path)
		{
			string[] array = path.Split('\\');
			for (int i = 0; i < array.Length - 1; i++)
			{
				if (i > 0)
				{
					array[i] = Path.Combine(array[i - 1], array[i]);
				}
				if (!Directory.Exists(array[i]))
				{
					Directory.CreateDirectory(array[i]);
				}
			}
		}

		public static void QueueSave(SCSSaveData saveData, SaveLocation saveLocation, UserData user, Action successCallback, Action<SaveLoadErrorResult> errorCallback)
		{
			Instance.queuedOperations.AddLast(new SaveDataParams(isSave: true, isLoad: false, saveData, saveLocation, user, successCallback, errorCallback, null));
		}

		public static void QueueLoad(SaveLocation saveLocation, UserData user, Action<SCSSaveData> successCallback, Action<SaveLoadErrorResult> errorCallback)
		{
			Instance.queuedOperations.AddLast(new SaveDataParams(isSave: false, isLoad: true, null, saveLocation, user, null, errorCallback, successCallback));
		}

		public static void Save(SCSSaveData saveData, SaveLocation saveLocation, Action successCallback, Action<SaveLoadErrorResult> errorCallback)
		{
			Save(saveData, saveLocation, SCSUserManager.Instance.GetActiveUser(), successCallback, errorCallback);
		}

		private void Update()
		{
			foreach (SaveDataParams queuedOperation in queuedOperations)
			{
				if (queuedOperation.isSaveOperation)
				{
					Save(queuedOperation.saveData, queuedOperation.saveLocation, queuedOperation.user, queuedOperation.saveCallback, queuedOperation.errorCallback);
				}
				if (queuedOperation.isLoadOperation)
				{
					Load(queuedOperation.saveLocation, queuedOperation.user, queuedOperation.loadCallback, queuedOperation.errorCallback);
				}
			}
			queuedOperations.Clear();
		}

		public static void Save(SCSSaveData saveData, SaveLocation saveLocation, UserData user, Action successCallback, Action<SaveLoadErrorResult> errorCallback)
		{
			if (saveLocation.RequireUser && user == null)
			{
				errorCallback?.Invoke(new SaveLoadErrorResult("The save location requires an user to be active", SaveLoadErrorResult.SaveLoadErrorType.REQUIRED_USER_NOT_FOUND));
			}
			else if (BlockThreadWhileSaving)
			{
				IEnumerator enumerator = Instance._Save(saveData, saveLocation, user, successCallback, errorCallback);
				while (enumerator.MoveNext())
				{
				}
			}
			else
			{
				Instance.StartCoroutine(Instance._Save(saveData, saveLocation, user, successCallback, errorCallback));
			}
		}

		public static void Load(SaveLocation saveLocation, SCSSaveData saveData, Action<SCSSaveData> successCallback, Action<SaveLoadErrorResult> errorCallback, string blopName = "")
		{
			Load(saveLocation, SCSUserManager.Instance.GetActiveUser(), saveData, successCallback, errorCallback, blopName);
		}

		public static void Load(SaveLocation saveLocation, Action<SCSSaveData> successCallback, Action<SaveLoadErrorResult> errorCallback, string blopName = "")
		{
			Load(saveLocation, SCSUserManager.Instance.GetActiveUser(), new SCSSaveData(), successCallback, errorCallback, blopName);
		}

		public static void Load(SaveLocation saveLocation, UserData user, Action<SCSSaveData> successCallback, Action<SaveLoadErrorResult> errorCallback, string blopName = "")
		{
			Load(saveLocation, user, new SCSSaveData(), successCallback, errorCallback, blopName);
		}

		public static void Load(SaveLocation saveLocation, UserData user, SCSSaveData saveData, Action<SCSSaveData> successCallback, Action<SaveLoadErrorResult> errorCallback, string blopName = "")
		{
			if (saveLocation.RequireUser && user == null)
			{
				errorCallback(new SaveLoadErrorResult("The save location requires an user to be active", SaveLoadErrorResult.SaveLoadErrorType.REQUIRED_USER_NOT_FOUND));
			}
			else
			{
				Instance.StartCoroutine(Instance._Load(saveLocation, user, saveData, successCallback, errorCallback, blopName));
			}
		}

		public static bool Exists(SaveLocation saveLocation)
		{
			return Instance._Exists(saveLocation, SCSUserManager.Instance.GetActiveUser());
		}

		public static void Delete(SaveLocation saveLocation, Action onDelete = null, Action onCancel = null, bool displayDialog = true)
		{
			Delete(saveLocation, SCSUserManager.Instance.GetActiveUser(), onDelete, onCancel, displayDialog);
		}

		public static void Delete(SaveLocation saveLocation, UserData user, Action onDelete = null, Action onCancel = null, bool displayDialog = true)
		{
			Instance.StartCoroutine(Instance._Delete(saveLocation, user, onDelete, onCancel, displayDialog));
		}

		protected string GetFullPath(SaveLocation saveLocation, UserData user)
		{
			string text = ((user != null) ? user.Name : "local") + "\\";
			return Application.persistentDataPath + "\\" + text + saveLocation.FullPath;
		}

		protected override void Init()
		{
			SCSUserManager.OnInitializationComplete -= Init;
			SCSUserManager.OnLogin += delegate
			{
				if (SCSSaveLoadManager.OnInitializationComplete != null)
				{
					SCSSaveLoadManager.OnInitializationComplete();
				}
			};
			SCSUserManager.OnLogout += delegate
			{
				if (SCSSaveLoadManager.OnInitializationComplete != null)
				{
					SCSSaveLoadManager.OnInitializationComplete();
				}
			};
			SetInitialized();
		}

		protected override void OnDestroy()
		{
			Initialized = false;
		}

		protected override IEnumerator _Save(SCSSaveData saveData, SaveLocation saveLocation, UserData user, Action successCallback, Action<SaveLoadErrorResult> errorCallback)
		{
			while (!Initialized)
			{
				yield return null;
			}
			while (saveOperations > 0 || loadOperations > 0)
			{
				yield return null;
			}
			saveOperations++;
			try
			{
				string fullPath = GetFullPath(saveLocation, user);
				CreateDirectoryRecursively(fullPath);
				//File.WriteAllBytes(fullPath, saveData.GetRawBuffer());
				successCallback?.Invoke();
			}
			catch (Exception ex)
			{
				errorCallback?.Invoke(new SaveLoadErrorResult(ex.Message, SaveLoadErrorResult.SaveLoadErrorType.GENERIC_ERROR));
			}
			finally
			{
				saveOperations--;
			}
		}

		protected override IEnumerator _Load(SaveLocation saveLocation, UserData user, SCSSaveData saveData, Action<SCSSaveData> successCallback, Action<SaveLoadErrorResult> errorCallback, string blopName = "")
		{
			while (!Initialized)
			{
				yield return null;
			}
			while (saveOperations > 0 || loadOperations > 0)
			{
				yield return null;
			}
			loadOperations++;
			try
			{
				string fullPath = GetFullPath(saveLocation, user);
				if (Exists(saveLocation))
				{
					saveData.Load(File.ReadAllBytes(fullPath));
					successCallback?.Invoke(saveData);
				}
				else
				{
					errorCallback?.Invoke(new SaveLoadErrorResult("File " + fullPath + " doesn't exist", SaveLoadErrorResult.SaveLoadErrorType.NO_DATA_TO_LOAD));
				}
			}
			catch (Exception ex)
			{
				errorCallback?.Invoke(new SaveLoadErrorResult(ex.Message, SaveLoadErrorResult.SaveLoadErrorType.DATA_CORRUPTED));
			}
			finally
			{
				loadOperations--;
			}
		}

		protected override bool _Exists(SaveLocation saveLocation, UserData user)
		{
			return File.Exists(GetFullPath(saveLocation, user));
		}

		protected override IEnumerator _Delete(SaveLocation saveLocation, UserData user, Action onDelete = null, Action onCancel = null, bool displayDialog = true)
		{
			while (!Initialized)
			{
				yield return null;
			}
			try
			{
				File.Delete(GetFullPath(saveLocation, user));
				onDelete?.Invoke();
			}
			catch
			{
				onCancel?.Invoke();
			}
			yield return null;
		}
	}
}
