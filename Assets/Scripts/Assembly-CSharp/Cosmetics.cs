using System;
using System.Collections.Generic;
using FlatBuffers;
using Islanders;

[Serializable]
public class Cosmetics
{
	public Dictionary<string, string> entries = new Dictionary<string, string>();

	public string theme = "";

	public bool ask;

	public string asked = "";

	public bool themePlaylist;

	public List<string> playlists = new List<string>();

	public bool controllerSpeakerDisabled;

	public bool HasEntries()
	{
		return entries.Count > 0;
	}

	public string GetCosmetic(CosmeticID cosmeticID)
	{
		string result = null;
		if (entries.TryGetValue(cosmeticID.id, out var value))
		{
			result = value;
		}
		return result;
	}

	public void AddCosmetic(CosmeticID cosmeticID, string comestic)
	{
		if (entries.ContainsKey(cosmeticID.id))
		{
			entries[cosmeticID.id] = comestic;
		}
		else
		{
			entries.Add(cosmeticID.id, comestic);
		}
		ClearTheme();
	}

	public void CleanCosmetic(CosmeticID cosmeticID)
	{
		if (entries.ContainsKey(cosmeticID.id))
		{
			entries.Remove(cosmeticID.id);
		}
		ClearTheme();
	}

	public void CleanAllCosmetics()
	{
		entries.Clear();
		ClearTheme();
	}

	private void ClearTheme()
	{
		CosmeticsManager.Cosmetics.theme = "";
	}

	public Offset<Islanders.Cosmetics> ToFlatBuffer(FlatBufferBuilder builder)
	{
		Offset<CosmeticEntry>[] array = new Offset<CosmeticEntry>[entries.Count];
		int num = 0;
		foreach (KeyValuePair<string, string> entry in entries)
		{
			StringOffset keyvalOffset = builder.CreateString(entry.Key);
			StringOffset cosmeticOffset = builder.CreateString(entry.Value);
			CosmeticEntry.StartCosmeticEntry(builder);
			CosmeticEntry.AddKeyval(builder, keyvalOffset);
			CosmeticEntry.AddCosmetic(builder, cosmeticOffset);
			array[num] = CosmeticEntry.EndCosmeticEntry(builder);
			num++;
		}
		VectorOffset entriesOffset = CosmeticEntry.CreateSortedVectorOfCosmeticEntry(builder, array);
		StringOffset themeOffset = builder.CreateString(theme);
		StringOffset askedOffset = builder.CreateString(asked);
		StringOffset[] array2 = new StringOffset[playlists.Count];
		for (int i = 0; i < playlists.Count; i++)
		{
			array2[i] = builder.CreateString(playlists[i]);
		}
		VectorOffset playlistsOffset = Islanders.Cosmetics.CreatePlaylistsVector(builder, array2);
		Islanders.Cosmetics.StartCosmetics(builder);
		Islanders.Cosmetics.AddEntries(builder, entriesOffset);
		Islanders.Cosmetics.AddTheme(builder, themeOffset);
		Islanders.Cosmetics.AddAsk(builder, ask);
		Islanders.Cosmetics.AddAsked(builder, askedOffset);
		Islanders.Cosmetics.AddThemePlaylist(builder, themePlaylist);
		Islanders.Cosmetics.AddPlaylists(builder, playlistsOffset);
		Islanders.Cosmetics.AddControllerSpeakerDisabled(builder, controllerSpeakerDisabled);
		return Islanders.Cosmetics.EndCosmetics(builder);
	}

	public void FromFlatBuffer(Islanders.Cosmetics cosmeticsBuffer)
	{
		entries.Clear();
		for (int i = 0; i < cosmeticsBuffer.EntriesLength; i++)
		{
			CosmeticEntry? cosmeticEntry = cosmeticsBuffer.Entries(i);
			if (cosmeticEntry.HasValue)
			{
				entries.Add(cosmeticEntry.Value.Keyval, cosmeticEntry.Value.Cosmetic);
			}
		}
		if (!string.IsNullOrEmpty(cosmeticsBuffer.Theme))
		{
			theme = cosmeticsBuffer.Theme;
		}
		ask = cosmeticsBuffer.Ask;
		if (!string.IsNullOrEmpty(cosmeticsBuffer.Asked))
		{
			asked = cosmeticsBuffer.Asked;
		}
		themePlaylist = cosmeticsBuffer.ThemePlaylist;
		playlists.Clear();
		for (int j = 0; j < cosmeticsBuffer.PlaylistsLength; j++)
		{
			playlists.Add(cosmeticsBuffer.Playlists(j));
		}
		controllerSpeakerDisabled = cosmeticsBuffer.ControllerSpeakerDisabled;
	}
}
