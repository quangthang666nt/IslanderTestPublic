public struct PlayerConnectionResult
{
	public enum ResultState
	{
		Success = 0,
		Cancelled = 1,
		Failed = 2
	}

	public ResultState Result;

	public int Index;
}
