using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITooltip : MonoBehaviour
{
	private static UITooltip singleton;

	[Header("Attributes")]
	[SerializeField]
	private float fPopUpDelay = 1f;

	[SerializeField]
	private Vector3 v3Offset = Vector3.one;

	[SerializeField]
	private int maxScoreInteractionsPerColumn = 7;

	[Header("Own References")]
	[SerializeField]
	private GameObject goTarget;

	[SerializeField]
	private GameObject goDescription;

	[SerializeField]
	private GameObject goBuildingTooltip;

	[SerializeField]
	private TextMeshProUGUI textHeader;

	[SerializeField]
	private TextMeshProUGUI textDescription;

	[SerializeField]
	private TextMeshProUGUI testBaseScoreNum;

	[SerializeField]
	private HorizontalLayoutGroup scoreInteractionGroup;

	[Header("Prefab References")]
	[SerializeField]
	private VerticalLayoutGroup scoreInteractionColumnPrefab;

	[SerializeField]
	private TextMeshProUGUI scoreInteractionTextPrefab;

	[SerializeField]
	private Image scoreInteractionLinePrefab;

	private RectTransform rtTarget;

	private bool bShow;

	private float fTimer;

	private bool showingBuildingTooltip;

	public static UITooltip Singleton => singleton;

	private void Awake()
	{
		if (singleton != null)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			singleton = this;
		}
	}

	private void Start()
	{
		rtTarget = goTarget.GetComponent<RectTransform>();
		ClearChilds(scoreInteractionGroup.transform);
		Disable();
	}

	private void Update()
	{
		if (UiCanvasManager.Singleton.UIState == UiCanvasManager.EUIState.ScreenshotMode || LocalGameManager.singleton.GameMode == LocalGameManager.EGameMode.Sandbox)
		{
			if (goTarget.activeSelf)
			{
				goTarget.SetActive(value: false);
			}
			return;
		}
		if (bShow && fTimer < fPopUpDelay)
		{
			fTimer += Time.deltaTime;
			if (fTimer >= fPopUpDelay)
			{
				goTarget.SetActive(value: true);
				StartCoroutine(RebuildLayoutNextTick());
			}
		}
		if (goTarget.activeSelf)
		{
			ShowTooltip();
		}
	}

	public void Enable(string _header, string _description, InputManager.InputMode inputMode = InputManager.InputMode.Mouse)
	{
		Enable(0f, _header, _description, inputMode);
	}

	public void Enable(string _header, Building building, InputManager.InputMode inputMode = InputManager.InputMode.Mouse)
	{
		Enable(0f, _header, "", inputMode, immediate: false, building);
	}

	public void EnableImmediate(string _header, string _description, InputManager.InputMode inputMode = InputManager.InputMode.Mouse)
	{
		Enable(fPopUpDelay, _header, _description, inputMode, immediate: true);
	}

	public void EnableImmediate(string _header, Building building, InputManager.InputMode inputMode = InputManager.InputMode.Mouse)
	{
		Enable(fPopUpDelay, _header, "", inputMode, immediate: true, building);
	}

	public void Disable(InputManager.InputMode inputMode = InputManager.InputMode.Mouse)
	{
		if (inputMode == InputManager.Singleton.imLastUsedInputMethod)
		{
			bShow = false;
			if ((bool)goTarget)
			{
				goTarget.SetActive(value: false);
			}
			showingBuildingTooltip = false;
		}
	}

	private void Enable(float delay, string _header, string _description, InputManager.InputMode inputMode, bool immediate = false, Building building = null)
	{
		if (UiCanvasManager.Singleton.UIState == UiCanvasManager.EUIState.InGamePlaying && inputMode == InputManager.Singleton.imLastUsedInputMethod && SettingsManager.Singleton.CurrentData.gameplayData.enableTooltips)
		{
			fTimer = delay;
			bShow = true;
			textHeader.text = _header;
			goDescription.SetActive(_description.Length > 0);
			textDescription.text = _description;
			if (building != null)
			{
				goBuildingTooltip.SetActive(value: true);
				ConfigureInteractionScoreTab(building);
				showingBuildingTooltip = true;
			}
			else
			{
				goBuildingTooltip.SetActive(value: false);
			}
			if (immediate)
			{
				goTarget.SetActive(value: true);
				StartCoroutine(RebuildLayoutNextTick());
			}
		}
	}

	private IEnumerator RebuildLayoutNextTick()
	{
		yield return null;
		LayoutRebuilder.ForceRebuildLayoutImmediate(rtTarget);
	}

	private void ConfigureInteractionScoreTab(Building building)
	{
		ClearChilds(scoreInteractionGroup.transform);
		Dictionary<string, int> interactions = building.GetInteractions();
		if (interactions.ContainsKey(building.strLocalizedBaseValueTemplate))
		{
			if (interactions[building.strLocalizedBaseValueTemplate] > 0)
			{
				testBaseScoreNum.text = string.Concat(building.strLocalizedBaseValueTemplate, "<style=POS> ", interactions[building.strLocalizedBaseValueTemplate].ToString(), "</style>");
			}
			else if (interactions[building.strLocalizedBaseValueTemplate] < 0)
			{
				testBaseScoreNum.text = string.Concat(building.strLocalizedBaseValueTemplate, "<style=NEG> ", interactions[building.strLocalizedBaseValueTemplate].ToString(), "</style>");
			}
			else
			{
				testBaseScoreNum.text = string.Concat(building.strLocalizedBaseValueTemplate, " ", interactions[building.strLocalizedBaseValueTemplate].ToString());
			}
			interactions.Remove(building.strLocalizedBaseValueTemplate);
		}
		Dictionary<string, int> dictionary = interactions.Where((KeyValuePair<string, int> p) => p.Value > 0).ToDictionary((KeyValuePair<string, int> p) => p.Key, (KeyValuePair<string, int> p) => p.Value);
		Dictionary<string, int> dictionary2 = (from p in interactions
			where p.Value < 0
			select p into k
			orderby k.Value
			select k).ToDictionary((KeyValuePair<string, int> p) => p.Key, (KeyValuePair<string, int> p) => p.Value);
		if (dictionary.Count > 0)
		{
			int num = maxScoreInteractionsPerColumn;
			if (dictionary.Count > num)
			{
				num = dictionary.Count / 2 + dictionary.Count % 2;
			}
			Transform transform = CreateScoreInteractionColumn();
			foreach (KeyValuePair<string, int> item in dictionary)
			{
				if (transform.childCount == num)
				{
					CreateScoreInteractionLine();
					transform = CreateScoreInteractionColumn();
				}
				CreateScoreInteractionText(transform).text = "<style=POS>" + item.Key + " (" + item.Value + ")</style>";
			}
		}
		if (dictionary.Count > 0 && dictionary2.Count > 0)
		{
			CreateScoreInteractionLine();
		}
		if (dictionary2.Count <= 0)
		{
			return;
		}
		int num2 = maxScoreInteractionsPerColumn;
		if (dictionary2.Count > num2)
		{
			num2 = dictionary2.Count / 2 + dictionary2.Count % 2;
		}
		Transform transform2 = CreateScoreInteractionColumn();
		foreach (KeyValuePair<string, int> item2 in dictionary2)
		{
			if (transform2.childCount == num2)
			{
				CreateScoreInteractionLine();
				transform2 = CreateScoreInteractionColumn();
			}
			CreateScoreInteractionText(transform2).text = "<style=NEG>" + item2.Key + " (" + Mathf.Abs(item2.Value) + ")</style>";
		}
	}

	private Transform CreateScoreInteractionColumn()
	{
		Transform transform = Object.Instantiate(scoreInteractionColumnPrefab, scoreInteractionGroup.transform).transform;
		ClearChilds(transform);
		return transform;
	}

	private void CreateScoreInteractionLine()
	{
		Object.Instantiate(scoreInteractionLinePrefab, scoreInteractionGroup.transform);
	}

	private TextMeshProUGUI CreateScoreInteractionText(Transform parent)
	{
		return Object.Instantiate(scoreInteractionTextPrefab, parent);
	}

	private void ShowTooltip()
	{
		Vector3 position = ((InputManager.Singleton.imLastUsedInputMethod != 0) ? new Vector3(Screen.width - Screen.width / 100, (float)Screen.height - (float)(Screen.height / 100) * 1.5f) : InputManager.Singleton.InputDataCurrent.v3PointerScreenPos);
		goTarget.transform.position = UICameraSpaceHelper.ScreenPointToCanvasPoint(position);
		goTarget.transform.localPosition += v3Offset;
		AdjustPivot();
	}

	private void AdjustPivot()
	{
		RectTransform transCnvs = UICameraSpaceHelper.TransCnvs;
		Vector3 vector = transCnvs.InverseTransformPoint(rtTarget.position);
		float x = 0f;
		if (showingBuildingTooltip)
		{
			x = (vector.x + transCnvs.sizeDelta.x / 2f) / transCnvs.sizeDelta.x;
		}
		else if (vector.x + rtTarget.sizeDelta.x > transCnvs.sizeDelta.x / 2f)
		{
			x = 1f;
		}
		float y = 1f;
		if (vector.y - rtTarget.sizeDelta.y < 0f - transCnvs.sizeDelta.y / 2f)
		{
			y = 0f;
		}
		rtTarget.pivot = new Vector2(x, y);
	}

	private void ClearChilds(Transform parent)
	{
		for (int i = 0; i < parent.childCount; i++)
		{
			Object.Destroy(parent.GetChild(i).gameObject);
		}
	}
}
