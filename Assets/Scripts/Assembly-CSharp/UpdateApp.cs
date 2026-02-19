using UnityEngine;

public class UpdateApp : CharacterPopup
{
	private const string ONLINESETTINGS_LATESTVERSION_KEY = "latestversion";

	private const string ONLINESETTINGS_LATESTVERSION_CHANGELIST_KEY = "latestversion_changelist";

	private const string NEWVERSIONPOPUP_LAST_SHOWN_VERSION_KEY = "newverlastshown";

	private const string APPSTORE_URL = "https://play.google.com/store/apps/details?id=com.kiloo.subwaysurf";

	[SerializeField]
	private UILabel changeListLabel;

	private void OnEnable()
	{
		SetCharacter(Characters.CharacterType.fresh);
		string valueString;
		if (OnlineSettings.instance.TryGetValue("latestversion_changelist", out valueString))
		{
			changeListLabel.text = valueString + "\n";
			return;
		}
		Debug.LogError("Showing NewVersion popup, but no changeList found in OnlineSettings", this);
		changeListLabel.text = "New Content\n";
	}

	private void CloseClicked()
	{
		UIScreenController.Instance.ClosePopup();
		Flurry.LogEventWithAParameter("New Version Popup Result", "Result", "Cancel");
	}

	private void OkClicked()
	{
		UIScreenController.Instance.ClosePopup();
		Flurry.LogEventWithAParameter("New Version Popup Result", "Result", "Ok");
	}

	public static void ShowIfNeeded()
	{
	}
}
