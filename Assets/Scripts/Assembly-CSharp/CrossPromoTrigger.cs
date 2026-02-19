using System;
using UnityEngine;

public class CrossPromoTrigger : MonoBehaviour
{
	private const string URL_TO_OPEN = "";

	private const string ALLOWAFTER_DATATIME_TICKS_KEY = "crprm_date_ticks";

	private const string HAS_SHOWN_KEY = "crprm_has_shown";

	private const int ALLOWAFTER_DELAY_HOURS = 6;

	private const int ALLOWAFTER_DELAY_MINUTES = 0;

	private void OnEnable()
	{
		FrontScreen.tweensFinishedAnimating += ShowPopUpIfNeeded;
	}

	private void OnDisable()
	{
		FrontScreen.tweensFinishedAnimating -= ShowPopUpIfNeeded;
	}

	private void ShowPopUpIfNeeded()
	{
		if (ShouldDisplayCrossPromo())
		{
			UIScreenController.Instance.QueuePopup("CrossPromoPopup");
		}
	}

	private bool ShouldDisplayCrossPromo()
	{
		if (!string.IsNullOrEmpty(string.Empty) && PlayerPrefs.GetInt("crprm_has_shown", 0) == 0)
		{
			DateTime dateTime;
			if (PlayerPrefs.HasKey("crprm_date_ticks"))
			{
				string s = PlayerPrefs.GetString("crprm_date_ticks");
				long result;
				if (!long.TryParse(s, out result))
				{
					result = DateTime.Now.Ticks;
				}
				dateTime = new DateTime(result);
			}
			else
			{
				dateTime = DateTime.Now + new TimeSpan(6, 0, 0);
				PlayerPrefs.SetString("crprm_date_ticks", dateTime.Ticks.ToString());
			}
			if (DateTime.Now >= dateTime)
			{
				PlayerPrefs.SetInt("crprm_has_shown", 1);
				return true;
			}
		}
		return false;
	}

	public static void OkButtonClicked()
	{
		UIScreenController.Instance.ClosePopup();
		Application.OpenURL(string.Empty);
		Flurry.LogEventWithAParameter("CrossPromoPopup", "Result", "Ok");
	}

	public static void CancelButtonClicked()
	{
		UIScreenController.Instance.ClosePopup();
		Flurry.LogEventWithAParameter("CrossPromoPopup", "Result", "Cancel");
	}
}
