using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Prime31;
using UnityEngine;

public class FacebookBinding
{
	static FacebookBinding()
	{
		FacebookManager.loginSucceededEvent += delegate
		{
			Facebook.instance.accessToken = getAccessToken();
		};
	}

	[DllImport("__Internal")]
	private static extern void _facebookInit(string applicationId);

	public static void init(string applicationId)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_facebookInit(applicationId);
		}
		Facebook.instance.accessToken = getAccessToken();
	}

	[DllImport("__Internal")]
	private static extern bool _facebookIsLoggedIn();

	public static bool isSessionValid()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			return _facebookIsLoggedIn();
		}
		return false;
	}

	[DllImport("__Internal")]
	private static extern string _facebookGetFacebookAccessToken();

	public static string getAccessToken()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			return _facebookGetFacebookAccessToken();
		}
		return string.Empty;
	}

	[DllImport("__Internal")]
	private static extern void _facebookExtendAccessToken();

	public static void extendAccessToken()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_facebookExtendAccessToken();
		}
	}

	public static void login()
	{
		loginWithRequestedPermissions(new string[0]);
	}

	public static void loginWithRequestedPermissions(string[] permissions)
	{
		loginWithRequestedPermissions(permissions, null);
	}

	[DllImport("__Internal")]
	private static extern void _facebookLoginWithRequestedPermissions(string perms, string urlSchemeSuffix);

	public static void loginWithRequestedPermissions(string[] permissions, string urlSchemeSuffix)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			string perms = string.Join(",", permissions);
			_facebookLoginWithRequestedPermissions(perms, urlSchemeSuffix);
		}
	}

	[DllImport("__Internal")]
	private static extern void _facebookLogout();

	public static void logout()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_facebookLogout();
		}
	}

	public static void showPostMessageDialog()
	{
		showPostMessageDialogWithOptions(null, null, null, null);
	}

	public static void showPostMessageDialogWithOptions(string link, string linkName, string linkToImage, string caption)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("link", link);
		dictionary.Add("name", linkName);
		dictionary.Add("picture", linkToImage);
		dictionary.Add("caption", caption);
		Dictionary<string, string> options = dictionary;
		showDialog("stream.publish", options);
	}

	[DllImport("__Internal")]
	private static extern void _facebookShowDialog(string dialogType, string json);

	public static void showDialog(string dialogType, Dictionary<string, string> options)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_facebookShowDialog(dialogType, MiniJSON.jsonEncode(options));
		}
	}

	[DllImport("__Internal")]
	private static extern void _facebookRestRequest(string restMethod, string httpMethod, string jsonDict);

	public static void restRequest(string restMethod, string httpMethod, Hashtable keyValueHash)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			string text = MiniJSON.jsonEncode(keyValueHash);
			if (text != null)
			{
				_facebookRestRequest(restMethod, httpMethod, text);
			}
		}
	}
}
