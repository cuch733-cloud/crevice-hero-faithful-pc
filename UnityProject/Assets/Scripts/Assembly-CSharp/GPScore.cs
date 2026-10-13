public class GPScore
{
	private int _rank;

	private long _score;

	private string _playerId;

	private string _leaderboardId;

	private GPCollectionType _collection;

	private GPBoardTimeSpan _timeSpan;

	public int rank
	{
		get
		{
			return _rank;
		}
	}

	public long score
	{
		get
		{
			return _score;
		}
	}

	public string playerId
	{
		get
		{
			return _playerId;
		}
	}

	public string leaderboardId
	{
		get
		{
			return _leaderboardId;
		}
	}

	public GPCollectionType collection
	{
		get
		{
			return _collection;
		}
	}

	public GPBoardTimeSpan timeSpan
	{
		get
		{
			return _timeSpan;
		}
	}

	public GPScore(long vScore, int vRank, GPBoardTimeSpan vTimeSpan, GPCollectionType sCollection, string lid, string pid)
	{
		_score = vScore;
		_rank = vRank;
		_playerId = pid;
		_leaderboardId = lid;
		_timeSpan = vTimeSpan;
		_collection = sCollection;
	}

	public void UpdateScore(long vScore)
	{
		_score = vScore;
	}
}
