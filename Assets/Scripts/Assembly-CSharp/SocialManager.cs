using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Prime31;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class SocialManager : MonoBehaviour
{
	private enum FacebookCurrentRequest
	{
		None = 0,
		Error = 1,
		LoggingIn = 2
	}

	private enum WWWRequestResult
	{
		Success = 0,
		Error = 1
	}

	private delegate void WWWComplete(WWWRequestResult result, string output, object cookie);

	private const byte VERSION = 1;

	private const string REGISTER_DEVICE_URL = "/register2.php?android";

	private const string REPORT_SCORE_URL = "/report2.php?android";

	private const string CONSOLIDATE_FRIENDS_URL = "/friends2.php?android";

	private const string UPDATE_FRIEND_SCORES_URL = "/scores2.php?android";

	private const string POKE_URL = "/poke.php?android";

	private const string BRAG_URL = "/brag.php?android";

	private const float FACEBOOK_LOGIN_TIMEOUT = 600f;

	private const string FACEBOOK_APPID = "254616967963463";

	public const string BASE_URL = "http://hoodrunner.kiloo.com";

	private const string SECRET = "resxrctrv7tgv7gb8h9h9u0909kllfmolkjnhghgjjkhjghg";

	private const float CONSOLIDATE_MINIMUM_INTERVAL = 3600f;

	private static SocialManager _instance;

	private int _userid;

	private bool _isRunningFacebookLoginCoroutine;

	private Action<FacebookProfile> _facebookPictureDownloadedHandler;

	private Action _friendsConsolidatedHandler;

	private FacebookProfile _fbProfile;

	private List<Friend> _friends;

	private Dictionary<string, Hashtable> _fbFriends;

	private bool _fbReady;

	private double lastFriendScoreUpdateTimestamp;

	private FacebookCurrentRequest _fbCurrentRequest;

	private float _lastConsolidateFriendCompleteTime;

	private List<string> _latestFacebookIdsWithGameInstalled;

	private bool _consolidatedFriendsCompleted;

	private Dictionary<string, Friend.Status> _friendStatus;

	private bool _dirty;

	private IAchievement[] achievement = new IAchievement[41];

	private Dictionary<string, FacebookProfile> _fbProfiles;

	public bool isRunningFacebookLogin
	{
		get
		{
			return _isRunningFacebookLoginCoroutine;
		}
	}

	public FacebookProfile facebookProfile
	{
		get
		{
			return _fbProfile;
		}
	}

	public Texture2D localUserImage
	{
		get
		{
			if (facebookProfile != null)
			{
				return facebookProfile.image;
			}
			if (Social.localUser != null)
			{
				return Social.localUser.image;
			}
			return null;
		}
	}

	public string localUserName
	{
		get
		{
			if (facebookProfile != null)
			{
				return facebookProfile.name;
			}
			if (Social.localUser != null)
			{
				return Social.localUser.userName;
			}
			return null;
		}
	}

	public static SocialManager instance
	{
		get
		{
			Init();
			return _instance;
		}
	}

	public bool facebookIsLoggedIn
	{
		get
		{
			return FacebookAndroid.isSessionValid();
		}
	}

	public bool consolidatedFriendsCompleted
	{
		get
		{
			return _consolidatedFriendsCompleted;
		}
	}

	public bool dirty
	{
		get
		{
			return _dirty;
		}
	}

	public void AddFacebookPictureDownloadedHandler(Action<FacebookProfile> handler)
	{
		_facebookPictureDownloadedHandler = (Action<FacebookProfile>)Delegate.Combine(_facebookPictureDownloadedHandler, handler);
	}

	public void RemoveFacebookPictureDownloadedHandler(Action<FacebookProfile> handler)
	{
		_facebookPictureDownloadedHandler = (Action<FacebookProfile>)Delegate.Remove(_facebookPictureDownloadedHandler, handler);
	}

	public void AddFriendsConsolidatedHandler(Action handler)
	{
		_friendsConsolidatedHandler = (Action)Delegate.Combine(_friendsConsolidatedHandler, handler);
	}

	public void RemoveFriendsConsolidatedHandler(Action handler)
	{
		_friendsConsolidatedHandler = (Action)Delegate.Remove(_friendsConsolidatedHandler, handler);
	}

	public Friend[] FriendsSortedByScore()
	{
		if (_friends != null)
		{
			Friend[] array = _friends.ToArray();
			Array.Sort(array, (Friend x, Friend y) => y.score - x.score);
			return array;
		}
		return new Friend[0];
	}

	public Friend[] FriendsSortedByCash()
	{
		if (_friends != null)
		{
			Friend[] array = _friends.ToArray();
			Array.Sort(array, (Friend x, Friend y) => y.gamesToCashIn - x.gamesToCashIn);
			return array;
		}
		return new Friend[0];
	}

	public static void Init()
	{
		if (_instance == null)
		{
			GameObject gameObject = new GameObject();
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			gameObject.AddComponent<FacebookManager>();
			gameObject.AddComponent<SocialManager>();
		}
	}

	private void InitPushNotifications()
	{
		SocialManagerAndroid.instance.InitPushNotifications();
	}

	public void InitGameCenter()
	{
	}

	public bool FacebookLogin(Action<bool> onComplete)
	{
		if (!_isRunningFacebookLoginCoroutine)
		{
			_fbReady = false;
			_isRunningFacebookLoginCoroutine = true;
			StartCoroutine(FacebookLoginCoroutine(onComplete));
			return true;
		}
		return false;
	}

	private void HandleLoggingInComplete(bool success, Action<bool> onComplete)
	{
		_isRunningFacebookLoginCoroutine = false;
		if (onComplete != null)
		{
			onComplete(success);
		}
	}

	public void FacebookLogout()
	{
		FacebookAndroid.logout();
	}

	private IEnumerator FacebookLoginCoroutine(Action<bool> onComplete)
	{
		if (!facebookIsLoggedIn)
		{
			_fbCurrentRequest = FacebookCurrentRequest.LoggingIn;
			FacebookAndroid.loginWithRequestedPermissions(new string[3] { "publish_stream", "email", "user_birthday" });
			float timeOutLeft = 600f;
			float lastRealTime = Time.realtimeSinceStartup;
			while (_fbCurrentRequest != FacebookCurrentRequest.None)
			{
				if (_fbCurrentRequest == FacebookCurrentRequest.Error)
				{
					HandleLoggingInComplete(false, onComplete);
					yield break;
				}
				float realTime = Time.realtimeSinceStartup;
				float deltaRealTime = realTime - lastRealTime;
				lastRealTime = realTime;
				if (deltaRealTime < 0.5f)
				{
					timeOutLeft -= deltaRealTime;
					if (timeOutLeft <= 0f)
					{
						_fbCurrentRequest = FacebookCurrentRequest.Error;
						HandleLoggingInComplete(false, onComplete);
						yield break;
					}
				}
				yield return null;
			}
		}
		Action<bool> onComplete2 = default(Action<bool>);
		Facebook.instance.graphRequest("me", HTTPVerb.GET, new Dictionary<string, object> { { "fields", "id,name,first_name" } }, delegate(string error, object result)
		{
			bool flag = false;
			if (error != null)
			{
				FacebookAndroid.logout();
			}
			else if (result != null && result.GetType() == typeof(Hashtable))
			{
				_fbProfile = new FacebookProfile();
				Hashtable hashtable = (Hashtable)result;
				_fbProfile.id = (string)hashtable["id"];
				_fbProfile.name = (string)hashtable["first_name"];
				_fbProfile.fullName = (string)hashtable["name"];
				flag = true;
			}
			if (flag)
			{
				StartCoroutine(DownloadFacebookPicture(_fbProfile));
				Facebook.instance.graphRequest("me/friends", HTTPVerb.GET, new Dictionary<string, object>
				{
					{ "fields", "id,name,first_name,installed" },
					{ "limit", "5000" }
				}, delegate(string friendsError, object friendsResult)
				{
					try
					{
						bool flag2 = false;
						if (friendsError != null)
						{
						}
						if (friendsResult != null)
						{
							ArrayList arrayList = null;
							if (friendsResult.GetType() == typeof(Hashtable))
							{
								if (((Hashtable)friendsResult).ContainsKey("data"))
								{
									object obj = ((Hashtable)friendsResult)["data"];
									if (obj.GetType() == typeof(ArrayList))
									{
										arrayList = (ArrayList)obj;
									}
								}
							}
							else if (friendsResult.GetType() == typeof(ArrayList))
							{
								arrayList = (ArrayList)friendsResult;
							}
							if (arrayList != null)
							{
								_fbFriends = new Dictionary<string, Hashtable>(arrayList.Count);
								foreach (Hashtable item in arrayList)
								{
									if (item.ContainsKey("id"))
									{
										_fbFriends[(string)item["id"]] = item;
									}
								}
								flag2 = true;
							}
						}
						if (flag2)
						{
							_fbReady = true;
							Invalidate();
							HandleLoggingInComplete(true, onComplete2);
						}
						else
						{
							_fbFriends = null;
							_fbReady = true;
							Invalidate();
							HandleLoggingInComplete(false, onComplete2);
						}
					}
					catch (Exception ex)
					{
						Debug.LogWarning("Error: " + ex);
						HandleLoggingInComplete(false, onComplete2);
					}
				});
			}
			else
			{
				HandleLoggingInComplete(false, onComplete2);
			}
		});
	}

	public void Invalidate()
	{
		bool flag = false;
		if (!SocialManagerAndroid.instance.isAuthenticated() || (facebookIsLoggedIn && !_fbReady))
		{
			return;
		}
		RegisterUser(delegate
		{
		});
		if (ShouldConsolidateFriends())
		{
			ConsolidateFriends(delegate
			{
				_consolidatedFriendsCompleted = true;
				_lastConsolidateFriendCompleteTime = Time.realtimeSinceStartup;
			});
		}
	}

	private bool ShouldConsolidateFriends()
	{
		if (!consolidatedFriendsCompleted)
		{
			return true;
		}
		if (_fbFriends != null)
		{
			if (_latestFacebookIdsWithGameInstalled == null)
			{
				return true;
			}
			foreach (KeyValuePair<string, Hashtable> fbFriend in _fbFriends)
			{
				Hashtable value = fbFriend.Value;
				if (value.ContainsKey("installed") && !_latestFacebookIdsWithGameInstalled.Contains(fbFriend.Key))
				{
					return true;
				}
			}
		}
		float num = Time.realtimeSinceStartup - _lastConsolidateFriendCompleteTime;
		if (num >= 3600f)
		{
			return true;
		}
		return false;
	}

	public void CollectFriendReward(Friend friend)
	{
		friend.status.gamesCashedIn = friend.games;
		_dirty = true;
	}

	public int CashIn(Friend friend, int max)
	{
		int num = friend.games - friend.status.gamesCashedIn;
		if (num > 0)
		{
			friend.status.gamesCashedIn = friend.games;
			_dirty = true;
			return Mathf.Max(num, max);
		}
		return 0;
	}

	public int CashInAll(int maxPerFriend)
	{
		if (_friends == null)
		{
			return 0;
		}
		int num = 0;
		foreach (Friend friend in _friends)
		{
			num += CashIn(friend, maxPerFriend);
		}
		return num;
	}

	public void WriteTo(Stream stream)
	{
		BinaryWriter binaryWriter = new BinaryWriter(stream);
		binaryWriter.Write((byte)1);
		if (_friendStatus != null)
		{
			binaryWriter.Write(_friendStatus.Count);
			{
				foreach (KeyValuePair<string, Friend.Status> item in _friendStatus)
				{
					binaryWriter.Write(item.Key);
					binaryWriter.Write(item.Value.gamesCashedIn);
					binaryWriter.Write(item.Value.lastPokeTime.ToBinary());
				}
				return;
			}
		}
		binaryWriter.Write(0);
	}

	public void ReadFrom(Stream stream)
	{
		BinaryReader binaryReader = new BinaryReader(stream);
		byte b = binaryReader.ReadByte();
		if (b == 1)
		{
			int num = binaryReader.ReadInt32();
			_friendStatus = new Dictionary<string, Friend.Status>(num);
			for (int i = 0; i < num; i++)
			{
				string text = binaryReader.ReadString();
				if (!string.IsNullOrEmpty(text))
				{
					Friend.Status status = new Friend.Status();
					status.gamesCashedIn = binaryReader.ReadInt32();
					status.lastPokeTime = DateTime.FromBinary(binaryReader.ReadInt64());
					_friendStatus[text] = status;
				}
			}
			return;
		}
		throw new IOException("Unsupported playerdata file version");
	}

	private static string GetSaveDataPath()
	{
		return Application.persistentDataPath + "/socialdata";
	}

	private static bool ArraysAreEqual<T>(T[] a, T[] b)
	{
		if (a == null && b == null)
		{
			return true;
		}
		if (a.Length != b.Length)
		{
			return false;
		}
		for (int i = 0; i < a.Length; i++)
		{
			if (!object.Equals(a[i], b[i]))
			{
				return false;
			}
		}
		return true;
	}

	public void Load()
	{
		try
		{
			string saveDataPath = GetSaveDataPath();
			byte[] buffer = FileUtil.Load(saveDataPath, "resxrctrv7tgv7gb8h9h9u0909kllfmolkjnhghgjjkhjghg");
			MemoryStream memoryStream = new MemoryStream(buffer);
			ReadFrom(memoryStream);
			memoryStream.Close();
			_dirty = false;
		}
		catch (Exception)
		{
		}
	}

	public bool Save()
	{
		try
		{
			MemoryStream memoryStream = new MemoryStream(8192);
			WriteTo(memoryStream);
			byte[] buffer = memoryStream.GetBuffer();
			FileUtil.Save(GetSaveDataPath(), "resxrctrv7tgv7gb8h9h9u0909kllfmolkjnhghgjjkhjghg", buffer, 0, (int)memoryStream.Length);
			memoryStream.Close();
			_dirty = false;
			return true;
		}
		catch (Exception)
		{
		}
		return false;
	}

	private void OnGCMRegistrationComplete(string regId)
	{
		Invalidate();
	}

	private void OnGCMRegistrationError()
	{
		Invalidate();
	}

	private void Awake()
	{
		if (_instance != null)
		{
			UnityEngine.Object.Destroy(this);
			return;
		}
		_instance = this;
		Load();
		string androidToken = SocialManagerAndroid.instance.AndroidToken;
		FacebookAndroid.init("254616967963463");
		InitPushNotifications();
		if (facebookIsLoggedIn)
		{
			FacebookLogin(null);
		}
	}

	private void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			Save();
			return;
		}
		if (facebookIsLoggedIn)
		{
			FacebookLogin(null);
		}
		SocialManagerAndroid.instance.TryToGetGCMToken();
	}

	private void OnEnable()
	{
		FacebookManager.loginSucceededEvent += facebookLoginSucceeded;
		FacebookManager.loginFailedEvent += facebookLoginFailed;
		FacebookManager.loggedOutEvent += facebookLoggedOut;
		FacebookManager.accessTokenExtendedEvent += facebookAccessTokenExtended;
		FacebookManager.failedToExtendTokenEvent += facebookFailedToExtendToken;
		FacebookManager.sessionInvalidatedEvent += facebookSessionInvalidatedEvent;
		FacebookManager.dialogCompletedEvent += facebokDialogCompleted;
		FacebookManager.dialogCompletedWithUrlEvent += facebookDialogCompletedWithUrl;
		FacebookManager.dialogDidNotCompleteEvent += facebookDialogDidNotComplete;
		FacebookManager.dialogFailedEvent += facebookDialogFailed;
		FacebookManager.customRequestReceivedEvent += facebookCustomRequestReceived;
		FacebookManager.customRequestFailedEvent += facebookCustomRequestFailed;
		SocialManagerAndroid.onGCMRegistrationComplete += OnGCMRegistrationComplete;
		SocialManagerAndroid.onGCMRegistrationError += OnGCMRegistrationError;
	}

	private void OnDisable()
	{
		FacebookManager.loginSucceededEvent -= facebookLoginSucceeded;
		FacebookManager.loginFailedEvent -= facebookLoginFailed;
		FacebookManager.loggedOutEvent -= facebookLoggedOut;
		FacebookManager.accessTokenExtendedEvent -= facebookAccessTokenExtended;
		FacebookManager.failedToExtendTokenEvent -= facebookFailedToExtendToken;
		FacebookManager.sessionInvalidatedEvent -= facebookSessionInvalidatedEvent;
		FacebookManager.dialogCompletedEvent -= facebokDialogCompleted;
		FacebookManager.dialogCompletedWithUrlEvent -= facebookDialogCompletedWithUrl;
		FacebookManager.dialogDidNotCompleteEvent -= facebookDialogDidNotComplete;
		FacebookManager.dialogFailedEvent -= facebookDialogFailed;
		FacebookManager.customRequestReceivedEvent -= facebookCustomRequestReceived;
		FacebookManager.customRequestFailedEvent -= facebookCustomRequestFailed;
		SocialManagerAndroid.onGCMRegistrationComplete -= OnGCMRegistrationComplete;
		SocialManagerAndroid.onGCMRegistrationError -= OnGCMRegistrationError;
	}

	public void ProgressThisAchievement(int achievementIndex, float percentCompleted)
	{
		if (!Social.localUser.authenticated)
		{
			return;
		}
		try
		{
			if (achievement[achievementIndex] == null)
			{
				achievement[achievementIndex] = Social.CreateAchievement();
			}
			if (achievement[achievementIndex].id == "unknown")
			{
				achievement[achievementIndex].id = Achievements.Instance.achievementIds[achievementIndex];
			}
			if (achievement[achievementIndex].percentCompleted == (double)percentCompleted)
			{
				return;
			}
			achievement[achievementIndex].percentCompleted = percentCompleted;
			achievement[achievementIndex].ReportProgress(delegate(bool result)
			{
				if (!result)
				{
				}
			});
		}
		catch (Exception)
		{
		}
	}

	public void CompleteThisAchievement(string achievementId, float percentCompleted = 100f)
	{
		if (!Social.localUser.authenticated)
		{
			return;
		}
		try
		{
			percentCompleted = Mathf.Clamp(percentCompleted, 0f, 100f);
			IAchievement achievement = Social.CreateAchievement();
			achievement.id = achievementId;
			achievement.percentCompleted = percentCompleted;
			achievement.ReportProgress(delegate(bool result)
			{
				if (!result)
				{
				}
			});
		}
		catch (Exception)
		{
		}
	}

	private void facebookLoginSucceeded()
	{
		if (_fbCurrentRequest == FacebookCurrentRequest.LoggingIn)
		{
			_fbCurrentRequest = FacebookCurrentRequest.None;
			Flurry.LogFacebookLogin();
		}
	}

	private void facebookLoginFailed(string error)
	{
		if (_fbCurrentRequest == FacebookCurrentRequest.LoggingIn)
		{
			_fbCurrentRequest = FacebookCurrentRequest.Error;
		}
	}

	private void facebookLoggedOut()
	{
		if (_friends != null)
		{
			_friends.RemoveAll((Friend item) => item.gcProfile == null && item.fbProfile != null);
			_friends.ForEach(delegate(Friend item)
			{
				item.fbProfile = null;
			});
		}
		_fbFriends = null;
	}

	private void facebookAccessTokenExtended(DateTime newExpiry)
	{
	}

	private void facebookFailedToExtendToken()
	{
	}

	private void facebookSessionInvalidatedEvent()
	{
	}

	private void facebookReceivedUsername(string username)
	{
	}

	private void facebookUsernameRequestFailed(string error)
	{
	}

	private void facebookPost()
	{
	}

	private void facebookPostFailed(string error)
	{
	}

	private void facebokDialogCompleted()
	{
	}

	private void facebookDialogCompletedWithUrl(string url)
	{
	}

	private void facebookDialogDidNotComplete()
	{
	}

	private void facebookDialogFailed(string error)
	{
	}

	private void facebookCustomRequestReceived(object obj)
	{
		ResultLogger.logObject(obj);
	}

	private void facebookCustomRequestFailed(string error)
	{
	}

	private static string GetRandomIdentifier()
	{
		string text = ((!Application.isEditor) ? SystemInfo.deviceUniqueIdentifier : "0000000000000000000000000000000000000000");
		return text + UnityEngine.Random.Range(0, int.MaxValue);
	}

	public static string GetChecksum(string data)
	{
		return GetSHA1Hash(data + "resxrctrv7tgv7gb8h9h9u0909kllfmolkjnhghgjjkhjghg");
	}

	private static string GetChecksum(params string[] data)
	{
		return GetChecksum(string.Join(null, data));
	}

	private static string GetSHA1Hash(string unhashed)
	{
		SHA1 sHA = SHA1.Create();
		byte[] array = sHA.ComputeHash(Encoding.Default.GetBytes(unhashed));
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < array.Length; i++)
		{
			stringBuilder.Append(array[i].ToString("x2"));
		}
		return stringBuilder.ToString();
	}

	private static IEnumerator WWWRequestCoroutine(WWWComplete onWWWComplete, string relativeUrl, object cookie, params string[] postItems)
	{
		string url = "http://hoodrunner.kiloo.com" + relativeUrl;
		string identifier = GetRandomIdentifier();
		StringBuilder checksumSB = new StringBuilder();
		for (int i = 0; i < postItems.Length; i += 2)
		{
			if (postItems[i] != null)
			{
				if (postItems[i + 1] == null)
				{
					postItems[i + 1] = string.Empty;
				}
				checksumSB.Append(postItems[i + 1]);
			}
		}
		string checksum = GetChecksum(identifier + checksumSB.ToString());
		WWWForm postData = new WWWForm();
		postData.AddField("identifier", identifier);
		postData.AddField("checksum", checksum);
		StringBuilder sb = new StringBuilder();
		sb.Append("WWWRequest(").Append(url).Append(")\n");
		for (int j = 0; j < postItems.Length; j += 2)
		{
			sb.Append("Adding post data: \"").Append(postItems[j]).Append("\" = \"")
				.Append(postItems[j + 1])
				.Append("\"\n");
			if (postItems[j] != null)
			{
				postData.AddField(postItems[j], postItems[j + 1]);
			}
		}
		WWW www = new WWW(url, postData);
		yield return www;
		if (!string.IsNullOrEmpty(www.error))
		{
			yield break;
		}
		if (www.text != null)
		{
			sb.Append("Text: \"").Append(www.text).Append("\"\n");
		}
		if (www.error != null)
		{
			sb.Append("Error: \"").Append(www.error).Append("\"\n");
		}
		if (onWWWComplete == null)
		{
			yield break;
		}
		if (www.error != null)
		{
			onWWWComplete(WWWRequestResult.Error, null, cookie);
			yield break;
		}
		string result = null;
		int resultStart = www.text.IndexOf("<result>", StringComparison.OrdinalIgnoreCase);
		if (resultStart >= 0)
		{
			resultStart += 8;
			int resultEnd = www.text.IndexOf("</result>", resultStart, StringComparison.OrdinalIgnoreCase);
			if (resultEnd > resultStart)
			{
				result = www.text.Substring(resultStart, resultEnd - resultStart);
			}
			else if (resultEnd == resultStart)
			{
				result = string.Empty;
			}
		}
		onWWWComplete((result == null) ? WWWRequestResult.Error : WWWRequestResult.Success, result, cookie);
	}

	private static string ByteArrayToHex(byte[] barray)
	{
		char[] array = new char[barray.Length * 2];
		for (int i = 0; i < barray.Length; i++)
		{
			byte b = (byte)(barray[i] >> 4);
			array[i * 2] = (char)((b <= 9) ? (b + 48) : (b + 55));
			b = (byte)(barray[i] & 0xF);
			array[i * 2 + 1] = (char)((b <= 9) ? (b + 48) : (b + 55));
		}
		return new string(array);
	}

	private static string GetBundleVersion()
	{
		return DeviceUtility.GetBundleVersion();
	}

	private void RegisterUser(Action<bool> registerUserCompleted)
	{
		string text = ((_fbProfile != null) ? _fbProfile.id : string.Empty);
		string androidToken = SocialManagerAndroid.instance.AndroidToken;
		StartCoroutine(WWWRequestCoroutine(WWWRegisterUserCompleted, "/register2.php?android", registerUserCompleted, "version", GetBundleVersion(), "fbid", text, "devicetoken", androidToken, "score", PlayerInfo.Instance.highestScore.ToString(), "meters", "0", "games", "0", "rank", PlayerInfo.Instance.GetCurrentRank().ToString()));
	}

	private void WWWRegisterUserCompleted(WWWRequestResult result, string output, object cookie)
	{
		bool obj = false;
		if (result == WWWRequestResult.Success)
		{
			obj = true;
			Dictionary<string, string> dictionary = StringUtility.ParseProperties(output);
			if (dictionary.ContainsKey("userid"))
			{
				string text = dictionary["userid"];
				string text2 = dictionary["score"];
				string text3 = dictionary["meters"];
				string text4 = dictionary["games"];
				string text5 = dictionary["rank"];
				string strA = dictionary["checksum"];
				string checksum = GetChecksum(text, text2, text3, text4, text5);
				if (string.Compare(strA, checksum, true) == 0)
				{
					try
					{
						int userid = int.Parse(text);
						int highestScore = int.Parse(text2);
						int highestMeters = int.Parse(text3);
						_userid = userid;
						PlayerInfo.Instance.highestScore = highestScore;
						PlayerInfo.Instance.highestMeters = highestMeters;
					}
					catch (Exception)
					{
						obj = false;
					}
				}
				else
				{
					obj = false;
				}
			}
		}
		if (cookie != null)
		{
			((Action<bool>)cookie)(obj);
		}
	}

	private void ConsolidateFriends(Action<bool> consolidateFriendsCompleted)
	{
		string text;
		if (_fbFriends != null)
		{
			if (_latestFacebookIdsWithGameInstalled == null)
			{
				_latestFacebookIdsWithGameInstalled = new List<string>();
			}
			else
			{
				_latestFacebookIdsWithGameInstalled.Clear();
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, Hashtable> fbFriend in _fbFriends)
			{
				Hashtable value = fbFriend.Value;
				if (value.ContainsKey("installed"))
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(';');
					}
					string key = fbFriend.Key;
					stringBuilder.Append(key);
					_latestFacebookIdsWithGameInstalled.Add(key);
				}
			}
			text = stringBuilder.ToString();
		}
		else
		{
			text = string.Empty;
		}
		string empty = string.Empty;
		if (string.IsNullOrEmpty(text) && string.IsNullOrEmpty(empty))
		{
			if (consolidateFriendsCompleted != null)
			{
				consolidateFriendsCompleted(true);
			}
			Action friendsConsolidatedHandler = _friendsConsolidatedHandler;
			if (friendsConsolidatedHandler != null)
			{
				friendsConsolidatedHandler();
			}
		}
		else
		{
			StartCoroutine(WWWRequestCoroutine(WWWConsolidateFriendsCompleted, "/friends2.php?android", consolidateFriendsCompleted, "fblist", text, "gclist", empty));
		}
	}

	private static string[][] ParseSets(string setsString)
	{
		string[] separator = new string[1] { ");(" };
		string[] array = setsString.Split(separator, StringSplitOptions.RemoveEmptyEntries);
		if (array.Length > 0)
		{
			if (array[0][0] == '(')
			{
				array[0] = array[0].Substring(1);
			}
			int num = array.Length - 1;
			int num2 = array[num].Length - 1;
			if (array[num][num2] == ')')
			{
				array[num] = array[num].Remove(num2);
			}
			string[][] array2 = new string[array.Length][];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = array[i].Split(';');
			}
			return array2;
		}
		return new string[0][];
	}

	private void WWWConsolidateFriendsCompleted(WWWRequestResult result, string output, object cookie)
	{
		bool obj = false;
		if (result == WWWRequestResult.Success)
		{
			Dictionary<string, string> dictionary = StringUtility.ParseProperties(output);
			if (dictionary.ContainsKey("friendslist"))
			{
				string text = dictionary["friendslist"];
				string strA = dictionary["checksum"];
				string checksum = GetChecksum(text);
				if (string.Compare(strA, checksum, true) == 0)
				{
					if (string.IsNullOrEmpty(text))
					{
						_friends = null;
					}
					else
					{
						string[][] array = ParseSets(text);
						_friends = new List<Friend>(array.Length);
						string[][] array2 = array;
						foreach (string[] array3 in array2)
						{
							if (array3.Length < 6 || (array3[1].Length <= 0 && array3[2].Length <= 0))
							{
								continue;
							}
							try
							{
								Friend friend = new Friend();
								friend.userid = int.Parse(array3[0]);
								string text2 = array3[1];
								if (text2.Length > 0)
								{
								}
								string text3 = array3[2];
								if (text3.Length > 0)
								{
									if (_fbProfiles == null)
									{
										_fbProfiles = new Dictionary<string, FacebookProfile>();
									}
									FacebookProfile facebookProfile;
									if (_fbProfiles.ContainsKey(text3))
									{
										facebookProfile = _fbProfiles[text3];
									}
									else
									{
										Hashtable hashtable = _fbFriends[text3];
										facebookProfile = new FacebookProfile();
										facebookProfile.id = text3;
										facebookProfile.name = (string)hashtable["first_name"];
										facebookProfile.fullName = (string)hashtable["name"];
										_fbProfiles[text3] = facebookProfile;
									}
									friend.fbProfile = facebookProfile;
								}
								friend.score = int.Parse(array3[3]);
								friend.meters = int.Parse(array3[4]);
								friend.games = int.Parse(array3[5]);
								friend.rank = ((array3.Length >= 7) ? int.Parse(array3[6]) : 0);
								Friend.Status status = null;
								if (_friendStatus == null)
								{
									_friendStatus = new Dictionary<string, Friend.Status>();
								}
								if (friend.fbProfile != null && _friendStatus.ContainsKey(friend.fbProfile.id))
								{
									status = _friendStatus[friend.fbProfile.id];
								}
								else if (friend.gcProfile != null && _friendStatus.ContainsKey(friend.gcProfile.id))
								{
									status = _friendStatus[friend.gcProfile.id];
								}
								else
								{
									status = new Friend.Status();
									status.gamesCashedIn = friend.games;
									string key = ((friend.fbProfile == null) ? friend.gcProfile.id : friend.fbProfile.id);
									_friendStatus[key] = status;
									_dirty = true;
								}
								friend.status = status;
								if (_friends != null)
								{
									_friends.Add(friend);
								}
							}
							catch (Exception)
							{
							}
						}
						if (_fbProfiles != null)
						{
							StartCoroutine(DownloadFacebookPictures(_fbProfiles));
						}
					}
					obj = true;
				}
			}
		}
		if (cookie != null)
		{
			((Action<bool>)cookie)(obj);
		}
		if (_friendsConsolidatedHandler != null)
		{
			_friendsConsolidatedHandler();
		}
	}

	public void ReportScore(int newScore, int newMeters)
	{
		if (_userid > 0)
		{
			StartCoroutine(WWWRequestCoroutine(null, "/report2.php?android", null, "userid", _userid.ToString(), "score", newScore.ToString(), "meters", newMeters.ToString(), "rank", PlayerInfo.Instance.GetCurrentRank().ToString()));
		}
	}

	public void UpdateFriendScores(Action<bool> updateFriendsScoresCompleted)
	{
		DateTime dateTime = new DateTime(1970, 1, 1, 8, 0, 0, DateTimeKind.Utc);
		double totalSeconds = (DateTime.UtcNow - dateTime).TotalSeconds;
		double num = totalSeconds - lastFriendScoreUpdateTimestamp;
		if (num < 300.0)
		{
			return;
		}
		lastFriendScoreUpdateTimestamp = totalSeconds;
		StringBuilder stringBuilder = new StringBuilder();
		if (_friends == null)
		{
			return;
		}
		foreach (Friend friend in _friends)
		{
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Append(';');
			}
			stringBuilder.Append(friend.userid);
		}
		string text = stringBuilder.ToString();
		StartCoroutine(WWWRequestCoroutine(WWWUpdateFriendScoresCompleted, "/scores2.php?android", updateFriendsScoresCompleted, "idlist", text));
	}

	private void WWWUpdateFriendScoresCompleted(WWWRequestResult result, string output, object cookie)
	{
		bool obj = false;
		if (result == WWWRequestResult.Success)
		{
			Dictionary<string, string> dictionary = StringUtility.ParseProperties(output);
			if (dictionary.ContainsKey("scores"))
			{
				string text = dictionary["scores"];
				string strA = dictionary["checksum"];
				string checksum = GetChecksum(text);
				if (string.Compare(strA, checksum, true) == 0)
				{
					try
					{
						string[][] array = ParseSets(text);
						string[][] array2 = array;
						foreach (string[] array3 in array2)
						{
							if (array3.Length >= 4)
							{
								int userid = int.Parse(array3[0]);
								Friend friend = _friends.Find((Friend f) => f.userid == userid);
								if (friend != null)
								{
									friend.score = int.Parse(array3[1]);
									friend.meters = int.Parse(array3[2]);
									friend.games = int.Parse(array3[3]);
									friend.rank = ((array3.Length >= 5) ? int.Parse(array3[4]) : 0);
								}
								continue;
							}
							throw new Exception();
						}
						obj = true;
					}
					catch (Exception)
					{
					}
				}
			}
		}
		if (cookie != null)
		{
			((Action<bool>)cookie)(obj);
		}
	}

	public void Poke(Friend friend)
	{
		Missions.Instance.PlayerDidThis(Missions.MissionTarget.PokeFriend);
		string text = ((friend.fbProfile != null) ? _fbProfile.fullName : ((!Social.localUser.authenticated) ? string.Empty : Social.localUser.userName));
		StartCoroutine(WWWRequestCoroutine(null, "/poke.php?android", null, "friend", friend.userid.ToString(), "name", text));
		friend.status.lastPokeTime = DateTime.UtcNow;
		_dirty = true;
		Flurry.LogGenericSocialAction();
		Flurry.LogEvent("Social friend poked");
	}

	public void SetPokeFirstTime(Friend friend)
	{
		friend.status.lastPokeTime = DateTime.UtcNow;
		_dirty = true;
	}

	public void BragNotify(int oldScore, List<Friend> friends)
	{
		if (friends == null)
		{
			return;
		}
		int count = friends.Count;
		StringBuilder stringBuilder = new StringBuilder(count * 8);
		StringBuilder stringBuilder2 = new StringBuilder(count * 2);
		foreach (Friend friend in friends)
		{
			int relation = friend.relation;
			int userid = friend.userid;
			if (relation != 0 && userid != 0)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(';');
					stringBuilder2.Append(';');
				}
				stringBuilder.Append(userid);
				stringBuilder2.Append(relation);
			}
		}
		if (stringBuilder.Length > 0)
		{
			string text = ((_fbProfile == null) ? string.Empty : _fbProfile.name);
			string text2 = ((!Social.localUser.authenticated) ? string.Empty : Social.localUser.userName);
			StartCoroutine(WWWRequestCoroutine(null, "/brag.php?android", null, "oldscore", oldScore.ToString(), "newscore", PlayerInfo.Instance.highestScore.ToString(), "useridlist", stringBuilder.ToString(), "relationlist", stringBuilder2.ToString(), "fbname", text, "gcname", text2));
			Flurry.LogGenericSocialAction();
			Flurry.LogEvent("Social bragged");
		}
	}

	private static string GetDeviceTypeString()
	{
		return "iDevice";
	}

	public void RecommendAppFacebook()
	{
		if (facebookIsLoggedIn)
		{
			FacebookAndroid.showPostMessageDialogWithOptions("http://redirect.kiloo.com/subwayapp.php", "Subway Surfers", "http://hoodrunner.kiloo.com/fblogo.png", "Dodge the trains! Help Jake, Tricky and Fresh escape.");
		}
	}

	public static void showPostMessageDialogWithOptions(string link, string linkName, string linkToImage, string caption)
	{
	}

	public void BragFacebook(List<Friend> friends)
	{
		if (!facebookIsLoggedIn)
		{
			return;
		}
		List<Friend> list = null;
		if (friends != null)
		{
			list = new List<Friend>(friends.Count);
			foreach (Friend friend in friends)
			{
				if (friend.fbProfile != null && friend.score < PlayerInfo.Instance.highestScore)
				{
					list.Add(friend);
				}
			}
			list.Sort((Friend x, Friend y) => y.score - x.score);
		}
		string value = ((list == null || list.Count == 0) ? ("I just scored " + PlayerInfo.Instance.highestScore + " points dodging trains in Subway Surfers on my " + GetDeviceTypeString() + ". Check it out!") : ((list.Count == 1) ? ("I just scored " + PlayerInfo.Instance.highestScore + " points in Subway Surfers on my " + GetDeviceTypeString() + " and beat " + list[0].fbProfile.fullName) : ((list.Count == 2) ? ("I just scored " + PlayerInfo.Instance.highestScore + " points in Subway Surfers on my " + GetDeviceTypeString() + " and beat " + list[0].fbProfile.fullName + " and " + list[1].fbProfile.fullName) : ((list.Count != 3) ? ("I just scored " + PlayerInfo.Instance.highestScore + " points in Subway Surfers on my " + GetDeviceTypeString() + " and beat " + list[0].fbProfile.fullName + ", " + list[1].fbProfile.fullName + " and " + (list.Count - 2) + " others") : ("I just scored " + PlayerInfo.Instance.highestScore + " points in Subway Surfers on my " + GetDeviceTypeString() + " and beat " + list[0].fbProfile.fullName + ", " + list[1].fbProfile.fullName + " and " + list[2].fbProfile.fullName)))));
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("link", "http://redirect.kiloo.com/subwayapp.php");
		dictionary.Add("name", "New Subway Surfers High Score");
		dictionary.Add("picture", "http://hoodrunner.kiloo.com/fblogo.png");
		dictionary.Add("caption", value);
		dictionary.Add("description", "Download Subway Surfers now");
		Dictionary<string, string> parameters = dictionary;
		FacebookAndroid.showDialog("stream.publish", parameters);
		Flurry.LogGenericSocialAction();
		Flurry.LogEvent("Social bragged Facebook");
	}

	private IEnumerator DownloadFacebookPicture(FacebookProfile profile)
	{
		if (profile == null)
		{
			yield break;
		}
		string url = "http://graph.facebook.com/" + profile.id + "/picture?type=square";
		Debug.Log("www getting facebook image for " + profile.name + " at \"" + url + "\"");
		WWW www = new WWW(url);
		yield return www;
		if (www.error != null)
		{
		}
		Texture2D image = www.texture;
		if (!(image == null) && (image.width != 8 || image.height != 8))
		{
			profile.image = image;
			if (_facebookPictureDownloadedHandler != null)
			{
				_facebookPictureDownloadedHandler(profile);
			}
		}
	}

	private IEnumerator DownloadFacebookPictures(Dictionary<string, FacebookProfile> fbProfiles)
	{
		List<FacebookProfile> profiles = new List<FacebookProfile>(fbProfiles.Count);
		foreach (FacebookProfile profile in fbProfiles.Values)
		{
			if (profile.image == null)
			{
				profiles.Add(profile);
			}
		}
		foreach (FacebookProfile profile2 in profiles)
		{
			yield return StartCoroutine(DownloadFacebookPicture(profile2));
		}
	}
}
