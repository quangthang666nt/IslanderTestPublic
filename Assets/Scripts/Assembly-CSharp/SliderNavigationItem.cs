using UnityEngine;
using UnityEngine.UI;

public class SliderNavigationItem : BasicNavigationItem
{
	public Slider m_Slider;

	public RectTransform m_TargetRect;

	public float m_CursorMoveSpeed = 1f;

	public float m_ThresholdToMove = 0.5f;

	public bool intSlider;

	private bool m_JustChangedValue;

	public override RectTransform RectTransform => m_TargetRect;

	public override void SendInput(Vector2 moveAxis)
	{
		if (m_Slider == null)
		{
			return;
		}
		float x = moveAxis.x;
		if ((!intSlider || !m_JustChangedValue) && Mathf.Abs(x) > m_ThresholdToMove)
		{
			if (intSlider)
			{
				m_JustChangedValue = true;
				if (x > 0f)
				{
					m_Slider.value += 1f;
				}
				else
				{
					m_Slider.value -= 1f;
				}
			}
			else
			{
				m_Slider.value += x * Time.deltaTime * m_CursorMoveSpeed;
			}
		}
		else if (intSlider && Mathf.Abs(x) < 0.3f)
		{
			m_JustChangedValue = false;
		}
	}
}
