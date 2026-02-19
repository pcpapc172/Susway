using System;
using UnityEngine;

public class VungleManager : MonoBehaviour
{
	public static event Action<string> vungleMoviePlayedEvent;

	public static event Action vungleViewDidDisappearEvent;

	public static event Action vungleViewWillAppearEvent;

	private void Awake()
	{
		base.gameObject.name = GetType().ToString();
		UnityEngine.Object.DontDestroyOnLoad(this);
	}

	public void vungleMoviePlayed(string percentPlayed)
	{
		if (VungleManager.vungleMoviePlayedEvent != null)
		{
			VungleManager.vungleMoviePlayedEvent(percentPlayed);
		}
	}

	public void vungleViewDidDisappear(string empty)
	{
		if (VungleManager.vungleViewDidDisappearEvent != null)
		{
			VungleManager.vungleViewDidDisappearEvent();
		}
	}

	public void vungleViewWillAppear(string empty)
	{
		if (VungleManager.vungleViewWillAppearEvent != null)
		{
			VungleManager.vungleViewWillAppearEvent();
		}
	}
}
