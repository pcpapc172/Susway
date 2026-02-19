using UnityEngine;

public class ChangeLogPopup : CharacterPopup
{
	[SerializeField]
	private UILabel topTitle;

	[SerializeField]
	private UILabel mainLabel;

	[SerializeField]
	private UILabel buttonText;

	private void OnEnable()
	{
		SetCharacter(Characters.CharacterType.fresh);
		topTitle.text = "Thanks\nfor\nUpdating!";
		mainLabel.text = "* 5 cool CHARACTERS\n* New MISSIONS\n* Awesome daily REWARDS\n* Up your game with the DOUBLE COIN Booster";
		buttonText.text = "OK";
	}

	private void OkClicked()
	{
		Flurry.LogEventWithAParameter("POPUP Screen ChangeLogPopup", "Result", "Ok");
		UIScreenController.Instance.ClosePopup();
	}

	private void CloseClicked()
	{
		Flurry.LogEventWithAParameter("POPUP Screen ChangeLogPopup", "Result", "Cancel");
		UIScreenController.Instance.ClosePopup();
	}
}
