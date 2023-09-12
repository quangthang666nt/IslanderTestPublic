using System;

namespace NexPlugin
{
	public static class Common
	{
		[Flags]
		public enum ThreadMode
		{
			ThreadModeSafeTransportBuffer = 1,
			ThreadModeUnsafeTransportBuffer = 2
		}

		[Flags]
		public enum DispachFlag
		{
			ContinueWhenEmpty = 1,
			DispatchKeepAliveOnly = 2
		}

		public enum NotificationEvents
		{
			ParticipationEvent = 3,
			OwnershipChangeEvent = 4,
			GatheringUnregistered = 109,
			HostChangeEvent = 110,
			GameNotificationLogout = 111,
			SubscriptionEvent = 112,
			GameServerMaintenance = 113,
			MaintenanceAnnouncement = 114,
			RoundStarted = 116,
			FirstRoundReportReceived = 117,
			RoundSummarized = 118,
			MatchmakeSystemConfigurationNotification = 119,
			MatchmakeSessionSystemPasswordSet = 120,
			MatchmakeSessionSystemPasswordClear = 121,
			AddedToGathering = 122,
			UserStatusUpdatedEvent = 128
		}

		public enum ParticipationEvents
		{
			Participate = 1,
			Disconnect = 7,
			EndParticipation = 8
		}

		public const uint INFINITE_TIME_INTERVAL = 2147483647u;
	}
}
