namespace SCS.Utils
{
	public static class SCSHelper
	{
		public enum DebugLevel
		{
			Default = 0,
			Warning = 1,
			Error = 2,
			Success = 3
		}

		private const string DEBUG_COLOR_WARNING = "orange";

		private const string DEBUG_COLOR_ERROR = "red";

		private const string DEBUG_COLOR_SUCCESS = "lime";

		private const string DEBUG_COLOR_DEFAULT = "white";

		private const string DEBUG_COLOR_STRING_TEMPLATE = "<color={0}>{1}</color>";

		public static string DebugColor(string message, DebugLevel level = DebugLevel.Default)
		{
			return level switch
			{
				DebugLevel.Warning => string.Format("<color={0}>{1}</color>", "orange", message), 
				DebugLevel.Error => string.Format("<color={0}>{1}</color>", "red", message), 
				DebugLevel.Success => string.Format("<color={0}>{1}</color>", "lime", message), 
				_ => string.Format("<color={0}>{1}</color>", "white", message), 
			};
		}
	}
}
