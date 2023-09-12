using UnityEngine;
using UnityEngine.UI;

public class VersionDisplay : MonoBehaviour
{
	public Text m_VersionText;

	public void Awake()
	{
		if (m_VersionText != null)
		{
			m_VersionText.text = "Version: " + VersionString.Value;
		}
	}
}
