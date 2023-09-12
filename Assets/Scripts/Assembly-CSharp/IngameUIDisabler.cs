using System.Collections.Generic;
using UnityEngine;

public class IngameUIDisabler : MonoBehaviour
{
	private static List<IngameUIDisabler> liIngameUiDisabler = new List<IngameUIDisabler>();

	private void Awake()
	{
		if (!liIngameUiDisabler.Contains(this))
		{
			liIngameUiDisabler.Add(this);
		}
	}

	private void OnDestroy()
	{
		liIngameUiDisabler.Remove(this);
	}

	public static void Enable()
	{
		foreach (IngameUIDisabler item in liIngameUiDisabler)
		{
			item.gameObject.SetActive(value: true);
		}
	}

	public static void Disable()
	{
		foreach (IngameUIDisabler item in liIngameUiDisabler)
		{
			item.gameObject.SetActive(value: false);
		}
	}
}
