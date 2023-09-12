namespace SCS.SaveLoad
{
	public class SaveLoadErrorResult
	{
		public enum SaveLoadErrorType
		{
			REQUIRED_USER_NOT_FOUND = 0,
			NO_DATA_TO_LOAD = 1,
			DATA_CORRUPTED = 2,
			GENERIC_ERROR = 3,
			NOT_ENOUGH_SPACE = 4,
			NOT_MOUNTED = 5
		}

		public const string NO_USER_ERROR = "The save location requires an user to be active";

		public string Message { get; private set; }

		public SaveLoadErrorType Type { get; private set; }

		public SaveLoadErrorResult(string message, SaveLoadErrorType type)
		{
			Message = message;
			Type = type;
		}

		public override string ToString()
		{
			return "[SAVELOAD ERROR " + Type.ToString() + "] " + Message;
		}
	}
}
