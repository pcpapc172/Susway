using UnityEngine;

public class UIMysteryBoxButton : UIBasicButton
{
	protected override void Send()
	{
		MysteryBoxReward mysteryBoxReward = MysteryBox.Roll(MysteryBox.Type.Normal);
		Debug.Log("TODO Reward: " + mysteryBoxReward.amount + "x" + mysteryBoxReward.type.ToString());
	}
}
