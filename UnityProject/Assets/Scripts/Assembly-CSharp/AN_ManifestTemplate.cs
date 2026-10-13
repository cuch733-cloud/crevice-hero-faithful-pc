using System.Collections.Generic;
using System.Xml;

public class AN_ManifestTemplate : AN_BaseTemplate
{
	private AN_ApplicationTemplate _applicationTemplate;

	private List<AN_PropertyTemplate> _permissions;

	public AN_ApplicationTemplate ApplicationTemplate
	{
		get
		{
			return _applicationTemplate;
		}
	}

	public List<AN_PropertyTemplate> Permissions
	{
		get
		{
			return _permissions;
		}
	}

	public AN_ManifestTemplate()
	{
		_applicationTemplate = new AN_ApplicationTemplate();
		_permissions = new List<AN_PropertyTemplate>();
	}

	public bool HasPermission(string name)
	{
		foreach (AN_PropertyTemplate permission in Permissions)
		{
			if (permission.Name.Equals(name))
			{
				return true;
			}
		}
		return false;
	}

	public void RemovePermission(string name)
	{
		while (HasPermission(name))
		{
			foreach (AN_PropertyTemplate permission in Permissions)
			{
				if (permission.Name.Equals(name))
				{
					RemovePermission(permission);
					break;
				}
			}
		}
	}

	public void RemovePermission(AN_PropertyTemplate permission)
	{
		_permissions.Remove(permission);
	}

	public void AddPermission(string name)
	{
		if (!HasPermission(name))
		{
			AN_PropertyTemplate aN_PropertyTemplate = new AN_PropertyTemplate("uses-permission");
			aN_PropertyTemplate.Name = name;
			AddPermission(aN_PropertyTemplate);
		}
	}

	public void AddPermission(AN_PropertyTemplate permission)
	{
		_permissions.Add(permission);
	}

	public override void ToXmlElement(XmlDocument doc, XmlElement parent)
	{
		AddAttributesToXml(doc, parent, this);
		AddPropertiesToXml(doc, parent, this);
		XmlElement xmlElement = doc.CreateElement("application");
		_applicationTemplate.ToXmlElement(doc, xmlElement);
		parent.AppendChild(xmlElement);
		foreach (AN_PropertyTemplate permission in Permissions)
		{
			XmlElement xmlElement2 = doc.CreateElement("uses-permission");
			permission.ToXmlElement(doc, xmlElement2);
			parent.AppendChild(xmlElement2);
		}
	}
}
