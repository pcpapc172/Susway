public class Globals
{
	public const bool DEBUG = false;

	public const bool DEBUG_SOCIAL_MANAGER_SERVER = false;

	public const bool DEBUG_FREE_PURCHASES = false;

	public const bool DEBUG_ALL_CHARS = false;

	public const bool DEBUG_ALWAYS_ONLINE = false;

	public const bool DEBUG_FREE_INAPP_PURCHASE = false;

	public const int MAX_MULTIPLIER = 30;

	public const int MAX_RANK = 1;

	public const string DEFAULT_ENDSEASON_DATETIME = "05-11-2012 00:00:00";

	public const int BONUS_FACEBOOK = 5000;

	public const int BONUS_GAMECENTER = 250;

	public const int MIN_FRIEND_SCORE_REQUEST_INTERVAL = 300;

	public const string FLURRY_API_KEY = "YR898G65YFPWNMQ6X5H5";

	public const string ADCOLONY_APPVERSION = "1.4";

	public const string ADCOLONY_APPID = "appc0d018638a3a47a3b725ab";

	public const string ADCOLONY_ZONEID = "vzc54d2d8389a24681852d05";

	public const string CHARTBOOST_APPID = "502e041317ba47dc7d000024";

	public const string CHARTBOOST_APPSIGNATURE = "e356ea8420eeb855ff880fba02ce0309364d0613";

	public const string VUNGLE_APPID = "507686ae771615941001aca5";

	public const int LAYER_2DGUI = 30;

	public const int LAYER_3DGUI = 31;

	public const int LAYER_2DOVERLAY = 28;

	public const int LAYER_3DCLIP = 29;

	public const float DRAG_THRESHOLD = 0.08f;

	public static PlayerInfo.Season UPDATE_DEFAULT_SEASON = PlayerInfo.Season.halloween;

	public static Theme UPDATE_DEFAULT_THEME = Theme.HALLOWEEN;

	public static Friend[] GetDebugFriends(int numberOfFriends = 10)
	{
		return new Friend[numberOfFriends];
	}
}
