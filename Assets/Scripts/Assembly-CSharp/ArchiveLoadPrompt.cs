using System.Collections.Generic;
using FlatBuffers;
using Islanders;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArchiveLoadPrompt : MonoBehaviour
{
	[SerializeField]
	private GameObject viewportContent;

	[SerializeField]
	private GameObject elementPrefab;

	[SerializeField]
	private GameObject emptyPrefab;

	[SerializeField]
	private GameObject selectedIslandElement;

	[SerializeField]
	private TMP_Text selectedIslandName;

	[SerializeField]
	private Image selectedIslandImage;

	[SerializeField]
	private ArchiveDeleteConfirmation deleteConfirmation;

	[SerializeField]
	private ArchiveIslandLoseConfirmation highscoreLoseConfirmation;

	[SerializeField]
	private ArchiveIslandLoseConfirmation sandboxLoseConfirmation;

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
	private Image loadButtonImg;

	[SerializeField]
	private Image deleteButtonImg;

	[SerializeField]
	private bool isLoadPrompt = true;

	[SerializeField]
	private GenericMenuList navigation;

	[SerializeField]
	public Color NavUnselectedColor;

	[SerializeField]
	public Color NavSelectedColor;

	[SerializeField]
	public Color ButtonUnselectedColor;

	public RectTransform scrollRectRectTransform;

	public RectTransform contentRectTransform;

	private GameObject lastSelected;

	private ArchiveIsland selectedIsland;

	private bool bEnabled;

	private Texture2D mainTexture;

	private void Start()
	{
		GetComponent<UIElement>().eventOnActivation.AddListener(Setup);
		GetComponent<UIElement>().eventOnDeactivation.AddListener(Deactivate);
		navigation.enabled = false;
	}

	private void Update()
	{
		if (!bEnabled)
		{
			return;
		}
		if (isLoadPrompt)
		{
			if (InputManager.Singleton.InputDataCurrent.bUIConfirm)
			{
				if (AudioManager.singleton != null)
				{
					AudioManager.singleton.PlayMenuClick();
				}
				LoadSlotSelect();
			}
			else if (InputManager.Singleton.InputDataCurrent.bUIModify)
			{
				if (AudioManager.singleton != null)
				{
					AudioManager.singleton.PlayMenuClick();
				}
				DeleteSlotSelected();
			}
		}
		else if (InputManager.Singleton.InputDataCurrent.bUIConfirm)
		{
			if (AudioManager.singleton != null)
			{
				AudioManager.singleton.PlayMenuClick();
			}
			DeleteSlotSelected();
		}
		if (InputManager.Singleton.InputDataCurrent.bUICancel)
		{
			UiCanvasManager.Singleton.BlockInput(0.1f);
			if (AudioManager.singleton != null)
			{
				AudioManager.singleton.PlayMenuClick();
			}
			ToArchiveIsland();
		}
	}

	private void Setup()
	{
		DeleteAllChilds();
		int num = 0;
		ArchiveIsland archiveIsland = null;
		foreach (KeyValuePair<ushort, ArchiveIsland> item in ArchiveManager.Archive.sandbox)
		{
			GameObject gameObject = Object.Instantiate(elementPrefab, viewportContent.transform);
			ArchiveIslandTab component = gameObject.GetComponent<ArchiveIslandTab>();
			if ((bool)component)
			{
				component.SetIsland(item.Value, this);
				if (component.TryGetComponent<BasicNavigationItem>(out var component2))
				{
					navigation.m_NavigationItems.Add(component2);
				}
				else
				{
					Debug.LogError("IslandTab without BasicNavigationItem");
				}
			}
			if (num == 0)
			{
				archiveIsland = item.Value;
				gameObject.GetComponent<Button>().Select();
			}
			num++;
		}
		for (int i = num; i < ArchiveManager.singleton.maxSlots; i++)
		{
			GameObject gameObject2 = Object.Instantiate(emptyPrefab, viewportContent.transform);
			ArchiveIslandTabEmpty component3 = gameObject2.GetComponent<ArchiveIslandTabEmpty>();
			if ((bool)component3)
			{
				component3.SetPrompt(this);
			}
			if (component3.TryGetComponent<BasicNavigationItem>(out var component4))
			{
				navigation.m_NavigationItems.Add(component4);
			}
			else
			{
				Debug.LogError("emptyTab without BasicNavigationItem");
			}
			if (archiveIsland == null)
			{
				gameObject2.GetComponent<Button>().Select();
			}
		}
		if (archiveIsland != null)
		{
			SelectIsland(archiveIsland);
		}
		else
		{
			SelectEmpty();
		}
		navigation.enabled = true;
		bEnabled = true;
	}

	private void Deactivate()
	{
		bEnabled = false;
		navigation.m_NavigationItems.Clear();
		navigation.enabled = false;
		selectedIsland = null;
		DeleteAllChilds();
		if (mainTexture != null)
		{
			Object.Destroy(mainTexture);
			selectedIslandImage.sprite = null;
			selectedIslandImage.color = Color.black;
		}
	}

	private void DeleteAllChilds()
	{
		foreach (UnityEngine.Transform item in viewportContent.transform)
		{
			Object.Destroy(item.gameObject);
		}
	}

	public void SelectIsland(ArchiveIsland island)
	{
		if (selectedIsland != island)
		{
			selectedIsland = island;
			selectedIslandElement.SetActive(value: true);
			selectedIslandName.text = island.name;
			SetSandboxGenerationData(island);
			LoadScreenshot();
			UpdateActionsButton(enabled: true);
		}
	}

	public void SelectItem(BasicNavigationItem item)
	{
		navigation.Select(item);
	}

	private void UpdateActionsButton(bool enabled)
	{
		Color color = Color.white;
		if (!enabled)
		{
			color = ButtonUnselectedColor;
		}
		Image[] componentsInChildren = loadButtonImg.GetComponentsInChildren<Image>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].color = color;
		}
		componentsInChildren = deleteButtonImg.GetComponentsInChildren<Image>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].color = color;
		}
	}

	private void SetSandboxGenerationData(ArchiveIsland island)
	{
		if (island.playerData)
		{
			islandType.text = SandboxGenerator.singleton.GetIslandTypeLocalized(island.selectedType);
			islandBiome.text = SandboxGenerator.singleton.GetIslandBiomeLocalized(island.biome);
			islandSize.text = SandboxGenerator.singleton.GetIslandSizeLocalized(island.size);
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

	public void SelectEmpty()
	{
		selectedIsland = null;
		selectedIslandElement.SetActive(value: false);
		UpdateActionsButton(enabled: false);
	}

	private void LoadScreenshot()
	{
		ArchiveManager.LoadIslandGallery(selectedIsland.id, OnScreenshotLoaded);
	}

	private void OnScreenshotLoaded(string fileName, LoadResult result, byte[] data)
	{
		if (result == LoadResult.Success)
		{
			ByteBuffer bb = new ByteBuffer(data);
			if (!Islanders.IslandGallery.IslandGalleryBufferHasIdentifier(bb))
			{
				Debug.LogWarning("[ArchiveLoadPrompt] Couldn't find identifier in archive buffer!");
				return;
			}
			Islanders.IslandGallery rootAsIslandGallery = Islanders.IslandGallery.GetRootAsIslandGallery(bb);
			IslandGallery islandGallery = new IslandGallery();
			islandGallery.FromFlatBuffer(rootAsIslandGallery);
			LoadMainScreenshot(islandGallery);
			Debug.Log("[ArchiveLoadPrompt] Screenshot loaded!");
		}
		else
		{
			Debug.LogWarning("[ArchiveLoadPrompt] Screenshot not loaded!");
			selectedIslandImage.sprite = null;
			selectedIslandImage.color = Color.black;
		}
	}

	public void LoadMainScreenshot(IslandGallery gallery)
	{
		if (mainTexture == null)
		{
			mainTexture = new Texture2D(ArchiveManager.GetSsWidth(), ArchiveManager.GetSsHeight());
		}
		mainTexture.LoadImage(gallery.highres);
		Sprite sprite = Sprite.Create(mainTexture, new Rect(0f, 0f, mainTexture.width, mainTexture.height), new Vector2(0.5f, 0.5f));
		selectedIslandImage.sprite = sprite;
		selectedIslandImage.color = Color.white;
	}

	public bool IsTabIsSelected(ArchiveIsland tab)
	{
		return selectedIsland == tab;
	}

	public void LoadSlotSelect()
	{
		if (selectedIsland != null)
		{
			if (LocalGameManager.singleton.GameMode == LocalGameManager.EGameMode.Default)
			{
				highscoreLoseConfirmation.SetNextIslandId(selectedIsland.id);
				UiCanvasManager.Singleton.ToArchiveIslandLoseHighscore();
			}
			else if (!ArchiveManager.HasCurrent() || !ArchiveManager.Archive.currentUpdated)
			{
				sandboxLoseConfirmation.SetNextIslandId(selectedIsland.id);
				UiCanvasManager.Singleton.ToArchiveIslandLoseSandbox();
			}
			else
			{
				ArchiveManager.LoadEntry(selectedIsland.id);
			}
		}
	}

	public void DeleteSlotSelected()
	{
		if (selectedIsland != null)
		{
			deleteConfirmation.SetArchiveIslandId(selectedIsland.id, isLoadPrompt);
			UiCanvasManager.Singleton.ToArchiveIslandDeleteConfirmation();
		}
	}

	public void ToArchiveIsland()
	{
		UiCanvasManager.Singleton.ToArchiveIsland();
	}
}
