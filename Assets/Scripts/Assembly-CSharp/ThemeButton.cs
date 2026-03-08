using System;
using System.Collections;
using UnityEngine;

// Lightweight helper to be attached to UI button GameObjects (NGUI/Unity UI).
// Exposes OnClick(), ToggleTheme() and ToggleThemeByName(string) so prefab
// UIButtonMessage can target this GameObject and call the function by name.
public class ThemeButton : MonoBehaviour
{
    private bool isBusy = false;
    // NGUI-style OnClick (no args)
    private void OnClick()
    {
        ToggleTheme();
    }

    private void OnEnable()
    {
        // subscribe to theme changes so the button stays in sync
        try
        {
            ThemeManager.Instance.OnChangeTheme += OnThemeChanged;
            // initialize visuals to current theme
            UpdateIcons(ThemeManager.Instance.Theme);
        }
        catch (System.Exception)
        {
        }
    }

    private void OnDisable()
    {
        try
        {
            ThemeManager.Instance.OnChangeTheme -= OnThemeChanged;
        }
        catch (System.Exception)
        {
        }
    }

    private void OnThemeChanged(Theme t)
    {
        UpdateIcons(t);
    }

    // Coroutine to crossfade between two UISprites (on -> off)
    private IEnumerator CrossfadeIconsCoroutine(UISprite iconOn, UISprite iconOff, float duration)
    {
        if (iconOn == null && iconOff == null)
            yield break;

        isBusy = true;
        float t = 0f;

        // Ensure both enabled so we can fade
        if (iconOn != null) { iconOn.enabled = true; var c = iconOn.color; c.a = 0f; iconOn.color = c; }
        if (iconOff != null) { iconOff.enabled = true; var c = iconOff.color; c.a = 1f; iconOff.color = c; }

        while (t < duration)
        {
            t += Time.deltaTime;
            float a = Mathf.Clamp01(t / duration);
            if (iconOn != null)
            {
                Color c = iconOn.color; c.a = a; iconOn.color = c;
            }
            if (iconOff != null)
            {
                Color c = iconOff.color; c.a = 1f - a; iconOff.color = c;
            }
            yield return null;
        }

        // Finalize
        if (iconOn != null)
        {
            Color c = iconOn.color; c.a = 1f; iconOn.color = c; iconOn.MakePixelPerfect();
        }
        if (iconOff != null)
        {
            Color c = iconOff.color; c.a = 0f; iconOff.color = c; iconOff.enabled = false;
        }

        // small cooldown to prevent rapid toggles
        yield return new WaitForSeconds(0.15f);
        isBusy = false;
    }

    // Update visuals (icons/text) on the button based on selected theme
    private void UpdateIcons(Theme theme)
    {
        try
        {
            // Prefer explicit Icon / IconOff children if present (NGUI prefab layout)
            UISprite iconOn = null;
            UISprite iconOff = null;
            foreach (UISprite s in gameObject.GetComponentsInChildren<UISprite>(true))
            {
                string n = s.gameObject.name.ToLowerInvariant();
                if (n.Contains("iconoff") || n.Contains("icon_off") || n.Contains("iconoff"))
                    iconOff = s;
                else if (n.Contains("icon") || n.Contains("icon_on") || n.Contains("iconon"))
                    iconOn = s;
            }

            // If explicit icons found, toggle them. Otherwise fall back to first sprite.
            if (iconOn != null || iconOff != null)
            {
                if (theme == Theme.HALLOWEEN)
                {
                    if (iconOn != null) iconOn.enabled = true;
                    if (iconOff != null) iconOff.enabled = false;
                }
                else
                {
                    if (iconOn != null) iconOn.enabled = false;
                    if (iconOff != null) iconOff.enabled = true;
                }
                if (iconOn != null) iconOn.MakePixelPerfect();
                if (iconOff != null) iconOff.MakePixelPerfect();
            }
            else
            {
                // fallback: set first sprite's name
                UISprite icon = gameObject.GetComponentInChildren<UISprite>(true);
                if (icon != null)
                {
                    if (theme == Theme.HALLOWEEN)
                        icon.spriteName = "icon_halloween";
                    else
                        icon.spriteName = "icon_default";
                    icon.MakePixelPerfect();
                }
            }

            UILabel title = gameObject.GetComponentInChildren<UILabel>(true);
            if (title != null)
            {
                if (theme == Theme.HALLOWEEN)
                    title.text = "Halloween ON";
                else
                    title.text = "Halloween OFF";
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning("ThemeButton.UpdateIcons failed: " + ex);
        }
    }

    // Toggle between NORMAL and HALLOWEEN
    public void ToggleTheme()
    {
        try
        {
            if (ThemeManager.Instance == null)
            {
                Debug.LogError("ThemeButton.ToggleTheme: ThemeManager.Instance is null");
                return;
            }

            Theme current = ThemeManager.Instance.Theme;
            Theme next = (current == Theme.NORMAL) ? Theme.HALLOWEEN : Theme.NORMAL;
            Debug.Log("ThemeButton.ToggleTheme: current=" + current + " next=" + next);
            // Use atomic SetTheme if available to avoid race conditions
            try
            {
                ThemeManager.Instance.SetTheme(next, next != Theme.NORMAL);
            }
            catch (System.Exception)
            {
                ThemeManager.Instance.ManualOverride = (next != Theme.NORMAL);
                ThemeManager.Instance.Theme = next;
            }
            UpdateIcons(next);
        }
        catch (Exception ex)
        {
            Debug.LogError("ThemeButton.ToggleTheme exception: " + ex);
        }
    }

    // Allows prefabs to call ToggleThemeByName with a string parameter
    // e.g. functionName: ToggleThemeByName and parameter: "halloween"
    public void ToggleThemeByName(string name)
    {
        try
        {
            Theme t = Theme.FindByName(name);
            if (t == null)
            {
                Debug.LogError("ThemeButton.ToggleThemeByName: unknown theme '" + name + "'");
                return;
            }
            if (ThemeManager.Instance == null)
            {
                Debug.LogError("ThemeButton.ToggleThemeByName: ThemeManager.Instance is null");
                return;
            }
            Debug.Log("ThemeButton.ToggleThemeByName: setting theme to '" + name + "'");
            try
            {
                ThemeManager.Instance.SetTheme(t, t != Theme.NORMAL);
            }
            catch (System.Exception)
            {
                ThemeManager.Instance.ManualOverride = (t != Theme.NORMAL);
                ThemeManager.Instance.Theme = t;
            }
            UpdateIcons(t);
        }
        catch (Exception ex)
        {
            Debug.LogError("ThemeButton.ToggleThemeByName exception: " + ex);
        }
    }
}
