using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class SocialManagerIOS : MonoBehaviour
{
	private static SocialManagerIOS _instance;

	private bool _gameCenterAuthenticationComplete;

	private Dictionary<string, IUserProfile> _gcFriends;

	public bool GameCenterAuthenticationComplete
	{
		get
		{
			return _gameCenterAuthenticationComplete;
		}
		set
		{
			_gameCenterAuthenticationComplete = value;
		}
	}

	public static SocialManagerIOS instance
	{
		get
		{
			Init();
			return _instance;
		}
	}

	private static void Init()
	{
		if (_instance == null)
		{
			GameObject gameObject = new GameObject();
			gameObject.name = "SocialManagerIOS";
			Object.DontDestroyOnLoad(gameObject);
			gameObject.AddComponent<SocialManagerIOS>();
		}
	}

	private void Awake()
	{
		_instance = this;
	}
}
