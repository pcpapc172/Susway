using UnityEngine;

public class BackgroundHelper : MonoBehaviour
{
	[SerializeField]
	private Material backgroundMat;

	[SerializeField]
	private Texture normalTex;

	[SerializeField]
	private Texture halloweenTex;

	private Theme cachedTheme = Theme.NORMAL;

	private bool _hasInited;

	private void OnEnable()
	{
		if (!_hasInited)
		{
			backgroundMat.mainTexture = normalTex;
			ThemeManager.Instance.OnChangeTheme += SetBackgroundTexture;
		}
		SetBackgroundTexture(ThemeManager.Instance.Theme);
	}

	private void SetBackgroundTexture(Theme newTheme)
	{
		if (newTheme != cachedTheme)
		{
			if (newTheme == Theme.HALLOWEEN)
			{
				backgroundMat.mainTexture = halloweenTex;
			}
			else
			{
				backgroundMat.mainTexture = normalTex;
			}
			cachedTheme = newTheme;
		}
	}
}
