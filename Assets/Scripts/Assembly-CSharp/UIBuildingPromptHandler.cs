using System.Collections.Generic;
using UnityEngine;

public class UIBuildingPromptHandler : MonoBehaviour
{
	public List<GameObject> m_Targets;

	private List<bool> m_Previous;

	private void OnEnable()
	{
		if (m_Previous == null)
		{
			m_Previous = new List<bool>();
		}
		m_Previous.Clear();
		for (int i = 0; i < m_Targets.Count; i++)
		{
			m_Previous.Add(m_Targets[i].activeSelf);
			m_Targets[i].SetActive(value: false);
		}
	}

	private void OnDisable()
	{
		for (int i = 0; i < m_Targets.Count; i++)
		{
			m_Targets[i].SetActive(m_Previous[i]);
		}
	}
}
