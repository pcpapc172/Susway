using UnityEngine;

public class OfflineScreenHelper : MonoBehaviour
{
	public GameObject GameCenterBonus;

	public GameObject GameCenterNoBonus;

	public GameObject FacebookBonus;

	public GameObject FacebookNoBonus;

	public GameObject facebookButton;

	public GameObject gameCenterButton;

	private void Start()
	{
		Vector3 position = facebookButton.transform.position;
		position.x = 0f;
		facebookButton.transform.position = position;
		NGUITools.Destroy(gameCenterButton);
	}

	public void InitOfflineScreen()
	{
		if (PlayerInfo.Instance.hasPayedOutFacebook)
		{
			NGUITools.SetActive(FacebookBonus, false);
			NGUITools.SetActive(FacebookNoBonus, true);
		}
		else
		{
			NGUITools.SetActive(FacebookBonus, true);
			NGUITools.SetActive(FacebookNoBonus, false);
		}
	}
}
