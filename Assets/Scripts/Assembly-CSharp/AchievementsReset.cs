using UnityEngine;

public class AchievementsReset : MonoBehaviour
{
	[SerializeField]
	private KeyCode key = KeyCode.F11;

	private void Start()
	{
	}

	private void Update()
	{
		if (Input.GetKeyDown(key))
		{
			SteamManagerMenu.ClearSteamWorksArchivemments();
		}
	}
}
