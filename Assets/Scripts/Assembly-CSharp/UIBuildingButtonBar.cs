using UnityEngine;

public class UIBuildingButtonBar : MonoBehaviour
{
	[SerializeField]
	private RectTransform transSelection;

	[SerializeField]
	private bool isSandbox;

	[SerializeField]
	private RectTransform plusButton;

	[SerializeField]
	private RectTransform undoButton;

	[SerializeField]
	private RectTransform deleteButton;

	[SerializeField]
	private float fSelectionMoveSpeed = 0.25f;

	private RectTransform currentSelectionTarget;

	private UiBuildingButtonManager uiBuildingButtonManager;

	private LocalGameManager localGameManager;

	private bool PlusOrUndoButtonSelected
	{
		get
		{
			if (!uiBuildingButtonManager.IsPlusButtonSelected())
			{
				return uiBuildingButtonManager.IsUndoButtonSelected();
			}
			return true;
		}
	}

	private void Start()
	{
		OnButtonDeselect();
		uiBuildingButtonManager = UiBuildingButtonManager.singleton;
		localGameManager = LocalGameManager.singleton;
	}

	private void OnEnable()
	{
		UiBuildingButtonManager.singleton.eventOnBuildingButtonClick.AddListener(OnButtonSelect);
	}

	private void OnDisable()
	{
		UiBuildingButtonManager.singleton.eventOnBuildingButtonClick.RemoveListener(OnButtonSelect);
	}

	private void OnButtonSelect()
	{
		if (isSandbox && localGameManager.GameMode == LocalGameManager.EGameMode.Sandbox)
		{
			if (uiBuildingButtonManager.IsDeleteBuildingButtonSelected())
			{
				currentSelectionTarget = deleteButton;
			}
			else
			{
				if (PlusOrUndoButtonSelected)
				{
					return;
				}
				currentSelectionTarget = uiBuildingButtonManager.GoSelectedButton.GetComponent<RectTransform>();
			}
		}
		else if (localGameManager.GameMode == LocalGameManager.EGameMode.Default)
		{
			if (uiBuildingButtonManager.IsPlusButtonSelected())
			{
				currentSelectionTarget = plusButton;
			}
			else if (uiBuildingButtonManager.IsUndoButtonSelected())
			{
				currentSelectionTarget = undoButton;
			}
			else
			{
				currentSelectionTarget = uiBuildingButtonManager.GoSelectedButton.GetComponent<RectTransform>();
			}
		}
		if (!transSelection.gameObject.activeSelf)
		{
			transSelection.gameObject.SetActive(value: true);
			if ((bool)currentSelectionTarget)
			{
				transSelection.position = currentSelectionTarget.position;
			}
		}
	}

	private void OnButtonDeselect()
	{
		transSelection.gameObject.SetActive(value: false);
		currentSelectionTarget = null;
	}

	private void Update()
	{
		if (transSelection.gameObject.activeInHierarchy)
		{
			if (currentSelectionTarget == null || (UiBuildingButtonManager.singleton.GoSelectedButton == null && !PlusOrUndoButtonSelected && !uiBuildingButtonManager.IsDeleteBuildingButtonSelected()))
			{
				OnButtonDeselect();
			}
			else
			{
				transSelection.position = Vector3.MoveTowards(transSelection.position, currentSelectionTarget.position, fSelectionMoveSpeed * Time.deltaTime);
			}
		}
	}
}
