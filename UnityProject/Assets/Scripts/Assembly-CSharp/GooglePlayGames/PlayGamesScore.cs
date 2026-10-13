using System;
using UnityEngine.SocialPlatforms;

namespace GooglePlayGames
{
	public class PlayGamesScore : IScore
	{
		private string mLbId;

		private long mValue;

		public string leaderboardID
		{
			get
			{
				return mLbId;
			}
			set
			{
				mLbId = value;
			}
		}

		public long value
		{
			get
			{
				return mValue;
			}
			set
			{
				mValue = value;
			}
		}

		public DateTime date
		{
			get
			{
				return new DateTime(1970, 1, 1, 0, 0, 0);
			}
		}

		public string formattedValue
		{
			get
			{
				return mValue.ToString();
			}
		}

		public string userID
		{
			get
			{
				return string.Empty;
			}
		}

		public int rank
		{
			get
			{
				return 1;
			}
		}

		public void ReportScore(Action<bool> callback)
		{
			PlayGamesPlatform.Instance.ReportScore(mValue, mLbId, callback);
		}
	}
}
