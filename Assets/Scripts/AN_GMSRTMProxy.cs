using UnityEngine;

public class AN_GMSRTMProxy : MonoBehaviour
{
	private static void CallActivityFunction(string methodName, params object[] args)
	{
		
	}

	public static void RTMFindMatch(int minPlayers, int maxPlayers, string[] pIds)
	{
		
	}

	public static void RTMFindMatch(string[] pIds)
	{
	}

	public static void sendDataToAll(string data, int sendType, int dataId = 0)
	{
		
	}

	public static void sendDataToPlayers(string data, string players, int sendType, int dataId = 0)
	{
	}

	public static void ShowWaitingRoomIntent()
	{
		
	}

	public static void InvitePlayers(int minPlayers, int maxPlayers)
	{
	
	}

	public static void RTM_SetVariant(int val)
	{
		
	}

	public static void RTM_SetExclusiveBitMask(int val)
	{
		
	}

	public static void RTM_AcceptInvitation(string invitationId)
	{
	
	}

	public static void RTM_DeclineInvitation(string invitationId)
	{
	
	}

	public static void RTM_DismissInvitation(string invitationId)
	{
		
	}

	public static void TBM_AcceptInvitation(string invitationId)
	{
	
	}

	public static void TBM_DeclineInvitation(string invitationId)
	{
	
	}

	public static void TBM_DismissInvitation(string invitationId)
	{
		
	}

	public static void TBM_CreateMatch(int minPlayers, int maxPlayers, string[] playersIds)
	{
		
	}

	public static void CancelMatch(string matchId)
	{
		
	}

	public static void DismissMatch(string matchId)
	{
		
	}

	public static void TBM_FinishMatch(string matchId, string matchData, string[] pIds, int[] results, int[] placing, int[] versions)
	{
		
	}

	public static void TBM_FinishMatchWithId(string matchId)
	{
		
	}

	public static void TBM_LeaveMatch(string matchId)
	{
		
	}

	public static void TBM_LeaveMatchDuringTurn(string matchId, string pendingParticipantId)
	{
		
	}

	public static void TBM_LoadMatchInfo(string matchId)
	{
		
	}

	public static void TBM_LoadMatchesInfo(int invitationSortOrder, int[] matchTurnStatuses)
	{
		
	}

	public static void TBM_LoadAllMatchesInfo(int invitationSortOrder)
	{
		
	}

	public static void TBM_Rematch(string matchId)
	{
		
	}

	public static void TBM_RegisterMatchUpdateListener()
	{
		
	}

	public static void TBM_UnregisterMatchUpdateListener()
	{
		
	}

	public static void TBM_TakeTrun(string matchId, string matchData, string pendingParticipantId, string[] pIds, int[] results, int[] placing, int[] versions)
	{
		
	}

	public static void StartSelectOpponentsView(int minPlayers, int maxPlayers, bool allowAutomatch)
	{
		
	}

	public static void TBM_ShowInbox()
	{
	
	}

	public static void TBM_SetVariant(int val)
	{
		
	}

	public static void TBM_SetExclusiveBitMask(int val)
	{
		
	}
}
