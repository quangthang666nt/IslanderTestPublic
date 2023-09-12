using UnityEngine;
using UnityEngine.UI;

public class ButtonNavigationItem : BasicNavigationItem
{
	public Button m_Button;

	public RectTransform m_TargetRect;

	public override RectTransform RectTransform => m_TargetRect;

	public override void OnSubmit()
	{
		if (m_Button != null)
		{
			m_Button.OnSubmit(null);
		}
		base.OnSubmit();
	}
}
