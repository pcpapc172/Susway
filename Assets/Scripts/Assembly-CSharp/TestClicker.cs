using UnityEngine;

public class TestClicker : MonoBehaviour
{
	private void OnEnable()
	{
	}

	private void OnDisable()
	{
	}

	private void OnClick()
	{
		Resources.UnloadUnusedAssets();
	}
}
