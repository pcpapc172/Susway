using System;
using UnityEngine;

public class DiscountButton : MonoBehaviour
{
	protected const int SALE_BUTTON_GROWTH = 10;

	protected readonly Color descriptionColor = new Color(0.76862746f, 63f / 85f, 0.8509804f);

	protected Vector3 originalDescriptionPosition;

	protected Vector3 originalPricePosition;

	protected Vector3 originalFillGrayScale;

	protected Vector3 originalFillColorScale;

	protected Vector3 originalOutlineScale;

	protected Vector3 originalContentPosition;

	protected string originalFillColorSpirteName;

	protected string originalFillGraySpriteName;

	[SerializeField]
	protected UISprite icon;

	[SerializeField]
	protected UILabel title;

	[SerializeField]
	protected UILabel description;

	[SerializeField]
	protected UILabel description2;

	[SerializeField]
	protected UISprite lineThrough;

	[SerializeField]
	protected UILabel price;

	[SerializeField]
	protected UISprite fillGraySprite;

	[SerializeField]
	protected UISprite fillColorSprite;

	[SerializeField]
	protected UISprite outlineSprite;

	[SerializeField]
	protected Transform content;

	[SerializeField]
	protected UISprite discountSticker;

	[SerializeField]
	protected UILabel discountLabel;

	[SerializeField]
	protected UILabel limitedTitle;

	[SerializeField]
	protected UILabel limitedTime;

	protected static double endTimeDouble;

	protected static TimeSpan timeLeft;

	protected int priceModifier;

	private static bool Discount
	{
		get
		{
			endTimeDouble = OnlineSettings.instance.GetValue("discount_end_time", 0.0);
			DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(endTimeDouble);
			if ((dateTime - DateTime.UtcNow).Ticks > 0)
			{
				return true;
			}
			return false;
		}
	}

	public static bool DiscountDoubleCoins
	{
		get
		{
			return false;
		}
	}

	public static bool DiscountInCoinShop
	{
		get
		{
			return false;
		}
	}

	protected static bool DiscountTier(int i)
	{
		return false;
	}

	public virtual void Awake()
	{
		originalFillColorSpirteName = fillColorSprite.spriteName;
		originalFillGraySpriteName = fillGraySprite.spriteName;
		originalFillGrayScale = fillGraySprite.cachedTransform.localScale;
		originalFillColorScale = fillColorSprite.cachedTransform.localScale;
		originalOutlineScale = outlineSprite.cachedTransform.localScale;
		originalContentPosition = content.localPosition;
		originalDescriptionPosition = description.cachedTransform.localPosition;
		originalPricePosition = price.cachedTransform.localPosition;
	}

	protected void ShowExtraStuff(string productString, string sendDescription, string sendDescription2)
	{
		fillGraySprite.cachedTransform.localScale = originalFillGrayScale + new Vector3(0f, 10f, 0f);
		fillColorSprite.cachedTransform.localScale = originalFillColorScale + new Vector3(0f, 10f, 0f);
		outlineSprite.cachedTransform.localScale = originalOutlineScale + new Vector3(0f, 10f, 0f);
		content.localPosition = originalContentPosition + new Vector3(0f, 5f, 0f);
		fillColorSprite.spriteName = "button_fill_shopItem_green_sale";
		fillGraySprite.spriteName = "button_fill_shopItem_gray_sale";
		limitedTitle.gameObject.active = true;
		limitedTime.gameObject.active = true;
		limitedTitle.enabled = true;
		limitedTime.enabled = true;
		discountSticker.enabled = true;
		discountLabel.enabled = true;
		discountSticker.gameObject.active = true;
		discountLabel.gameObject.active = true;
		discountLabel.text = "COOL\nDEAL";
		price.transform.localPosition = originalPricePosition + new Vector3(0f, 4f, 0f);
		title.text = OnlineSettings.instance.GetValue("discount_deal_name", "Special Deal");
		description.text = sendDescription;
		description.cachedTransform.localPosition = originalDescriptionPosition + new Vector3(0f, 3f, 0f);
		description2.enabled = true;
		description2.text = sendDescription2;
		description.color = descriptionColor;
		description2.color = Color.white;
		lineThrough.enabled = true;
		lineThrough.cachedTransform.localScale = new Vector3(6f + description.relativeSize.x * description.cachedTransform.localScale.x, lineThrough.cachedTransform.localScale.y, 1f);
		if (DeviceInfo.isHighres)
		{
			lineThrough.spriteName = "sale_strikeover_hi";
		}
		else
		{
			lineThrough.spriteName = "sale_strikeover_lo";
		}
		Common(productString);
	}

	protected void ShowOnSale(string productString, string sendDescription = "")
	{
		fillGraySprite.cachedTransform.localScale = originalFillGrayScale + new Vector3(0f, 10f, 0f);
		fillColorSprite.cachedTransform.localScale = originalFillColorScale + new Vector3(0f, 10f, 0f);
		outlineSprite.cachedTransform.localScale = originalOutlineScale + new Vector3(0f, 10f, 0f);
		content.localPosition = originalContentPosition + new Vector3(0f, 5f, 0f);
		fillColorSprite.spriteName = "button_fill_shopItem_green_sale";
		fillGraySprite.spriteName = "button_fill_shopItem_gray_sale";
		limitedTitle.gameObject.active = true;
		limitedTime.gameObject.active = true;
		limitedTitle.enabled = true;
		limitedTime.enabled = true;
		discountSticker.enabled = true;
		discountLabel.enabled = true;
		discountSticker.gameObject.active = true;
		discountLabel.gameObject.active = true;
		discountLabel.text = Mathf.Abs(priceModifier) + " %\nOFF";
		price.transform.localPosition = originalPricePosition + new Vector3(0f, 4f, 0f);
		title.text = OnlineSettings.instance.GetValue("discount_deal_name", "Special Deal");
		description.text = InAppData.inAppData[productString].title;
		description.cachedTransform.localPosition = originalDescriptionPosition + new Vector3(0f, 3f, 0f);
		description2.enabled = true;
		if (string.IsNullOrEmpty(InAppData.inAppData[productString].description))
		{
			description2.text = sendDescription;
			description.color = descriptionColor;
			description2.color = Color.white;
		}
		else
		{
			description2.text = InAppData.inAppData[productString].description;
			description.color = Color.white;
			description2.color = descriptionColor;
		}
		if (lineThrough != null)
		{
			lineThrough.enabled = false;
		}
		Common(productString);
	}

	protected void ShowNoDiscount(string productString, string backupDescription = "")
	{
		fillGraySprite.cachedTransform.localScale = originalFillGrayScale;
		fillColorSprite.cachedTransform.localScale = originalFillColorScale;
		outlineSprite.cachedTransform.localScale = originalOutlineScale;
		content.localPosition = originalContentPosition;
		fillColorSprite.spriteName = originalFillColorSpirteName;
		fillGraySprite.spriteName = originalFillGraySpriteName;
		limitedTitle.gameObject.active = false;
		limitedTime.gameObject.active = false;
		limitedTitle.enabled = false;
		limitedTime.enabled = false;
		discountSticker.enabled = false;
		discountLabel.enabled = false;
		discountSticker.gameObject.active = false;
		discountLabel.gameObject.active = false;
		price.transform.localPosition = originalPricePosition;
		title.text = InAppData.inAppData[productString].title;
		if (string.IsNullOrEmpty(InAppData.inAppData[productString].description))
		{
			description.text = backupDescription;
		}
		else
		{
			description.text = InAppData.inAppData[productString].description;
		}
		description.color = Color.white;
		description.cachedTransform.localPosition = originalDescriptionPosition;
		description2.enabled = false;
		if (lineThrough != null)
		{
			lineThrough.enabled = false;
		}
		Common(productString);
	}

	private void Common(string productString)
	{
		icon.spriteName = InAppData.inAppData[productString].iconName;
		price.text = InAppData.inAppData[productString].price;
	}
}
