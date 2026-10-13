using System.Collections;
using System.Collections.Generic;
using UnionAssets.FLE;
using UnityEngine;

public abstract class TW_APIRequest : EventDispatcher
{
	private bool IsFirst = true;

	private string GetParams = string.Empty;

	private string requestUrl;

	private Dictionary<string, string> Headers = new Dictionary<string, string>();

	public void Send()
	{
		if (SA_Singleton<TwitterApplicationOnlyToken>.instance.currentToken == null)
		{
			SA_Singleton<TwitterApplicationOnlyToken>.instance.addEventListener("complete", OnTokenLoaded);
			SA_Singleton<TwitterApplicationOnlyToken>.instance.RetrieveToken();
		}
		else
		{
			StartCoroutine(Request());
		}
	}

	public void AddParam(string name, int value)
	{
		AddParam(name, value.ToString());
	}

	public void AddParam(string name, string value)
	{
		if (!IsFirst)
		{
			GetParams += "&";
		}
		else
		{
			GetParams += "?";
		}
		GetParams = GetParams + name + "=" + value;
		IsFirst = false;
	}

	protected void SetUrl(string url)
	{
		requestUrl = url;
	}

	private IEnumerator Request()
	{
		requestUrl += GetParams;
		Headers.Add("Content-Type", "application/x-www-form-urlencoded;charset=UTF-8");
		Headers.Add("Authorization", "Bearer " + SA_Singleton<TwitterApplicationOnlyToken>.instance.currentToken);
		WWW www = new WWW(requestUrl, null, Headers);
		yield return www;
		if (www.error == null)
		{
			OnResult(www.text);
		}
		else
		{
			dispatch("complete", new TW_APIRequstResult(false, www.error));
		}
		Object.Destroy(base.gameObject);
	}

	protected abstract void OnResult(string data);

	private void OnTokenLoaded()
	{
		SA_Singleton<TwitterApplicationOnlyToken>.instance.removeEventListener("complete", OnTokenLoaded);
		if (SA_Singleton<TwitterApplicationOnlyToken>.instance.currentToken != null)
		{
			StartCoroutine(Request());
		}
		else
		{
			dispatch("complete", new TW_APIRequstResult(false, "Retirving auth tocen failed"));
		}
	}
}
