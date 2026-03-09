using UnityEngine;

public class SettingsHelper : MonoBehaviour
{
	private const int SCALEINCREASE = 75;

	[SerializeField]
	private Transform fillet;

	[SerializeField]
	private Transform outline;

	[SerializeField]
	private Transform grid;

	[SerializeField]
	private Transform versionNr;

	[SerializeField]
	private GameObject seasonButton;

	[SerializeField]
	private GameObject gameCenterButton;

	private GameObject seasonButtonInstance;

	private int originalFilletScale;

	private int originalOutlineScale;

	private int originalVersionNrHeight;

	private void Awake()
	{
		originalFilletScale = (int)fillet.localScale.y;
		originalOutlineScale = (int)outline.localScale.y;
		originalVersionNrHeight = (int)versionNr.localPosition.y;
	}

	private void OnEnable()
	{
		// Ensure the season button exists in the settings grid so users can toggle themes
		if (seasonButton != null && seasonButtonInstance == null)
		{
			seasonButtonInstance = NGUITools.AddChild(grid.gameObject, seasonButton);
			seasonButtonInstance.name = "3 Season";

			// Ensure the instantiated prefab has a ThemeButton and is wired to call it on click
			ThemeButton tb = seasonButtonInstance.GetComponent<ThemeButton>();
			if (tb == null)
			{
				tb = seasonButtonInstance.AddComponent<ThemeButton>();
			}
			UIEventListener uel = UIEventListener.Get(seasonButtonInstance);
			// replace any existing handler to ensure it calls our ThemeButton.ToggleTheme
			uel.onClick = (GameObject go) =>
			{
				try { tb.ToggleTheme(); } catch (System.Exception ex) { Debug.LogWarning("SeasonButton click handler error: " + ex); }
			};

			// Force the instantiated SeasonButton's icon layers to a correct immediate state
			// so the slash overlay is guaranteed visible when theme is OFF.
			try
			{
				Theme current = ThemeManager.Instance != null ? ThemeManager.Instance.Theme : Theme.NORMAL;
				UISprite box = null;
				UISprite halloween = null;
				UISprite slash = null;
				foreach (UISprite s in seasonButtonInstance.GetComponentsInChildren<UISprite>(true))
				{
					string n = s.gameObject.name.ToLowerInvariant();
					if (n.Contains("outline") || n.Contains("box") || n.Contains("frame"))
						box = s;
					else if (n.Contains("off") || n.Contains("iconoff") || n.Contains("slash") || n.Contains("x"))
						slash = s;
					else if ((n.Contains("hallow") || n.Contains("halloween")) || (n.Contains("icon") && !n.Contains("off")))
						halloween = s;
				}

				if (box != null)
				{
					// don't override prefab scale/position — only ensure it's enabled and pixel-perfect
					box.enabled = true;
					try { box.MakePixelPerfect(); } catch (System.Exception) { }
				}
				if (halloween != null)
				{
					// keep prefab positioning/scale, only ensure enabled and correct sprite
					halloween.enabled = true;
					try { halloween.spriteName = "icon_halloween"; } catch (System.Exception) { }
					try { halloween.MakePixelPerfect(); } catch (System.Exception) { }
				}
				if (slash != null)
				{
					// ensure slash is topmost but don't change prefab scale/position
					try { slash.transform.SetAsLastSibling(); } catch (System.Exception) { }
					if (current == Theme.HALLOWEEN)
					{
						Color sc = slash.color; sc.a = 0f; slash.color = sc; slash.enabled = false;
					}
					else
					{
						Color sc = slash.color; sc.a = 1f; slash.color = sc; slash.enabled = true;
					}
				}

				// Ensure the Outline sprite is preserved and visible — prefer placing it
				// right before the icon so it draws behind the icon but above the fill.
				try
				{
					UISprite outlineSprite = null;
					UISprite iconSprite = null;
					foreach (UISprite s in seasonButtonInstance.GetComponentsInChildren<UISprite>(true))
					{
						string n = s.gameObject.name.ToLowerInvariant();
						if (n == "outline" || n.Contains("outline") || n.Contains("frame"))
							outlineSprite = s;
						if (n == "icon" || n.Contains("icon") && !n.Contains("off"))
							iconSprite = s;
					}
					if (outlineSprite != null)
					{
						outlineSprite.enabled = true;
						// place outline just before the icon if possible, otherwise keep it near front
						try
						{
							int targetIndex = 2;
							if (iconSprite != null)
								targetIndex = Mathf.Max(0, iconSprite.transform.GetSiblingIndex());
							outlineSprite.transform.SetSiblingIndex(Mathf.Max(0, targetIndex - 1));
						}
						catch (System.Exception) { }
						try { outlineSprite.MakePixelPerfect(); } catch (System.Exception) { }
					}
				}
				catch (System.Exception) { }
			}
			catch (System.Exception ex)
			{
				Debug.LogWarning("SettingsHelper: failed to force season button icon state: " + ex);
			}

			// Fix common misspelling in prefab labels at runtime if present
			UILabel[] labels = seasonButtonInstance.GetComponentsInChildren<UILabel>(true);
			foreach (UILabel lbl in labels)
			{
				if (!string.IsNullOrEmpty(lbl.text) && lbl.text.Contains("Seasonel"))
				{
					lbl.text = lbl.text.Replace("Seasonel", "Seasonal");
				}
			}
		}

		if (PlayerInfo.Instance.currentSeasonAvailable == PlayerInfo.Season.halloween)
		{
			if (gameCenterButton != null)
			{
				fillet.localScale = new Vector3(fillet.localScale.x, originalFilletScale, 1f);
				outline.localScale = new Vector3(outline.localScale.x, originalOutlineScale, 1f);
				versionNr.localPosition = new Vector3(versionNr.localPosition.x, originalVersionNrHeight, -1f);
				NGUITools.Destroy(gameCenterButton);
			}
		}
		else if (gameCenterButton != null)
		{
			fillet.localScale = new Vector3(fillet.localScale.x, originalFilletScale - 75, 1f);
			outline.localScale = new Vector3(outline.localScale.x, originalOutlineScale - 75, 1f);
			versionNr.localPosition = new Vector3(versionNr.localPosition.x, originalVersionNrHeight + 75, -1f);
			NGUITools.Destroy(gameCenterButton);
		}
		grid.GetComponent<UIGrid>().repositionNow = true;
	}
}
