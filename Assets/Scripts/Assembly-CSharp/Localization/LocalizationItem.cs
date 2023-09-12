using System;
using UnityEngine;

namespace Localization
{
	[Serializable]
	public class LocalizationItem
	{
		public string key;

		[TextArea]
		public string value;
	}
}
