using System.Collections.Generic;
using System.Xml;

public abstract class AN_BaseTemplate
{
	protected Dictionary<string, List<AN_PropertyTemplate>> _properties;

	protected Dictionary<string, string> _values;

	public Dictionary<string, string> Values
	{
		get
		{
			return _values;
		}
	}

	public Dictionary<string, List<AN_PropertyTemplate>> Properties
	{
		get
		{
			return _properties;
		}
	}

	public AN_BaseTemplate()
	{
		_values = new Dictionary<string, string>();
		_properties = new Dictionary<string, List<AN_PropertyTemplate>>();
	}

	public AN_PropertyTemplate GetOrCreateIntentFilterWithName(string name)
	{
		AN_PropertyTemplate aN_PropertyTemplate = GetIntentFilterWithName(name);
		if (aN_PropertyTemplate == null)
		{
			aN_PropertyTemplate = new AN_PropertyTemplate("intent-filter");
			AN_PropertyTemplate aN_PropertyTemplate2 = new AN_PropertyTemplate("action");
			aN_PropertyTemplate2.SetValue("android:name", name);
			aN_PropertyTemplate.AddProperty(aN_PropertyTemplate2);
			AddProperty(aN_PropertyTemplate);
		}
		return aN_PropertyTemplate;
	}

	public AN_PropertyTemplate GetIntentFilterWithName(string name)
	{
		string tag = "intent-filter";
		List<AN_PropertyTemplate> propertiesWithTag = GetPropertiesWithTag(tag);
		foreach (AN_PropertyTemplate item in propertiesWithTag)
		{
			string intentFilterName = GetIntentFilterName(item);
			if (intentFilterName.Equals(name))
			{
				return item;
			}
		}
		return null;
	}

	public string GetIntentFilterName(AN_PropertyTemplate intent)
	{
		List<AN_PropertyTemplate> propertiesWithTag = intent.GetPropertiesWithTag("action");
		if (propertiesWithTag.Count > 0)
		{
			return propertiesWithTag[0].GetValue("android:name");
		}
		return string.Empty;
	}

	public AN_PropertyTemplate GetOrCreatePropertyWithName(string tag, string name)
	{
		AN_PropertyTemplate aN_PropertyTemplate = GetPropertyWithName(tag, name);
		if (aN_PropertyTemplate == null)
		{
			aN_PropertyTemplate = new AN_PropertyTemplate(tag);
			aN_PropertyTemplate.SetValue("android:name", name);
			AddProperty(aN_PropertyTemplate);
		}
		return aN_PropertyTemplate;
	}

	public AN_PropertyTemplate GetPropertyWithName(string tag, string name)
	{
		List<AN_PropertyTemplate> propertiesWithTag = GetPropertiesWithTag(tag);
		foreach (AN_PropertyTemplate item in propertiesWithTag)
		{
			if (item.Values.ContainsKey("android:name") && item.Values["android:name"] == name)
			{
				return item;
			}
		}
		return null;
	}

	public AN_PropertyTemplate GetOrCreatePropertyWithTag(string tag)
	{
		AN_PropertyTemplate aN_PropertyTemplate = GetPropertyWithTag(tag);
		if (aN_PropertyTemplate == null)
		{
			aN_PropertyTemplate = new AN_PropertyTemplate(tag);
			AddProperty(aN_PropertyTemplate);
		}
		return aN_PropertyTemplate;
	}

	public AN_PropertyTemplate GetPropertyWithTag(string tag)
	{
		List<AN_PropertyTemplate> propertiesWithTag = GetPropertiesWithTag(tag);
		if (propertiesWithTag.Count > 0)
		{
			return propertiesWithTag[0];
		}
		return null;
	}

	public List<AN_PropertyTemplate> GetPropertiesWithTag(string tag)
	{
		if (Properties.ContainsKey(tag))
		{
			return Properties[tag];
		}
		return new List<AN_PropertyTemplate>();
	}

	public abstract void ToXmlElement(XmlDocument doc, XmlElement parent);

	public void AddProperty(AN_PropertyTemplate property)
	{
		AddProperty(property.Tag, property);
	}

	public void AddProperty(string tag, AN_PropertyTemplate property)
	{
		if (!_properties.ContainsKey(tag))
		{
			List<AN_PropertyTemplate> value = new List<AN_PropertyTemplate>();
			_properties.Add(tag, value);
		}
		_properties[tag].Add(property);
	}

	public void SetValue(string key, string value)
	{
		if (_values.ContainsKey(key))
		{
			_values[key] = value;
		}
		else
		{
			_values.Add(key, value);
		}
	}

	public string GetValue(string key)
	{
		if (_values.ContainsKey(key))
		{
			return _values[key];
		}
		return string.Empty;
	}

	public void RemoveProperty(AN_PropertyTemplate property)
	{
		_properties[property.Tag].Remove(property);
	}

	public void RemoveValue(string key)
	{
		_values.Remove(key);
	}

	public void AddPropertiesToXml(XmlDocument doc, XmlElement parent, AN_BaseTemplate template)
	{
		foreach (string key in template.Properties.Keys)
		{
			foreach (AN_PropertyTemplate item in template.Properties[key])
			{
				XmlElement xmlElement = doc.CreateElement(key);
				AddAttributesToXml(doc, xmlElement, item);
				AddPropertiesToXml(doc, xmlElement, item);
				parent.AppendChild(xmlElement);
			}
		}
	}

	public void AddAttributesToXml(XmlDocument doc, XmlElement parent, AN_BaseTemplate template)
	{
		foreach (string key in template.Values.Keys)
		{
			string name = key;
			if (key.Contains("android:"))
			{
				name = key.Replace("android:", "android___");
			}
			XmlAttribute xmlAttribute = doc.CreateAttribute(name);
			xmlAttribute.Value = template.Values[key];
			parent.Attributes.Append(xmlAttribute);
		}
	}
}
