using I2.Loc;
using UnityEngine;

[CreateAssetMenu(menuName = "Islanders/Island Collection")]
public class IslandCollection : ScriptableObject
{
	public ushort id;

	public new LocalizedString name;

	public GameObject[] prefabs;

	public override string ToString()
	{
		return $"Island {id} name {name} prefabs count {prefabs.Length} collection name {name}";
	}
}
