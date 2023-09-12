using System;
using I2.Loc;
using TMPro;
using UnityEngine;

public class UpdateTextStringObject : MonoBehaviour
{
	[SerializeField]
	private EventObject stringActionObject;

	[SerializeField]
	private TextMeshProUGUI text;

	[SerializeField]
	private bool btranslate = true;

	[SerializeField]
	private Localize localize;

	[SerializeField]
	private string sLocalizeGroups = "";

	private string sCurrentFilterName = "None";

	private void Awake()
	{
		EventObject eventObject = stringActionObject;
		eventObject.objectEvent = (EventObject.ObjectEvent)Delegate.Combine(eventObject.objectEvent, new EventObject.ObjectEvent(UpdateText));
	}

	private void OnEnable()
	{
		if (!btranslate)
		{
			return;
		}
		string term = sLocalizeGroups + sCurrentFilterName;
		text.text = LocalizationManager.GetTranslation(term);
		if (text.text == "")
		{
			text.text = sCurrentFilterName;
		}
		string termTranslation = LocalizationManager.GetTermTranslation("Font/Bold");
		if (termTranslation != "")
		{
			TMP_FontAsset tMP_FontAsset = localize.FindTranslatedObject<TMP_FontAsset>(termTranslation);
			if (tMP_FontAsset != null)
			{
				text.font = tMP_FontAsset;
			}
		}
	}

	private void UpdateText(object value)
	{
		sCurrentFilterName = (string)value;
		if (btranslate)
		{
			string term = sLocalizeGroups + sCurrentFilterName;
			text.text = LocalizationManager.GetTranslation(term);
			if (text.text == "")
			{
				text.text = sCurrentFilterName;
			}
		}
		else
		{
			text.text = sCurrentFilterName;
		}
	}
}
