using System;
using System.Collections;
using UnityEngine;

public class CharacterPickupParticles : MonoBehaviour
{
	public static Vector3 coinEfxOffset = 1.2f * Vector3.forward;

	public GameObject CoinEFX;

	public GameObject PowerUpEFX;

	// Optional: assign the AnimationClip asset for the "pickup" animation in the Inspector.
	// If left null the script will attempt to play "pickup" only if already present on the Animation.
	public AnimationClip pickupClip;

	public Transform master;

	public AudioClipInfo CoinPickup;

	public AudioClipInfo PowerUpPickup;

	public float CoinDistanceForStairway;

	private int coinStairway;

	private int flyWay;

	private int[] pentatonicScale = new int[17]
	{
		12, 13, 14, 15, 16, 17, 18, 19, 20, 21,
		22, 23, 24, 25, 26, 27, 28
	};

	public AnimationCurve compressCurve;

	public void Awake()
	{
		// Ensure the "pickup" clip is available at runtime on the EFX Animation components.
		AddPickupClipIfMissing(CoinEFX);
		AddPickupClipIfMissing(PowerUpEFX);
	}

	public void Start()
	{
	}

	public void PickedUpCoin(Pickup pickup)
	{
		if (80f < pickup.transform.position.y)
		{
			coinStairway = 0;
			CoinPickup.maxPitch = Mathf.Pow(2f, compressCurve.Evaluate((float)flyWay / 48f));
			CoinPickup.minPitch = Mathf.Pow(2f, compressCurve.Evaluate((float)flyWay / 48f));
			flyWay++;
		}
		else if (pickup.transform.position.y < 0.1f || (8.795f < pickup.transform.position.y && pickup.transform.position.y < 8.805f) || (9.95f < pickup.transform.position.y && pickup.transform.position.y < 10.05f) || (28.95f < pickup.transform.position.y && pickup.transform.position.y < 29.05f) || (34.95f < pickup.transform.position.y && pickup.transform.position.y < 35.05f))
		{
			flyWay = 0;
			coinStairway = 0;
			CoinPickup.maxPitch = Mathf.Pow(2f, (float)pentatonicScale[coinStairway % pentatonicScale.Length] / 12f) * 0.5f;
			CoinPickup.minPitch = Mathf.Pow(2f, (float)pentatonicScale[coinStairway % pentatonicScale.Length] / 12f) * 0.5f;
		}
		else
		{
			flyWay = 0;
			if (coinStairway < pentatonicScale.Length - 1)
			{
				coinStairway++;
			}
			CoinPickup.maxPitch = Mathf.Pow(2f, (float)pentatonicScale[coinStairway % pentatonicScale.Length] / 12f) * 0.5f;
			CoinPickup.minPitch = Mathf.Pow(2f, (float)pentatonicScale[coinStairway % pentatonicScale.Length] / 12f) * 0.5f;
		}
		So.Instance.playSound(CoinPickup);
		DoCoinEFX();
	}

	private void DoCoinEFX()
	{
		float zAngle = UnityEngine.Random.Range(0f, 360f);
		CoinEFX.transform.Rotate(0f, 0f, zAngle);
		PlayPickupAnimation(CoinEFX);
	}

	public void PickedUpPowerUp()
	{
		So.Instance.playSound(PowerUpPickup);
		PickedUpDefaultPowerUp();
	}

	public void PickedUpDefaultPowerUp()
	{
		DoCoinEFX();
		float zAngle = UnityEngine.Random.Range(0f, 360f);
		PowerUpEFX.transform.Rotate(0f, 0f, zAngle);
		PlayPickupAnimation(PowerUpEFX);
	}

	private void AddPickupClipIfMissing(GameObject efx)
	{
		if (efx == null)
			return;
		Animation anim = efx.GetComponent<Animation>();
		if (anim == null)
			return;
		if (FindClipName(anim, "pickup") == null)
		{
			if (pickupClip != null)
			{
				anim.AddClip(pickupClip, "pickup");
			}
			else
			{
				Debug.LogWarning("CharacterPickupParticles: 'pickup' clip missing and pickupClip not assigned. Available clips: " + GetClipList(anim));
			}
		}
	}

	private string GetClipList(Animation anim)
	{
		if (anim == null)
			return "<null>";
		System.Text.StringBuilder sb = new System.Text.StringBuilder();
		foreach (AnimationState s in anim)
		{
			sb.Append(s.name).Append(",");
		}
		return sb.ToString().TrimEnd(',');
	}

	private string FindClipName(Animation anim, string baseName)
	{
		if (anim == null)
		{
			return null;
		}
		if (anim.GetClip(baseName) != null)
		{
			return baseName;
		}
		foreach (AnimationState state in anim)
		{
			if (state.name.Equals(baseName, StringComparison.OrdinalIgnoreCase))
			{
				return state.name;
			}
		}
		foreach (AnimationState state in anim)
		{
			if (state.name.IndexOf(baseName, StringComparison.OrdinalIgnoreCase) >= 0)
			{
				return state.name;
			}
		}
		return null;
	}

	private void PlayPickupAnimation(GameObject efx)
	{
		if (efx == null)
		{
			return;
		}
		Animation anim = efx.GetComponent<Animation>();
		if (anim == null)
		{
			return;
		}
		string clipName = FindClipName(anim, "pickup");
		if (clipName != null && anim[clipName] != null)
		{
			anim.Stop(clipName);
			anim.Play(clipName);
			StartCoroutine(AnimateAlpha(efx, anim[clipName].length));
		}
		else
		{
			Debug.LogWarning("CharacterPickupParticles: 'pickup' clip not found on " + efx.name + " Animation. Available clips: " + GetClipList(anim));
		}
	}

	private IEnumerator AnimateAlpha(GameObject efx, float time)
	{
		return pTween.To(time, delegate(float t)
		{
			efx.GetComponent<Renderer>().sharedMaterial.SetColor("_MainColor", Color.Lerp(Color.white, Color.black, t));
		});
	}

	private IEnumerator TimeScaleTest(float time)
	{
		Time.timeScale = 0.5f;
		yield return new WaitForSeconds(time);
		Time.timeScale = 1f;
	}
}
