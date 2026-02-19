using UnityEngine;

public class HackFix : MonoBehaviour
{
	private int originalMask;

	private void Awake()
	{
		originalMask = base.camera.cullingMask;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.C))
		{
			base.camera.cullingMask = 2048;
		}
		if (Input.GetKeyDown(KeyCode.V))
		{
			base.camera.cullingMask = originalMask;
		}
	}
}
