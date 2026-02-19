using System;
using UnityEngine;

public class UIPowerupHelper : MonoBehaviour
{
	private enum FadeTarget
	{
		none = 0,
		white = 1,
		green = 2,
		darkgreen = 3
	}

	private const float COLOR_CHANGE_SPEED = 8.2343f;

	[SerializeField]
	private UISlider slider;

	[SerializeField]
	private UISprite icon;

	[SerializeField]
	private UILabel amountLabel;

	[SerializeField]
	private UISprite foreground;

	private Color originalColor = new Color(1f, 73f / 85f, 0f);

	private float sliderSteps;

	private UIIngameUpdater _uiIngameUpdater;

	private bool inverse;

	private ActivePowerup _powerup;

	private FadeTarget currentFadeTarget;

	private FadeTarget oldFadeTarget;

	private Color storedColor;

	private float lerpTime = 1f;

	private UIIngameUpdater uiIngameUpdater
	{
		get
		{
			if (_uiIngameUpdater == null)
			{
				_uiIngameUpdater = UnityEngine.Object.FindObjectOfType(typeof(UIIngameUpdater)) as UIIngameUpdater;
			}
			return _uiIngameUpdater;
		}
	}

	private void Awake()
	{
		sliderSteps = (int)slider.fullSize.x * 2;
	}

	private void OnDisable()
	{
		if (_powerup.type == PowerupType.doubleMultiplier)
		{
			ReturnToNormal();
		}
	}

	public void ReturnToNormal()
	{
		lerpTime = 1f;
		inverse = false;
		if (uiIngameUpdater.multiplierLabel.color != originalColor)
		{
			uiIngameUpdater.multiplierLabel.color = originalColor;
		}
	}

	public void SetPowerup(ActivePowerup powerup)
	{
		_powerup = powerup;
		icon.spriteName = Upgrades.upgrades[powerup.type].iconName;
		float num = powerup.timeLeft / PlayerInfo.Instance.GetPowerupDuration(powerup.type);
		slider.sliderValue = (float)Mathf.RoundToInt(num * sliderSteps) / sliderSteps;
		if (powerup.type == PowerupType.hoverboard)
		{
			amountLabel.text = PlayerInfo.Instance.GetUpgradeAmount(powerup.type).ToString();
			amountLabel.gameObject.active = true;
		}
		else
		{
			amountLabel.gameObject.active = false;
		}
		if (powerup.type == PowerupType.doubleMultiplier)
		{
			FadingToWhiteToGreenToDarkGreen(inverse);
		}
		if (powerup.timeLeft < 0f)
		{
			if (slider.gameObject.active)
			{
				NGUITools.SetActive(slider.gameObject, false);
			}
			icon.color = Color.Lerp(Color.grey, Color.white, 0.5f + 0.5f * Mathf.Cos(powerup.timeLeft * (float)Math.PI * 4f));
			ReturnToNormal();
		}
		else
		{
			if (!slider.gameObject.active)
			{
				NGUITools.SetActive(slider.gameObject, true);
			}
			icon.color = Color.white;
		}
	}

	private void FadingToWhiteToGreenToDarkGreen(bool sendInverse)
	{
		if (sendInverse)
		{
			if (lerpTime <= 1f)
			{
				currentFadeTarget = FadeTarget.darkgreen;
				if (currentFadeTarget != oldFadeTarget)
				{
					storedColor = uiIngameUpdater.multiplierLabel.color;
				}
				uiIngameUpdater.multiplierLabel.color = new Color(Mathf.Lerp(storedColor.r, 28f / 85f, lerpTime), Mathf.Lerp(storedColor.g, 0.4627451f, lerpTime), Mathf.Lerp(storedColor.b, 0.2509804f, lerpTime));
			}
			else if (lerpTime <= 2f)
			{
				currentFadeTarget = FadeTarget.green;
				if (currentFadeTarget != oldFadeTarget)
				{
					storedColor = uiIngameUpdater.multiplierLabel.color;
				}
				uiIngameUpdater.multiplierLabel.color = new Color(Mathf.Lerp(storedColor.r, 0.58431375f, lerpTime - 1f), Mathf.Lerp(storedColor.g, 64f / 85f, lerpTime - 1f), Mathf.Lerp(storedColor.b, 2f / 15f, lerpTime - 1f));
			}
			else
			{
				currentFadeTarget = FadeTarget.none;
				inverse = false;
				lerpTime = 0f;
			}
		}
		else if (lerpTime <= 1f)
		{
			currentFadeTarget = FadeTarget.white;
			if (currentFadeTarget != oldFadeTarget)
			{
				storedColor = uiIngameUpdater.multiplierLabel.color;
			}
			uiIngameUpdater.multiplierLabel.color = new Color(Mathf.Lerp(storedColor.r, 1f, lerpTime), Mathf.Lerp(storedColor.g, 1f, lerpTime), Mathf.Lerp(storedColor.b, 1f, lerpTime));
		}
		else if (lerpTime <= 2f)
		{
			currentFadeTarget = FadeTarget.green;
			if (currentFadeTarget != oldFadeTarget)
			{
				storedColor = uiIngameUpdater.multiplierLabel.color;
			}
			uiIngameUpdater.multiplierLabel.color = new Color(Mathf.Lerp(storedColor.r, 0.58431375f, lerpTime - 1f), Mathf.Lerp(storedColor.g, 64f / 85f, lerpTime - 1f), Mathf.Lerp(storedColor.b, 2f / 15f, lerpTime - 1f));
		}
		else
		{
			currentFadeTarget = FadeTarget.none;
			inverse = true;
			lerpTime = 0f;
		}
		lerpTime += Time.deltaTime * 8.2343f;
		oldFadeTarget = currentFadeTarget;
	}
}
