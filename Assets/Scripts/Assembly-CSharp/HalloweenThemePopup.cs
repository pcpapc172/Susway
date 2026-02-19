using UnityEngine;

public class HalloweenThemePopup : CharacterPopup
{
	[SerializeField]
	private UILabel topTitle;

	[SerializeField]
	private UILabel centerTitle;

	[SerializeField]
	private UILabel bottomTitle;

	[SerializeField]
	private UILabel line1Big;

	[SerializeField]
	private UILabel line1Small;

	[SerializeField]
	private UILabel line2Big;

	[SerializeField]
	private UILabel line2Small;

	[SerializeField]
	private UILabel line3Big;

	[SerializeField]
	private UILabel line3Small;

	[SerializeField]
	private UILabel buttonText;

	private void OnEnable()
	{
		SetCharacter(Characters.CharacterType.zombiejake);
		topTitle.text = "Happy";
		centerTitle.text = "Halloween";
		bottomTitle.text = "Everyone!";
		line1Big.text = "Get SPECIAL Zombie Jake!";
		line1Small.text = "Only in the Shop for a LIMITED time";
		line2Big.text = "SPOOKTACULAR new look!";
		line2Small.text = "Get in the MOOD for the holiday!";
		line3Big.text = "Want the CLASSIC look?";
		line3Small.text = "You can switch back in SETTINGS!";
		buttonText.text = "OK";
	}

	private void OkClicked()
	{
		Flurry.LogEventWithAParameter("POPUP Screen HalloweenThemePopup", "Result", "Ok");
		UIScreenController.Instance.ClosePopup();
	}

	private void CloseClicked()
	{
		Flurry.LogEventWithAParameter("POPUP Screen HalloweenThemePopup", "Result", "Cancel");
		UIScreenController.Instance.ClosePopup();
	}
}
