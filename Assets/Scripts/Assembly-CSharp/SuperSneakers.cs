using System;
using System.Collections;
using UnityEngine;

public class SuperSneakers : CharacterModifier
{
	public GameObject powerupMesh;

	[HideInInspector]
	public bool isActive;

	public OnTriggerObject coinMagnetCollider;

	public float pullSpeed = 200f;

	private CharacterController characterController;

	private Character character;

	private Game game;

	public AudioClipInfo powerDownSound;

	public ActivePowerup Powerup;

	public override bool ShouldPauseInJetpack
	{
		get
		{
			return true;
		}
	}

	public void Awake()
	{
		character = Character.Instance;
		characterController = character.characterController;
		game = Game.Instance;
	}

	public override void Reset()
	{
		Paused = false;
	}

	public override IEnumerator Begin()
	{
		GameStats.Instance.pickedUpPowerups++;
		Paused = false;
		character.Stumble = false;
		isActive = true;
		powerupMesh.active = true;
		character.ChangeAnimations();
		Powerup = GameStats.Instance.TriggerPowerup(PowerupType.supersneakers);
		coinMagnetCollider.OnEnter = CoinHit;
		coinMagnetCollider.collider.enabled = true;
		character.jumpHeight = character.jumpHeightSuperSneakers;
		stop = StopSignal.DONT_STOP;
		while (Powerup.timeLeft > 0f && stop == StopSignal.DONT_STOP)
		{
			yield return 0;
		}
		coinMagnetCollider.collider.enabled = false;
		OnTriggerObject onTriggerObject = coinMagnetCollider;
		onTriggerObject.OnEnter = (OnTriggerObject.OnEnterDelegate)Delegate.Remove(onTriggerObject.OnEnter, new OnTriggerObject.OnEnterDelegate(CoinHit));
		character.jumpHeight = character.jumpHeightNormal;
		powerupMesh.active = false;
		isActive = false;
		character.ChangeAnimations();
		if (Powerup.timeLeft <= 0f)
		{
			So.Instance.playSound(powerDownSound);
		}
	}

	public void CoinHit(Collider collider)
	{
		Coin component = collider.GetComponent<Coin>();
		if (component != null)
		{
			component.collider.enabled = false;
			StartCoroutine(Pull(component));
			return;
		}
		Pickup componentInChildren = collider.GetComponentInChildren<Pickup>();
		if (componentInChildren != null)
		{
			componentInChildren.NotifyPickup(character.CharacterPickupParticleSystem);
		}
	}

	private IEnumerator Pull(Coin coin)
	{
		Transform pivot = coin.pivot.transform;
		Vector3 position = pivot.position;
		float distance = (position - characterController.transform.position).magnitude;
		yield return StartCoroutine(pTween.To(distance / (pullSpeed * game.NormalizedGameSpeed), delegate(float t)
		{
			pivot.position = Vector3.Lerp(position, powerupMesh.transform.position, t * t);
		}));
		Pickup pickup = coin.GetComponent<Pickup>();
		character.NotifyPickup(pickup);
	}
}
