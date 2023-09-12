using System;
using UnityEngine;

[Serializable]
public class SmoothDirection
{
	public Vector3 m_Curr;

	public Vector3 m_CurrAngles;

	public Vector3 m_DestAngles;

	public Vector3 m_Vel;

	public float m_SlideRate;

	public bool m_isClose;

	public bool m_IsDone;

	public bool IsClose => m_isClose;

	public bool IsDone => m_IsDone;

	public Vector3 Value
	{
		get
		{
			return m_Curr;
		}
		set
		{
			MathUtils.CalcAnglesFromDir(value, ref m_DestAngles.y, ref m_DestAngles.x);
			Vector3 vector = default(Vector3);
			vector.x = MathUtils.CalcMinAngleDif(m_CurrAngles.x, m_DestAngles.x);
			vector.y = MathUtils.CalcMinAngleDif(m_CurrAngles.y, m_DestAngles.y);
			vector.z = 0f;
			m_DestAngles = m_CurrAngles + vector;
			float magnitude = vector.magnitude;
			if (magnitude < 0.0001f)
			{
				m_isClose = true;
				if (magnitude < 1E-06f)
				{
					m_IsDone = true;
				}
			}
			else
			{
				m_IsDone = false;
				m_isClose = false;
			}
		}
	}

	public SmoothDirection()
	{
		m_SlideRate = 2f;
	}

	public SmoothDirection(float slideRate)
	{
		m_SlideRate = slideRate;
	}

	public void SetTrackingRate(float slideRate)
	{
		m_SlideRate = slideRate;
	}

	public void Update(float timestep)
	{
		if (m_IsDone || !(timestep > 0f))
		{
			return;
		}
		Vector3 vector = default(Vector3);
		vector.x = MathUtils.CalcMinAngleDif(m_CurrAngles.x, m_DestAngles.x);
		vector.y = MathUtils.CalcMinAngleDif(m_CurrAngles.y, m_DestAngles.y);
		vector.z = 0f;
		m_DestAngles = m_CurrAngles + vector;
		m_CurrAngles = SmoothAnglesCD(m_CurrAngles, m_DestAngles, ref m_Vel, m_SlideRate, timestep);
		MathUtils.CalcDirFromAngles(ref m_Curr, m_CurrAngles.y, m_CurrAngles.x);
		float magnitude = vector.magnitude;
		if (magnitude < 0.01f)
		{
			m_isClose = true;
			if (magnitude < 0.0001f)
			{
				m_IsDone = true;
			}
		}
		else
		{
			m_IsDone = false;
			m_isClose = false;
		}
	}

	public void SetNow(Vector3 val)
	{
		m_Curr = val;
		m_Curr.Normalize();
		MathUtils.CalcAnglesFromDir(m_Curr, ref m_DestAngles.y, ref m_DestAngles.x);
		m_CurrAngles = m_DestAngles;
		m_Vel = new Vector3(0f, 0f, 0f);
	}

	public void SetNowFromAngles(float yAxis, float xAxis)
	{
		m_DestAngles.y = yAxis;
		m_DestAngles.x = xAxis;
		m_DestAngles.z = 0f;
		MathUtils.CalcDirFromAngles(ref m_Curr, m_DestAngles.y, m_DestAngles.x);
		m_CurrAngles = m_DestAngles;
		m_Vel = new Vector3(0f, 0f, 0f);
	}

	public void AddFromAngles(float yAxis, float xAxis)
	{
		float angY = m_DestAngles.y + yAxis;
		float angX = m_DestAngles.x + xAxis;
		Vector3 dest = default(Vector3);
		MathUtils.CalcDirFromAngles(ref dest, angY, angX);
		Value = dest;
	}

	private Vector3 SmoothAnglesCD(Vector3 from, Vector3 to, ref Vector3 vel, float smoothTime, float timestep)
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
