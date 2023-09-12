using System;
using UnityEngine;

namespace SCS.SaveLoad
{
	public class SaveLocation
	{
		public static readonly string ApplicationProductName = Application.productName;

		public static readonly string ApplicationStreamingAssetsPath = Application.streamingAssetsPath;

		public string FullPath { get; private set; }

		public bool RequireUser { get; private set; }

		public string Title { get; private set; }

		public string NewTitle { get; private set; }

		public string SubTitle { get; private set; }

		public string Details { get; private set; }

		public string IconPath { get; private set; }

		public int SlotIndex { get; private set; }

		public SaveLocation(string path, bool requireUser = false, string title = null, string subtitle = null, string details = null, string iconPath = null, int slotIndex = 0)
		{
			FullPath = path;
			RequireUser = requireUser;
			Title = title ?? ApplicationProductName;
			SubTitle = subtitle ?? ("A " + title + " savegame");
			Details = details ?? ("Saved " + DateTime.Now);
			IconPath = iconPath ?? (ApplicationStreamingAssetsPath + "/SaveIcon.png");
			SlotIndex = slotIndex;
		}

		public string GetDirectory()
		{
			if (!FullPath.Contains("\\"))
			{
				return "";
			}
			return FullPath.Substring(0, FullPath.LastIndexOf('\\'));
		}

		public string GetFilename()
		{
			return FullPath.Substring(FullPath.LastIndexOf('\\') + 1);
		}
	}
}
