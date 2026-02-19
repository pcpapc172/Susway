using UnityEngine;

[RequireComponent(typeof(UIRoot))]
public class RootScaler : MonoBehaviour
{
	private const float factor = 1.16f;

	private UIRoot myUIRoot;

	[SerializeField]
	private Camera clippingCamera3D;

	[SerializeField]
	private Camera clippingCamera2D;

	private void Awake()
	{
		myUIRoot = base.gameObject.GetComponent<UIRoot>();
		if (DeviceInfo.formFactor == DeviceInfo.FormFactor.iPad)
		{
			int manualHeight = 512;
			myUIRoot.manualHeight = manualHeight;
			if (clippingCamera3D != null && clippingCamera2D != null)
			{
				clippingCamera3D.rect = new Rect(0.125f, 0f, 0.75f, 1f);
				clippingCamera2D.rect = new Rect(0.125f, 0f, 0.75f, 1f);
				clippingCamera3D.fieldOfView = 65f;
			}
		}
		else if (DeviceInfo.formFactor == DeviceInfo.FormFactor.iPhone5)
		{
			Debug.Log("Iphone5 screen");
			myUIRoot.manualHeight = 568;
			if (clippingCamera3D != null && clippingCamera2D != null)
			{
				clippingCamera3D.fieldOfView = 76f;
			}
		}
		else if (DeviceInfo.formFactor == DeviceInfo.FormFactor.small)
		{
			float num = (float)Screen.width / 320f;
			num = (float)Screen.height / num;
			myUIRoot.manualHeight = (int)num;
		}
		else if (DeviceInfo.formFactor == DeviceInfo.FormFactor.medium)
		{
			float num2 = (float)Screen.width / 320f;
			num2 = (float)Screen.height / num2;
			myUIRoot.manualHeight = (int)num2;
		}
		else if (DeviceInfo.formFactor == DeviceInfo.FormFactor.large)
		{
			float num3 = (float)Screen.width / 320f;
			num3 = (float)Screen.height / num3;
			myUIRoot.manualHeight = (int)num3;
		}
		else if (DeviceInfo.formFactor == DeviceInfo.FormFactor.unknown)
		{
			myUIRoot.manualHeight = 512;
		}
	}
}
