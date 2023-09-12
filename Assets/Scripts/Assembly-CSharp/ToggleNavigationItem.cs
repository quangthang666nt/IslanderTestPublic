using UnityEngine;
using UnityEngine.UI;

public class ToggleNavigationItem : BasicNavigationItem
{
	public Toggle m_Toggle;

	public RectTransform m_TargetRect;

	public override RectTransform RectTransform => m_TargetRect;

	public override void OnSubmit()
	{
		if (m_Toggle != null)
		{
			m_Toggle.OnSubmit(null);
		}
		base.OnSubmit();
	}
}
