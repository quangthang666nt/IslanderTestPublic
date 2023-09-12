using System;
using System.Collections;
using System.Collections.Generic;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SandboxGenerationView : MonoBehaviour
{
	public enum UIOpenState
	{
		CLOSED = 0,
		OPENING = 1,
		OPENED = 2,
		CLOSING = 3
	}

	public delegate UIOpenState OpenStateCallback();

	[SerializeField]
	private UISelector usBiomes;

	[SerializeField]
	private UISelector usSize;

	[SerializeField]
	private Slider slFlowers;

	[SerializeField]
	private TMP_Text ttFlowers;

	[SerializeField]
	private Slider slTrees;

	[SerializeField]
	private TMP_Text ttTrees;

	[SerializeField]
	private UISelector usWeather;

	[SerializeField]
	private TMP_Text ttWeather;

	[SerializeField]
	private Slider slWeather;

	[SerializeField]
	private Button resetButton;

	[SerializeField]
	private Button generateButton;

	[SerializeField]
	private TMP_Text ttGenerateRerollButton;

	[SerializeField]
	private LocalizedString generateString;

	[SerializeField]
	private LocalizedString rerollString;

	[SerializeField]
	private RectTransform typeLabels;

	[SerializeField]
	private RectTransform typeHandlers;

	[SerializeField]
	private GameObject typeLabelPrefab;

	[SerializeField]
	private GameObject typeHandlerPrefab;

	[SerializeField]
	private RectTransform visualParent;

	[SerializeField]
	private VerticalLayoutGroup content;

	[SerializeField]
	private GenericMenuList navigation;

	[SerializeField]
	private Image[] generateButtonDisabledImages;

	[SerializeField]
	private Image[] resetButtonDisabledImages;

	[SerializeField]
	private Color buttonEnabledColor;

	[SerializeField]
	private Color buttonDisabledColor;

	[SerializeField]
	private Image[] enableImagesOnOpen;

	[SerializeField]
	private Image[] disableImagesOnOpen;

	[SerializeField]
	private float secondsInputBlockedOnOpen = 1f;

	[SerializeField]
	private float secondsInputBlockedOnClose = 1f;

	[Header("Ingame")]
	public UIElement sandboxBuildingButtons;

	private SandboxConfiguration originalValues;

	private bool fromSaveLoad;

	private bool dynamicContentCreated;

	private bool dataLocked;

	private UIOpenState openState;

	private static List<OpenStateCallback> openStateList;

	private UIElement UiElement;

	public static bool IsOpened()
	{
		foreach (OpenStateCallback openState in openStateList)
		{
			if (openState == null || openState() != 0)
			{
				return true;
			}
		}
		return false;
	}

	private UIOpenState GetOpenState()
	{
		return openState;
	}

	private void Start()
	{
		if (openStateList == null)
		{
			openStateList = new List<OpenStateCallback>();
		}
		openStateList.Add(GetOpenState);
		UiElement = GetComponent<UIElement>();
		UiElement.eventOnActivation.AddListener(OnActivation);
		UiElement.eventOnDeactivation.AddListener(OnDeactivation);
		if ((bool)sandboxBuildingButtons)
		{
			sandboxBuildingButtons.eventOnActivation.AddListener(CleanLockData);
			sandboxBuildingButtons.eventOnDeactivation.AddListener(CheckIfOpen);
		}
	}

	private void Update()
	{
		if (openState == UIOpenState.OPENED && dynamicContentCreated)
		{
			DetectInput();
			LayoutRebuilder.ForceRebuildLayoutImmediate(visualParent);
		}
	}

	private void DetectInput()
	{
		if (InputManager.Singleton.InputDataCurrent.bUIGenerateIsland)
		{
			if (AudioManager.singleton != null)
			{
				AudioManager.singleton.PlayButtonClick();
			}
			GenerateIsland();
		}
		if (InputManager.Singleton.InputDataCurrent.bUIResetIsland && resetButton.interactable)
		{
			if (AudioManager.singleton != null)
			{
				AudioManager.singleton.PlayMenuClick();
			}
			Reset();
		}
		if (InputManager.Singleton.InputDataCurrent.bUICancel && UiCanvasManager.Singleton.UIState == UiCanvasManager.EUIState.InGamePlaying)
		{
			InteractCurrentExpanded();
		}
	}

	private void OnActivation()
	{
		if (!(SandboxGenerator.singleton == null))
		{
			SubscribeEvents();
			openState = UIOpenState.OPENING;
			StartCoroutine(BlockInputRoutine(secondsInputBlockedOnOpen, OnOpened));
		}
	}

	private void OnDeactivation()
	{
		if (!(SandboxGenerator.singleton == null))
		{
			UnsubscribeEvents();
			openState = UIOpenState.CLOSING;
			StartCoroutine(BlockInputRoutine(secondsInputBlockedOnClose, OnClosed));
		}
	}

	private IEnumerator BlockInputRoutine(float secondsBlocked, Action callback)
	{
		yield return new WaitForSeconds(secondsBlocked);
		callback?.Invoke();
	}

	private void OnOpened()
	{
		openState = UIOpenState.OPENED;
	}

	private void OnClosed()
	{
		openState = UIOpenState.CLOSED;
	}

	private void CleanLockData()
	{
		dataLocked = false;
	}

	private void CheckIfOpen()
	{
		if (openState == UIOpenState.OPENED)
		{
			UiElement.DisableElement();
			visualParent.gameObject.SetActive(value: false);
			openState = UIOpenState.CLOSED;
		}
	}

	private void Setup()
	{
		if (!dynamicContentCreated)
		{
			foreach (Transform typeHandler in typeHandlers)
			{
				UnityEngine.Object.Destroy(typeHandler);
			}
			foreach (Transform typeLabel in typeLabels)
			{
				_ = typeLabel;
				UnityEngine.Object.Destroy(typeLabels);
			}
			for (int i = 0; i < SandboxGenerator.singleton.allTypes.Count; i++)
			{
				IslandCollection islandCollection = SandboxGenerator.singleton.allTypes[i];
				GameObject gameObject = UnityEngine.Object.Instantiate(typeLabelPrefab, typeLabels.transform);
				GameObject gameObject2 = UnityEngine.Object.Instantiate(typeHandlerPrefab, typeHandlers.transform);
				gameObject.GetComponent<TMP_Text>().text = islandCollection.name;
				UICombinedElement component = gameObject.GetComponent<UICombinedElement>();
				component.elements.Clear();
				component.elements.Add(gameObject.GetComponent<RectTransform>());
				component.elements.Add(gameObject2.GetComponent<RectTransform>());
				UISandboxTypeLabel component2 = gameObject.GetComponent<UISandboxTypeLabel>();
				gameObject2.GetComponent<UISandboxTypeHandler>().cursor = component2.cursor;
				ToggleNavigationItem component3 = gameObject2.GetComponent<ToggleNavigationItem>();
				component3.m_TargetRect = gameObject.GetComponent<RectTransform>();
				navigation.m_NavigationItems.Insert(i, component3);
			}
			dynamicContentCreated = true;
		}
		else
		{
			int num = 0;
			foreach (Transform typeLabel2 in typeLabels)
			{
				typeLabel2.GetComponent<TMP_Text>().text = SandboxGenerator.singleton.allTypes[num].name;
				num++;
			}
		}
		usBiomes.options.Clear();
		for (int j = 0; j < SandboxGenerator.singleton.allBiomes.Count; j++)
		{
			usBiomes.options.Add(SandboxGenerator.singleton.allBiomes[j].name);
		}
		usSize.options.Clear();
		for (int k = 0; k < SandboxGenerator.singleton.allSizes.Count; k++)
		{
			usSize.options.Add(SandboxGenerator.singleton.allSizes[k].name);
		}
		usWeather.options.Clear();
		foreach (SandboxGenerator.Weather value in Enum.GetValues(typeof(SandboxGenerator.Weather)))
		{
			usWeather.options.Add(SandboxGenerator.singleton.GetWeatherLocalized(value));
		}
		StartCoroutine(ResetLayoutRoutine(content));
	}

	private IEnumerator ResetLayoutRoutine(LayoutGroup layoutGroup)
	{
		layoutGroup.enabled = false;
		yield return new WaitForSeconds(0.1f);
		layoutGroup.enabled = true;
		OnWeatherSelectorModified();
	}

	private void SubscribeEvents()
	{
		foreach (Transform typeHandler in typeHandlers)
		{
			typeHandler.GetComponent<Toggle>().onValueChanged.AddListener(OnTypeToogleModified);
		}
		usBiomes.eventOnSelectionChange.AddListener(OnBiomeSelectorModified);
		usSize.eventOnSelectionChange.AddListener(OnSizeSelectorModified);
		usWeather.eventOnSelectionChange.AddListener(OnWeatherSelectorModified);
		slFlowers.onValueChanged.AddListener(OnFlowersSliderModified);
		slTrees.onValueChanged.AddListener(OnTreesSliderModified);
		slWeather.onValueChanged.AddListener(OnWeatherSliderModified);
	}

	private void UnsubscribeEvents()
	{
		foreach (Transform typeHandler in typeHandlers)
		{
			typeHandler.GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
		}
		usBiomes.eventOnSelectionChange.RemoveAllListeners();
		usSize.eventOnSelectionChange.RemoveAllListeners();
		usWeather.eventOnSelectionChange.RemoveAllListeners();
		slFlowers.onValueChanged.RemoveAllListeners();
		slTrees.onValueChanged.RemoveAllListeners();
		slWeather.onValueChanged.RemoveAllListeners();
	}

	public void SetOriginalValues(SandboxConfiguration original, bool fromSaveLoad)
	{
		originalValues = original;
		this.fromSaveLoad = fromSaveLoad;
		Setup();
		CopyOriginalValues();
		UpdateResetButton();
		UpdateButton(generateButton, generateButtonDisabledImages, IsValidTypeAndSize(GetSelectedTypes(), GetSelectedSize()));
	}

	private void OpenCurrentExpanded()
	{
		SandboxConfiguration sandboxConfiguration = ScriptableObject.CreateInstance<SandboxConfiguration>();
		sandboxConfiguration.types = SandboxGenerator.singleton.GetIslandTypesByIdsFromSave();
		sandboxConfiguration.biome = SandboxGenerator.singleton.GetBiomeFromSave();
		sandboxConfiguration.size = SandboxGenerator.singleton.GetIslandSizeFromSave();
		sandboxConfiguration.advanced = SandboxGenerator.singleton.GetAdvancedOptionsFromSave();
		if (!dataLocked)
		{
			SetOriginalValues(sandboxConfiguration, fromSaveLoad: true);
			dataLocked = true;
		}
		UiElement.EnableElement();
	}

	public void InteractCurrentExpanded()
	{
		if (openState == UIOpenState.CLOSED)
		{
			OpenCurrentExpanded();
			UiBuildingButtonManager.singleton.DeselectAll();
			MonoBehaviour[] components = enableImagesOnOpen;
			EnableAll(components, enable: true);
			components = disableImagesOnOpen;
			EnableAll(components, enable: false);
			if (AudioManager.singleton != null)
			{
				AudioManager.singleton.PlayButtonClick();
			}
		}
		else if (openState == UIOpenState.OPENED)
		{
			UiElement.DisableElement();
			MonoBehaviour[] components = enableImagesOnOpen;
			EnableAll(components, enable: false);
			components = disableImagesOnOpen;
			EnableAll(components, enable: true);
			if (AudioManager.singleton != null)
			{
				AudioManager.singleton.PlayButtonClick();
			}
		}
	}

	public void Reset()
	{
		CopyOriginalValues();
		OnWeatherSelectorModified();
		UpdateResetButton();
	}

	public void GenerateIsland()
	{
		MonoBehaviour[] components = enableImagesOnOpen;
		EnableAll(components, enable: false);
		components = disableImagesOnOpen;
		EnableAll(components, enable: true);
		IslandCollection[] selectedTypes = GetSelectedTypes();
		BiomeColors selectedBiome = GetSelectedBiome();
		IslandCollection selectedSize = GetSelectedSize();
		if (!IsValidTypeAndSize(selectedTypes, selectedSize))
		{
			Debug.LogError("No available composition for types-size selection");
			return;
		}
		SandboxGenerator.AdvancedOptions selectedAdvancedOptions = GetSelectedAdvancedOptions();
		SandboxGenerator.singleton.SetData(selectedTypes, selectedBiome, selectedSize, selectedAdvancedOptions);
		if (fromSaveLoad)
		{
			UiElement.DisableElement();
			visualParent.gameObject.SetActive(value: false);
			openState = UIOpenState.CLOSED;
		}
		LocalGameManager.singleton.GameModeToSandbox();
	}

	private void CopyOriginalValues()
	{
		List<IslandCollection> list = new List<IslandCollection>(originalValues.types);
		for (int i = 0; i < SandboxGenerator.singleton.allTypes.Count; i++)
		{
			Toggle toggle = typeHandlers.GetChild(i)?.GetComponent<Toggle>();
			if ((bool)toggle)
			{
				toggle.isOn = list.Contains(SandboxGenerator.singleton.allTypes[i]);
			}
		}
		for (int j = 0; j < SandboxGenerator.singleton.allBiomes.Count; j++)
		{
			if (originalValues.biome == SandboxGenerator.singleton.allBiomes[j])
			{
				usBiomes.SetIndex(j);
				break;
			}
		}
		for (int k = 0; k < SandboxGenerator.singleton.allSizes.Count; k++)
		{
			if (originalValues.size == SandboxGenerator.singleton.allSizes[k])
			{
				usSize.SetIndex(k);
				break;
			}
		}
		slFlowers.value = originalValues.advanced.flowerWeight;
		ttFlowers.text = originalValues.advanced.flowerWeight.ToString();
		slTrees.value = originalValues.advanced.treeWeight;
		ttTrees.text = originalValues.advanced.treeWeight.ToString();
		int num = 0;
		foreach (SandboxGenerator.Weather value in Enum.GetValues(typeof(SandboxGenerator.Weather)))
		{
			if (originalValues.advanced.Weather == value)
			{
				usWeather.SetIndex(num);
			}
			num++;
		}
		slWeather.value = originalValues.advanced.weatherWeight;
		ttWeather.text = originalValues.advanced.weatherWeight.ToString();
	}

	private void OnTypeToogleModified(bool value)
	{
		UpdateResetButton();
		UpdateButton(generateButton, generateButtonDisabledImages, IsValidTypeAndSize(GetSelectedTypes(), GetSelectedSize()));
	}

	private void OnBiomeSelectorModified()
	{
		UpdateResetButton();
	}

	private void OnSizeSelectorModified()
	{
		UpdateResetButton();
		UpdateButton(generateButton, generateButtonDisabledImages, IsValidTypeAndSize(GetSelectedTypes(), GetSelectedSize()));
	}

	private void OnWeatherSelectorModified()
	{
		if (usWeather.Index == 0)
		{
			slWeather.value = 0f;
			ttWeather.text = "0";
			slWeather.gameObject.SetActive(value: false);
			BasicNavigationItem componentInParent = slWeather.GetComponentInParent<BasicNavigationItem>();
			if ((bool)componentInParent)
			{
				componentInParent.enabled = false;
			}
			ttWeather.gameObject.SetActive(value: false);
		}
		else
		{
			slWeather.gameObject.SetActive(value: true);
			BasicNavigationItem componentInParent2 = slWeather.GetComponentInParent<BasicNavigationItem>();
			if ((bool)componentInParent2)
			{
				componentInParent2.enabled = true;
			}
			ttWeather.gameObject.SetActive(value: true);
		}
		UpdateResetButton();
	}

	private void OnFlowersSliderModified(float value)
	{
		ttFlowers.text = ((int)value).ToString();
		UpdateResetButton();
	}

	private void OnTreesSliderModified(float value)
	{
		ttTrees.text = ((int)value).ToString();
		UpdateResetButton();
	}

	private void OnWeatherSliderModified(float value)
	{
		ttWeather.text = ((int)value).ToString();
		UpdateResetButton();
	}

	private void UpdateResetButton()
	{
		if (CheckTypesWithOriginal() && CheckBiomeWithOriginal() && CheckSizeWithOriginal() && CheckAdvancedWithOriginal())
		{
			if (!fromSaveLoad)
			{
				ttGenerateRerollButton.text = generateString;
			}
			else
			{
				ttGenerateRerollButton.text = rerollString;
			}
			UpdateButton(resetButton, resetButtonDisabledImages, enabled: false);
		}
		else
		{
			ttGenerateRerollButton.text = generateString;
			UpdateButton(resetButton, resetButtonDisabledImages, enabled: true);
		}
	}

	private void UpdateButton(Button button, Image[] enableImages, bool enabled)
	{
		button.interactable = enabled;
		if (button.TryGetComponent<BasicNavigationItem>(out var component))
		{
			component.enabled = enabled;
		}
		EnableColorAll(enableImages, enabled);
	}

	private bool CheckTypesWithOriginal()
	{
		List<IslandCollection> collection = new List<IslandCollection>(originalValues.types);
		for (int i = 0; i < SandboxGenerator.singleton.allTypes.Count; i++)
		{
			Toggle toggle = typeHandlers.GetChild(i)?.GetComponent<Toggle>();
			if (!toggle || !CheckSingleType(i, toggle, collection))
			{
				return false;
			}
		}
		return true;
	}

	private IslandCollection[] GetSelectedTypes()
	{
		List<IslandCollection> list = new List<IslandCollection>();
		for (int i = 0; i < SandboxGenerator.singleton.allTypes.Count; i++)
		{
			Toggle toggle = typeHandlers.GetChild(i)?.GetComponent<Toggle>();
			if ((bool)toggle && toggle.isOn)
			{
				list.Add(SandboxGenerator.singleton.allTypes[i]);
			}
		}
		return list.ToArray();
	}

	private BiomeColors GetSelectedBiome()
	{
		return SandboxGenerator.singleton.allBiomes[usBiomes.Index];
	}

	private bool CheckBiomeWithOriginal()
	{
		return GetSelectedBiome() == originalValues.biome;
	}

	private IslandCollection GetSelectedSize()
	{
		return SandboxGenerator.singleton.allSizes[usSize.Index];
	}

	private bool CheckSizeWithOriginal()
	{
		return GetSelectedSize() == originalValues.size;
	}

	private SandboxGenerator.AdvancedOptions GetSelectedAdvancedOptions()
	{
		return new SandboxGenerator.AdvancedOptions
		{
			flowerWeight = (int)slFlowers.value,
			treeWeight = (int)slTrees.value,
			Weather = (SandboxGenerator.Weather)usWeather.Index,
			weatherWeight = (int)slWeather.value
		};
	}

	private bool CheckAdvancedWithOriginal()
	{
		if (slFlowers.value != (float)originalValues.advanced.flowerWeight)
		{
			return false;
		}
		if (slTrees.value != (float)originalValues.advanced.treeWeight)
		{
			return false;
		}
		if (usWeather.Index != (int)originalValues.advanced.Weather)
		{
			return false;
		}
		if (slWeather.value != (float)originalValues.advanced.weatherWeight)
		{
			return false;
		}
		return true;
	}

	private bool CheckSingleType(int index, Toggle tgType, List<IslandCollection> collection)
	{
		bool flag = collection.Contains(SandboxGenerator.singleton.allTypes[index]);
		if (!flag || !tgType.isOn)
		{
			if (!flag)
			{
				return !tgType.isOn;
			}
			return false;
		}
		return true;
	}

	private bool IsValidTypeAndSize(IslandCollection[] types, IslandCollection size)
	{
		if (size.prefabs.Length == 0)
		{
			return false;
		}
		List<GameObject> list = new List<GameObject>();
		list.AddRange(size.prefabs);
		for (int i = 0; i < types.Length; i++)
		{
			for (int j = 0; j < types[i].prefabs.Length; j++)
			{
				GameObject item = types[i].prefabs[j];
				if (list.Contains(item))
				{
					return true;
				}
			}
		}
		return false;
	}

	private void EnableAll(MonoBehaviour[] components, bool enable)
	{
		for (int i = 0; i < components.Length; i++)
		{
			components[i].enabled = enable;
		}
	}

	private void EnableColorAll(Image[] images, bool enable)
	{
		foreach (Image image in images)
		{
			if (enable)
			{
				image.color = buttonEnabledColor;
			}
			else
			{
				image.color = buttonDisabledColor;
			}
		}
	}
}
