using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIPlusBuildingsButton : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerExitHandler, IPointerEnterHandler
{
	public static UIPlusBuildingsButton singleton;

	private LocalGameManager localGameManager;

	private int iAmount;

	[SerializeField]
	private GameObject goAmountBackground;

	[SerializeField]
	private Text textAmount;

	private Animator animator;

	private void Awake()
	{
		singleton = this;
	}

	private void Start()
	{
		UpdateButton();
	}

	private void OnEnable()
	{
		if (!animator)
		{
			animator = GetComponent<Animator>();
		}
		animator.SetTrigger("Enable");
	}

	public void UpdateButton()
	{
		if (!localGameManager)
		{
			localGameManager = LocalGameManager.singleton;
		}
		iAmount = localGameManager.liIPlusBuildingButtonsIncludingBuildingCounts.Count;
		if (iAmount == 0)
		{
			base.gameObject.SetActive(value: false);
			return;
		}
		base.gameObject.SetActive(value: true);
		if (iAmount > 1)
		{
			goAmountBackground.SetActive(value: true);
			textAmount.text = iAmount.ToString();
		}
		else
		{
			goAmountBackground.SetActive(value: false);
		}
	}

	public void OnPointerDown(PointerEventData eventData = null)
	{
		AudioManager.singleton.PlayButtonClick();
		localGameManager.OpenBuildingChoice();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
	}

	public void OnPointerExit(PointerEventData eventData)
	{
	}
}
