using System;
using UnityEngine;

public class ThemeToggler : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            ToggleTheme();
        }
    }

    public void ToggleTheme()
    {
        try
        {
            if (ThemeManager.Instance == null)
            {
                Debug.LogError("ThemeToggler.ToggleTheme: ThemeManager.Instance is null");
                return;
            }

            Theme currentTheme = ThemeManager.Instance.Theme ?? Theme.NORMAL;
            Theme nextTheme = (currentTheme == Theme.NORMAL) ? Theme.HALLOWEEN : Theme.NORMAL;
            Debug.Log("ThemeToggler.ToggleTheme: current=" + currentTheme + " next=" + nextTheme);
            // Atomically set theme and manual override to avoid race with HouseKeeper
            try
            {
                ThemeManager.Instance.SetTheme(nextTheme, nextTheme != Theme.NORMAL);
            }
            catch (System.Exception)
            {
                // fallback if SetTheme missing for any reason
                ThemeManager.Instance.ManualOverride = (nextTheme != Theme.NORMAL);
                ThemeManager.Instance.Theme = nextTheme;
            }
            // If theme is season-based, update PlayerInfo and Settings so the choice persists
            try
            {
                if (nextTheme == Theme.HALLOWEEN)
                {
                    PlayerInfo.Instance.currentSeasonPicked = PlayerInfo.Season.halloween;
                    Settings.optionSeason = true;
                    ThemeManager.Instance.themeExpirationDate = DateTime.MaxValue;
                }
                else
                {
                    PlayerInfo.Instance.currentSeasonPicked = PlayerInfo.Season.none;
                    Settings.optionSeason = false;
                    ThemeManager.Instance.themeExpirationDate = DateTime.MinValue;
                }
                PlayerInfo.Instance.SaveIfDirty();
            }
            catch (System.Exception ex2)
            {
                Debug.LogWarning("ThemeToggler: could not update PlayerInfo/Settings: " + ex2);
            }
            Debug.Log("ThemeToggler.ToggleTheme: Theme set to " + ThemeManager.Instance.Theme);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("ThemeToggler.ToggleTheme exception: " + ex);
        }
    }

    // Helper for UI systems that pass a string argument (NGUI/Unity UI)
    public void ToggleThemeByName(string name)
    {
        try
        {
            Theme t = Theme.FindByName(name);
            if (t == null)
            {
                Debug.LogError("ThemeToggler.ToggleThemeByName: unknown theme '" + name + "'");
                return;
            }
            Debug.Log("ThemeToggler.ToggleThemeByName: setting theme to '" + name + "'");
            ThemeManager.Instance.Theme = t;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("ThemeToggler.ToggleThemeByName exception: " + ex);
        }
    }
}
