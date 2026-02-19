public static class Flurry
{
	public const string EVENT_UISCREEN_CHANGED_PREFIX = "UI Screen ";

	public const string EVENT_POPUPSCREEN_CHANGED_PREFIX = "POPUP Screen ";

	public const string EVENT_10_SOCIAL_ACTIONS_TAKEN = "10 social actions taken";

	public const string EVENT_FIRST_GAMECENTER_LOGIN = "First GameCenter Login";

	public const string EVENT_FIRST_FACEBOOK_LOGIN = "First Facebook Login";

	public const string EVENT_SOCIAL_POKE = "Social friend poked";

	public const string EVENT_SOCIAL_BRAG = "Social bragged";

	public const string EVENT_SOCIAL_BRAGFACEBOOK = "Social bragged Facebook";

	public const string EVENT_MYSTERY_BOX_OPENED = "Mystery Box opened";

	public const string EVENT_INAPPPURCHASE_COMPLETED = "InApp purchase completed";

	public const string EVENT_INAPPPURCHASE_COINPACK1 = "InApp Coin Pack 1 purchased";

	public const string EVENT_INAPPPURCHASE_COINPACK2 = "InApp Coin Pack 2 purchased";

	public const string EVENT_INAPPPURCHASE_COINPACK3 = "InApp Coin Pack 3 purchased";

	public const string EVENT_INAPPPURCHASE_COINPACK4 = "InApp Coin Pack 4 purchased";

	public const string EVENT_INAPPPURCHASE_COINPACK5 = "InApp Coin Pack 5 purchased";

	public const string EVENT_INAPPPURCHASE_DOUBLECOINS = "Double Coin purchased";

	public const string EVENT_INAPPPURCHASE_DOUBLECOINS_POPUP = "Double Coin purchased";

	public const string EVENT_CHARACTER_UNLOCKED = "Character unlocked";

	public const string EVENT_AUTOMESSAGE_TURNED_OFF = "AutoBrag turned off";

	public const string EVENT_SEASON_TURNED_OFF = "Season turned off";

	public const string EVENT_MISSIONSET_COMPLETED = "Mission Set completed";

	public const string EVENT_DAILY_CHALLENGE_COMPLETED = "Daily Challenge completed";

	public const string EVENT_UPDATE_APP_POPUP_RESULT = "New Version Popup Result";

	public const string EVENT_FILEUTIL_LOAD_CORRUPTED = "FileUtil load corrupted";

	public const string EVENT_VIDEOADS_REQUEST = "VideoAds request";

	public const string EVENT_VIDEOADS_PROVIDER_REQUEST = "VideoAds {0} request";

	public const string EVENT_FRIENDREWARD_COLLECTED = "Friend reward collect";

	public const string EVENT_EARNCOINS_ITEM_CLICKED = "Earn Coins item clicked";

	public const string EVENT_BOOST_HEADSTART500_PURCHASED = "Boost Headstart500 purchased";

	public const string EVENT_BOOST_HEADSTART2000_PURCHASED = "Boost Headstart2000 purchased";

	public const string EVENT_BOOST_HOVERBOARD_PURCHASED = "Boost Hoverboard purchased";

	public const string EVENT_BOOST_COINMAGNET_PURCHASED = "Boost Coinmagnet purchased";

	public const string EVENT_BOOST_DOUBLEMULTIPLIER_PURCHASED = "Boost 2x multiplier purchased";

	public const string EVENT_BOOST_JETPACK_PURCHASED = "Boost jetpack purchased";

	public const string EVENT_BOOST_LETTERS_PURCHASED = "Boost letters purchased";

	public const string EVENT_BOOST_SUPERSNEAKERS_PURCHASED = "Boost supersneakers purchased";

	public const string EVENT_BOOST_MYSTERYBOX_PURCHASED = "Boost MysteryBox purchased";

	public const string EVENT_BOOST_MISSION_SKIP_PURCHASED = "Boost Mission Skip purchased";

	public const string EVENT_BOOST_MYSTERYBOX_VIEW_PRIZES = "Mysterybox view prices";

	public const string EVENT_ARGKEY_ID = "Id";

	public const string EVENT_ARGKEY_TIER = "Tier";

	public const string EVENT_ARGKEY_UI_SCREENNAME = "Screen Name";

	public const string EVENT_ARGKEY_MISSIONSET = "Mission Set";

	public const string EVENT_ARGKEY_MISSIONSET_AND_INDEX = "Mission Set and Index";

	public const string EVENT_ARGKEY_TOTAL = "Total";

	public const string EVENT_ARGKEY_POPUPRESULT = "Result";

	public const string EVENT_ARGKEY_FILENAME = "Filename";

	public const string EVENT_ARGKEY_VIDEOADRESULT = "Result";

	public const string EVENT_ARGKEY_ITEM_IAP = "Item tiggered iap";

	public const string EVENT_ARGKEY_POPUPRESULT_OK = "Ok";

	public const string EVENT_ARGKEY_POPUPRESULT_CANCEL = "Cancel";

	public const string EVENT_ARGKEY_VIDEOADRESULT_OK = "Ok";

	public const string EVENT_ARGKEY_VIDEOADRESULT_NOVIDEO = "No video";

	public static void LogGenericSocialAction()
	{
	}

	public static void LogGameCenterLogin()
	{
	}

	public static void LogFacebookLogin()
	{
	}

	public static void StartSession(string apiKey)
	{
	}

	public static void SetUserInfo(string userId)
	{
	}

	public static void SetUserInfo(string userId, int age, int gender)
	{
	}

	public static void LogEvent(string eventName)
	{
	}

	public static void LogEventWithAParameter(string eventName, string argKey, string argValue)
	{
	}

	public static void LogEventWithSeveralParameters(string eventName, string argKeys, string argValues)
	{
	}

	public static void LogError(string errorName, string message)
	{
	}

	private static void SetUserInfoAndroid(string userId, int age, int gender)
	{
	}

	private static void LogEventAndroid(string eventName)
	{
	}

	private static void LogEventWithAParameterAndroid(string eventName, string argKey, string argValue)
	{
	}

	private static void LogEventWithSeveralParametersAndroid(string eventName, string argKeys, string argValues)
	{
	}

	private static void LogErrorAndroid(string errorName, string message)
	{
	}
}
