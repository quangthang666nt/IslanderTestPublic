using System.Collections;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UiBuildingButton : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerExitHandler, IPointerEnterHandler
{
	private UiBuildingButtonManager uiBuildingButtonManager;

	private LocalGameManager localGameManager;

	[Header("Settings")]
	public GameObject goResourceBuilding;

	public AnimationCurve acScaleAnimationCurve;

	public float fScaleDefault = 1f;

	public float fScaleFactorOnHover;

	public float fAnimationTime;

	[Header("References")]
	[SerializeField]
	private GameObject goNumberToUpdate;

	[SerializeField]
	private Text textNumberToUpdate;

	[SerializeField]
	private GameObject goTextToUpdate;

	[SerializeField]
	private TextMeshProUGUI textName;

	private LocalizedString m_BuildingName;

	private bool m_BuildingNameIsDirty;

	[HideInInspector]
	public bool bPrewarm = true;

	[HideInInspector]
	public int iBuildingSeed = -10000000;

	[HideInInspector]
	public int iBuildingSeedNext = -10000000;

	private GameObject goImageParent;

	private Coroutine crCurrentFadeInAnimation;

	private Coroutine crCurrentFadeOutAnimation;

	private float fAnimationTimer;

	private bool bStartCalled;

	public Text TextNumberToUpdate => textNumberToUpdate;

	public LocalizedString buildingName
	{
		get
		{
			return m_BuildingName;
		}
		set
		{
			if (value.mTerm != m_BuildingName.mTerm)
			{
				m_BuildingName = value;
				m_BuildingNameIsDirty = true;
			}
		}
	}

	private void OnEnable()
	{
		uiBuildingButtonManager = UiBuildingButtonManager.singleton;
		if (!uiBuildingButtonManager.LiBuildingButtonsExisting.Contains(this))
		{
			uiBuildingButtonManager.LiBuildingButtonsExisting.Add(this);
		}
	}

	private void OnDestroy()
	{
		I2Manager.OnLanguageChange -= OnLanguageChange;
		uiBuildingButtonManager = UiBuildingButtonManager.singleton;
		uiBuildingButtonManager.LiBuildingButtonsExisting.Remove(this);
	}

	private void OnLanguageChange(string language)
	{
		m_BuildingNameIsDirty = true;
	}

	protected void Start()
	{
		I2Manager.OnLanguageChange += OnLanguageChange;
		bStartCalled = true;
		if (iBuildingSeed == -10000000)
		{
			iBuildingSeed = Random.Range(0, 32000);
		}
		if (iBuildingSeedNext == -10000000)
		{
			iBuildingSeedNext = Random.Range(0, 32000);
		}
		uiBuildingButtonManager = UiBuildingButtonManager.singleton;
		Building component = goResourceBuilding.GetComponent<Building>();
		localGameManager = LocalGameManager.singleton;
		buildingName = component.strBuildingName;
		goImageParent = Object.Instantiate(component.goButtonImage, base.transform);
		goImageParent.transform.SetAsFirstSibling();
		goTextToUpdate.SetActive(value: false);
	}

	public void OnPlace()
	{
		iBuildingSeed = iBuildingSeedNext;
		iBuildingSeedNext = Random.Range(0, 32000);
	}

	public bool UpdateButton()
	{
		if (!bStartCalled)
		{
			return false;
		}
		bool result = false;
		int num = localGameManager.IBuildingsInInventory(goResourceBuilding);
		if (num > 0)
		{
			bPrewarm = false;
			base.gameObject.SetActive(value: true);
			goImageParent.SetActive(value: true);
			if (num > 1)
			{
				goNumberToUpdate.SetActive(value: true);
				textNumberToUpdate.text = num.ToString();
			}
			else
			{
				goNumberToUpdate.SetActive(value: false);
			}
			result = true;
		}
		else if (bPrewarm)
		{
			base.gameObject.SetActive(value: true);
			goImageParent.SetActive(value: false);
			goNumberToUpdate.SetActive(value: false);
			result = true;
		}
		else
		{
			base.gameObject.SetActive(value: false);
		}
		if (uiBuildingButtonManager.GoSelectedButton == this)
		{
			goTextToUpdate.SetActive(value: true);
		}
		else
		{
			goTextToUpdate.SetActive(value: false);
		}
		if (m_BuildingNameIsDirty)
		{
			textName.text = buildingName;
			m_BuildingNameIsDirty = false;
		}
		return result;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (UiCanvasManager.Singleton.UIState != UiCanvasManager.EUIState.InGamePicking)
		{
			uiBuildingButtonManager.ButtonClicked(this);
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		Building component = goResourceBuilding.GetComponent<Building>();
		UITooltip.Singleton.Enable(component.strBuildingName, component);
		if (crCurrentFadeInAnimation != null)
		{
			StopCoroutine(crCurrentFadeInAnimation);
		}
		if (crCurrentFadeOutAnimation != null)
		{
			StopCoroutine(crCurrentFadeOutAnimation);
			crCurrentFadeOutAnimation = null;
		}
		crCurrentFadeInAnimation = StartCoroutine(AnimationOnEnter());
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		UITooltip.Singleton.Disable();
		uiBuildingButtonManager.ButtonExit(this);
		if (crCurrentFadeOutAnimation != null)
		{
			StopCoroutine(crCurrentFadeOutAnimation);
		}
		crCurrentFadeOutAnimation = StartCoroutine(AnimationOnExit());
	}

	private IEnumerator AnimationOnEnter()
	{
		while (fAnimationTimer < fAnimationTime)
		{
			fAnimationTimer += Time.deltaTime;
			float t = acScaleAnimationCurve.Evaluate(Mathf.InverseLerp(0f, fAnimationTime, fAnimationTimer));
			goImageParent.transform.localScale = Vector3.one * Mathf.Lerp(fScaleDefault, fScaleFactorOnHover, t);
			yield return null;
		}
		fAnimationTimer = fAnimationTime;
		crCurrentFadeInAnimation = null;
	}

	private IEnumerator AnimationOnExit()
	{
		while (crCurrentFadeInAnimation != null)
		{
			yield return null;
		}
		while (fAnimationTimer > 0f)
		{
			fAnimationTimer -= Time.deltaTime;
			float t = acScaleAnimationCurve.Evaluate(1f - Mathf.InverseLerp(0f, fAnimationTime, fAnimationTimer));
			goImageParent.transform.localScale = Vector3.one * Mathf.Lerp(fScaleFactorOnHover, fScaleDefault, t);
			yield return null;
		}
		fAnimationTimer = 0f;
		crCurrentFadeOutAnimation = null;
	}

	private string GetTooltip()
	{
		return " ";
	}
}
