using UnityEngine;

public class IntRange
{
	public int m_min;

	public int m_max;

	public int Min => m_min;

	public int Max => m_max;

	public int Random => UnityEngine.Random.Range(m_min, m_max);

	public bool IsValid => m_min <= m_max;

	public IntRange()
	{
		m_min = 0;
		m_max = 0;
	}

	public IntRange(int min, int max)
	{
		Set(min, max);
	}

	public void Set(int min, int max)
	{
		m_min = Mathf.Min(min, max);
		m_max = Mathf.Max(min, max);
	}

	public void SetRaw(int min, int max)
	{
		m_min = min;
		m_max = max;
	}
}
