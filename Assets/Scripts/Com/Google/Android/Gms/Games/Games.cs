using System;
using Com.Google.Android.Gms.Common.Api;
using Com.Google.Android.Gms.Games.Stats;
using Google.Developers;

namespace Com.Google.Android.Gms.Games
{
	public class Games : JavaObjWrapper
	{
		private const string CLASS_NAME = "com/google/android/gms/games/Games";

		public static string EXTRA_PLAYER_IDS
		{
			get
			{
				return JavaObjWrapper.GetStaticStringField("com/google/android/gms/games/Games", "EXTRA_PLAYER_IDS");
			}
		}

		public static string EXTRA_STATUS
		{
			get
			{
				return JavaObjWrapper.GetStaticStringField("com/google/android/gms/games/Games", "EXTRA_STATUS");
			}
		}

		public static object SCOPE_GAMES
		{
			get
			{
				return JavaObjWrapper.GetStaticObjectField<object>("com/google/android/gms/games/Games", "SCOPE_GAMES", "Lcom/google/android/gms/common/api/Scope;");
			}
		}

		public static object API
		{
			get
			{
				return JavaObjWrapper.GetStaticObjectField<object>("com/google/android/gms/games/Games", "API", "Lcom/google/android/gms/common/api/Api;");
			}
		}

		public static object GamesMetadata
		{
			get
			{
				return JavaObjWrapper.GetStaticObjectField<object>("com/google/android/gms/games/Games", "GamesMetadata", "Lcom/google/android/gms/games/GamesMetadata;");
			}
		}

		public static object Achievements
		{
			get
			{
				return JavaObjWrapper.GetStaticObjectField<object>("com/google/android/gms/games/Games", "Achievements", "Lcom/google/android/gms/games/achievement/Achievements;");
			}
		}

		public static object Events
		{
			get
			{
				return JavaObjWrapper.GetStaticObjectField<object>("com/google/android/gms/games/Games", "Events", "Lcom/google/android/gms/games/event/Events;");
			}
		}

		public static object Leaderboards
		{
			get
			{
				return JavaObjWrapper.GetStaticObjectField<object>("com/google/android/gms/games/Games", "Leaderboards", "Lcom/google/android/gms/games/leaderboard/Leaderboards;");
			}
		}

		public static object Invitations
		{
			get
			{
				return JavaObjWrapper.GetStaticObjectField<object>("com/google/android/gms/games/Games", "Invitations", "Lcom/google/android/gms/games/multiplayer/Invitations;");
			}
		}

		public static object TurnBasedMultiplayer
		{
			get
			{
				return JavaObjWrapper.GetStaticObjectField<object>("com/google/android/gms/games/Games", "TurnBasedMultiplayer", "Lcom/google/android/gms/games/multiplayer/turnbased/TurnBasedMultiplayer;");
			}
		}

		public static object RealTimeMultiplayer
		{
			get
			{
				return JavaObjWrapper.GetStaticObjectField<object>("com/google/android/gms/games/Games", "RealTimeMultiplayer", "Lcom/google/android/gms/games/multiplayer/realtime/RealTimeMultiplayer;");
			}
		}

		public static object Players
		{
			get
			{
				return JavaObjWrapper.GetStaticObjectField<object>("com/google/android/gms/games/Games", "Players", "Lcom/google/android/gms/games/Players;");
			}
		}

		public static object Notifications
		{
			get
			{
				return JavaObjWrapper.GetStaticObjectField<object>("com/google/android/gms/games/Games", "Notifications", "Lcom/google/android/gms/games/Notifications;");
			}
		}

		public static object Snapshots
		{
			get
			{
				return JavaObjWrapper.GetStaticObjectField<object>("com/google/android/gms/games/Games", "Snapshots", "Lcom/google/android/gms/games/snapshot/Snapshots;");
			}
		}

		public static StatsObject Stats
		{
			get
			{
				return JavaObjWrapper.GetStaticObjectField<StatsObject>("com/google/android/gms/games/Games", "Stats", "Lcom/google/android/gms/games/stats/Stats;");
			}
		}

		public Games(IntPtr ptr)
			: base(ptr)
		{
		}
	}
}
