using UnityEngine;

public static class DeviceInfo
{
	public enum FormFactor
	{
		iPhone = 0,
		iPad = 1,
		small = 2,
		medium = 3,
		large = 4,
		iPhone5 = 5,
		unknown = 6
	}

	public enum PerformanceLevel
	{
		Low = 0,
		Medium = 1,
		High = 2
	}

	public static string deviceModel;

	public static readonly float dpi;

	public static readonly FormFactor formFactor;

	public static readonly bool isHighres;

	public static float cachedHeight;

	public static float cachedWidth;

	static DeviceInfo()
	{
		cachedHeight = -1f;
		cachedWidth = -1f;
		deviceModel = SystemInfo.deviceModel;
		cachedHeight = Screen.height;
		cachedWidth = Screen.width;
		if (Screen.height == 0)
		{
			formFactor = FormFactor.unknown;
		}
		else if (Screen.height < 500)
		{
			formFactor = FormFactor.small;
			Debug.Log("Form factor small");
		}
		else if (Screen.height < 900)
		{
			formFactor = FormFactor.medium;
			Debug.Log("Form factor medium");
		}
		else
		{
			formFactor = FormFactor.large;
			Debug.Log("Form factor large");
		}
		if (isTablet())
		{
			formFactor = FormFactor.iPad;
			Debug.Log("Form factor tablet");
		}
		if ((Screen.height >= 960 && Screen.width >= 640) || formFactor == FormFactor.unknown)
		{
			isHighres = true;
		}
		else
		{
			isHighres = false;
		}
		dpi = Screen.dpi;
		if (dpi <= 0f)
		{
			dpi = 300f;
		}
		Debug.Log("Dpi: " + dpi);
		Debug.Log("High res set to: " + isHighres);
	}

	private static bool isTablet()
	{
		float f = ((!(Screen.dpi > 0f)) ? ((float)Screen.width) : ((float)Screen.width / Screen.dpi));
		float f2 = ((!(Screen.dpi > 0f)) ? ((float)Screen.height) : ((float)Screen.height / Screen.dpi));
		double num = Mathf.Sqrt(Mathf.Pow(f, 2f) + Mathf.Pow(f2, 2f));
		Debug.Log("size of inches: " + num);
		return num >= 6.0;
	}
}
