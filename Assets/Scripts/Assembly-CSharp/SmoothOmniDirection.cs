using UnityEngine;

public class SmoothOmniDirection
{
	private Vector3 m_Curr;

	private Vector3 m_Dest;

	private Vector3 m_Vel;

	private float m_SlideRate;

	private bool m_isClose;

	private bool m_IsDone;

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
			m_Dest = value;
			float magnitude = (m_Dest - m_Curr).magnitude;
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
	}

	public bool IsWithin(float difference)
	{
		return (m_Dest - m_Curr).magnitude <= difference;
	}

	public SmoothOmniDirection()
	{
		m_SlideRate = 2f;
	}

	public SmoothOmniDirection(float slideRate)
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
		m_Curr = SmoothDirectionCD(m_Curr, m_Dest, ref m_Vel, m_SlideRate, timestep);
		float magnitude = (m_Dest - m_Curr).magnitude;
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
		m_Vel = new Vector3(0f, 0f, 0f);
	}

	private Vector3 SmoothDirectionCD(Vector3 from, Vector3 to, ref Vector3 vel, float smoothTime, float timestep)
	{
		return Vector3.Slerp(from, to, smoothTime * timestep);
	}
}
