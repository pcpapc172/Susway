using UnityEngine;

public class RRInappBillingCallback : MonoBehaviour
{
	public delegate void ProductPurchasedEventHandler(string productIdentifier);

	public delegate void StoreKitErrorEventHandler(string error);

	public delegate void PurchaseStateChangeEventHandler(string itemId);

	public delegate void ConfirmationFailedEventHandler(string productIdentifier);

	public static event PurchaseStateChangeEventHandler OnPurchaseStatePurchased;

	public static event PurchaseStateChangeEventHandler OnPurchaseStateCanceled;

	public static event PurchaseStateChangeEventHandler OnPurchaseStateRefunded;

	public static event ProductPurchasedEventHandler OnRequestSuccessful;

	public static event StoreKitErrorEventHandler OnRequestFailed;

	public static event StoreKitErrorEventHandler OnRequestCancelled;

	public static event ConfirmationFailedEventHandler OnConfirmationFail;

	private void Awake()
	{
		base.gameObject.name = GetType().ToString();
		Object.DontDestroyOnLoad(base.gameObject);
	}

	public void PurchaseStatePurchased(string itemId)
	{
		if (RRInappBillingCallback.OnPurchaseStatePurchased != null)
		{
			RRInappBillingCallback.OnPurchaseStatePurchased(itemId);
		}
	}

	public void PurchaseStateCanceled(string itemId)
	{
		if (RRInappBillingCallback.OnPurchaseStateCanceled != null)
		{
			RRInappBillingCallback.OnPurchaseStateCanceled(itemId);
		}
	}

	public void PurchaseStateRefunded(string itemId)
	{
		if (RRInappBillingCallback.OnPurchaseStateRefunded != null)
		{
			RRInappBillingCallback.OnPurchaseStateRefunded(itemId);
		}
	}

	public void RequestProductPurchased(string productIdentifier)
	{
		if (RRInappBillingCallback.OnRequestSuccessful != null)
		{
			RRInappBillingCallback.OnRequestSuccessful(productIdentifier);
		}
	}

	public void RequestProductCancelled(string error)
	{
		if (RRInappBillingCallback.OnRequestCancelled != null)
		{
			RRInappBillingCallback.OnRequestCancelled(error);
		}
	}

	public void RequestProductFailed(string error)
	{
		if (RRInappBillingCallback.OnRequestFailed != null)
		{
			RRInappBillingCallback.OnRequestFailed(error);
		}
	}

	public void ConfirmationFailed(string productIdentifier)
	{
		if (RRInappBillingCallback.OnConfirmationFail != null)
		{
			RRInappBillingCallback.OnConfirmationFail(productIdentifier);
		}
	}
}
