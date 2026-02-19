using System;
using UnityEngine;

public class ChartBoostAndroidManager : MonoBehaviour
{
	public static event Action<string> didFinishInterstitialEvent;

	public static event Action<string> didFinishMoreAppsEvent;

	public static event Action didFailToLoadInterstitialEvent;

	public static event Action didFailToLoadMoreAppsEvent;

	private void Awake()
	{
		base.gameObject.name = GetType().ToString();
		UnityEngine.Object.DontDestroyOnLoad(this);
	}

	public void didFinishInterstitial(string param)
	{
		if (ChartBoostAndroidManager.didFinishInterstitialEvent != null)
		{
			ChartBoostAndroidManager.didFinishInterstitialEvent(param);
		}
	}

	public void didFinishMoreApps(string param)
	{
		if (ChartBoostAndroidManager.didFinishMoreAppsEvent != null)
		{
			ChartBoostAndroidManager.didFinishMoreAppsEvent(param);
		}
	}

	public void didFailToLoadInterstitial(string empty)
	{
		if (ChartBoostAndroidManager.didFailToLoadInterstitialEvent != null)
		{
			ChartBoostAndroidManager.didFailToLoadInterstitialEvent();
		}
	}

	public void didFailToLoadMoreApps(string empty)
	{
		if (ChartBoostAndroidManager.didFailToLoadMoreAppsEvent != null)
		{
			ChartBoostAndroidManager.didFailToLoadMoreAppsEvent();
		}
	}
}
