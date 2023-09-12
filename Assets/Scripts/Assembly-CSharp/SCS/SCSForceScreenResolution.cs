using System;
using SCS.Utils;
using UnityEngine;

namespace SCS
{
	public class SCSForceScreenResolution : MonoBehaviour
	{
		public const int DEFAULT_WIDTH = 1920;

		public const int DEFAULT_HEIGHT = 1080;

		public int Width = 1920;

		public int Height = 1080;

		public float Factor = 1f;

		private void Start()
		{
			try
			{
				Debug.LogError(SCSHelper.DebugColor($"Force resolution to {Width}x{Height}...", SCSHelper.DebugLevel.Warning));
				Screen.SetResolution(Width, Height, Screen.fullScreen);
				Debug.LogError(SCSHelper.DebugColor($"Resolution setup sucessfull to {Width}x{Height}!", SCSHelper.DebugLevel.Success));
			}
			catch (Exception ex)
			{
				Debug.LogError($"Error to forcing resolution to {Width}x{Height}: {ex.ToString()}");
			}
		}
	}
}
