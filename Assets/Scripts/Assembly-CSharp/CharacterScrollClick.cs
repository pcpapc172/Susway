using UnityEngine;

public class CharacterScrollClick : MonoBehaviour
{
	[SerializeField]
	private CharacterScreen characterScreen;

	private void OnClick()
	{
		Vector2 pos = UICamera.currentTouch.pos;
		characterScreen.ScrollClicked(pos);
	}
}
