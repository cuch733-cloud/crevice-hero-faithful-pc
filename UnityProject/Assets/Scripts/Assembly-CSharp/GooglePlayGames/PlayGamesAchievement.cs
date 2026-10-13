using System;
using UnityEngine.SocialPlatforms;

namespace GooglePlayGames
{
	public class PlayGamesAchievement : IAchievement
	{
		private string mId = string.Empty;

		private double mPercentComplete;

		private DateTime mLastReportedDate = new DateTime(1970, 1, 1, 0, 0, 0, 0);

		public string id
		{
			get
			{
				return mId;
			}
			set
			{
				mId = value;
			}
		}

		public double percentCompleted
		{
			get
			{
				return mPercentComplete;
			}
			set
			{
				mPercentComplete = value;
			}
		}

		public bool completed
		{
			get
			{
				return false;
			}
		}

		public bool hidden
		{
			get
			{
				return false;
			}
		}

		public DateTime lastReportedDate
		{
			get
			{
				return mLastReportedDate;
			}
		}

		internal PlayGamesAchievement()
		{
		}

		public void ReportProgress(Action<bool> callback)
		{
			PlayGamesPlatform.Instance.ReportProgress(mId, mPercentComplete, callback);
		}
	}
}
