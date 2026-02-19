using System;
using System.Collections;
using System.Collections.Generic;
using Extra;
using UnityEngine;

public class InAppManager : MonoBehaviour
{
	private enum InAppPurchaseState
	{
		NotStarted = 0,
		Started = 1,
		Failed = 2,
		Complete = 3
	}

	private const string GAME_OBJECT_NAME = "InAppManager";

	private static InAppManager _instance;

	private static InAppData inAppData;

	[NonSerialized]
	public AudioClip purchaseSuccessSound;

	private InAppPurchaseState _inAppPurchaseState;

	[HideInInspector]
	public bool productRequestSucceeded;

	public Action onPurchaseSuccess;

	public Action onProductRequestSuccess;

	private string inAppPurchaseKey = string.Empty;

	private string itemRequested = string.Empty;

	public static InAppManager Instance
	{
		get
		{
			Init();
			return _instance;
		}
	}

	public static bool IsInstanced()
	{
		return _instance != null;
	}

	public static void Init()
	{
		if (_instance == null)
		{
			Debug.Log("InAppManager init()");
			GameObject gameObject = new GameObject();
			gameObject.name = "InAppManager";
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			gameObject.AddComponent<InAppManager>();
		}
	}

	private void Awake()
	{
		_instance = this;
	}

	private void Start()
	{
		productRequestSucceeded = RRInappBillingPluginKit.InitInAppBillingSupport();
		inAppData = new InAppData();
		StartCoroutine(RestoreManagedAppPurchases());
	}

	public void QueryInApps()
	{
	}

	public void BuyInApp(string purchaseId)
	{
		StartPurchase(purchaseId);
	}

	public void BuyFromPopup(string purchaseId)
	{
		StartPurchase(purchaseId);
	}

	private void StartPurchase(string inAppPurchaseId)
	{
		StartPurchaseAndroid(inAppPurchaseId);
	}

	public void SetupNativePopup(int cost, string senderName)
	{
		itemRequested = senderName;
		int num = 0;
		num = cost - PlayerInfo.Instance.amountOfCoins;
		string text = string.Empty;
		if (Instance.productRequestSucceeded)
		{
			foreach (KeyValuePair<string, InAppProfile> inAppDatum in InAppData.inAppData)
			{
				if (!inAppDatum.Value.validInApp || inAppDatum.Value.amountOfCoins <= num)
				{
					continue;
				}
				if (!string.IsNullOrEmpty(text))
				{
					if (InAppData.inAppData[text].amountOfCoins > InAppData.inAppData[inAppDatum.Key].amountOfCoins)
					{
						text = inAppDatum.Key;
					}
				}
				else
				{
					text = inAppDatum.Key;
				}
			}
		}
		inAppPurchaseKey = text;
		string textNotEnough = Wrapper.GetTextNotEnough();
		if (!string.IsNullOrEmpty(inAppPurchaseKey))
		{
			string message = string.Format(Wrapper.GetTextYouNeedMoreCoins(), num, InAppData.inAppData[text].amountOfCoins);
			DeviceUtility.showNativePopup(textNotEnough, message, "Ok");
		}
		else
		{
			string message2 = string.Format(Wrapper.GetTextYouNeedMoreCoins(), num);
			DeviceUtility.showNativePopup(textNotEnough, message2, "Ok");
		}
	}

	public void NativePurchaseInappPack(string message)
	{
		if (message == "0")
		{
			inAppPurchaseKey = string.Empty;
		}
		else if (Instance.productRequestSucceeded)
		{
			Instance.BuyFromPopup(inAppPurchaseKey);
		}
		else
		{
			inAppPurchaseKey = string.Empty;
		}
	}

	public void PurchaseStatePurchased(string productId)
	{
		PlayerInfo.Instance.inAppPurchaseCount++;
		for (int i = 0; i < InAppData.inAppTiersAndInAppTiersDiscount.Length; i++)
		{
			if (InAppData.inAppTiersAndInAppTiersDiscount[i] == productId)
			{
				if (i < InAppData.inAppTiersAndInAppTiersDiscount.Length / 2)
				{
					PlayerInfo.Instance.amountOfCoins += InAppData.inAppData[productId].amountOfCoins;
					continue;
				}
				string key = "in_app_tier_" + (i - InAppData.inAppTiersAndInAppTiersDiscount.Length / 2 + 1);
				PlayerInfo.Instance.amountOfCoins += InAppData.inAppData[productId].amountOfCoins + Mathf.Clamp(OnlineSettings.instance.GetValue(key, 0), 0, int.MaxValue);
			}
		}
		if (productId == "com.kiloo.subways.doublecoins" || productId == "com.kiloo.subways.doublecoinsdiscount")
		{
			PlayerInfo.Instance.hasDoubleCoins = true;
			if (UIScreenController.isInstanced)
			{
				UIScreenController.Instance.QueueSlideIn(UIScreenController.SlideInType.DoubleCoins, string.Empty);
			}
		}
		PlayerInfo.Instance.SaveIfDirty();
		Action action = onPurchaseSuccess;
		if (action != null)
		{
			action();
		}
		if (UIScreenController.isInstanced)
		{
			UIScreenController.Instance.HideInAppPurchaseOverlay();
		}
		string[] inAppTiersAndInAppTiersDiscount = InAppData.inAppTiersAndInAppTiersDiscount;
		foreach (string text in inAppTiersAndInAppTiersDiscount)
		{
			if (text == productId)
			{
				NGUITools.PlaySound(purchaseSuccessSound);
			}
		}
		if (productId == "com.kiloo.subways.doublecoins" || productId == "com.kiloo.subways.doublecoinsdiscount")
		{
		}
		Flurry.LogEventWithAParameter("InApp purchase completed", "Id", productId);
		switch (productId)
		{
		case "com.kiloo.subways.coinstier1":
		case "com.kiloo.subways.coinstier1discount":
			if (string.IsNullOrEmpty(itemRequested))
			{
				Flurry.LogEventWithAParameter("InApp Coin Pack 1 purchased", "Mission Set", PlayerInfo.Instance.currentMissionSet.ToString());
			}
			else
			{
				Flurry.LogEventWithSeveralParameters("InApp Coin Pack 1 purchased", "Mission Set;Item tiggered iap", PlayerInfo.Instance.currentMissionSet + ";" + itemRequested);
			}
			break;
		case "com.kiloo.subways.coinstier2":
		case "com.kiloo.subways.coinstier2_discount":
			if (string.IsNullOrEmpty(itemRequested))
			{
				Flurry.LogEventWithAParameter("InApp Coin Pack 2 purchased", "Mission Set", PlayerInfo.Instance.currentMissionSet.ToString());
			}
			else
			{
				Flurry.LogEventWithSeveralParameters("InApp Coin Pack 2 purchased", "Mission Set;Item tiggered iap", PlayerInfo.Instance.currentMissionSet + ";" + itemRequested);
			}
			break;
		case "com.kiloo.subways.coinstier3":
		case "com.kiloo.subways.coinstier3discount":
			if (string.IsNullOrEmpty(itemRequested))
			{
				Flurry.LogEventWithAParameter("InApp Coin Pack 3 purchased", "Mission Set", PlayerInfo.Instance.currentMissionSet.ToString());
			}
			else
			{
				Flurry.LogEventWithSeveralParameters("InApp Coin Pack 3 purchased", "Mission Set;Item tiggered iap", PlayerInfo.Instance.currentMissionSet + ";" + itemRequested);
			}
			break;
		case "com.kiloo.subways.coinstier4":
		case "com.kiloo.subways.coinstier4discount":
			if (string.IsNullOrEmpty(itemRequested))
			{
				Flurry.LogEventWithAParameter("InApp Coin Pack 4 purchased", "Mission Set", PlayerInfo.Instance.currentMissionSet.ToString());
			}
			else
			{
				Flurry.LogEventWithSeveralParameters("InApp Coin Pack 4 purchased", "Mission Set;Item tiggered iap", PlayerInfo.Instance.currentMissionSet + ";" + itemRequested);
			}
			break;
		case "com.kiloo.subways.coinstier5":
		case "com.kiloo.subways.coinstier5discount":
			if (string.IsNullOrEmpty(itemRequested))
			{
				Flurry.LogEventWithAParameter("InApp Coin Pack 5 purchased", "Mission Set", PlayerInfo.Instance.currentMissionSet.ToString());
			}
			else
			{
				Flurry.LogEventWithSeveralParameters("InApp Coin Pack 5 purchased", "Mission Set;Item tiggered iap", PlayerInfo.Instance.currentMissionSet + ";" + itemRequested);
			}
			break;
		case "com.kiloo.subways.doublecoins":
		case "com.kiloo.subways.doublecoinsdiscount":
			if (UIScreenController.isInstanced)
			{
				if (string.IsNullOrEmpty(UIScreenController.Instance.GetCurrentPopupName()))
				{
					Flurry.LogEventWithAParameter("Double Coin purchased", "Mission Set", PlayerInfo.Instance.currentMissionSet.ToString());
				}
				else if (UIScreenController.Instance.GetCurrentPopupName() == "TutorialDoubleCoinsPopup")
				{
					Flurry.LogEventWithAParameter("Double Coin purchased", "Mission Set", PlayerInfo.Instance.currentMissionSet.ToString());
				}
				else
				{
					Flurry.LogEventWithAParameter("Double Coin purchased", "Mission Set", PlayerInfo.Instance.currentMissionSet.ToString());
				}
			}
			else
			{
				Flurry.LogEventWithAParameter("Double Coin purchased", "Mission Set", PlayerInfo.Instance.currentMissionSet.ToString());
			}
			break;
		}
		itemRequested = string.Empty;
		Screen.sleepTimeout = -2;
	}

	public void PurchaseStateCanceled(string itemId)
	{
		UIScreenController.Instance.HideInAppPurchaseOverlay();
		Screen.sleepTimeout = -2;
	}

	public void PurchaseStateRefunded(string itemId)
	{
		UIScreenController.Instance.HideInAppPurchaseOverlay();
		Screen.sleepTimeout = -2;
	}

	public void RequestSuccessful(string productId)
	{
		UIScreenController.Instance.HideInAppPurchaseOverlay();
		Screen.sleepTimeout = -2;
	}

	public void RequestFailed(string error)
	{
		UIScreenController.Instance.HideInAppPurchaseOverlay();
		Screen.sleepTimeout = -2;
	}

	public void RequestCancelled(string error)
	{
		UIScreenController.Instance.HideInAppPurchaseOverlay();
		Screen.sleepTimeout = -2;
	}

	public void ConfirmationFail(string productId)
	{
		PlayerInfo.Instance.inAppPurchaseCount--;
		for (int i = 0; i < InAppData.inAppTiersAndInAppTiersDiscount.Length; i++)
		{
			if (InAppData.inAppTiersAndInAppTiersDiscount[i] == productId)
			{
				if (i < InAppData.inAppTiersAndInAppTiersDiscount.Length / 2)
				{
					PlayerInfo.Instance.amountOfCoins -= InAppData.inAppData[productId].amountOfCoins;
					continue;
				}
				string key = "in_app_tier_" + (i - InAppData.inAppTiersAndInAppTiersDiscount.Length / 2 + 1);
				PlayerInfo.Instance.amountOfCoins -= InAppData.inAppData[productId].amountOfCoins + Mathf.Clamp(OnlineSettings.instance.GetValue(key, 0), 0, int.MaxValue);
			}
		}
		PlayerInfo.Instance.SaveIfDirty();
	}

	private void StartPurchaseAndroid(string inAppPurchaseId)
	{
		if (RRInappBillingPluginKit.InitInAppBillingSupport())
		{
			Screen.sleepTimeout = -1;
			UIScreenController.Instance.ShowInAppPurchaseOverlay();
			if (!RRInappBillingPluginKit.BuyProduct(inAppPurchaseId))
			{
				UIScreenController.Instance.HideInAppPurchaseOverlay();
				EtceteraAndroid.showAlert("Alert", "There was an error while trying to connect to the server. Please try again later!", "Ok");
				Screen.sleepTimeout = -2;
			}
		}
	}

	private static IEnumerator RestoreManagedAppPurchases()
	{
		if (!RestoreTransactions())
		{
			yield return new WaitForSeconds(5f);
			RestoreManagedAppPurchases();
		}
	}

	private static bool RestoreTransactions()
	{
		if (!PlayerPrefs.HasKey("inapp_restore_transactions"))
		{
			Debug.Log("Restore transactions");
			if (RRInappBillingPluginKit.RestoreTransactions())
			{
				PlayerPrefs.SetInt("inapp_restore_transactions", 1);
				return true;
			}
			return false;
		}
		return true;
	}

	private void OnEnable()
	{
		RRInappBillingCallback.OnRequestSuccessful += RequestSuccessful;
		RRInappBillingCallback.OnRequestFailed += RequestFailed;
		RRInappBillingCallback.OnRequestCancelled += RequestCancelled;
		RRInappBillingCallback.OnPurchaseStatePurchased += PurchaseStatePurchased;
		RRInappBillingCallback.OnPurchaseStateCanceled += PurchaseStateCanceled;
		RRInappBillingCallback.OnPurchaseStateRefunded += PurchaseStateRefunded;
		RRInappBillingCallback.OnConfirmationFail += ConfirmationFail;
	}

	private void OnDisable()
	{
		RRInappBillingCallback.OnRequestSuccessful -= RequestSuccessful;
		RRInappBillingCallback.OnRequestFailed -= RequestFailed;
		RRInappBillingCallback.OnRequestCancelled -= RequestCancelled;
		RRInappBillingCallback.OnPurchaseStatePurchased -= PurchaseStatePurchased;
		RRInappBillingCallback.OnPurchaseStateCanceled -= PurchaseStateCanceled;
		RRInappBillingCallback.OnPurchaseStateRefunded -= PurchaseStateRefunded;
		RRInappBillingCallback.OnConfirmationFail -= ConfirmationFail;
	}
}
