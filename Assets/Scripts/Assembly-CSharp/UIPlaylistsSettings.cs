using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPlaylistsSettings : MonoBehaviour
{
	[SerializeField]
	private RectTransform labelsLeft;

	[SerializeField]
	private RectTransform handlersLeft;

	[SerializeField]
	private RectTransform labelsRight;

	[SerializeField]
	private RectTransform handlersRight;

	[SerializeField]
	private GameObject labelPrefab;

	[SerializeField]
	private GameObject handlerPrefab;

	[SerializeField]
	private UISettingsMenu settingsMenu;

	private int constructOrder = -1;

	private int leftChildNumberCount = -1;

	private int rightChildNumberCount = -1;

	private bool dynamicContentCreated;

	private List<string> lLocalPlaylists = new List<string>();

	private bool interacted;

	private void OnEnable()
	{
		if (CosmeticsReady())
		{
			Setup();
		}
		else
		{
			StartCoroutine(WaitForCosmetics());
		}
	}

	private void OnDisable()
	{
		StopAllCoroutines();
		CompareChangesInPlaylistSelection();
	}

	private IEnumerator WaitForCosmetics()
	{
		while (!CosmeticsReady())
		{
			yield return null;
		}
	}

	private bool CosmeticsReady()
	{
		if ((bool)CosmeticsManager.singleton && CosmeticsManager.singleton.IsConfigLoaded())
		{
			return CosmeticsManager.singleton.bDataLoaded;
		}
		return false;
	}

	private void Setup()
	{
		if (!dynamicContentCreated)
		{
			constructOrder = -1;
			CreateLabelHandler(AudioManager.singleton.GetOriginalPlaylistName(), AudioManager.singleton.GetOriginalPlaylistID());
			if (CosmeticsManager.singleton.CatalogConfig.extrasPlaylists != null)
			{
				for (int i = 0; i < CosmeticsManager.singleton.CatalogConfig.extrasPlaylists.Length; i++)
				{
					CreateLabelHandler(CosmeticsManager.singleton.CatalogConfig.extrasPlaylists[i].localizedName, CosmeticsManager.singleton.CatalogConfig.extrasPlaylists[i].id);
				}
			}
			List<CatalogConfig.PackageData> allAvailablePackagesData = CosmeticsManager.singleton.CatalogConfig.GetAllAvailablePackagesData();
			for (int j = 0; j < allAvailablePackagesData.Count; j++)
			{
				CreateLabelHandler(allAvailablePackagesData[j].localizedName, allAvailablePackagesData[j].id);
			}
			StartCoroutine(ResizeMenu());
			dynamicContentCreated = true;
		}
		else
		{
			constructOrder = -1;
			leftChildNumberCount = -1;
			rightChildNumberCount = -1;
			UpdateLabelText(AudioManager.singleton.GetOriginalPlaylistName());
			if (CosmeticsManager.singleton.CatalogConfig.extrasPlaylists != null)
			{
				for (int k = 0; k < CosmeticsManager.singleton.CatalogConfig.extrasPlaylists.Length; k++)
				{
					UpdateLabelText(CosmeticsManager.singleton.CatalogConfig.extrasPlaylists[k].localizedName);
				}
			}
			List<CatalogConfig.PackageData> allAvailablePackagesData2 = CosmeticsManager.singleton.CatalogConfig.GetAllAvailablePackagesData();
			for (int l = 0; l < allAvailablePackagesData2.Count; l++)
			{
				UpdateLabelText(allAvailablePackagesData2[l].localizedName);
			}
		}
		lLocalPlaylists = new List<string>(CosmeticsManager.Cosmetics.playlists);
		if (lLocalPlaylists.Count == 0 && handlersLeft.childCount > 0)
		{
			handlersLeft.GetChild(0).GetComponent<ToggleNavigationItem>().m_Toggle.isOn = true;
		}
		interacted = false;
	}

	private void CreateLabelHandler(string name, string id)
	{
		constructOrder++;
		Transform parent;
		Transform parent2;
		if (constructOrder % 2 == 0)
		{
			parent = labelsLeft;
			parent2 = handlersLeft;
		}
		else
		{
			parent = labelsRight;
			parent2 = handlersRight;
		}
		GameObject gameObject = Object.Instantiate(labelPrefab, parent);
		GameObject gameObject2 = Object.Instantiate(handlerPrefab, parent2);
		gameObject.GetComponent<TMP_Text>().text = name;
		UICombinedElement component = gameObject.GetComponent<UICombinedElement>();
		component.elements.Clear();
		component.elements.Add(gameObject.GetComponent<RectTransform>());
		component.elements.Add(gameObject2.GetComponent<RectTransform>());
		UISandboxTypeLabel component2 = gameObject.GetComponent<UISandboxTypeLabel>();
		gameObject2.GetComponent<UISandboxTypeHandler>().cursor = component2.cursor;
		ToggleNavigationItem component3 = gameObject2.GetComponent<ToggleNavigationItem>();
		component3.m_TargetRect = gameObject.GetComponent<RectTransform>();
		component3.OnUnselect();
		settingsMenu.m_NavigationItems.Add(component3);
		PlaylistElement component4 = gameObject2.GetComponent<PlaylistElement>();
		component4.id = id;
		component4.settings = this;
		component3.m_Toggle.isOn = CosmeticsManager.Cosmetics.playlists.Contains(id);
	}

	private void UpdateLabelText(string name)
	{
		constructOrder++;
		Transform transform;
		int num;
		Transform transform2;
		if (constructOrder % 2 == 0)
		{
			transform = labelsLeft;
			leftChildNumberCount++;
			num = leftChildNumberCount;
			transform2 = handlersLeft;
		}
		else
		{
			transform = labelsRight;
			rightChildNumberCount++;
			num = rightChildNumberCount;
			transform2 = handlersRight;
		}
		if (num < transform.childCount)
		{
			transform.GetChild(num).GetComponent<TMP_Text>().text = name;
		}
		else
		{
			Debug.LogError("Cant update child label text");
		}
		if (num < transform2.childCount)
		{
			Toggle component = transform2.GetChild(num).GetComponent<Toggle>();
			string id = transform2.GetChild(num).GetComponent<PlaylistElement>().id;
			component.isOn = CosmeticsManager.Cosmetics.playlists.Contains(id);
		}
		else
		{
			Debug.LogError("Cant update child toggle value");
		}
	}

	private IEnumerator ResizeMenu()
	{
		yield return null;
		settingsMenu.ForceResize();
		settingsMenu.ToAudio();
	}

	public void ToggleInteracted(bool isOn, string id)
	{
		if (isOn && !lLocalPlaylists.Contains(id))
		{
			lLocalPlaylists.Add(id);
		}
		else if (!isOn && lLocalPlaylists.Contains(id))
		{
			lLocalPlaylists.Remove(id);
		}
		interacted = true;
	}

	private void CompareChangesInPlaylistSelection()
	{
		if (!interacted || (CosmeticsManager.Cosmetics.playlists.Count == 0 && lLocalPlaylists.Count == 1 && lLocalPlaylists[0].Equals(AudioManager.singleton.GetOriginalPlaylistID())))
		{
			return;
		}
		bool flag = lLocalPlaylists.Count != CosmeticsManager.Cosmetics.playlists.Count;
		if (!flag)
		{
			for (int i = 0; i < CosmeticsManager.Cosmetics.playlists.Count; i++)
			{
				if (!lLocalPlaylists.Contains(CosmeticsManager.Cosmetics.playlists[i]))
				{
					flag = true;
					break;
				}
			}
		}
		if (flag)
		{
			CosmeticsManager.Cosmetics.playlists = lLocalPlaylists;
			CosmeticsManager.Save();
			MusicPlaylistController component = AudioManager.singleton.GetComponent<MusicPlaylistController>();
			if ((bool)component)
			{
				component.CheckActivePlaylist();
			}
		}
	}
}
