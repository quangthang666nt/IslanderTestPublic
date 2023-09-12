using System;
using FlatBuffers;
using Islanders;

[Serializable]
public class SandboxConfig
{
	public ushort[] types = new ushort[0];

	public ushort biome;

	public ushort size;

	public ushort flowerWeight;

	public ushort treeWeight;

	public ushort weather;

	public ushort weatherWeight;

	public ushort selectedType;

	public ushort selectedIndex;

	public bool playerData;

	public bool globalTutorialDone;

	public bool matchTutorialDone;

	public ushort matchBuildsPlaced;

	public Offset<Islanders.SandboxConfig> ToFlatBuffer(FlatBufferBuilder builder)
	{
		VectorOffset typesOffset = Islanders.SandboxConfig.CreateTypesVector(builder, types);
		Islanders.SandboxConfig.StartSandboxConfig(builder);
		Islanders.SandboxConfig.AddTypes(builder, typesOffset);
		Islanders.SandboxConfig.AddBiome(builder, biome);
		Islanders.SandboxConfig.AddSize(builder, size);
		Islanders.SandboxConfig.AddFlowerWeight(builder, flowerWeight);
		Islanders.SandboxConfig.AddTreeWeight(builder, treeWeight);
		Islanders.SandboxConfig.AddWeather(builder, weather);
		Islanders.SandboxConfig.AddWeatherWeight(builder, weatherWeight);
		Islanders.SandboxConfig.AddSelectedType(builder, selectedType);
		Islanders.SandboxConfig.AddSelectedIndex(builder, selectedIndex);
		Islanders.SandboxConfig.AddPlayerData(builder, playerData);
		Islanders.SandboxConfig.AddGlobalTutorialDone(builder, globalTutorialDone);
		Islanders.SandboxConfig.AddMatchTutorialDone(builder, matchTutorialDone);
		Islanders.SandboxConfig.AddMatchBuildsPlaced(builder, matchBuildsPlaced);
		return Islanders.SandboxConfig.EndSandboxConfig(builder);
	}

	public void FromFlatBuffer(Islanders.SandboxConfig sandboxConfigBuffer)
	{
		types = new ushort[sandboxConfigBuffer.TypesLength];
		for (int i = 0; i < sandboxConfigBuffer.TypesLength; i++)
		{
			types[i] = sandboxConfigBuffer.Types(i);
		}
		biome = sandboxConfigBuffer.Biome;
		size = sandboxConfigBuffer.Size;
		flowerWeight = sandboxConfigBuffer.FlowerWeight;
		treeWeight = sandboxConfigBuffer.TreeWeight;
		weather = sandboxConfigBuffer.Weather;
		weatherWeight = sandboxConfigBuffer.WeatherWeight;
		selectedType = sandboxConfigBuffer.SelectedType;
		selectedIndex = sandboxConfigBuffer.SelectedIndex;
		playerData = sandboxConfigBuffer.PlayerData;
		globalTutorialDone = sandboxConfigBuffer.GlobalTutorialDone;
		matchTutorialDone = sandboxConfigBuffer.MatchTutorialDone;
		matchBuildsPlaced = sandboxConfigBuffer.MatchBuildsPlaced;
	}
}
