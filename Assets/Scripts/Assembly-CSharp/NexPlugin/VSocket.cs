namespace NexPlugin
{
	public class VSocket
	{
		public enum Result
		{
			Success = 0,
			Error = 1,
			WouldBlock = 2,
			PacketBufferFull = 3
		}

		public const uint MAX_DATA_SIZE = 1250u;

		public const uint MARGIN_SIZE = 12u;

		public const uint DEFAULT_MAX_RECV_PACKTES = 128u;

		public const uint TOTAL_NAT_TRAVERSAL_TIMEOUT_MS = 120000u;
	}
}
