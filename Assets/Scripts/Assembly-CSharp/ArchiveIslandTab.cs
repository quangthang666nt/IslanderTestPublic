using System;
using FlatBuffers;
using Islanders;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ArchiveIslandTab : MonoBehaviour
{
	[SerializeField]
	private Image previewImage;

	[SerializeField]
	private TMP_Text islandName;

	[SerializeField]
	private TMP_Text metadata;

	private ArchiveIsland island;

	private bool bReady;

	private ArchiveLoadPrompt loadPrompt;

	private Image image;

	private BasicNavigationItem item;

	private Texture2D mainTexture;

	private void Start()
	{
		item = GetComponent<BasicNavigationItem>();
	}

	private void OnDestroy()
	{
		if (mainTexture != null)
		{
			UnityEngine.Object.Destroy(mainTexture);
		}
	}

	private void SetIslandName(string name)
	{
		islandName.text = name;
	}

	private void SetMetadata(string metadata)
	{
		this.metadata.text = metadata;
	}

	public void SetIsland(ArchiveIsland island, ArchiveLoadPrompt archiveLoadPrompt)
	{
		this.island = island;
		LoadScreenshot();
		SetIslandName(island.name);
		DateTime dateFromData = ArchiveManager.singleton.GetDateFromData(island.datetime);
		SetMetadata($"{PlatformPlayerManagerSystem.Instance.FormatDateWithCultureDatePattern(dateFromData)} - {ArchiveManager.singleton.GetHourFromData(island.datetime)}");
		loadPrompt = archiveLoadPrompt;
		bReady = true;
	}

	private void LoadScreenshot()
	{
		ArchiveManager.LoadIslandGallery(island.id, OnScreenshotLoaded);
	}

	private void OnScreenshotLoaded(string fileName, LoadResult result, byte[] data)
	{
		if (result == LoadResult.Success)
		{
			ByteBuffer bb = new ByteBuffer(data);
			if (!Islanders.IslandGallery.IslandGalleryBufferHasIdentifier(bb))
			{
				Debug.LogWarning("[ArchiveIslandTab] Couldn't find identifier in archive buffer!");
				return;
			}
			Islanders.IslandGallery rootAsIslandGallery = Islanders.IslandGallery.GetRootAsIslandGallery(bb);
			IslandGallery islandGallery = new IslandGallery();
			islandGallery.FromFlatBuffer(rootAsIslandGallery);
			if (mainTexture == null)
			{
				mainTexture = new Texture2D(ArchiveManager.GetLowSsWidth(), ArchiveManager.GetLowSsHeight());
			}
			mainTexture.LoadImage(islandGallery.lowres);
			Sprite sprite = Sprite.Create(mainTexture, new Rect(0f, 0f, mainTexture.width, mainTexture.height), new Vector2(0.5f, 0.5f));
			previewImage.sprite = sprite;
			previewImage.color = Color.white;
			Debug.Log("[ArchiveIslandTab] Screenshot loaded!");
		}
		else
		{
			Debug.LogWarning("[ArchiveIslandTab] Screenshot not loaded!");
		}
	}

	public void InteractFromClick()
	{
		if (!item)
		{
			Debug.LogError("BasicNavigationItem reference is null");
		}
		else
		{
			loadPrompt.SelectItem(item);
		}
	}

	public void Interact()
	{
		if (bReady)
		{
			loadPrompt.SelectIsland(island);
		}
	}

	public void SetSelectedColor()
	{
		if (!image)
		{
			image = GetComponent<Image>();
		}
		image.color = loadPrompt.NavSelectedColor;
	}

	public void SetUnselectedColor()
	{
		if (!image)
		{
			image = GetComponent<Image>();
		}
		image.color = loadPrompt.NavUnselectedColor;
	}
}
