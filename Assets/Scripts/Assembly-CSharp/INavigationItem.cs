using UnityEngine;

public interface INavigationItem
{
	bool IsAvailableForNavigation { get; }

	RectTransform RectTransform { get; }

	bool TryCallOnClickFunction();
}
