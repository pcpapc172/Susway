using System;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class DailyWord : MonoBehaviour
{
	public enum DailyWordRewardType
	{
		Coins = 0,
		MysteryBox = 1
	}

	public struct DailyWordReward
	{
		public DailyWordRewardType type;

		public int coins;

		public MysteryBox.Type mysteryBoxType;
	}

	private bool synchronizingNow;

	private static readonly DailyWordReward[] REWARDS = new DailyWordReward[5]
	{
		new DailyWordReward
		{
			coins = 500
		},
		new DailyWordReward
		{
			coins = 750
		},
		new DailyWordReward
		{
			coins = 1050
		},
		new DailyWordReward
		{
			coins = 1500
		},
		new DailyWordReward
		{
			type = DailyWordRewardType.MysteryBox,
			mysteryBoxType = MysteryBox.Type.Super
		}
	};

	private bool stoppingFromEditor;

	private string key = string.Empty;

	private int expireSecondsD;

	private string wordD;

	private int expireSecondsS;

	private string wordS;

	private DateTime GMTTimeS;

	private static readonly string SECRET_KEY = "aIN0UXP4NNoANVGi5w3raGAFN1n5OLQZFDhwjs6HoX";

	private static readonly string[] WORDS = new string[30]
	{
		"GAME", "WHEEL", "CAB", "TOY", "TRIP", "LIGHT", "RAIL", "PIT", "FAN", "PLAZA",
		"SIGN", "DOOR", "BELL", "CURVE", "CRASH", "BOX", "TUBE", "SLIDE", "METRO", "ROUND",
		"TRAIN", "CAR", "RED", "TURBO", "TRACK", "MAP", "LINE", "BUS", "CITY", "KEY",
	};

	private static DailyWord _instance;

	public static DailyWord Instance
	{
		get
		{
			return _instance ?? (_instance = UnityEngine.Object.FindObjectOfType(typeof(DailyWord)) as DailyWord);
		}
	}

	private void Start()
	{
		ForceSync();
		UIScreenController instance = UIScreenController.Instance;
		instance.OnChangedScreen = (Action<string>)Delegate.Combine(instance.OnChangedScreen, new Action<string>(AutoSyncer));
	}

	private void OnApplicationQuit()
	{
		stoppingFromEditor = true;
	}

	private void OnDestroy()
	{
		if (!stoppingFromEditor)
		{
			UIScreenController instance = UIScreenController.Instance;
			instance.OnChangedScreen = (Action<string>)Delegate.Remove(instance.OnChangedScreen, new Action<string>(AutoSyncer));
		}
	}

	private void AutoSyncer(string screenName)
	{
		if (screenName == "FrontUI" || screenName == "GameoverUI")
		{
			SyncIfExpired();
		}
	}

	public void ForceSync()
	{
		if (!synchronizingNow)
		{
			StartCoroutine(DownloadDaily());
		}
	}

	public void SyncIfExpired()
	{
		if (PlayerInfo.Instance.dailyWordExpireTime.CompareTo(DateTime.Now) < 0)
		{
			ForceSync();
		}
	}

	private IEnumerator DownloadDaily()
	{
		synchronizingNow = true;
		key = GenerateKey();
		string response = LocalDailyWordPayload(key);
		string[] rawData = response.Split(';');
		if (!SHA1HashCheck(rawData))
		{
			Debug.LogError("DailyWord: Checksum failed");
			synchronizingNow = false;
			yield break;
		}
		StoreLocalVariables();
		StoreWWWResponse(rawData);
		OverWriteDeviceMemory();
		SendWordAndExpireTime();
		synchronizingNow = false;
		yield break;
	}

	private DateTime ConvertFromUnixTimestamp(double timestamp)
	{
		return new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(timestamp);
	}

	private double ConvertToUnixTimestamp(DateTime date)
	{
		DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
		return Math.Floor((date - dateTime).TotalSeconds);
	}

	private void StoreWWWResponse(string[] rawData)
	{
		wordS = rawData[1];
		GMTTimeS = new DateTime(Convert.ToInt32(rawData[2]), Convert.ToInt32(rawData[4]), Convert.ToInt32(rawData[5]), Convert.ToInt32(rawData[6]), Convert.ToInt32(rawData[7]), Convert.ToInt32(rawData[8]));
		expireSecondsS = Convert.ToInt32(rawData[9]);
	}

	private bool SHA1HashCheck(string[] rawData)
	{
		if (rawData.Length < 10)
		{
			for (int i = 0; i < rawData.Length; i++)
			{
			}
			return false;
		}
		string text = rawData[3];
		string sHA1Hash = GetSHA1Hash(rawData[0] + rawData[1] + rawData[2] + key + SECRET_KEY + rawData[4] + rawData[5] + rawData[6] + rawData[7] + rawData[8] + rawData[9]);
		if (text == sHA1Hash)
		{
			return true;
		}
		return false;
	}

	private void StoreLocalVariables()
	{
	}

	private void OverWriteDeviceMemory()
	{
		wordD = wordS;
		expireSecondsD = expireSecondsS;
	}

	private void SendWordAndExpireTime()
	{
		PlayerInfo.Instance.InitDailyWord(wordD, GMTTimeS.AddSeconds(expireSecondsD));
	}

	private string GetSHA1Hash(string dataString)
	{
		SHA1 sHA = SHA1.Create();
		byte[] bytes = Encoding.ASCII.GetBytes(dataString);
		byte[] array = sHA.ComputeHash(bytes);
		return BitConverter.ToString(array).Replace("-", string.Empty).ToLowerInvariant();
	}

	private string RandomString(int size)
	{
		StringBuilder stringBuilder = new StringBuilder();
		System.Random random = new System.Random();
		for (int i = 0; i < size; i++)
		{
			char value = Convert.ToChar(Convert.ToInt32(Math.Floor(26.0 * random.NextDouble() + 65.0)));
			stringBuilder.Append(value);
		}
		return stringBuilder.ToString();
	}

	private int RandomNumber(int min, int max)
	{
		System.Random random = new System.Random();
		return random.Next(min, max);
	}

	private string GenerateKey()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(RandomString(4));
		stringBuilder.Append(RandomNumber(0, 999));
		stringBuilder.Append(RandomString(2));
		return stringBuilder.ToString();
	}

	private string LocalDailyWordPayload(string clientKey)
	{
		DateTime now = DateTime.UtcNow;
		DateTime nextReset = new DateTime(now.Year, now.Month, now.Day, 1, 0, 0, DateTimeKind.Utc);
		if (now >= nextReset)
		{
			nextReset = nextReset.AddDays(1);
		}
		int expireSeconds = (int)(nextReset - now).TotalSeconds;
		int dayIndex = now.DayOfYear - 1;
		int wordIndex = (nextReset.Day - 1) % WORDS.Length;
		string word = WORDS[wordIndex];
		string payloadSource =
			$"{dayIndex};{word};{now.Year};" +
			$"{GetSHA1Hash($"{dayIndex}{word}{now.Year}{clientKey}{SECRET_KEY}{now.Month}{now.Day}{now.Hour}{now.Minute}{now.Second}{expireSeconds}")};" +
			$"{now.Month};{now.Day};{now.Hour};{now.Minute};{now.Second};{expireSeconds}";
		return payloadSource;
	}

	public static DailyWordReward GetRewardForDay(int dayIndex)
	{
		if (dayIndex >= REWARDS.Length)
		{
			dayIndex = REWARDS.Length - 1;
		}
		return REWARDS[dayIndex];
	}
}
