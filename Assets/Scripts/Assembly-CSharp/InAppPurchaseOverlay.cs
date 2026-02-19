using UnityEngine;

public class InAppPurchaseOverlay : MonoBehaviour
{
	public UILabel titleLabel;

	private void Awake()
	{
		if (titleLabel != null)
		{
			titleLabel.text = "Contacting Google Play...";
		}
		else
		{
			Debug.Log("In App Purchase Overlay: Title Label not set");
		}
	}
}
