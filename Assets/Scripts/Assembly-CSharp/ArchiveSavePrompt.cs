using System;
using System.Collections;
using System.Text.RegularExpressions;
using FlatBuffers;
using I2.Loc;
using Islanders;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArchiveSavePrompt : MonoBehaviour
{
	private const string VALIDATION_EXPR = "^([a-z]|[A-Z]|\\s|[0-9]|[À-ÖØ-öø-ÿ])+$";

	[SerializeField]
	private Image screenshotImage;

	[SerializeField]
	private TMP_Text islandName;

	[SerializeField]
	private TMP_InputField islandNameInput;

	[SerializeField]
	private TMP_Text islandNameLength;

	[SerializeField]
	private TMP_Text islandNameDate;

	[SerializeField]
	private TMP_Text islandNameTime;

	[SerializeField]
	private GameObject islandNameElement;

	[SerializeField]
	private GameObject islandNameEditorElement;

	[SerializeField]
	private GameObject islandInformationElement;

	[SerializeField]
	private GameObject islandInvalidInputElement;

	[SerializeField]
	private ArchivePhotoPrompt archivePhotoPrompt;

	[SerializeField]
	private RectTransform islandTypeProperty;

	[SerializeField]
	private TMP_Text islandType;

	[SerializeField]
	private RectTransform islandBiomeProperty;

	[SerializeField]
	private TMP_Text islandBiome;

	[SerializeField]
	private RectTransform islandSizeProperty;

	[SerializeField]
	private TMP_Text islandSize;

	[SerializeField]
	private LocalizedString localizedIslandName;

	private Regex validationRegex;

	private bool promptEnabled;

	private string currentDatetime = string.Empty;

	private string previousName;

	private SimpleTwoWaysDialog dialogReference;

	private GenericMenuList keyboardNavigation;

	private bool bInConsoleEdition;

	private Texture2D mainTexture;

	private void Start()
	{
		GetComponent<UIElement>().eventOnActivation.AddListener(Setup);
		GetComponent<UIElement>().eventOnDeactivation.AddListener(Deactivate);
		validationRegex = new Regex("^([a-z]|[A-Z]|\\s|[0-9]|[À-ÖØ-öø-ÿ])+$");
		dialogReference = GetComponentInChildren<SimpleTwoWaysDialog>();
		if (!dialogReference)
		{
			Debug.LogError("Dialog Reference is not a child of ArchiveSamePrompt");
		}
		keyboardNavigation = GetComponentInChildren<GenericMenuList>();
		if (!keyboardNavigation)
		{
			Debug.LogError("GenericMenuList Reference is not a child of ArchiveSamePrompt");
		}
	}

	private void Setup()
	{
		if (ArchiveManager.singleton.bWaitingScreenshot)
		{
			StartCoroutine(WaitForScreenshot());
		}
		else
		{
			LoadScreenshot();
		}
		islandName.text = ArchiveManager.singleton.GetIslandDefaultName(GetId());
		islandInformationElement.SetActive(value: true);
		islandInvalidInputElement.SetActive(value: false);
		SetSandboxGenerationData();
		currentDatetime = ArchiveManager.singleton.GetCurrentDateTime();
		DateTime dateFromData = ArchiveManager.singleton.GetDateFromData(currentDatetime);
		islandNameDate.text = PlatformPlayerManagerSystem.Instance.FormatDateWithCultureDatePattern(dateFromData);
		islandNameTime.text = ArchiveManager.singleton.GetHourFromData(currentDatetime);
		promptEnabled = true;
		bInConsoleEdition = false;
	}

	private void SetSandboxGenerationData()
	{
		if (SandboxGenerator.SandboxConfig.playerData)
		{
			islandType.text = SandboxGenerator.singleton.GetIslandTypeLocalized(SandboxGenerator.SandboxConfig.selectedType);
			islandBiome.text = SandboxGenerator.singleton.GetIslandBiomeLocalized(SandboxGenerator.SandboxConfig.biome);
			islandSize.text = SandboxGenerator.singleton.GetIslandSizeLocalized(SandboxGenerator.SandboxConfig.size);
			islandTypeProperty.gameObject.SetActive(value: true);
			islandBiomeProperty.gameObject.SetActive(value: true);
			islandSizeProperty.gameObject.SetActive(value: true);
		}
		else
		{
			islandTypeProperty.gameObject.SetActive(value: false);
			islandBiomeProperty.gameObject.SetActive(value: false);
			islandSizeProperty.gameObject.SetActive(value: false);
		}
	}

	private void Deactivate()
	{
		promptEnabled = false;
		if (mainTexture != null)
		{
			UnityEngine.Object.Destroy(mainTexture);
			screenshotImage.sprite = null;
			screenshotImage.color = Color.black;
		}
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}

	private void Update()
	{
		if (!promptEnabled)
		{
			return;
		}
		if (dialogReference.enabled)
		{
			if (InputManager.Singleton.InputDataCurrent.bUIModify)
			{
				previousName = islandName.text;
				EditIslandName();
			}
			else if (InputManager.Singleton.InputDataCurrent.bUINewScreenshot)
			{
				ToArchivePhotoPrompt();
			}
		}
		else if (InputManager.Singleton.imLastUsedInputMethod == InputManager.InputMode.Controller)
		{
			if (InputManager.Singleton.InputDataCurrent.bUIConfirm)
			{
				EndEdit(islandName.text);
			}
			else if (InputManager.Singleton.InputDataCurrent.bUICancel)
			{
				EndEdit(previousName);
			}
		}
	}

	private IEnumerator WaitForScreenshot()
	{
		while (ArchiveManager.singleton.bWaitingScreenshot)
		{
			yield return null;
		}
		LoadScreenshot();
	}

	private void LoadScreenshot()
	{
		ArchiveManager.LoadIslandGallery(GetId(), OnScreenshotLoaded);
	}

	private void OnScreenshotLoaded(string fileName, LoadResult result, byte[] data)
	{
		if (result == LoadResult.Success)
		{
			ByteBuffer bb = new ByteBuffer(data);
			if (!Islanders.IslandGallery.IslandGalleryBufferHasIdentifier(bb))
			{
				Debug.LogWarning("[ArchiveSavePrompt] Couldn't find identifier in archive buffer!");
				return;
			}
			Islanders.IslandGallery rootAsIslandGallery = Islanders.IslandGallery.GetRootAsIslandGallery(bb);
			IslandGallery islandGallery = new IslandGallery();
			islandGallery.FromFlatBuffer(rootAsIslandGallery);
			if (mainTexture == null)
			{
				mainTexture = new Texture2D(ArchiveManager.GetSsWidth(), ArchiveManager.GetSsHeight());
			}
			mainTexture.LoadImage(islandGallery.highres);
			Sprite sprite = Sprite.Create(mainTexture, new Rect(0f, 0f, mainTexture.width, mainTexture.height), new Vector2(0.5f, 0.5f));
			screenshotImage.sprite = sprite;
			screenshotImage.color = Color.white;
			Debug.Log("[ArchiveSavePrompt] Screenshot loaded!");
		}
		else
		{
			Debug.LogWarning("[ArchiveSavePrompt] Screenshot not loaded!");
		}
	}

	private ushort GetId()
	{
		return ArchiveManager.singleton.GetSandboxAvailableId();
	}

	public void EditIslandName()
	{
		islandNameElement.SetActive(value: false);
		islandNameEditorElement.SetActive(value: true);
		dialogReference.enabled = false;
		keyboardNavigation.enabled = false;
		SetupIslandNameEditor();
		islandNameInput.Select();
		islandNameInput.ActivateInputField();
	}

	private void SetupIslandNameEditor()
	{
		islandNameInput.text = islandName.text;
		islandNameInput.characterLimit = ArchiveManager.singleton.maxIslandNameLength;
		UpdateIslandNameLenght();
		islandNameInput.onValueChanged.AddListener(OnIslandNameChanged);
		islandNameInput.onEndEdit.AddListener(EndEdit);
	}

	private void EndEdit(string value)
	{
		islandNameInput.onValueChanged.RemoveAllListeners();
		islandNameInput.onEndEdit.RemoveAllListeners();
		dialogReference.enabled = true;
		keyboardNavigation.enabled = true;
		islandNameElement.SetActive(value: true);
		islandNameEditorElement.SetActive(value: false);
		islandName.text = value;
	}

	private void UpdateIslandNameLenght()
	{
		islandNameLength.text = GetIslandNameLength();
	}

	private void UpdateFeedback(string newValue)
	{
		UpdateIslandNameLenght();
		ExecValidation(newValue);
	}

	private void OnIslandNameChanged(string newValue)
	{
		UpdateFeedback(newValue);
		islandName.text = islandNameInput.text;
	}

	private bool ExecValidation(string value)
	{
		bool flag = true;
		if (ArchiveManager.singleton.lLanguagesWithValidation.Contains(LocalizationManager.CurrentLanguage))
		{
			flag = IsValidInput(value);
			if (flag)
			{
				islandInformationElement.SetActive(value: true);
				islandInvalidInputElement.SetActive(value: false);
			}
			else
			{
				islandInformationElement.SetActive(value: false);
				islandInvalidInputElement.SetActive(value: true);
			}
		}
		return flag;
	}

	private bool IsValidInput(string input)
	{
		return validationRegex.IsMatch(input);
	}

	private string GetIslandNameLength()
	{
		return $"{islandNameInput.text.Length}/{ArchiveManager.singleton.maxIslandNameLength}";
	}

	public void TryCreateEntry()
	{
		if (ExecValidation(islandName.text))
		{
			ArchiveManager.SaveEntry(null, islandName.text, currentDatetime);
			UiCanvasManager.Singleton.ToMenuWithCurrent();
		}
	}

	public void ToArchiveIsland()
	{
		UiCanvasManager.Singleton.ToArchiveIsland();
	}

	public void ToArchivePhotoPrompt()
	{
		archivePhotoPrompt.SetId(GetId());
		UiCanvasManager.Singleton.ToArchivePhotoPrompt();
	}
}
