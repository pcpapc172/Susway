using UnityEngine;

public class UIFooterHandler : MonoBehaviour
{
	public GameObject Button1;

	public GameObject Button2;

	public GameObject Button3;

	public UISlicedSprite Fill1;

	public UISlicedSprite Fill2;

	public UISlicedSprite Fill3;

	public GameObject boostDiscount;

	public GameObject coinshopDiscount;

	private void OnEnable()
	{
		if (!(boostDiscount == null) && !(coinshopDiscount == null))
		{
			boostDiscount.SetActiveRecursively(false);
			coinshopDiscount.SetActiveRecursively(false);
			if ((UIScreenController.Instance.GetTopScreenName() == "CoinsUI_shop" || UIScreenController.Instance.GetTopScreenName() == "CharacterScreen") && DiscountButton.DiscountDoubleCoins)
			{
				boostDiscount.SetActiveRecursively(true);
			}
			if ((UIScreenController.Instance.GetTopScreenName() == "UpgradesUI_shop" || UIScreenController.Instance.GetTopScreenName() == "CharacterScreen") && DiscountButton.DiscountInCoinShop)
			{
				coinshopDiscount.SetActiveRecursively(true);
			}
		}
	}
}
