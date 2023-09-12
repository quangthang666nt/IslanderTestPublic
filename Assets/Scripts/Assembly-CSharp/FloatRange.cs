using System;
using UnityEngine;

[Serializable]
public class FloatRange
{
	public float m_min;

	public float m_max;

	public float Min => m_min;

	public float Max => m_max;

	public float Random => UnityEngine.Random.Range(m_min, m_max);

	public bool IsValid => m_min <= m_max;

	public FloatRange()
	{
		m_min = 0f;
		m_max = 0f;
	}

	public FloatRange(float min, float max)
	{
		Set(min, max);
	}

	public float RandomLOD(Transform owner, float extra = 0f)
	{
		float num = Mathf.Clamp01(((owner.position - Camera.main.transform.position).magnitude - 75f) / 50f);
		return UnityEngine.Random.Range(m_min + (m_max - m_min) * num, m_max) + extra * num;
	}

	public void Set(float min, float max)
	{
		m_min = Mathf.Min(min, max);
		m_max = Mathf.Max(min, max);
	}

	public void SetRaw(float min, float max)
	{
		m_min = min;
		m_max = max;
	}

	public float Clamp(float value)
	{
		return Mathf.Clamp(value, m_min, m_max);
	}
}
