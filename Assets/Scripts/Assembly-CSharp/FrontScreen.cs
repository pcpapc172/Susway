using System;
using UnityEngine;

public class FrontScreen : MonoBehaviour
{
	private bool _hasTriggeredTweens;

	[SerializeField]
	private GameObject[] gameobjectsToTween;

	[SerializeField]
	private GameObject discountSticker;

	public bool buttonsHaveTweened;

	public static event Action tweensFinishedAnimating;

	private void Awake()
	{
		GameObject[] array = gameobjectsToTween;
		foreach (GameObject gameObject in array)
		{
			gameObject.SetActiveRecursively(false);
		}
	}

	private void OnEnable()
	{
		if (!_hasTriggeredTweens)
		{
			Invoke("triggerTween", 0.5f);
			_hasTriggeredTweens = true;
		}
		checkDiscount();
	}

	private void triggerTween()
	{
		int num = 0;
		GameObject[] array = gameobjectsToTween;
		foreach (GameObject gameObject in array)
		{
			gameObject.SetActiveRecursively(true);
			SpringPosition springPosition = SpringPosition.Begin(gameObject, Vector3.zero, 5f);
			if (num == 0)
			{
				springPosition.callWhenFinished = "FinishedTweening";
				springPosition.eventReceiver = base.gameObject;
				num++;
			}
		}
		checkDiscount();
	}

	private void checkDiscount()
	{
		if (DiscountButton.DiscountDoubleCoins || DiscountButton.DiscountInCoinShop)
		{
			discountSticker.SetActiveRecursively(true);
		}
		else
		{
			discountSticker.SetActiveRecursively(false);
		}
	}

	private void FinishedTweening()
	{
		UpdateApp.ShowIfNeeded();
		buttonsHaveTweened = true;
		FrontScreen.tweensFinishedAnimating();
	}
}
