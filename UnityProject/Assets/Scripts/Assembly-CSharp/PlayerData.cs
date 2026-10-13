using SimpleJSON;

public class PlayerData
{
	public const int SKILL_STOP = 1;

	public const int SKILL_SHIELD = 2;

	public const int SKILL_HINT = 3;

	public const int SKILL_SPEED = 4;

	public const int SKILL_3D = 5;

	public const int SKILL_REBIRTH = 6;

	public const int SKILL_GEM = 7;

	public const int SKILL_FLY = 8;

	public const int SKILL_SPEED2 = 9;

	public const int SKILL_SLOW = 10;

	public const int SKILL_TRANS = 11;

	public const int SKILL_HAMMER = 12;

	public const int SKILL_ICE = 13;

	public const int SKILL_HULK = 14;

	public const int SKILL_SOCCER = 15;

	public const int SKILL_ROBOT = 16;

	private int id;

	private int skillId;

	private bool isEnabled;

	public int animId;

	public int gemId;

	public int cash;

	public int speed;

	private int[] levelupGems = new int[7];

	private int[] coolTimes = new int[7];

	private int[] skillValues = new int[7];

	public float width;

	public float height;

	public static string GetSkillSprite(int skillID)
	{
		string result = null;
		switch (skillID)
		{
		case 1:
			result = "ui_skill_stop";
			break;
		case 2:
			result = "ui_skill_shield";
			break;
		case 3:
			result = "ui_skill_see";
			break;
		case 4:
			result = "ui_skill_speed";
			break;
		case 9:
			result = "ui_skill_speed2";
			break;
		case 5:
			result = "ui_skill_acrobatic";
			break;
		case 6:
			result = "ui_skill_revival";
			break;
		case 8:
			result = "ui_skill_supergirl";
			break;
		case 7:
			result = "ui_skill_lucky";
			break;
		case 10:
			result = "ui_skill_slow";
			break;
		case 11:
			result = "ui_skill_invisible";
			break;
		case 12:
			result = "ui_skill_hammer";
			break;
		case 13:
			result = "ui_skill_ice";
			break;
		case 14:
			result = "ui_skill_hulk";
			break;
		case 15:
			result = "ui_skill_ball";
			break;
		case 16:
			result = "ui_skill_missile";
			break;
		}
		return result;
	}

	public void Parse(JSONNode node)
	{
		id = node["id"].AsInt;
		for (int i = 0; i < 7; i++)
		{
			levelupGems[i] = node["levelup_gem"][i].AsInt;
			coolTimes[i] = node["cooltime"][i].AsInt;
			skillValues[i] = node["skill_value"][i].AsInt;
		}
		speed = node["speed"].AsInt;
		skillId = node["skill_id"].AsInt;
		animId = node["anim_id"].AsInt;
		gemId = node["gem_anim_id"].AsInt;
		width = node["width"].AsFloat;
		height = node["height"].AsFloat;
		cash = node["cash"].AsInt;
		PlayerManager.Instance.gemIDs[id] = gemId;
	}

	public int GetID()
	{
		return id;
	}

	public string GetName()
	{
		return Lang.Instance.Get("player" + id);
	}

	public int GetLevelupGems(int level)
	{
		if (level >= 0 && level < coolTimes.Length)
		{
			return levelupGems[level];
		}
		return 0;
	}

	public int GetCooltime(int level)
	{
		if (level >= 1 && level <= coolTimes.Length)
		{
			return coolTimes[level - 1];
		}
		return -1;
	}

	public int GetSkillID()
	{
		return skillId;
	}

	public int GetSkillValue(int level)
	{
		if (level >= 1 && level <= coolTimes.Length)
		{
			return skillValues[level - 1];
		}
		return -1;
	}

	public string GetSkillDetail(int level)
	{
		return string.Format(Lang.Instance.Get("skill" + skillId));
	}

	public bool IsEnabled()
	{
		return isEnabled;
	}

	public int GetCash()
	{
		return cash;
	}
}
