namespace NexPlugin
{
	public class StationURL
	{
		public enum URLType
		{
			unknown = 0,
			prudp = 1,
			prudps = 2,
			udp = 3
		}

		public enum Flags
		{
			BehindNAT = 1,
			Public = 2,
			DetectedByNatCheck = 4,
			DetectedByNgs = 8
		}
	}
}
