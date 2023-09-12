using System;
using UnityEngine;

[Serializable]
public class SmoothFloat
{
	private float m_Curr;

	private float m_Target;

	private float m_Vel;

	private float m_SlideRate;

	private bool m_IsDone;

	private float m_GapLine;

	public float Target => m_Target;

	public float GapLine => m_GapLine;

	public bool IsDone => m_IsDone;

	public float Value
	{
		get
		{
			return m_Curr;
		}
		set
		{
			float num = value - m_Curr;
			if (num * num > 1E-05f)
			{
				m_Target = value;
				m_IsDone = false;
				m_GapLine = Mathf.Abs(num);
			}
			else
			{
				m_Curr = value;
				m_Target = value;
				m_IsDone = true;
				m_GapLine = 0f;
				m_Vel = 0f;
			}
		}
	}

	public float NextValue
	{
		get
		{
			float fixedDeltaTime = Time.fixedDeltaTime;
			if (fixedDeltaTime > 0f)
			{
				float vel = m_Vel;
				return SmoothFloatCD(m_Curr, m_Target, ref vel, m_SlideRate, fixedDeltaTime);
			}
			return m_Curr;
		}
	}

	public SmoothFloat(float trackingRate, float initialValue = 0f)
	{
		m_SlideRate = trackingRate;
		SetNow(initialValue);
	}

	public static implicit operator float(SmoothFloat val)
	{
		return val.Value;
	}

	public SmoothFloat()
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
			m_Curr = SmoothFloatCD(m_Curr, m_Target, ref m_Vel, m_SlideRate, timestep);
			m_GapLine = Math.Abs(m_Target - m_Curr);
			if (m_GapLine < 0.0001f)
			{
				m_Curr = m_Target;
				m_Vel = 0f;
				m_IsDone = true;
			}
		}
	}

	public void SetNow(float val)
	{
		m_Curr = val;
		m_Target = val;
		m_IsDone = true;
		m_Vel = 0f;
		m_GapLine = 0f;
	}

	private float SmoothFloatCD(float from, float to, ref float vel, float smoothTime, float timestep)
	{
		float num = smoothTime * 2f;
		float num2 = num * timestep;
		float num3 = 1f / (1f + num2 + num2 * num2 * 0.48f + num2 * num2 * num2 * 0.235f);
		float num4 = from - to;
		float num5 = (vel + num * num4) * timestep;
		vel = (vel - num * num5) * num3;
		return to + (num4 + num5) * num3;
	}
}
