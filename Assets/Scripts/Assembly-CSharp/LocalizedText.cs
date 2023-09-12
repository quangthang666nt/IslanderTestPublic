using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class LocalizedText : MonoBehaviour
{
	[SerializeField]
	private string key;

	private Text txt;

	private void Start()
	{
		txt = GetComponent<Text>();
		txt.text = LegacyLocalizationManager.StrGetLocalizedString(key);
	}
}
