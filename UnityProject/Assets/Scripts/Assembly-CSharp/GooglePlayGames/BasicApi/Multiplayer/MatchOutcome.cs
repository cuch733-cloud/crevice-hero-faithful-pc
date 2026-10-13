using System.Collections.Generic;

namespace GooglePlayGames.BasicApi.Multiplayer
{
	public class MatchOutcome
	{
		public enum ParticipantResult
		{
			Unset = -1,
			None = 0,
			Win = 1,
			Loss = 2,
			Tie = 3
		}

		public const int PlacementUnset = -1;

		private List<string> mParticipantIds = new List<string>();

		private Dictionary<string, int> mPlacements = new Dictionary<string, int>();

		private Dictionary<string, ParticipantResult> mResults = new Dictionary<string, ParticipantResult>();

		public List<string> ParticipantIds
		{
			get
			{
				return mParticipantIds;
			}
		}

		public void SetParticipantResult(string participantId, ParticipantResult result, int placement)
		{
			if (!mParticipantIds.Contains(participantId))
			{
				mParticipantIds.Add(participantId);
			}
			mPlacements[participantId] = placement;
			mResults[participantId] = result;
		}

		public void SetParticipantResult(string participantId, ParticipantResult result)
		{
			SetParticipantResult(participantId, result, -1);
		}

		public void SetParticipantResult(string participantId, int placement)
		{
			SetParticipantResult(participantId, ParticipantResult.Unset, placement);
		}

		public ParticipantResult GetResultFor(string participantId)
		{
			return (!mResults.ContainsKey(participantId)) ? ParticipantResult.Unset : mResults[participantId];
		}

		public int GetPlacementFor(string participantId)
		{
			return (!mPlacements.ContainsKey(participantId)) ? (-1) : mPlacements[participantId];
		}

		public override string ToString()
		{
			string text = "[MatchOutcome";
			foreach (string mParticipantId in mParticipantIds)
			{
				text += string.Format(" {0}->({1},{2})", mParticipantId, GetResultFor(mParticipantId), GetPlacementFor(mParticipantId));
			}
			return text + "]";
		}
	}
}
