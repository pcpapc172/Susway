using UnityEngine;

public class PauseScreen : MonoBehaviour
{
	[SerializeField]
	private GameObject[] rewardIcons = new GameObject[0];

	[SerializeField]
	private UILabel coinRewardLabel;

	[SerializeField]
	private UISprite coinRewardSprite;

	[SerializeField]
	private UIWidget[] challengeActiveWidgets = new UIWidget[0];

	[SerializeField]
	private UIWidget[] challengeCompletedWidgets = new UIWidget[0];

	private void OnEnable()
	{
		PlayerInfo.Instance.CheckIfWeShouldRemoveProgressForDailyQuestInRow();
		RefreshDailyChallenge();
	}

	private void RefreshDailyChallenge()
	{
		bool mostRecentIsToday;
		int num = Mathf.Clamp(PlayerInfo.Instance.GetDailyWordDaysInARow(out mostRecentIsToday), 0, 5);
		int num2 = 0;
		num2 = num;
		if (num2 > 4)
		{
			num2 = 4;
		}
		Debug.Log("Active day index: " + num2);
		for (int i = 0; i < rewardIcons.Length; i++)
		{
			UISprite[] componentsInChildren = rewardIcons[i].GetComponentsInChildren<UISprite>();
			foreach (UISprite uISprite in componentsInChildren)
			{
				uISprite.enabled = i == num2;
			}
		}
		if (num2 >= 4)
		{
			UISprite[] componentsInChildren2 = rewardIcons[rewardIcons.Length - 1].GetComponentsInChildren<UISprite>();
			foreach (UISprite uISprite2 in componentsInChildren2)
			{
				uISprite2.enabled = true;
			}
			coinRewardLabel.enabled = false;
			coinRewardSprite.enabled = false;
		}
		else
		{
			coinRewardLabel.enabled = true;
			coinRewardSprite.enabled = true;
			coinRewardLabel.text = DailyWord.GetRewardForDay(num2).coins.ToString();
		}
		for (int l = 0; l < challengeActiveWidgets.Length; l++)
		{
			challengeActiveWidgets[l].enabled = !mostRecentIsToday;
		}
		for (int m = 0; m < challengeCompletedWidgets.Length; m++)
		{
			challengeCompletedWidgets[m].enabled = mostRecentIsToday;
		}
	}
}
