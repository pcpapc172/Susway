using UnityEngine;

public class PrizeListScreen : MonoBehaviour
{
	public GameObject commonSecondBox;

	private void Start()
	{
		if (commonSecondBox != null)
		{
			commonSecondBox.transform.localScale = new Vector3(184f, 64f, 1f);
		}
	}

	private void ClosePrizeList()
	{
		if (UIScreenController.Instance.GetCurrentPopupName() == "MysteryBoxPopup")
		{
			base.transform.parent.SendMessage("ClosePrizeList", SendMessageOptions.DontRequireReceiver);
		}
		else if (UIScreenController.Instance.GetTopScreenName() == "UpgradesUI_shop")
		{
			UIScreenController.Instance.ClosePopup(base.gameObject);
		}
		else if (UIScreenController.Instance.GetTopScreenName() == "GameoverUI")
		{
			UIScreenController.Instance.ClosePopup(base.gameObject);
			UIScreenController.Instance.QueuePopup("UpgradesUI_quick");
		}
		else
		{
			Debug.LogWarning("unhandel case, " + UIScreenController.Instance.GetCurrentPopupName() + " " + UIScreenController.Instance.GetTopScreenName());
			UIScreenController.Instance.ClosePopup(base.gameObject);
		}
	}
}
