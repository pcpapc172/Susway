using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class OnlineSettings
{
	private const string URL = "/onlinesettings_android.php";

	private const string FILE_SECRET = "pdvshbhkndf92k19zvbckawd92fjk";

	private const int FILEFORMAT_VERSION = 1;

	private static readonly char[] LINEBREAK_CHARS = new char[2] { '\n', '\r' };

	private static OnlineSettings _instance;

	private Dictionary<string, string> _settings = new Dictionary<string, string>();

	private bool _isDownloading;

	private GameObject _downloadGameObject;

	public static bool isInstanced
	{
		get
		{
			return _instance != null;
		}
	}

	public static OnlineSettings instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new OnlineSettings();
			}
			return _instance;
		}
	}

	public bool isDownloading
	{
		get
		{
			return _isDownloading;
		}
	}

	private OnlineSettings()
	{
		LoadFromFile();
	}

	public void DownloadNow()
	{
		if (!_isDownloading)
		{
			if (_downloadGameObject != null)
			{
				UnityEngine.Object.Destroy(_downloadGameObject);
			}
			_downloadGameObject = new GameObject("OnlineSettingsDownloader");
			UnityEngine.Object.DontDestroyOnLoad(_downloadGameObject);
			_downloadGameObject.AddComponent<MonoBehaviour>().StartCoroutine(DownloadCoroutine());
		}
		else
		{
			Debug.LogWarning("OnlineSettings.DownloadNow(): Already downloading");
		}
	}

	public double GetValue(string key, double defaultValue)
	{
		string value;
		if (_settings.TryGetValue(key, out value))
		{
			try
			{
				return double.Parse(value);
			}
			catch
			{
				Debug.LogError(key + " returns the defaultValue because send string was " + value);
			}
		}
		return defaultValue;
	}

	public int GetValue(string key, int defaultValue)
	{
		string value;
		if (_settings.TryGetValue(key, out value))
		{
			try
			{
				return int.Parse(value);
			}
			catch
			{
				Debug.LogError(key + " returns the defaultValue because send string was " + value);
			}
		}
		return defaultValue;
	}

	public string GetValue(string key, string defaultValue)
	{
		string value;
		if (_settings.TryGetValue(key, out value))
		{
			return value;
		}
		return defaultValue;
	}

	public bool TryGetValue(string key, out string valueString)
	{
		return _settings.TryGetValue(key, out valueString);
	}

	public bool HasValue(string key)
	{
		return _settings.ContainsKey(key);
	}

	private IEnumerator DownloadCoroutine()
	{
		WWW www = new WWW("http://hoodrunner.kiloo.com/onlinesettings_android.php");
		yield return www;
		if (www.error != null)
		{
			Debug.LogWarning("OnlineSettings download failed: " + www.error);
		}
		else
		{
			string response = www.text;
			if (response.Contains("<html"))
			{
				Debug.LogError("OnlineSettings download failed: " + response);
			}
			else if (ValidateChecksum(response))
			{
				_settings = StringUtility.ParseProperties(www.text);
				SaveToFile();
			}
			else
			{
				Debug.LogError("OnlineSettings checksum verification failed");
			}
		}
		UnityEngine.Object.Destroy(_downloadGameObject);
		_downloadGameObject = null;
		_isDownloading = false;
	}

	private static bool ValidateChecksum(string response)
	{
		int num = response.IndexOfAny(LINEBREAK_CHARS);
		if (num > 0)
		{
			string text = response.Substring(0, num);
			string data = response.Substring(num + 1).Trim();
			string checksum = SocialManager.GetChecksum(data);
			if (string.Compare(text, checksum, true) == 0)
			{
				return true;
			}
			Debug.LogWarning("OnlineSettings.ValidateChecksum failed: Computed checksum " + checksum + " does not match " + text);
		}
		else
		{
			Debug.LogWarning("OnlineSettings.ValidateChecksum failed: Unable to find checksum");
		}
		return false;
	}

	private void LoadFromFile()
	{
		try
		{
			string saveDataPath = GetSaveDataPath();
			byte[] buffer = FileUtil.Load(saveDataPath, "pdvshbhkndf92k19zvbckawd92fjk");
			MemoryStream memoryStream = new MemoryStream(buffer);
			BinaryReader binaryReader = new BinaryReader(memoryStream);
			int num = binaryReader.ReadInt32();
			_settings = FileUtil.ReadStringStringDictionary(binaryReader);
			memoryStream.Close();
		}
		catch (FileNotFoundException)
		{
		}
		catch (Exception ex2)
		{
			Debug.LogError("OnlineSettings: Failed to load OnlineSettings from file: " + ex2);
		}
	}

	private void SaveToFile()
	{
		try
		{
			MemoryStream memoryStream = new MemoryStream(4096);
			BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
			binaryWriter.Write(1);
			FileUtil.WriteStringStringDictionary(binaryWriter, _settings);
			string saveDataPath = GetSaveDataPath();
			FileUtil.Save(saveDataPath, "pdvshbhkndf92k19zvbckawd92fjk", memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
			memoryStream.Close();
		}
		catch (Exception ex)
		{
			Debug.LogError("Failed to save OnlineSettings to file: " + ex);
		}
	}

	private static string GetSaveDataPath()
	{
		return Application.persistentDataPath + "/onlinesettings";
	}
}
