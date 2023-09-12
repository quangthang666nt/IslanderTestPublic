using UnityEngine;
using UnityEngine.UI;

public class SelectorNavigationItem : BasicNavigationItem
{
	public RectTransform m_TargetRect;

	public float m_ChangeThreshold = 0.9f;

	public Button m_LeftButton;

	public Button m_RightButton;

	private bool m_JustChangedValue;

	public override RectTransform RectTransform => m_TargetRect;

	public override void SendInput(Vector2 moveAxis)
	{
		float x = moveAxis.x;
		if (!m_JustChangedValue)
		{
			if (!(Mathf.Abs(x) > m_ChangeThreshold))
			{
				return;
			}
			if (x > 0f)
			{
				if (m_RightButton != null)
				{
					m_RightButton.OnSubmit(null);
				}
			}
			else if (m_LeftButton != null)
			{
				m_LeftButton.OnSubmit(null);
			}
			m_JustChangedValue = true;
		}
		else if (Mathf.Abs(x) < 0.3f)
		{
			m_JustChangedValue = false;
		}
	}
}
