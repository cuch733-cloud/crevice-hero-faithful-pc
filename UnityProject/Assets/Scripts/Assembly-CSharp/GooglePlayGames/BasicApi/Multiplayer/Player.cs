namespace GooglePlayGames.BasicApi.Multiplayer
{
	public class Player
	{
		private string mDisplayName = string.Empty;

		private string mPlayerId = string.Empty;

		public string DisplayName
		{
			get
			{
				return mDisplayName;
			}
		}

		public string PlayerId
		{
			get
			{
				return mPlayerId;
			}
		}

		internal Player(string displayName, string playerId)
		{
			mDisplayName = displayName;
			mPlayerId = playerId;
		}

		public override string ToString()
		{
			return string.Format("[Player: '{0}' (id {1})]", mDisplayName, mPlayerId);
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (object.ReferenceEquals(this, obj))
			{
				return true;
			}
			if (obj.GetType() != typeof(Player))
			{
				return false;
			}
			Player player = (Player)obj;
			return mPlayerId == player.mPlayerId;
		}

		public override int GetHashCode()
		{
			return (mPlayerId != null) ? mPlayerId.GetHashCode() : 0;
		}
	}
}
