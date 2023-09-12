using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class UIBuildingChoice : MonoBehaviour
{
	private static UIBuildingChoice singleton;

	[SerializeField]
	private GameObject goButtonPrefab;

	[SerializeField]
	private Transform goButtonParent;

	[SerializeField]
	private GameObject goText;

	[SerializeField]
	private GameObject goBackground;

	private List<GameObject> currentButtonObjs = new List<GameObject>();

	private List<UIBuildingChoiceButton> currentButtons = new List<UIBuildingChoiceButton>();

	public UnityEvent eventOnBuildingChoiceClick;

	private float fActivationIntervall = 0.25f;

	private const float fButtonScaleFactor = 2.5f;

	private Coroutine crtDelayedActivation;

	public List<BBPack> liBuildingPackSelection = new List<BBPack>();

	private bool active;

	private int selectedIndex = -1;

	private UIBuildingChoiceButton selectedButton;

	private List<BBPack> liBBPacksSave = new List<BBPack>();

	public static UIBuildingChoice Singleton => singleton;

	public List<BBPack> LiBBPacksSave => liBBPacksSave;

	private void Awake()
	{
		if (singleton == null)
		{
			singleton = this;
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (!active || InputManager.Singleton.imLastUsedInputMethod != InputManager.InputMode.Controller)
		{
			return;
		}
		int buildingPackSelectionScroll = InputManager.Singleton.InputDataCurrent.buildingPackSelectionScroll;
		if (buildingPackSelectionScroll != 0)
		{
			if (selectedButton != null)
			{
				selectedButton.Deselect();
			}
			selectedIndex += buildingPackSelectionScroll;
			if (selectedIndex < 0)
			{
				selectedIndex = currentButtons.Count - 1;
			}
			else if (selectedIndex >= currentButtons.Count)
			{
				selectedIndex = 0;
			}
			selectedButton = currentButtons[selectedIndex];
			selectedButton.Select();
		}
		if (selectedButton != null && InputManager.Singleton.InputDataCurrent.bsBuildingPlace == InputManager.ButtonState.Pressed)
		{
			selectedButton.Apply();
		}
	}

	public void Select(UIBuildingChoiceButton newSelectedButton)
	{
		if (!currentButtons.Contains(newSelectedButton))
		{
			Debug.LogError("UIBuildingChoice currentButtons does not contains: " + newSelectedButton);
			return;
		}
		if (selectedButton != null)
		{
			selectedButton.Deselect();
		}
		selectedButton = newSelectedButton;
		selectedIndex = currentButtons.IndexOf(newSelectedButton);
		selectedButton.Select();
	}

	public void Activate()
	{
		UiCanvasManager.Singleton.ToPickBuilding();
		if (crtDelayedActivation != null)
		{
			StopCoroutine(crtDelayedActivation);
		}
		List<BBPack> list = new List<BBPack>();
		GenerateBuildingPackSelection();
		list.AddRange(liBuildingPackSelection);
		if (list.Count > 0)
		{
			crtDelayedActivation = StartCoroutine(DelayedActivation(list));
		}
		else
		{
			OnNoMoreBBPacksAvailable();
		}
		goBackground.SetActive(value: true);
	}

	private void GenerateBuildingPackSelection()
	{
		if (liBuildingPackSelection == null || liBuildingPackSelection.Count == 0)
		{
			liBuildingPackSelection = BuildorderBrainB.singleton.LiBBPacksGetUnlockable(2);
		}
		liBBPacksSave = liBuildingPackSelection;
	}

	public void Deactivate()
	{
		if (crtDelayedActivation != null)
		{
			StopCoroutine(crtDelayedActivation);
		}
		goText.SetActive(value: false);
		List<GameObject> list = new List<GameObject>();
		foreach (Transform item in goButtonParent)
		{
			list.Add(item.gameObject);
		}
		for (int i = 0; i < list.Count; i++)
		{
			Object.Destroy(list[i]);
		}
		active = false;
		selectedIndex = -1;
		selectedButton = null;
		goBackground.SetActive(value: false);
	}

	public void OnButtonPress(BBPack _bbPack)
	{
		liBuildingPackSelection.Clear();
		liBBPacksSave.Clear();
		BuildorderBrainB.singleton.UnlockBBPack(_bbPack);
		BuildorderBrainB.singleton.randomStateForNextChoice = default(Random.State);
		eventOnBuildingChoiceClick.Invoke();
		UiCanvasManager.Singleton.ToStartMatch();
		LocalGameManager.singleton.BuildingPackUnlocked();
		UIPlusBuildingsButton.singleton.UpdateButton();
		SaveLoadManager.PerformAutosave();
	}

	public void OnNoMoreBBPacksAvailable()
	{
		liBuildingPackSelection.Clear();
		liBBPacksSave.Clear();
		UiCanvasManager.Singleton.ToStartMatch();
		LocalGameManager.singleton.BuildingPackUnlocked();
		UIPlusBuildingsButton.singleton.UpdateButton();
		SaveLoadManager.PerformAutosave();
	}

	private IEnumerator DelayedActivation(List<BBPack> _liBBPack)
	{
		WaitForSeconds wfsIntervall = new WaitForSeconds(fActivationIntervall);
		List<GameObject> list = new List<GameObject>();
		foreach (Transform item in goButtonParent)
		{
			list.Add(item.gameObject);
		}
		for (int i = 0; i < list.Count; i++)
		{
			Object.Destroy(list[i]);
		}
		goText.SetActive(value: true);
		yield return wfsIntervall;
		currentButtonObjs.Clear();
		currentButtons.Clear();
		int num = 0;
		foreach (BBPack item2 in _liBBPack)
		{
			GameObject gameObject = Object.Instantiate(goButtonPrefab, goButtonParent);
			gameObject.transform.localScale = Vector3.zero;
			UIBuildingChoiceButton component = gameObject.GetComponent<UIBuildingChoiceButton>();
			currentButtons.Add(component);
			component.bbPack = item2;
			component.uiBuildingChoice = this;
			component.iHotkey = num;
			num++;
			GameObject original = ((!(item2.goPackIcon != null)) ? item2.liGoBuildings[0].GetComponent<Building>().goButtonImage : item2.goPackIcon);
			GameObject gameObject2 = Object.Instantiate(original, gameObject.transform);
			gameObject2.transform.localScale = Vector3.one * 2.5f;
			if ((bool)FeedbackManager.Singleton.goBuildingPackBackground)
			{
				GameObject gameObject3 = Object.Instantiate(FeedbackManager.Singleton.goBuildingPackBackground, gameObject2.transform);
				gameObject3.transform.SetSiblingIndex(0);
				component.selector = gameObject3.GetComponent<UIBuildingPackSelectable>();
			}
			if (string.IsNullOrEmpty(item2.StrPackName))
			{
				gameObject.GetComponentInChildren<TMP_Text>().text = item2.liGoBuildings[0].GetComponent<Building>().strBuildingName;
			}
			else
			{
				gameObject.GetComponentInChildren<TMP_Text>().text = item2.StrPackName;
			}
			currentButtonObjs.Add(gameObject);
		}
		foreach (GameObject currentButtonObj in currentButtonObjs)
		{
			StartCoroutine(currentButtonObj.GetComponent<PopAnimation>().Wiggle());
			yield return wfsIntervall;
		}
		active = true;
		if (InputManager.Singleton.imLastUsedInputMethod == InputManager.InputMode.Controller)
		{
			Select(currentButtons[0]);
		}
	}
}
