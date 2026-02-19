using System.Collections.Generic;
using UnityEngine;

public class ChartBoostManager
{
	private const string ALLOWNEXT_TICKS_KEY = "cb_alwnxt_ticks";

	private const int FIRSTTIME_DELAY_SECONDS = 72000;

	private const int DEFAULT_DELAY_SECONDS = 30;

	private const string DELAY_SECONDS_ONLINESETTINGSKEY = "chartboost_delay_seconds";

	private static readonly List<string> ALLOWED_SHOW_SCREENS = new List<string> { "FrontUI" };

	private static readonly List<string> ALLOWED_CACHE_SCREENS = new List<string> { "CoinsUI_shop", "CharacterScreen", "GameoverUI", "TokensUI", "UpgradesUI_shop" };

	private static ChartBoostManager _instance;

	private bool _isCaching;

	public bool isInstanced
	{
		get
		{
			return _instance != null;
		}
	}

	public static ChartBoostManager instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new ChartBoostManager();
			}
			return _instance;
		}
	}

	private bool interstitialsEnabled
	{
		get
		{
			if (Application.systemLanguage == SystemLanguage.Chinese)
			{
				return true;
			}
			bool flag = Screen.height >= 2500;
			return PlayerInfo.Instance.inAppPurchaseCount <= 0 && !flag;
		}
	}

	private ChartBoostManager()
	{
	}

	public void GameScreenChanged(string screenName)
	{
		if (string.IsNullOrEmpty(screenName))
		{
			Debug.LogError("ChartBoostManager.GameScreenChanged() invalid screenName: " + screenName);
		}
		else
		{
			ShowOrCacheForScreen(screenName);
		}
	}

	public void LastQueuedPopupsClosed(string currentScreenName)
	{
		if (string.IsNullOrEmpty(currentScreenName))
		{
			Debug.LogError("ChartBoostManager.LastQueuedPopupsClosed() invalid screenName: " + currentScreenName);
		}
		else
		{
			ShowOrCacheForScreen(currentScreenName);
		}
	}

	public void CacheNow()
	{
	}

	private void ShowOrCacheForScreen(string screenName)
	{
	}

	private bool ChartBoostHasCachedInterstitial()
	{
		return ChartBoostAndroid.hasCachedInterstitial(null);
	}

	private void ChartBoostShowInterstitial()
	{
		ChartBoostAndroid.showInterstitial(null);
	}

	private void CacheNowOnAndroid()
	{
	}

	private void OnDidCacheInterstitial(string location)
	{
		_isCaching = false;
	}

	private void OnDidFailToLoadInterstitial(string location)
	{
		_isCaching = false;
	}

	private void didFinishInterstitialEvent(string param)
	{
		OnDidCacheInterstitial(null);
	}

	private void didFailToLoadInterstitialEvent()
	{
		OnDidFailToLoadInterstitial(null);
	}
}
