using System;
using UnityEngine;

public class ThemeManager
{
    public delegate void OnChangeThemeDelegate(Theme theme);

    private Theme theme = null;
    // When true, manual changes should not be overridden by automatic season checks
    private bool manualOverride = false;

	private DateTime _themeExpirationDate;

	private static ThemeManager instance;

    public Theme Theme
    {
        get
        {
            return theme ?? Theme.NORMAL;
        }
        set
        {
            Theme newTheme = (value ?? Theme.NORMAL);
            // If a manual override is active, ignore external attempts to change the theme
            if (manualOverride && newTheme != theme)
            {
                try
                {
                    Debug.Log("ThemeManager: change to '" + newTheme + "' ignored due to ManualOverride");
                }
                catch (System.Exception)
                {
                }
                return;
            }
            theme = newTheme;
            try
            {
                Debug.Log("ThemeManager: Theme set -> " + (theme != null ? theme.Name : "null"));
            }
            catch (System.Exception)
            {
                // swallow logging errors in non-Unity test environments
            }
            // unset manual override if we set theme programmatically to NORMAL via system logic
            if (theme == Theme.NORMAL && manualOverride)
            {
                // keep manualOverride true until explicitly cleared
            }
            if (this.OnChangeTheme != null)
            {
                this.OnChangeTheme(theme);
            }
        }
    }

    public bool ManualOverride
    {
        get { return manualOverride; }
        set { manualOverride = value; }
    }

    // Atomically set theme and manual override flag to avoid race conditions
    public void SetTheme(Theme t, bool manualOverrideFlag)
    {
        Theme newTheme = (t ?? Theme.NORMAL);
        // Accept the change even if manualOverride was previously set
        manualOverride = manualOverrideFlag;
        theme = newTheme;
        try
        {
            Debug.Log("ThemeManager.SetTheme: Theme set -> " + (theme != null ? theme.Name : "null") + " manualOverride=" + manualOverride);
        }
        catch (System.Exception)
        {
        }
        if (this.OnChangeTheme != null)
        {
            this.OnChangeTheme(theme);
        }
    }

	public DateTime themeExpirationDate
	{
		get
		{
			return _themeExpirationDate;
		}
		set
		{
			_themeExpirationDate = value;
		}
	}

	public static ThemeManager Instance
	{
		get
		{
			return instance ?? (instance = new ThemeManager());
		}
	}

	public event OnChangeThemeDelegate OnChangeTheme;

    private ThemeManager()
    {
        // Ensure a sane default theme so callers don't observe a null Theme
        theme = Theme.NORMAL;
    }

	public void ForceRefresh()
	{
		Theme = theme;
	}

	public Theme themeForSeason(PlayerInfo.Season season)
	{
		Theme result = Theme.NORMAL;
		switch (season)
		{
		case PlayerInfo.Season.easter:
			result = Theme.EASTER;
			break;
		case PlayerInfo.Season.halloween:
			result = Theme.HALLOWEEN;
			break;
		case PlayerInfo.Season.xmas:
			result = Theme.XMAS;
			break;
		}
		return result;
	}
}
