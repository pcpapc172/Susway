using System;
using System.Globalization;
using UnityEngine;

public class HouseKeeper : MonoBehaviour
{
	private const string EVENT_SEASON_RUNNING_KEY = "season";

	private const string END_SEASON_DATETIME = "end_season_datetime";

	private const string ANDROID_FLURRY_GAMEOBJECT_NAME = "FlurryAndroidGameObject";

	private const string ANDROID_IN_APP_BILLING_GAMEOBJECT_NAME = "InAppBillingAndroidGameObject";

	private const string ANDROID_AD_COLONY_GAMEOBJECT_NAME = "AdColony";

	private const string ANDROID_CHARTBOOST_GAMEOBJECT_NAME = "ChartBoost";

	private const float MIN_REFRESH_INTERVAL = 3600f;

	private const string REFRESH_INTERVAL_KEY = "refreshinterval";

	private static float _onlineSettingsLastDownloadTime = float.NegativeInfinity;

	private void Start()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		FlurryClips.InitAndEnableVideoAds();
		Flurry.StartSession("YR898G65YFPWNMQ6X5H5");
		RefreshOnlineSettingsAndInappsIfNeeded();
		ChartBoostManager.instance.CacheNow();
		CreateAndroidFlurryGameObject();
		CreateAndroidEtceteraGameObject();
		CreateAndroidInAppBillingCallbackObject();
		CreateAndroidIAdColonyCallbackObject();
		AudioListener.volume = 0f;
		SocialManager.Init();
		SocialManager.instance.AddFriendsConsolidatedHandler(CheckForLoginBonus);
		InAppManager.Init();
		Layers instance = Layers.Instance;
		CheckForSeason();
		AddVungleCallbackObject();
	}

	private void CheckForSeason()
	{
        DateTime seasonExpirationDateTime = GetSeasonExpirationDateTime();
        // Don't overwrite a manual override's expiration date
        if (!ThemeManager.Instance.ManualOverride)
        {
            ThemeManager.Instance.themeExpirationDate = seasonExpirationDateTime;
        }
		PlayerInfo instance = PlayerInfo.Instance;
		string valueString = string.Empty;
		if (OnlineSettings.instance.TryGetValue("season", out valueString))
		{
			Theme nORMAL = Theme.NORMAL;
			if (valueString.Equals("halloween"))
			{
				instance.currentSeasonAvailable = PlayerInfo.Season.halloween;
				nORMAL = Theme.HALLOWEEN;
				Debug.Log("Halloween time");
			}
			else if (valueString.Equals("xmas"))
			{
				nORMAL = Theme.NORMAL;
				instance.currentSeasonAvailable = PlayerInfo.Season.none;
				Debug.Log("xmas time");
			}
			else if (valueString.Equals("easter"))
			{
				nORMAL = Theme.NORMAL;
				instance.currentSeasonAvailable = PlayerInfo.Season.none;
				Debug.Log("easter time");
			}
			else if (valueString.Equals("normal"))
			{
				nORMAL = Theme.NORMAL;
				instance.currentSeasonAvailable = PlayerInfo.Season.none;
				Debug.Log("normal time");
			}
			else
			{
				instance.currentSeasonAvailable = Globals.UPDATE_DEFAULT_SEASON;
				nORMAL = Globals.UPDATE_DEFAULT_THEME;
				Debug.Log("Default theme");
			}
            if (PlayerPrefs.GetInt("OPTION_SEASON", 1) != 0)
            {
                instance.currentSeasonPicked = instance.currentSeasonAvailable;
                // Respect manual override if present
                if (!ThemeManager.Instance.ManualOverride)
                {
                    ThemeManager.Instance.Theme = nORMAL;
                }
            }
			else
			{
				ThemeManager.Instance.Theme = Theme.NORMAL;
			}
		}
		else
		{
			instance.currentSeasonAvailable = Globals.UPDATE_DEFAULT_SEASON;
            if (PlayerPrefs.GetInt("OPTION_SEASON", 1) != 0)
            {
                instance.currentSeasonPicked = instance.currentSeasonAvailable;
                if (!ThemeManager.Instance.ManualOverride)
                {
                    ThemeManager.Instance.Theme = Globals.UPDATE_DEFAULT_THEME;
                    Debug.Log("Default theme: not online settings");
                }
            }
			else
			{
				ThemeManager.Instance.Theme = Theme.NORMAL;
			}
		}
        if (PlayerInfo.Instance.currentSeasonAvailable != PlayerInfo.Season.none)
        {
            // Respect manual override: do not auto-revert if the user manually set a theme
            if (!ThemeManager.Instance.ManualOverride && ThemeManager.Instance.themeForSeason(PlayerInfo.Instance.currentSeasonAvailable).TimeToExpire.Ticks < 0)
            {
                Debug.Log("Theme expired!");
                ThemeManager.Instance.Theme = Theme.NORMAL;
                instance.currentSeasonAvailable = PlayerInfo.Season.none;
                instance.currentSeasonPicked = instance.currentSeasonAvailable;
            }
        }
        else
        {
            if (!ThemeManager.Instance.ManualOverride)
            {
                ThemeManager.Instance.Theme = Theme.NORMAL;
            }
        }
	}

	public DateTime GetSeasonExpirationDateTime()
	{
		string text = "dd-MM-yyyy hh:mm:ss";
		PlayerInfo instance = PlayerInfo.Instance;
		DateTime result;
		if (instance.currentSeasonExpirationDate != null)
		{
			string currentSeasonExpirationDate = instance.currentSeasonExpirationDate;
			result = DateTime.Parse(currentSeasonExpirationDate, new CultureInfo("da-DK"));
		}
		else
		{
			result = DateTime.Parse("05-11-2012 00:00:00", new CultureInfo("da-DK"));
			instance.currentSeasonExpirationDate = result.ToString(text);
		}
		string valueString;
		if (OnlineSettings.instance.TryGetValue("end_season_datetime", out valueString))
		{
			try
			{
				DateTime dateTime = DateTime.Parse(valueString, new CultureInfo("da-DK"));
				result = dateTime;
				instance.currentSeasonExpirationDate = result.ToString(text);
			}
			catch (FormatException)
			{
				Debug.Log(string.Format("Unable to convert '{0}'.", valueString));
				Debug.Log("Reading from online failed ");
			}
		}
		return result;
	}

	private void AddVungleCallbackObject()
	{
		GameObject gameObject = new GameObject("VungleManager");
		gameObject.name = "VungleManager";
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
		gameObject.AddComponent<VungleManager>();
	}

	private void OnApplicationPause(bool pause)
	{
		if (!pause)
		{
			if (DailyWord.Instance != null)
			{
				DailyWord.Instance.ForceSync();
			}
			CheckForSeason();
		}
	}

	private void CreateAndroidFlurryGameObject()
	{
		GameObject gameObject = new GameObject();
		gameObject.name = "FlurryAndroidGameObject";
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
		gameObject.AddComponent<FlurryInit>();
	}

	private void CreateAndroidEtceteraGameObject()
	{
		GameObject original = Resources.Load("Prefabs/Plugins/EtceteraAndroidManager", typeof(GameObject)) as GameObject;
		GameObject gameObject = UnityEngine.Object.Instantiate(original) as GameObject;
	}

	private void CreateAndroidInAppBillingCallbackObject()
	{
		GameObject gameObject = new GameObject("InAppBillingAndroidGameObject");
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
		gameObject.AddComponent<RRInappBillingCallback>();
	}

	private void CreateAndroidIAdColonyCallbackObject()
	{
		GameObject gameObject = new GameObject("AdColony");
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
		gameObject.AddComponent<AdColonyAndroid>();
	}

	private void CreateAndroidIChartBoostCallbackObject()
	{
		GameObject gameObject = new GameObject("ChartBoost");
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
		gameObject.AddComponent<ChartBoostAndroidManager>();
	}

	public void CheckForLoginBonus()
	{
		if (SocialManager.instance.facebookIsLoggedIn && !PlayerInfo.Instance.hasPayedOutFacebook)
		{
			PlayerInfo.Instance.amountOfCoins += 5000;
			PlayerInfo.Instance.hasPayedOutFacebook = true;
			PlayerInfo.Instance.SaveIfDirty();
			UIScreenController.QueueFacebookPayoutPopup();
		}
		if (Social.localUser.authenticated && !PlayerInfo.Instance.hasPayedOutGameCenter)
		{
			PlayerInfo.Instance.amountOfCoins += 250;
			PlayerInfo.Instance.hasPayedOutGameCenter = true;
			PlayerInfo.Instance.SaveIfDirty();
			UIScreenController.QueueGameCenterPayoutPopup();
		}
	}

	public static void RefreshOnlineSettingsAndInappsIfNeeded()
	{
		float num = 3600f;
		string valueString;
		if (OnlineSettings.instance.TryGetValue("refreshinterval", out valueString))
		{
			float result;
			if (float.TryParse(valueString, out result))
			{
				if (result >= 10f)
				{
					num = result;
				}
				else
				{
					Debug.LogError("OnlineSettings refresh interval too small: " + num);
				}
			}
			else
			{
				Debug.LogError("Failed to parse Onlinesettings refresh interval from: " + valueString);
			}
		}
		if (Time.realtimeSinceStartup > _onlineSettingsLastDownloadTime + num)
		{
			_onlineSettingsLastDownloadTime = Time.realtimeSinceStartup;
			if (!OnlineSettings.instance.isDownloading)
			{
				OnlineSettings.instance.DownloadNow();
			}
			InAppManager.Instance.QueryInApps();
		}
	}
}
