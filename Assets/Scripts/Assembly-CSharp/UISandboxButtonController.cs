using System.Collections.Generic;
using UnityEngine;

public class UISandboxButtonController : MonoBehaviour
{
	[SerializeField]
	private List<GameObject> buttonSets = new List<GameObject>();

	[SerializeField]
	private SandboxGenerationView sandboxGenerationView;

	private int index;

	public static UISandboxButtonController singleton;

	private InputManager inputManager;

	public int IIndex => index;

	private void Awake()
	{
		singleton = this;
	}

	private void Update()
	{
		if (LocalGameManager.singleton.GameMode == LocalGameManager.EGameMode.Sandbox && inputManager.InputDataCurrent.bsNextIsland == InputManager.ButtonState.Down && UiCanvasManager.Singleton.UIState == UiCanvasManager.EUIState.InGamePlaying)
		{
			sandboxGenerationView?.InteractCurrentExpanded();
		}
	}

	private void Start()
	{
		ApplyIndex();
		inputManager = InputManager.Singleton;
	}

	private void ApplyIndex()
	{
		List<GameObject> list = new List<GameObject>(buttonSets);
		list.RemoveAt(index);
		foreach (GameObject item in list)
		{
			item.SetActive(value: false);
		}
		buttonSets[index].SetActive(value: true);
	}

	public void IncreaseIndex()
	{
		index++;
		if (index >= buttonSets.Count)
		{
			index = 0;
		}
		buttonSets[index].GetComponent<UIAnimationGroup>().animationDirection = UIAnimationGroup.AnimationDirection.TopDown;
		UiBuildingButtonManager.singleton.GoSelectedButton = null;
		ApplyIndex();
	}

	public void DecreaseIndex()
	{
		index--;
		if (index < 0)
		{
			index = buttonSets.Count - 1;
		}
		buttonSets[index].GetComponent<UIAnimationGroup>().animationDirection = UIAnimationGroup.AnimationDirection.BottomUp;
		UiBuildingButtonManager.singleton.GoSelectedButton = null;
		ApplyIndex();
	}

	public void SetIndex(int _iIndex)
	{
		index = _iIndex;
		buttonSets[index].GetComponent<UIAnimationGroup>().animationDirection = UIAnimationGroup.AnimationDirection.TopDown;
		UiBuildingButtonManager.singleton.GoSelectedButton = null;
		ApplyIndex();
	}

	public Transform TransReturnCurrentButtonParent()
	{
		return buttonSets[index].transform;
	}
}
