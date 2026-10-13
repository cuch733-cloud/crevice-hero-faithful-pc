using System.Collections.Generic;
using System.Xml;

public class AN_ActivityTemplate : AN_BaseTemplate
{
	public bool IsOpen;

	private int _id;

	private bool _isLauncher;

	private string _name = string.Empty;

	public bool IsLauncher
	{
		get
		{
			return _isLauncher;
		}
	}

	public string Name
	{
		get
		{
			return _name;
		}
	}

	public int Id
	{
		get
		{
			return _id;
		}
	}

	public AN_ActivityTemplate(bool isLauncher, string name)
	{
		_isLauncher = isLauncher;
		_name = name;
		_id = GetHashCode();
		_values = new Dictionary<string, string>();
		_properties = new Dictionary<string, List<AN_PropertyTemplate>>();
		SetValue("android:name", name);
	}

	public void SetName(string name)
	{
		_name = name;
		SetValue("android:name", name);
	}

	public void SetAsLauncher(bool isLauncher)
	{
		_isLauncher = isLauncher;
	}

	public static AN_PropertyTemplate GetLauncherPropertyTemplate()
	{
		AN_PropertyTemplate aN_PropertyTemplate = new AN_PropertyTemplate("intent-filter");
		AN_PropertyTemplate aN_PropertyTemplate2 = new AN_PropertyTemplate("action");
		aN_PropertyTemplate2.SetValue("android:name", "android.intent.action.MAIN");
		aN_PropertyTemplate.AddProperty("action", aN_PropertyTemplate2);
		aN_PropertyTemplate2 = new AN_PropertyTemplate("category");
		aN_PropertyTemplate2.SetValue("android:name", "android.intent.category.LAUNCHER");
		aN_PropertyTemplate.AddProperty("category", aN_PropertyTemplate2);
		return aN_PropertyTemplate;
	}

	public bool IsLauncherProperty(AN_PropertyTemplate property)
	{
		if (property.Tag.Equals("intent-filter"))
		{
			foreach (AN_PropertyTemplate item in property.Properties["category"])
			{
				if (item.Values.ContainsKey("android:name") && item.Values["android:name"].Equals("android.intent.category.LAUNCHER"))
				{
					return true;
				}
			}
		}
		return false;
	}

	public override void ToXmlElement(XmlDocument doc, XmlElement parent)
	{
		AddAttributesToXml(doc, parent, this);
		AN_PropertyTemplate aN_PropertyTemplate = null;
		if (_isLauncher)
		{
			aN_PropertyTemplate = GetLauncherPropertyTemplate();
			AddProperty(aN_PropertyTemplate.Tag, aN_PropertyTemplate);
		}
		AddPropertiesToXml(doc, parent, this);
		if (_isLauncher)
		{
			_properties["intent-filter"].Remove(aN_PropertyTemplate);
		}
	}
}
