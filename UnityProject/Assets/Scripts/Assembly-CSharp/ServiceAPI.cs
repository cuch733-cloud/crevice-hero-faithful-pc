using GooglePlayGames;
using UnityEngine;

public class ServiceAPI : MonoBehaviour
{
	public delegate void ApiInitCallback();

	public static ServiceAPI Instance;

	public static bool IsAuthenticated
	{
		get
		{
			return Social.Active.localUser.authenticated;
		}
	}

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void OnGooglePlayButton()
	{
		if (GameConfig.useGA)
		{
			if (IsAuthenticated)
			{
				Util.d("already login");
			}
			else
			{
				Init(() =>
				{
				}, () =>
				{
				});
			}
		}
	}

	public void OnLeaderboardButton()
	{
		if (!GameConfig.useGA)
		{
			return;
		}
		if (IsAuthenticated)
		{
			Util.d("auth ok, show Leaderboard");
			((PlayGamesPlatform)Social.Active).ShowLeaderboardUI();
			return;
		}
		Init(() =>
		{
			Util.d("... show Leaderboard");
			User.Instance.gpsLoginState = 1;
			((PlayGamesPlatform)Social.Active).ShowLeaderboardUI();
		}, () =>
		{
		});
	}

	public void OnAchievementButton()
	{
		if (!GameConfig.useGA)
		{
			return;
		}
		if (IsAuthenticated)
		{
			Social.ShowAchievementsUI();
			return;
		}
		Init(() =>
		{
			User.Instance.gpsLoginState = 1;
			Social.ShowAchievementsUI();
		}, () =>
		{
		});
	}

	public void Init(ApiInitCallback callbackSuccess, ApiInitCallback callbackFail)
	{
		if (GameConfig.useGA)
		{
			PlayGamesPlatform.DebugLogEnabled = true;
			PlayGamesPlatform.Activate();
			Util.d("google play Init");
			Social.localUser.Authenticate((bool success) =>
			{
				if (success)
				{
					Util.d("google play auth ok");
					callbackSuccess();
					if ((bool)ResultMenu.Instance)
					{
						ResultMenu.Instance.UpdateGooglePlayButton(!IsAuthenticated);
					}
				}
				else
				{
					Util.d("google play auth fail");
					if (callbackFail == null)
					{
						callbackSuccess();
					}
					else
					{
						callbackFail();
					}
					if ((bool)ResultMenu.Instance)
					{
						ResultMenu.Instance.UpdateGooglePlayButton(!IsAuthenticated);
					}
				}
			});
		}
		else
		{
			Util.d("GameConfig.useGA = false");
			if (callbackFail == null)
			{
				callbackSuccess();
			}
			else
			{
				callbackFail();
			}
		}
	}

	public void UpdateWithCurrent()
	{
		if (Application.isEditor || !GameConfig.useGA || !IsAuthenticated)
		{
			return;
		}
		Util.d("UpdateWithCurrent");
		bool flag = false;
		bool flag2 = true;
		for (int i = 0; i < 16; i++)
		{
			int playerLevel = User.Instance.GetPlayerLevel(i);
			if (playerLevel >= 1 && i >= 1)
			{
				Social.ReportProgress(GameConfig.achievementUnlocks[i], 100.0, (bool success) =>
				{
				});
			}
			switch (playerLevel)
			{
			case 0:
				flag2 = false;
				break;
			case 7:
				flag = true;
				break;
			}
		}
		if (flag)
		{
			Social.ReportProgress("CgkIsaDA__0OEAIQBg", 100.0, (bool success) =>
			{
			});
		}
		if (flag2)
		{
			Social.ReportProgress("CgkIsaDA__0OEAIQBQ", 100.0, (bool success) =>
			{
			});
		}
	}

	public void UpdateWithLevelup(int playerID, int newLevel)
	{
		if (Application.isEditor || !GameConfig.useGA || !IsAuthenticated)
		{
			return;
		}
		if (newLevel >= 1)
		{
			bool flag = true;
			for (int i = 0; i < 16; i++)
			{
				if (User.Instance.GetPlayerLevel(i) == 0)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				Social.ReportProgress("CgkIsaDA__0OEAIQBQ", 100.0, (bool success) =>
				{
				});
			}
			if (playerID >= 1 && playerID < 16)
			{
				Social.ReportProgress(GameConfig.achievementUnlocks[playerID], 100.0, (bool success) =>
				{
				});
			}
		}
		if (newLevel == 7)
		{
			Social.ReportProgress("CgkIsaDA__0OEAIQBg", 100.0, (bool success) =>
			{
			});
		}
	}

	public void UpdateWithDistance(int playerID, int distance)
	{
		if (!Application.isEditor && GameConfig.useGA && IsAuthenticated)
		{
			Social.ReportScore(distance, "CgkIsaDA__0OEAIQAQ", (bool success) =>
			{
			});
			if (playerID >= 0 && playerID < 16)
			{
				Social.ReportScore(distance, GameConfig.leaderboardMaxHeros[playerID], (bool success) =>
				{
				});
			}
			if (distance >= 100)
			{
				Social.ReportProgress("CgkIsaDA__0OEAIQAg", 100.0, (bool success) =>
				{
				});
			}
			if (distance >= 200)
			{
				Social.ReportProgress("CgkIsaDA__0OEAIQDw", 100.0, (bool success) =>
				{
				});
			}
			if (distance >= 300)
			{
				Social.ReportProgress("CgkIsaDA__0OEAIQEA", 100.0, (bool success) =>
				{
				});
			}
			if (distance >= 400)
			{
				Social.ReportProgress("CgkIsaDA__0OEAIQEQ", 100.0, (bool success) =>
				{
				});
			}
			if (distance >= 500)
			{
				Social.ReportProgress("CgkIsaDA__0OEAIQAw", 100.0, (bool success) =>
				{
				});
			}
			if (distance >= 600)
			{
				Social.ReportProgress("CgkIsaDA__0OEAIQEg", 100.0, (bool success) =>
				{
				});
			}
			if (distance >= 700)
			{
				Social.ReportProgress("CgkIsaDA__0OEAIQBA", 100.0, (bool success) =>
				{
				});
			}
			if (distance >= 1000)
			{
				Social.ReportProgress("CgkIsaDA__0OEAIQGw", 100.0, (bool success) =>
				{
				});
			}
		}
	}
}
