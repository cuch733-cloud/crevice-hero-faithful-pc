public class SALoadedSceneOnClick : SAOnClickAction
{
	public string levelName;

	protected override void OnClick()
	{
		SA_Singleton<SALevelLoader>.instance.LoadLevel(levelName);
	}
}
