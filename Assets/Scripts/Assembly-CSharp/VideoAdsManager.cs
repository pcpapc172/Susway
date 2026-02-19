using UnityEngine;

public class VideoAdsManager : VideoAdProvider
{
	private enum State
	{
		Uninitialized = 0,
		ReadyToPlay = 1,
		TakeoverActive = 2
	}

	private const int MAX_PROVIDERS_TO_INIT_PER_CALL = 1;

	private const string ONLINESETTINGS_PROVIDERLIST_KEY = "videoads_providerlist";

	private const string PROVIDER_ADCOLONY = "adcolony";

	private const string PROVIDER_FLURRYCLIPS = "flurryclips";

	private const string PROVIDER_VUNGLECLIPS = "vungleclips";

	private static readonly string[] DEFAULT_PROVIDER_LIST = new string[1] { "vungleclips" };

	private State _state;

	private VideoAdProvider _adColonyProvider;

	private VideoAdProvider _flurryClipsProvider;

	private VideoAdProvider _vungleClipsProvider;

	private static VideoAdsManager _instance;

	private bool _gameSoundOnBeforeMute;

	public static VideoAdsManager instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new VideoAdsManager();
			}
			return _instance;
		}
	}

	public override bool isInitialized
	{
		get
		{
			return _state != State.Uninitialized;
		}
	}

	private VideoAdsManager()
	{
	}

	private string[] GetProviderList()
	{
		string[] array = DEFAULT_PROVIDER_LIST;
		string valueString;
		if (OnlineSettings.instance.TryGetValue("videoads_providerlist", out valueString))
		{
			if (string.IsNullOrEmpty(valueString))
			{
				array = new string[0];
			}
			else
			{
				array = valueString.Split(',');
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = array[i].Trim();
				}
			}
		}
		return array;
	}

	private VideoAdProvider GetProvider(string providerId)
	{
		switch (providerId)
		{
		case "adcolony":
			if (_adColonyProvider == null && AdColonyProvider.isVideoProviderSupported)
			{
				_adColonyProvider = AdColonyProvider.instance;
				_adColonyProvider.AddTakeoverBeganHandler(OnProviderTakeoverBegan);
				_adColonyProvider.AddTakeoverEndedHandler(OnProviderTakeoverEnded);
				_adColonyProvider.AddVideoWatchedHandler(OnProviderVideoWatched);
				_adColonyProvider.AddVideoUnavailableHandler(OnProviderVideoUnavailable);
			}
			return _adColonyProvider;
		case "flurryclips":
			if (_flurryClipsProvider == null && FlurryClipsProvider.isVideoProviderSupported)
			{
				_flurryClipsProvider = FlurryClipsProvider.instance;
				_flurryClipsProvider.AddTakeoverBeganHandler(OnProviderTakeoverBegan);
				_flurryClipsProvider.AddTakeoverEndedHandler(OnProviderTakeoverEnded);
				_flurryClipsProvider.AddVideoWatchedHandler(OnProviderVideoWatched);
				_flurryClipsProvider.AddVideoUnavailableHandler(OnProviderVideoUnavailable);
			}
			return _flurryClipsProvider;
		case "vungleclips":
			if (_vungleClipsProvider == null && VungleClipsProvider.isVideoProviderSupported)
			{
				_vungleClipsProvider = VungleClipsProvider.instance;
				_vungleClipsProvider.AddTakeoverBeganHandler(OnProviderTakeoverBegan);
				_vungleClipsProvider.AddTakeoverEndedHandler(OnProviderTakeoverEnded);
				_vungleClipsProvider.AddVideoWatchedHandler(OnProviderVideoWatched);
				_vungleClipsProvider.AddVideoUnavailableHandler(OnProviderVideoUnavailable);
			}
			return _vungleClipsProvider;
		default:
			Debug.LogWarning("VideoAdsManager WARNING: Unknown provider id: " + providerId);
			return null;
		}
	}

	private string GetProviderId(VideoAdProvider provider)
	{
		if (provider != null)
		{
			if (provider == _adColonyProvider)
			{
				return "adcolony";
			}
			if (provider == _flurryClipsProvider)
			{
				return "flurryclips";
			}
			if (provider == _vungleClipsProvider)
			{
				return "vungleclips";
			}
		}
		Debug.LogError("VideoAdsManager GetProviderId() ERROR: Unknown provider");
		return null;
	}

	public override void Init()
	{
	}

	public override bool PlayVideoIfAvailable(int defaultReward)
	{
		return false;
	}

	private void OnProviderTakeoverBegan(VideoAdProvider provider)
	{
		if (_state != State.ReadyToPlay)
		{
			Debug.LogWarning(string.Concat("VideoAdsManager: Unexpected state in OnProviderTakeoverBegan (", _state, ")"));
		}
		_state = State.TakeoverActive;
		MuteSound();
		InvokeHandlerIfNotNull(_onTakeoverBegan, this);
	}

	private void OnProviderTakeoverEnded(VideoAdProvider provider)
	{
		if (_state != State.TakeoverActive)
		{
			Debug.LogWarning(string.Concat("VideoAdsManager: Unexpected state in OnProviderTakeoverEnded (", _state, ")"));
		}
		_state = State.ReadyToPlay;
		RestoreSound();
		InvokeHandlerIfNotNull(_onTakeoverEnded, this);
	}

	private void OnProviderVideoWatched(VideoAdProvider provider, int rewardAmount)
	{
		_state = State.ReadyToPlay;
		VideoWatched(rewardAmount);
		InvokeHandlerIfNotNull(_onVideoWatched, this, rewardAmount);
	}

	private void OnProviderVideoUnavailable(VideoAdProvider provider)
	{
		string providerId = GetProviderId(provider);
		if (string.IsNullOrEmpty(providerId))
		{
			return;
		}
		bool flag = false;
		string[] providerList = GetProviderList();
		string[] array = providerList;
		foreach (string text in array)
		{
			if (!flag)
			{
				if (text == providerId)
				{
					flag = true;
				}
				continue;
			}
			VideoAdProvider provider2 = GetProvider(text);
			if (provider2 != null)
			{
				if (!provider2.isInitialized)
				{
					provider2.Init();
				}
				break;
			}
		}
	}

	private void MuteSound()
	{
		_gameSoundOnBeforeMute = Settings.optionSound;
		Settings.optionSound = false;
	}

	private void RestoreSound()
	{
		Settings.optionSound = _gameSoundOnBeforeMute;
	}

	private void VideoWatched(int rewardAmount)
	{
		if (rewardAmount > 0)
		{
			PlayerInfo.Instance.amountOfCoins += rewardAmount;
			PlayerInfo.Instance.SaveIfDirty();
		}
	}
}
