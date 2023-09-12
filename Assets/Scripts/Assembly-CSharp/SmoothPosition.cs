using System;
using UnityEngine;

[Serializable]
public class SmoothPosition
{
	public Vector3 m_Curr;

	public Vector3 m_Target;

	public Vector3 m_Vel;

	public float m_SlideRate;

	public bool m_IsDone;

	public bool IsDone => m_IsDone;

	public Vector3 Value
	{
		get
		{
			return m_Curr;
		}
		set
		{
			if ((double)(value - m_Curr).magnitude > 1E-06)
			{
				m_Target = value;
				m_IsDone = false;
				return;
			}
			m_Curr = value;
			m_Target = value;
			m_IsDone = true;
			m_Vel = new Vector3(0f, 0f, 0f);
		}
	}

	public bool IsCloseEnough(float distance)
	{
		return Vector3.Distance(m_Curr, m_Target) < distance;
	}

	public SmoothPosition(float slideRate)
	{
		m_SlideRate = slideRate;
	}

	public SmoothPosition()
	{
		m_SlideRate = 2f;
	}

	public void SetTrackingRate(float slideRate)
	{
		m_SlideRate = slideRate;
	}

	public void Update(float timestep)
	{
		if (!m_IsDone && timestep > 0f)
		{
			m_Curr = SmoothPositionCD(m_Curr, m_Target, ref m_Vel, m_SlideRate, timestep);
			if ((m_Target - m_Curr).magnitude < 0.001f)
			{
				m_Curr = m_Target;
				m_Vel = new Vector3(0f, 0f, 0f);
				m_IsDone = true;
			}
		}
	}

	public void SetNow(Vector3 val)
	{
		m_Curr = val;
		m_Target = val;
		m_IsDone = true;
		m_Vel = new Vector3(0f, 0f, 0f);
	}

	private Vector3 SmoothPositionCD(Vector3 from, Vector3 to, ref Vector3 vel, float smoothTime, float timestep)
	{
		float num = smoothTime * 2f;
		float num2 = num * timestep;
		float num3 = 1f / (1f + num2 + num2 * num2 * 0.48f + num2 * num2 * num2 * 0.235f);
		Vector3 vector = from - to;
		Vector3 vector2 = (vel + num * vector) * timestep;
		vel = (vel - num * vector2) * num3;
		return to + (vector + vector2) * num3;
	}
}
