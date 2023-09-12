using System;

[Serializable]
public class ArchiveIsland
{
	public ushort id;

	public string slot = "";

	public string name = "";

	public string datetime = "";

	public string screenshot = "";

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

	public bool matchTutorialDone;

	public ushort matchBuildsPlaced;
}
