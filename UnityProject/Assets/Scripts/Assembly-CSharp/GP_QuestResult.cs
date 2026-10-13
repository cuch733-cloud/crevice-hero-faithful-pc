public class GP_QuestResult : GooglePlayResult
{
	public string questId = string.Empty;

	public string reward = string.Empty;

	public GP_Quest quest;

	public GP_QuestResult(string code)
		: base(code)
	{
	}

	public GP_Quest GetQuest()
	{
		return quest;
	}
}
