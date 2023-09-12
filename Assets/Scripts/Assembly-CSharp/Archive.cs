using System;
using System.Collections.Generic;
using FlatBuffers;
using Islanders;

[Serializable]
public class Archive
{
	public ushort current;

	public bool currentUpdated;

	public Dictionary<ushort, ArchiveIsland> sandbox = new Dictionary<ushort, ArchiveIsland>();

	public Offset<Islanders.Archive> ToFlatBuffer(FlatBufferBuilder builder)
	{
		Offset<ArchiveEntry>[] array = new Offset<ArchiveEntry>[sandbox.Count];
		int num = 0;
		foreach (KeyValuePair<ushort, ArchiveIsland> item in sandbox)
		{
			array[num] = CreateArchiveEntry(builder, item.Value);
			num++;
		}
		VectorOffset sandboxOffset = ArchiveEntry.CreateSortedVectorOfArchiveEntry(builder, array);
		Islanders.Archive.StartArchive(builder);
		Islanders.Archive.AddCurrent(builder, current);
		Islanders.Archive.AddCurrentUpdated(builder, currentUpdated);
		Islanders.Archive.AddSandbox(builder, sandboxOffset);
		return Islanders.Archive.EndArchive(builder);
	}

	public void FromFlatBuffer(Islanders.Archive archiveBuffer)
	{
		sandbox.Clear();
		current = archiveBuffer.Current;
		currentUpdated = archiveBuffer.CurrentUpdated;
		for (int i = 0; i < archiveBuffer.SandboxLength; i++)
		{
			ArchiveEntry? entry = archiveBuffer.Sandbox(i);
			if (entry.HasValue)
			{
				sandbox.Add(entry.Value.Id, CreateArchiveIsland(entry));
			}
		}
	}

	private Offset<ArchiveEntry> CreateArchiveEntry(FlatBufferBuilder builder, ArchiveIsland island)
	{
		StringOffset slotOffset = builder.CreateString(island.slot);
		StringOffset nameOffset = builder.CreateString(island.name);
		StringOffset datetimeOffset = builder.CreateString(island.datetime);
		StringOffset screenshotOffset = builder.CreateString(island.screenshot);
		VectorOffset typesOffset = Islanders.SandboxConfig.CreateTypesVector(builder, island.types);
		ArchiveEntry.StartArchiveEntry(builder);
		ArchiveEntry.AddId(builder, island.id);
		ArchiveEntry.AddSlot(builder, slotOffset);
		ArchiveEntry.AddName(builder, nameOffset);
		ArchiveEntry.AddDatetime(builder, datetimeOffset);
		ArchiveEntry.AddScreenshot(builder, screenshotOffset);
		ArchiveEntry.AddTypes(builder, typesOffset);
		ArchiveEntry.AddBiome(builder, island.biome);
		ArchiveEntry.AddSize(builder, island.size);
		ArchiveEntry.AddFlowerWeight(builder, island.flowerWeight);
		ArchiveEntry.AddTreeWeight(builder, island.treeWeight);
		ArchiveEntry.AddWeather(builder, island.weather);
		ArchiveEntry.AddWeatherWeight(builder, island.weatherWeight);
		ArchiveEntry.AddSelectedType(builder, island.selectedType);
		ArchiveEntry.AddSelectedIndex(builder, island.selectedIndex);
		ArchiveEntry.AddPlayerData(builder, island.playerData);
		ArchiveEntry.AddMatchTutorialDone(builder, island.matchTutorialDone);
		ArchiveEntry.AddMatchBuildsPlaced(builder, island.matchBuildsPlaced);
		return ArchiveEntry.EndArchiveEntry(builder);
	}

	private ArchiveIsland CreateArchiveIsland(ArchiveEntry? entry)
	{
		ArchiveIsland archiveIsland = new ArchiveIsland();
		archiveIsland.id = entry.Value.Id;
		if (!string.IsNullOrEmpty(entry.Value.Slot))
		{
			archiveIsland.slot = entry.Value.Slot;
		}
		if (!string.IsNullOrEmpty(entry.Value.Name))
		{
			archiveIsland.name = entry.Value.Name;
		}
		archiveIsland.datetime = entry.Value.Datetime;
		if (!string.IsNullOrEmpty(entry.Value.Screenshot))
		{
			archiveIsland.screenshot = entry.Value.Screenshot;
		}
		archiveIsland.types = new ushort[entry.Value.TypesLength];
		for (int i = 0; i < entry.Value.TypesLength; i++)
		{
			archiveIsland.types[i] = entry.Value.Types(i);
		}
		archiveIsland.biome = entry.Value.Biome;
		archiveIsland.size = entry.Value.Size;
		archiveIsland.flowerWeight = entry.Value.FlowerWeight;
		archiveIsland.treeWeight = entry.Value.TreeWeight;
		archiveIsland.weather = entry.Value.Weather;
		archiveIsland.weatherWeight = entry.Value.WeatherWeight;
		archiveIsland.selectedType = entry.Value.SelectedType;
		archiveIsland.selectedIndex = entry.Value.SelectedIndex;
		archiveIsland.playerData = entry.Value.PlayerData;
		archiveIsland.matchTutorialDone = entry.Value.MatchTutorialDone;
		archiveIsland.matchBuildsPlaced = entry.Value.MatchBuildsPlaced;
		return archiveIsland;
	}
}
