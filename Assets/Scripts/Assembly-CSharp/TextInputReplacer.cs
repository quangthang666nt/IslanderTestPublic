using TMPro;
using UnityEngine;

public class TextInputReplacer : MonoBehaviour
{
	public KeyMappingUpdater[] m_KeyMappings;

	public TextMeshProUGUI m_Text;

	public float m_VerticalOffset;

	public void Replace()
	{
		KeyMappingUpdater.ReplaceSpecialText(m_Text, m_KeyMappings, m_VerticalOffset);
	}

	private void Start()
	{
		Replace();
	}
}
