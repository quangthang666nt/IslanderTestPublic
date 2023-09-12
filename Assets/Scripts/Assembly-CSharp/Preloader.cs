using System.Collections;
using SCS.AppManagement;
using SCS.SaveLoad;
using SCS.UserManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Preloader : MonoBehaviour
{
	private bool AppInit;

	private bool SaveInit;

	private bool UserInit;

	private bool GameManagerInit;

	private bool loaded;

	private static bool Loaded;

	private void SetAppInit()
	{
		Debug.Log("[SCS - Preloader] SetAppInit");
		AppInit = true;
		SCSAppManager.OnAppInitializationComplete -= SetAppInit;
	}

	private void SetSaveInit()
	{
		Debug.Log("[SCS - Preloader] SetSaveInit");
		SaveInit = true;
		SCSSaveLoadManager.OnInitializationComplete -= SetSaveInit;
	}

	private void SetUserInit()
	{
		Debug.Log("[SCS - Preloader] SetUserInit");
		UserInit = true;
		SCSUserManager.OnInitializationComplete -= SetUserInit;
	}

	private void Awake()
	{
		Application.runInBackground = false;
		SCSAppManager.OnAppInitializationComplete += SetAppInit;
		SCSSaveLoadManager.OnInitializationComplete += SetSaveInit;
		SCSUserManager.OnInitializationComplete += SetUserInit;
		Input.simulateMouseWithTouches = false;
	}

	private void Update()
	{
		if (!loaded && AppInit && UserInit)
		{
			loaded = true;
			OnGameManagerInit();
		}
	}

	private void OnGameManagerInit()
	{
		StartCoroutine(LoadCoroutine());
	}

	private IEnumerator LoadCoroutine()
	{
		yield return new WaitForSecondsRealtime(3f);
		AsyncOperation op = SceneManager.LoadSceneAsync("main_01", LoadSceneMode.Single);
		Debug.Log("[SCS - Preloader] Loading new Scene");
		while (!op.isDone)
		{
			yield return null;
		}
		Debug.Log("[SCS - Preloader] New Scene Loaded");
	}
}
