using I2.Loc;
using UnityEngine;

[CreateAssetMenu(menuName = "Islanders/Biome Colors")]
public class BiomeColors : ScriptableObject
{
	public ushort id;

	public new LocalizedString name;

	public ColorSetup[] colors;
}
