namespace GooglePlayGames.Native.Cwrapper
{
	internal static class Types
	{
		internal enum DataSource
		{
			CACHE_OR_NETWORK = 1,
			NETWORK_ONLY
		}

		internal enum VideoCaptureMode
		{
			UNKNOWN = -1,
			FILE,
			STREAM
		}

		internal enum VideoQualityLevel
		{
			UNKNOWN = -1,
			SD,
			HD,
			XHD,
			FULLHD
		}

		internal enum VideoCaptureOverlayState
		{
			UNKNOWN = -1,
			SHOWN = 1,
			STARTED = 2,
			STOPPED = 3,
			DISMISSED = 4
		}

		internal enum AchievementState
		{
			HIDDEN = 1,
			REVEALED,
			UNLOCKED
		}

		internal enum AchievementType
		{
			STANDARD = 1,
			INCREMENTAL
		}

		internal enum LogLevel
		{
			VERBOSE = 1,
			INFO,
			WARNING,
			ERROR
		}

		internal enum AuthOperation
		{
			SIGN_IN = 1,
			SIGN_OUT
		}

		internal enum MultiplayerEvent
		{
			UPDATED = 1,
			UPDATED_FROM_APP_LAUNCH,
			REMOVED
		}

		internal enum EventVisibility
		{
			HIDDEN = 1,
			REVEALED
		}

		internal enum LeaderboardOrder
		{
			LARGER_IS_BETTER = 1,
			SMALLER_IS_BETTER
		}

		internal enum LeaderboardTimeSpan
		{
			DAILY = 1,
			WEEKLY,
			ALL_TIME
		}

		internal enum LeaderboardCollection
		{
			PUBLIC = 1,
			SOCIAL
		}

		internal enum LeaderboardStart
		{
			TOP_SCORES = 1,
			PLAYER_CENTERED
		}

		internal enum MultiplayerInvitationType
		{
			TURN_BASED = 1,
			REAL_TIME
		}

		internal enum ParticipantStatus
		{
			INVITED = 1,
			JOINED,
			DECLINED,
			LEFT,
			NOT_INVITED_YET,
			FINISHED,
			UNRESPONSIVE
		}

		internal enum ImageResolution
		{
			ICON = 1,
			HI_RES
		}

		internal enum MatchResult
		{
			DISAGREED = 1,
			DISCONNECTED,
			LOSS,
			NONE,
			TIE,
			WIN
		}

		internal enum RealTimeRoomStatus
		{
			INVITING = 1,
			CONNECTING,
			AUTO_MATCHING,
			ACTIVE,
			DELETED
		}

		internal enum SnapshotConflictPolicy
		{
			MANUAL = 1,
			LONGEST_PLAYTIME,
			LAST_KNOWN_GOOD,
			MOST_RECENTLY_MODIFIED
		}

		internal enum MatchStatus
		{
			INVITED = 1,
			THEIR_TURN,
			MY_TURN,
			PENDING_COMPLETION,
			COMPLETED,
			CANCELED,
			EXPIRED
		}
	}
}
