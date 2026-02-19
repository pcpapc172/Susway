using Extra;
using UnityEngine;

public class ExitButton : MonoBehaviour
{
	private void OnClick()
	{
		EtceteraAndroidManager.alertButtonClickedEvent += alertButtonClickedEvent;
		EtceteraAndroid.showAlert("", Wrapper.GetTextExitDialog(), Wrapper.GetTextQuit(), Wrapper.GetTextReturn());
	}

	private void alertButtonClickedEvent(string positiveButton)
	{
		EtceteraAndroidManager.alertButtonClickedEvent -= alertButtonClickedEvent;
		Debug.Log("alertButtonClickedEvent: " + positiveButton);
		if (positiveButton.Equals(Wrapper.GetTextQuit()))
		{
			Application.Quit();
		}
	}
}
