using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiNewIslandManager : MonoBehaviour
{
	[SerializeField]
	private Button travelButton;

	[SerializeField]
	private List<Image> imgButton;

	[SerializeField]
	private Vector2 fillRange;

	[SerializeField]
	private Animator visualParentAnimator;

	[SerializeField]
	private Image imgOutline;

	[SerializeField]
	private Image imgInactiveOverlay;

	[SerializeField]
	private Color colOutlineActive;

	[SerializeField]
	private Color colOutlineInactive;

	[SerializeField]
	private Button lockedProgressButton;

	public KeyMappingUpdater m_InputDisplay;

	private bool unlocked;

	private InputManager inputManager;

	private void Start()
	{
		DeactivateTravelButton();
		inputManager = InputManager.Singleton;
	}

	private void Update()
	{
		CheckIslandProgressLocked();
		if (LocalGameManager.singleton.BNextIslandAvailable && !travelButton.interactable)
		{
			ActivateTravelButton();
		}
		else if (!LocalGameManager.singleton.BNextIslandAvailable && travelButton.interactable)
		{
			DeactivateTravelButton();
		}
		if (unlocked && inputManager.InputDataCurrent.bsNextIsland == InputManager.ButtonState.Down && UiCanvasManager.Singleton.UIState == UiCanvasManager.EUIState.InGamePlaying)
		{
			AudioManager.singleton.PlayButtonClick();
			UiCanvasManager.Singleton.ToNewIslandPrompt();
		}
		if (visualParentAnimator.enabled && visualParentAnimator.gameObject.activeInHierarchy)
		{
			if (unlocked && !visualParentAnimator.GetBool("Unlocked"))
			{
				visualParentAnimator.SetBool("Unlocked", value: true);
			}
			else if (!unlocked && visualParentAnimator.GetBool("Unlocked"))
			{
				visualParentAnimator.SetBool("Unlocked", value: false);
			}
		}
		float fillAmount = Mathf.Lerp(fillRange.x, fillRange.y, LocalGameManager.singleton.FUnlockIslandProgress);
		if (LocalGameManager.singleton.FUnlockIslandProgress >= 1f)
		{
			fillAmount = 1f;
		}
		foreach (Image item in imgButton)
		{
			item.fillAmount = fillAmount;
		}
	}

	private void CheckIslandProgressLocked()
	{
		if (LocalGameManager.singleton.IsProgressLocked())
		{
			if (travelButton.gameObject.activeSelf)
			{
				travelButton.gameObject.SetActive(value: false);
			}
			if (!lockedProgressButton.gameObject.activeSelf)
			{
				lockedProgressButton.gameObject.SetActive(value: true);
			}
		}
		else
		{
			if (!travelButton.gameObject.activeSelf)
			{
				travelButton.gameObject.SetActive(value: true);
			}
			if (lockedProgressButton.gameObject.activeSelf)
			{
				lockedProgressButton.gameObject.SetActive(value: false);
			}
		}
	}

	private void ActivateTravelButton()
	{
		unlocked = true;
		imgInactiveOverlay.enabled = false;
		travelButton.interactable = true;
		imgOutline.color = colOutlineActive;
		if (m_InputDisplay != null)
		{
			m_InputDisplay.Enable();
		}
	}

	private void DeactivateTravelButton()
	{
		travelButton.interactable = false;
		imgOutline.color = colOutlineInactive;
		imgInactiveOverlay.enabled = true;
		unlocked = false;
		if (m_InputDisplay != null)
		{
			m_InputDisplay.Disable();
		}
	}
}
