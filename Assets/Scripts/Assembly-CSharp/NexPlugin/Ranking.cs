using System;

namespace NexPlugin
{
	public static class Ranking
	{
		public enum RankingMode
		{
			RANKING_MODE_RANGE = 0,
			RANKING_MODE_NEAR = 1,
			RANKING_MODE_FRIEND_RANGE = 2,
			RANKING_MODE_FRIEND_NEAR = 3,
			RANKING_MODE_USER = 4
		}

		public enum FilterGroupIndex
		{
			FILTER_GROUP_INDEX_0 = 0,
			FILTER_GROUP_INDEX_1 = 1,
			FILTER_GROUP_INDEX_2 = 2,
			FILTER_GROUP_INDEX_3 = 3,
			FILTER_GROUP_INDEX_NONE = 255
		}

		public enum OrderBy
		{
			ORDER_BY_ASC = 0,
			ORDER_BY_DESC = 1
		}

		public enum UpdateMode
		{
			UPDATE_MODE_NORMAL = 0,
			UPDATE_MODE_DELETE_OLD = 1
		}

		public enum OrderCalculation
		{
			ORDER_CALCULATION_113 = 0,
			ORDER_CALCULATION_123 = 1
		}

		public enum TimeScope
		{
			TIME_SCOPE_CUSTOM_0 = 0,
			TIME_SCOPE_CUSTOM_1 = 1,
			TIME_SCOPE_ALL = 2
		}

		[Flags]
		public enum ModificationFlag
		{
			MODIFICATION_FLAG_NONE = 0,
			MODIFICATION_FLAG_GROUP0 = 1,
			MODIFICATION_FLAG_GROUP1 = 2,
			MODIFICATION_FLAG_GROUP2 = 4,
			MODIFICATION_FLAG_GROUP3 = 8,
			MODIFICATION_FLAG_PARAM = 0x10
		}

		[Flags]
		public enum StatsFlag
		{
			STATS_FLAG_TOTAL = 1,
			STATS_FLAG_SUM = 2,
			STATS_FLAG_MIN = 4,
			STATS_FLAG_MAX = 8,
			STATS_FLAG_AVERAGE = 0x10
		}

		public const byte MAX_COMMON_DATA_SIZE = byte.MaxValue;

		public const uint MAX_RANGE_RANKING_ORDER = 1000u;

		public const uint MAX_ACCURATE_ORDER = 5000u;
	}
}
