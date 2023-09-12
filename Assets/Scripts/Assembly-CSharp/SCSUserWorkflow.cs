using SCS.UserManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class SCSUserWorkflow : MonoBehaviour
{
	protected bool paused;

	protected UserData lastActiveUser;

	protected static bool initialized;

	protected bool userInitialized;

	protected abstract string GetFallbackScene();

	protected virtual void Fallback()
	{
		SceneManager.LoadScene(GetFallbackScene());
	}

	protected virtual void DisplayAccountPicker()
	{
	}

	protected abstract void Pause();

	protected abstract void Unpause();

	protected virtual void Awake()
	{
		if (initialized)
		{
			Object.Destroy(base.gameObject);
		}
		initialized = true;
		SCSUserManager.OnInitializationComplete += SCSUserManager_OnInitializationComplete;
		Object.DontDestroyOnLoad(this);
	}

	private void SCSUserManager_OnInitializationComplete()
	{
		userInitialized = true;
		lastActiveUser = SCSUserManager.Instance.GetActiveUser();
		Debug.LogError("<color=green>Initialization complete, active user is " + ((lastActiveUser == null) ? " null " : " not null") + " </color>");
		SCSUserManager.OnInitializationComplete -= SCSUserManager_OnInitializationComplete;
	}

	private void Update()
	{
		if (!userInitialized || !initialized)
		{
			return;
		}
		UserData activeUser = SCSUserManager.Instance.GetActiveUser();
		if (activeUser != lastActiveUser && activeUser != null && lastActiveUser != null)
		{
			Fallback();
			lastActiveUser = activeUser;
		}
		if (activeUser == null)
		{
			DisplayAccountPicker();
			if (!paused)
			{
				Pause();
				paused = true;
			}
		}
		else if (paused)
		{
			Unpause();
			paused = false;
		}
	}
}
