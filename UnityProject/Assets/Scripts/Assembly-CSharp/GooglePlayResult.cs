using System;

public class GooglePlayResult
{
	private GP_GamesStatusCodes _response;

	private string _message;

	public GP_GamesStatusCodes response
	{
		get
		{
			return _response;
		}
	}

	public string message
	{
		get
		{
			return _message;
		}
	}

	public bool isSuccess
	{
		get
		{
			return _response == GP_GamesStatusCodes.STATUS_OK;
		}
	}

	public bool isFailure
	{
		get
		{
			return !isSuccess;
		}
	}

	public GooglePlayResult(GP_GamesStatusCodes code)
	{
		SetCode(code);
	}

	public GooglePlayResult(string code)
	{
		SetCode((GP_GamesStatusCodes)Convert.ToInt32(code));
	}

	private void SetCode(GP_GamesStatusCodes code)
	{
		_response = code;
		_message = _response.ToString();
	}
}
