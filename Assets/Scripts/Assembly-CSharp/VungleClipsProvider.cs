public class VungleClipsProvider : VideoAdProvider
{
	private bool _initialized;

	private int _defaultReward;

	private bool rewardFlag;

	private static VungleClipsProvider _instance;

	public static VungleClipsProvider instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new VungleClipsProvider();
			}
			return _instance;
		}
	}

	public static bool isVideoProviderSupported
	{
		get
		{
			return true;
		}
	}

	public override bool isInitialized
	{
		get
		{
			return _initialized;
		}
	}

	private VungleClipsProvider()
	{
	}

	public override void Init()
	{
	}

	private void HandleVungleManagervungleMoviePlayedEvent(string obj)
	{
	}

	public override bool PlayVideoIfAvailable(int defaultReward)
	{
		return false;
	}

	private void vungleMoviePlayedEventAndroid(string percentPlayed)
	{
		InvokeHandlerIfNotNull(_onVideoWatched, this, _defaultReward);
	}

	private void vungleViewDidDisappearEventAndroid()
	{
		InvokeHandlerIfNotNull(_onTakeoverEnded, this);
	}

	private void vungleViewWillAppearEventAndroid()
	{
		InvokeHandlerIfNotNull(_onTakeoverBegan, this);
	}
}
