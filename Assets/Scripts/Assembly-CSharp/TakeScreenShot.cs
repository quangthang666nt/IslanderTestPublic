using UnityEngine;

public class TakeScreenShot : MonoBehaviour
{
	public ushort id = 99;

	public void Take()
	{
		ArchiveManager.TakeScreenShot(id);
	}
}
