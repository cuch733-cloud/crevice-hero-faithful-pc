using System.Collections.Generic;
using System.Xml;

public class AN_ApplicationTemplate : AN_BaseTemplate
{
	private Dictionary<int, AN_ActivityTemplate> _activities;

	public Dictionary<int, AN_ActivityTemplate> Activities
	{
		get
		{
			return _activities;
		}
	}

	public AN_ApplicationTemplate()
	{
		_activities = new Dictionary<int, AN_ActivityTemplate>();
	}

	public void AddActivity(AN_ActivityTemplate activity)
	{
		_activities.Add(activity.Id, activity);
	}

	public void RemoveActivity(AN_ActivityTemplate activity)
	{
		_activities.Remove(activity.Id);
	}

	public AN_ActivityTemplate GetOrCreateActivityWithName(string name)
	{
		AN_ActivityTemplate aN_ActivityTemplate = GetActivityWithName(name);
		if (aN_ActivityTemplate == null)
		{
			aN_ActivityTemplate = new AN_ActivityTemplate(false, name);
			AddActivity(aN_ActivityTemplate);
		}
		return aN_ActivityTemplate;
	}

	public AN_ActivityTemplate GetActivityWithName(string name)
	{
		foreach (KeyValuePair<int, AN_ActivityTemplate> activity in Activities)
		{
			if (activity.Value.Name.Equals(name))
			{
				return activity.Value;
			}
		}
		return null;
	}

	public AN_ActivityTemplate GetLauncherActivity()
	{
		foreach (KeyValuePair<int, AN_ActivityTemplate> activity in Activities)
		{
			if (activity.Value.IsLauncher)
			{
				return activity.Value;
			}
		}
		return null;
	}

	public override void ToXmlElement(XmlDocument doc, XmlElement parent)
	{
		AddAttributesToXml(doc, parent, this);
		AddPropertiesToXml(doc, parent, this);
		foreach (int key in _activities.Keys)
		{
			XmlElement xmlElement = doc.CreateElement("activity");
			_activities[key].ToXmlElement(doc, xmlElement);
			parent.AppendChild(xmlElement);
		}
	}
}
