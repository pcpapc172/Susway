using System;
using UnityEngine;

public class UISocialScreen : MonoBehaviour
{
	public FriendHandlerHighScore _highScoreHandler;

	public FriendHandlerCrew _crewHandler;

	private unsafe void OnEnable()
	{
		SocialManager.instance.AddFriendsConsolidatedHandler(new Action(this, (IntPtr)__ldftn(UISocialScreen.ReloadFriends)));
	}

	private unsafe void OnDisable()
	{
		SocialManager.instance.RemoveFriendsConsolidatedHandler(new Action(this, (IntPtr)__ldftn(UISocialScreen.ReloadFriends)));
	}

	public void ReloadFriends(bool val = false)
	{
		if (_highScoreHandler != null)
		{
			_highScoreHandler.LoadHighScore();
		}
		else if (_crewHandler != null)
		{
			_crewHandler.InitCrew();
		}
	}
}
