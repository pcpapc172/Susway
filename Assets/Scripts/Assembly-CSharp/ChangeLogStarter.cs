using UnityEngine;

public class ChangeLogStarter : MonoBehaviour
{
	private const string LAST_SEEN_BUNDLE_VERSION_KEY = "lastSeenBundleVersionKey";

	private void OnEnable()
	{
		FrontScreen.tweensFinishedAnimating += ShowPopUpsIfNeeded;
	}

	private void OnDisable()
	{
		FrontScreen.tweensFinishedAnimating -= ShowPopUpsIfNeeded;
	}

	private void Start()
	{
	}

	public static void ShowPopUpsIfNeeded()
	{
		bool flag = ShouldDisplayChangeLog();
	}

	private static bool ShouldDisplayChangeLog()
	{
		string bundleVersion = DeviceUtility.GetBundleVersion();
		string text = PlayerPrefs.GetString("lastSeenBundleVersionKey", string.Empty);
		if (text.Equals(bundleVersion))
		{
			return false;
		}
		PlayerPrefs.SetString("lastSeenBundleVersionKey", DeviceUtility.GetBundleVersion());
		return true;
	}
}
