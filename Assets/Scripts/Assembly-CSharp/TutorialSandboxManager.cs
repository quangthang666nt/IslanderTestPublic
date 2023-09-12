using SCS.Gameplay;
using UnityEngine;

public class TutorialSandboxManager : MonoBehaviour
{
	public enum ETutorialState
	{
		DoneAndIgnore = 0,
		WaitingPlacements = 1,
		LaunchInNextGame = 2,
		DeleteStructures = 3
	}

	[Tooltip("How many builds need to be placed to launch tutorial")]
	public int buildsPlacedToLaunch = 2;

	[Tooltip("Tooltip asking to destroy buildings.")]
	public GameObject goDeleteStructuresTooltip;

	[Tooltip("DemolitionController Ref")]
	public static ETutorialState eTutorialState;

	public static TutorialSandboxManager singleton;

	public CustomEventHandler onDemolitionEvent;

	private void Awake()
	{
		singleton = this;
	}

	public static void AfterLoad()
	{
		if (LocalGameManager.singleton.GameMode != 0)
		{
			if ((SandboxGenerator.SandboxConfig.globalTutorialDone && !SettingsManager.Singleton.CurrentData.gameplayData.bShowTutorial) || SandboxGenerator.SandboxConfig.matchTutorialDone)
			{
				eTutorialState = ETutorialState.DoneAndIgnore;
			}
			else if (SandboxGenerator.SandboxConfig.matchBuildsPlaced >= singleton.buildsPlacedToLaunch)
			{
				eTutorialState = ETutorialState.LaunchInNextGame;
			}
			else
			{
				eTutorialState = ETutorialState.WaitingPlacements;
			}
		}
	}

	private void Update()
	{
		if (LocalGameManager.singleton.GameMode != 0 && eTutorialState != 0)
		{
			if (eTutorialState == ETutorialState.WaitingPlacements && SandboxGenerator.SandboxConfig.matchBuildsPlaced >= singleton.buildsPlacedToLaunch)
			{
				eTutorialState = ETutorialState.LaunchInNextGame;
			}
			if (eTutorialState == ETutorialState.LaunchInNextGame && LocalGameManager.singleton.GameState == LocalGameManager.EGameState.InGame && UiCanvasManager.Singleton.UIState == UiCanvasManager.EUIState.InGamePlaying)
			{
				goDeleteStructuresTooltip.SetActive(value: true);
				eTutorialState = ETutorialState.DeleteStructures;
				onDemolitionEvent.ClearEvent();
				onDemolitionEvent.RegisterEvent(DemolitionDone);
			}
			if (eTutorialState == ETutorialState.DeleteStructures && UiCanvasManager.Singleton.UIState != UiCanvasManager.EUIState.InGamePlaying)
			{
				SkipTutorial();
				eTutorialState = ETutorialState.LaunchInNextGame;
			}
		}
	}

	private void DemolitionDone()
	{
		SkipTutorial();
		eTutorialState = ETutorialState.DoneAndIgnore;
		SandboxGenerator.SandboxConfig.globalTutorialDone = true;
		SandboxGenerator.SandboxConfig.matchTutorialDone = true;
		SandboxGenerator.Save();
	}

	public void SkipTutorial()
	{
		onDemolitionEvent.ClearEvent();
		goDeleteStructuresTooltip.SetActive(value: false);
	}
}
