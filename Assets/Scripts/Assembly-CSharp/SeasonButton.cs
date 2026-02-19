using UnityEngine;

public class SeasonButton : MonoBehaviour
{
	[SerializeField]
	private UISprite iconOff;

	[SerializeField]
	private UISprite icon;

	[SerializeField]
	private UILabel desc;

	[SerializeField]
	private UILabel title;

	private PlayerInfo playerInfoInstance;

	private void IconAndLabelUpdate()
	{
		if (playerInfoInstance.currentSeasonPicked == PlayerInfo.Season.halloween)
		{
			icon.spriteName = "icon_halloween";
			title.text = "Halloween ON";
			desc.text = "Disable/Enable\nHalloween theme";
			iconOff.GetComponent<UISprite>().enabled = false;
		}
		else if (playerInfoInstance.currentSeasonAvailable == PlayerInfo.Season.halloween)
		{
			icon.spriteName = "icon_halloween";
			title.text = "Halloween OFF";
			desc.text = "Disable/Enable\nHalloween theme";
			iconOff.GetComponent<UISprite>().enabled = true;
		}
		icon.MakePixelPerfect();
	}

	private void OnEnable()
	{
		playerInfoInstance = PlayerInfo.Instance;
		IconAndLabelUpdate();
	}

	private void Click()
	{
		if (playerInfoInstance.currentSeasonPicked == PlayerInfo.Season.none)
		{
			playerInfoInstance.currentSeasonPicked = playerInfoInstance.currentSeasonAvailable;
			Settings.optionSeason = true;
		}
		else
		{
			playerInfoInstance.currentSeasonPicked = PlayerInfo.Season.none;
			Settings.optionSeason = false;
		}
		IconAndLabelUpdate();
		if (playerInfoInstance.currentSeasonPicked == PlayerInfo.Season.none && playerInfoInstance.currentSeasonAvailable == PlayerInfo.Season.none)
		{
			Object.Destroy(base.gameObject);
			UIScreenController.Instance.ClosePopup(base.gameObject);
			UIScreenController.Instance.QueuePopup("SettingsPopup");
		}
		PlayerInfo.Instance.SaveIfDirty();
	}
}
