using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ArchiveHelperTests : MonoBehaviour
{
	public TMP_Dropdown dropdown;

	private Dictionary<string, ArchiveIsland> archiveLoaded = new Dictionary<string, ArchiveIsland>();

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void LoadArchive()
	{
		dropdown.ClearOptions();
		archiveLoaded.Clear();
		List<TMP_Dropdown.OptionData> list = new List<TMP_Dropdown.OptionData>();
		foreach (KeyValuePair<ushort, ArchiveIsland> item in ArchiveManager.Archive.sandbox)
		{
			TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData();
			optionData.text = item.Value.name;
			list.Add(optionData);
			archiveLoaded.Add(item.Value.name, item.Value);
		}
		dropdown.options = list;
	}

	public void LoadSlotSelect()
	{
		if (dropdown.options.Count != 0)
		{
			ArchiveManager.LoadEntry(archiveLoaded[dropdown.options[dropdown.value].text].id);
		}
	}

	public void DeleteSelected()
	{
		if (dropdown.options.Count != 0)
		{
			ArchiveManager.DeleteEntry(archiveLoaded[dropdown.options[dropdown.value].text].id);
		}
	}

	public void SaveCurrentSandbox()
	{
		if (LocalGameManager.singleton.GameMode == LocalGameManager.EGameMode.Sandbox)
		{
			ArchiveManager.SaveEntry();
		}
	}
}
