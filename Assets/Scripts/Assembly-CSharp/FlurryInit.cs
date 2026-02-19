using UnityEngine;

public class FlurryInit : MonoBehaviour
{
	private const string FLURRY_ALLOW_NEW_SESSION = "flurry_allow_new_ss";

	private const int FLURRY_MINUTES_DELAY = 2;

	private void Awake()
	{
	}

	private void OnDestroy()
	{
	}

	private void OnApplicationPause(bool pause)
	{
	}
}
