using UnityEngine;

[CreateAssetMenu(menuName = "Islanders/Island Configuration")]
public class SandboxConfiguration : ScriptableObject
{
	public IslandCollection[] types;

	public BiomeColors biome;

	public IslandCollection size;

	public SandboxGenerator.AdvancedOptions advanced;
}
