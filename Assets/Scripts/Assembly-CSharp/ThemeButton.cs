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

    // Smoothly fade a single sprite alpha to target value
    private IEnumerator CrossfadeAlpha(UISprite sprite, float targetAlpha, float duration)
    {
        if (sprite == null)
            yield break;
        sprite.enabled = true;
        float start = sprite.color.a;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(start, targetAlpha, Mathf.Clamp01(t / duration));
            Color c = sprite.color; c.a = a; sprite.color = c;
            yield return null;
        }
        Color fc = sprite.color; fc.a = targetAlpha; sprite.color = fc;
        if (targetAlpha <= 0f)
            sprite.enabled = false;
    }

    // Update visuals (icons/text) on the button based on selected theme
    private void UpdateIcons(Theme theme)
    {
        try
        {
            // Prefer explicit layered children: box, halloween icon, slash overlay
            UISprite box = null;
            UISprite outline = null;
            UISprite halloween = null;
            UISprite slash = null;
            foreach (UISprite s in gameObject.GetComponentsInChildren<UISprite>(true))
            {
                string n = s.gameObject.name.ToLowerInvariant();
                // map exact roles
                if (n == "outline" || n.Contains("outline"))
                {
                    outline = s;
                }
                else if (n.Contains("box") || n.Contains("frame") || n.Contains("fill"))
                {
                    box = s;
                }
                else if (n.Contains("off") || n.Contains("iconoff") || n.Contains("slash") || n.Contains("x"))
                {
                    // treat any "off" or "iconoff" or "slash" child as the slash overlay
                    slash = s;
                }
                else if ((n.Contains("hallow") || n.Contains("halloween")) || (n.Contains("icon") && !n.Contains("off")))
                {
                    // halloween icon may be named explicitly or simply 'Icon' (but not 'IconOff')
                    halloween = s;
                }
            }

            // Ensure box and halloween sprite present
            if (box != null)
            {
                box.enabled = true;
                try { box.MakePixelPerfect(); } catch (System.Exception) { }
            }
            if (halloween != null)
            {
                halloween.enabled = true;
                try { halloween.spriteName = "icon_halloween"; } catch (System.Exception) { }
                try { halloween.MakePixelPerfect(); } catch (System.Exception) { }
            }

            // Slash indicates disabled state. When theme is HALLOWEEN -> hide slash. When NORMAL -> show slash over icon.
            if (slash != null)
            {
                // arrange draw order: box (bottom), halloween (middle), slash (top)
                try
                {
                    // Desired draw order (bottom -> top): box/fill, outline, halloween/icon, slash
                    if (box != null) box.transform.SetAsFirstSibling();
                    if (outline != null)
                    {
                        // place outline just after box
                        if (box != null)
                            outline.transform.SetSiblingIndex(Mathf.Min(box.transform.GetSiblingIndex() + 1, outline.transform.parent.childCount - 1));
                        else
                            outline.transform.SetAsFirstSibling();
                        outline.enabled = true;
                        try { outline.MakePixelPerfect(); } catch (System.Exception) { }
                    }
                    if (halloween != null)
                    {
                        if (outline != null)
                            halloween.transform.SetSiblingIndex(Mathf.Min(outline.transform.GetSiblingIndex() + 1, halloween.transform.parent.childCount - 1));
                        else if (box != null)
                            halloween.transform.SetSiblingIndex(Mathf.Min(box.transform.GetSiblingIndex() + 1, halloween.transform.parent.childCount - 1));
                        else
                            halloween.transform.SetAsFirstSibling();
                    }
                    // slash on top
                    if (slash != null) slash.transform.SetAsLastSibling();
                    // don't override prefab transforms for slash/icon positioning
                }
                catch (System.Exception) { }

                // if already running a crossfade, stop it by stopping all coroutines on this component
                StopAllCoroutines();
                // Ensure immediate state is correct before animating to avoid missing slash due to race
                if (theme == Theme.HALLOWEEN)
                {
                    if (slash != null)
                    {
                        // make slash fully transparent and disabled
                        Color sc = slash.color; sc.a = 0f; slash.color = sc; slash.enabled = false;
                    }
                    if (halloween != null)
                    {
                        Color hc = halloween.color; hc.a = 1f; halloween.color = hc; halloween.enabled = true;
                    }
                    // fade to final state gently
                    if (slash != null) StartCoroutine(CrossfadeAlpha(slash, 0f, 0.18f));
                    if (halloween != null) StartCoroutine(CrossfadeAlpha(halloween, 1f, 0.18f));
                }
                else
                {
                    if (slash != null)
                    {
                        // ensure slash is enabled and visible immediately
                        slash.enabled = true;
                        Color sc = slash.color; sc.a = 1f; slash.color = sc;
                    }
                    if (halloween != null)
                    {
                        Color hc = halloween.color; hc.a = 0.95f; halloween.color = hc; halloween.enabled = true;
                    }
                    // animate in case we need smoothness
                    if (slash != null) StartCoroutine(CrossfadeAlpha(slash, 1f, 0.18f));
                    if (halloween != null) StartCoroutine(CrossfadeAlpha(halloween, 0.95f, 0.18f));
                }
            }
            else
            {
                // fallback behavior: single sprite handling
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
