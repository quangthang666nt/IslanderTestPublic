using UnityEngine;

public class UIThemedEventsChecker : MonoBehaviour
{
	private static bool askCheked;

	private void Update()
	{
		if (!askCheked && CosmeticsManager.singleton.IsConfigLoaded() && CosmeticsManager.singleton.bDataLoaded)
		{
			CheckAsk();
		}
	}

	private void CheckAsk()
	{
		if (CosmeticsManager.Cosmetics.ask)
		{
			UiCanvasManager.Singleton.ToNewThemeEvent();
		}
		askCheked = true;
	}
}
