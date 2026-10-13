using System.Collections.Generic;
using GooglePlayGames.OurUtils;

namespace GooglePlayGames.BasicApi.Multiplayer
{
	public class TurnBasedMatch
	{
		public enum MatchStatus
		{
			Active = 0,
			AutoMatching = 1,
			Cancelled = 2,
			Complete = 3,
			Expired = 4,
			Unknown = 5,
			Deleted = 6
		}

		public enum MatchTurnStatus
		{
			Complete = 0,
			Invited = 1,
			MyTurn = 2,
			TheirTurn = 3,
			Unknown = 4
		}

		private string mMatchId;

		private byte[] mData;

		private bool mCanRematch;

		private int mAvailableAutomatchSlots;

		private string mSelfParticipantId;

		private List<Participant> mParticipants;

		private string mPendingParticipantId;

		private MatchTurnStatus mTurnStatus;

		private MatchStatus mMatchStatus;

		private int mVariant;

		public string MatchId
		{
			get
			{
				return mMatchId;
			}
		}

		public byte[] Data
		{
			get
			{
				return mData;
			}
		}

		public bool CanRematch
		{
			get
			{
				return mCanRematch;
			}
		}

		public string SelfParticipantId
		{
			get
			{
				return mSelfParticipantId;
			}
		}

		public Participant Self
		{
			get
			{
				return GetParticipant(mSelfParticipantId);
			}
		}

		public List<Participant> Participants
		{
			get
			{
				return mParticipants;
			}
		}

		public string PendingParticipantId
		{
			get
			{
				return mPendingParticipantId;
			}
		}

		public Participant PendingParticipant
		{
			get
			{
				return (mPendingParticipantId != null) ? GetParticipant(mPendingParticipantId) : null;
			}
		}

		public MatchTurnStatus TurnStatus
		{
			get
			{
				return mTurnStatus;
			}
		}

		public MatchStatus Status
		{
			get
			{
				return mMatchStatus;
			}
		}

		public int Variant
		{
			get
			{
				return mVariant;
			}
		}

		public int AvailableAutomatchSlots
		{
			get
			{
				return mAvailableAutomatchSlots;
			}
		}

		internal TurnBasedMatch(string matchId, byte[] data, bool canRematch, string selfParticipantId, List<Participant> participants, int availableAutomatchSlots, string pendingParticipantId, MatchTurnStatus turnStatus, MatchStatus matchStatus, int variant)
		{
			mMatchId = matchId;
			mData = data;
			mCanRematch = canRematch;
			mSelfParticipantId = selfParticipantId;
			mParticipants = participants;
			mParticipants.Sort();
			mAvailableAutomatchSlots = availableAutomatchSlots;
			mPendingParticipantId = pendingParticipantId;
			mTurnStatus = turnStatus;
			mMatchStatus = matchStatus;
			mVariant = variant;
		}

		public Participant GetParticipant(string participantId)
		{
			foreach (Participant mParticipant in mParticipants)
			{
				if (mParticipant.ParticipantId.Equals(participantId))
				{
					return mParticipant;
				}
			}
			Logger.w("Participant not found in turn-based match: " + participantId);
			return null;
		}

		public override string ToString()
		{
			return string.Format("[TurnBasedMatch: mMatchId={0}, mData={1}, mCanRematch={2}, mSelfParticipantId={3}, mParticipants={4}, mPendingParticipantId={5}, mTurnStatus={6}, mMatchStatus={7}, mVariant={8}]", mMatchId, mData, mCanRematch, mSelfParticipantId, mParticipants, mPendingParticipantId, mTurnStatus, mMatchStatus, mVariant);
		}
	}
}
