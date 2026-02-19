using System;
using System.Collections;
using UnityEngine;

public class FollowingGuard : MonoBehaviour
{
	[Serializable]
	public class CatchAnimationSet
	{
		public AnimationClip avatar;

		public AnimationClip guard;

		public AnimationClip dog;

		public float catchAvatarAnimationPlayOffset;
	}

	public float distanceToCharacterMin = 10f;

	public float distanceToCharacterMax = 50f;

	public float catchUpDuration = 0.7f;

	public float resetCatchUpDuration = 1.5f;

	public float lastGroundedSmoothTime = 0.3f;

	public float xSmoothTime = 0.1f;

	public float gravity = 200f;

	public bool isShowing;

	public Animation guardAnimation;

	public Animation dogRightAnimation;

	public CatchAnimationSet[] caughtLeft;

	public CatchAnimationSet[] caughtRight;

	private string previusAvatarCaughtLeft;

	private string previusAvatarCaughtRight;

	public int debugCatchAnimationToPlay = -1;

	private Renderer[] enemyRenderers;

	public Transform[] enemies;

	private Vector3[] enemiesStartPos;

	private float y;

	private bool closeToCharacter;

	private float distanceToCharacter;

	private float lastGroundedSmooth;

	private float lastGroundedVelocity;

	private SmoothDampFloat x;

	private Game game;

	private Character character;

	private Transform characterTransform;

	private float verticalSpeed;

	public float guardProximityLoopVolume = 0.9f;

	private static FollowingGuard instance;

	private bool isPaused = true;

	public static FollowingGuard Instance
	{
		get
		{
			return instance ?? (instance = UnityEngine.Object.FindObjectOfType(typeof(FollowingGuard)) as FollowingGuard);
		}
	}

	private void Awake()
	{
		game = Game.Instance;
		character = Character.Instance;
		characterTransform = character.transform;
		enemyRenderers = base.gameObject.GetComponentsInChildren<Renderer>();
		enemiesStartPos = new Vector3[enemies.Length];
		for (int i = 0; i < enemies.Length; i++)
		{
			enemiesStartPos[i] = enemies[i].position;
		}
		x = new SmoothDampFloat(0f, xSmoothTime);
		base.GetComponent<AudioSource>().volume = guardProximityLoopVolume;
		Game obj = game;
		obj.OnPauseChange = (Game.OnPauseChangeDelegate)Delegate.Combine(obj.OnPauseChange, new Game.OnPauseChangeDelegate(HandleOnPauseChange));
		CatchAnimationSet[] array = caughtLeft;
		foreach (CatchAnimationSet catchAnimationSet in array)
		{
			SetupAvatarAnimationsStates(character.characterAnimation, catchAnimationSet.avatar);
			SetupDogGuardAnimationsStates(guardAnimation, catchAnimationSet.guard);
			SetupDogGuardAnimationsStates(dogRightAnimation, catchAnimationSet.dog);
		}
		CatchAnimationSet[] array2 = caughtRight;
		foreach (CatchAnimationSet catchAnimationSet2 in array2)
		{
			SetupAvatarAnimationsStates(character.characterAnimation, catchAnimationSet2.avatar);
			SetupDogGuardAnimationsStates(guardAnimation, catchAnimationSet2.guard);
			SetupDogGuardAnimationsStates(dogRightAnimation, catchAnimationSet2.dog);
		}
	}

	private void SetupAvatarAnimationsStates(Animation animation, AnimationClip animationClip)
	{
		AnimationClip clip = animation.GetClip(animationClip.name);
		if (clip == null)
		{
			animation.AddClip(animationClip, animationClip.name);
		}
		animation[animationClip.name].enabled = false;
		animation[animationClip.name].layer = 4;
	}

	private void SetupDogGuardAnimationsStates(Animation animation, AnimationClip animationClip)
	{
		AnimationClip clip = animation.GetClip(animationClip.name);
		if (clip == null)
		{
			animation.AddClip(animationClip, animationClip.name);
		}
	}

	private void HandleOnPauseChange(bool pause)
	{
		if (pause)
		{
			if (base.GetComponent<AudioSource>().isPlaying)
			{
				base.GetComponent<AudioSource>().Pause();
			}
			isPaused = true;
		}
		else
		{
			if (isPaused)
			{
				base.GetComponent<AudioSource>().Play();
			}
			isPaused = false;
		}
	}

	public void Restart(bool closeToCharacter)
	{
		StopAllCoroutines();
		this.closeToCharacter = closeToCharacter;
		distanceToCharacter = ((!closeToCharacter) ? distanceToCharacterMax : distanceToCharacterMin);
	}

	public void OnEnable()
	{
		lastGroundedSmooth = character.lastGroundedY;
		lastGroundedVelocity = 0f;
		y = character.lastGroundedY;
		x.Value = character.transform.position.x;
		distanceToCharacter = distanceToCharacterMin;
		closeToCharacter = true;
		verticalSpeed = 0f;
		character.OnJump += OnJump;
		character.OnRoll += OnRoll;
	}

	public void OnDisable()
	{
		character.OnJump -= OnJump;
		character.OnRoll -= OnRoll;
	}

	public void CatchUp()
	{
		CatchUp(catchUpDuration);
	}

	public void CatchUp(float duration)
	{
		if (!closeToCharacter)
		{
			float distanceFrom = distanceToCharacter;
			ShowEnemies(true);
			StopAllCoroutines();
			guardAnimation.Play("Guard_grap after");
			guardAnimation.PlayQueued("Guard_Run");
			base.GetComponent<AudioSource>().timeSamples = UnityEngine.Random.Range(0, base.GetComponent<AudioSource>().timeSamples);
			base.GetComponent<AudioSource>().Play();
			base.GetComponent<AudioSource>().pitch = UnityEngine.Random.Range(0.9f, 1.05f);
			StartCoroutine(pTween.To(duration, delegate(float t)
			{
				distanceToCharacter = Mathf.SmoothStep(distanceFrom, distanceToCharacterMin, t);
			}));
			StartCoroutine(pTween.To(duration, delegate(float t)
			{
				base.GetComponent<AudioSource>().volume = Mathf.SmoothStep(0f, guardProximityLoopVolume, t);
			}));
			closeToCharacter = true;
		}
	}

	public void ResetCatchUp()
	{
		ResetCatchUp(resetCatchUpDuration);
	}

	public void ResetCatchUp(float duration)
	{
		StartCoroutine(ResetCatchUpCoroutine(duration));
	}

	public IEnumerator ResetCatchUpCoroutine(float duration)
	{
		if (closeToCharacter)
		{
			float distanceFrom = distanceToCharacter;
			closeToCharacter = false;
			StartCoroutine(pTween.To(duration, delegate(float t)
			{
				distanceToCharacter = Mathf.SmoothStep(distanceFrom, distanceToCharacterMax, t);
			}));
			yield return StartCoroutine(pTween.To(duration * 2f, delegate(float t)
			{
				base.GetComponent<AudioSource>().volume = Mathf.SmoothStep(guardProximityLoopVolume, 0f, t);
			}));
			base.GetComponent<AudioSource>().Stop();
			if (!game.isDead)
			{
				ShowEnemies(false);
			}
		}
	}

	public void MuteProximityLoop()
	{
		base.GetComponent<AudioSource>().Stop();
	}

	public void PlayIntro()
	{
		base.gameObject.transform.position = new Vector3(0f, 0f, -10f);
		for (int i = 0; i < enemies.Length; i++)
		{
			enemies[i].position = enemiesStartPos[i];
			enemies[i].rotation = Quaternion.Euler(0f, 0f, 0f);
		}
		guardAnimation.Play("playIntro");
		dogRightAnimation.Play("playIntro");
		guardAnimation.CrossFadeQueued("Guard_Run", 0.2f);
		dogRightAnimation.CrossFadeQueued("Dog_Fast Run", 0.2f);
	}

	public void CatchPlayer(float pos)
	{
		base.GetComponent<AudioSource>().Stop();
		StopAllCoroutines();
		character.characterAnimation.Stop(previusAvatarCaughtLeft);
		character.characterAnimation.Stop(previusAvatarCaughtRight);
		int num = ((debugCatchAnimationToPlay <= -1 || debugCatchAnimationToPlay >= caughtLeft.Length) ? UnityEngine.Random.Range(0, caughtLeft.Length) : debugCatchAnimationToPlay);
		int num2 = ((debugCatchAnimationToPlay <= -1 || debugCatchAnimationToPlay >= caughtRight.Length) ? UnityEngine.Random.Range(0, caughtRight.Length) : debugCatchAnimationToPlay);
		float num3;
		if (pos < 20f)
		{
			guardAnimation.CrossFade(caughtLeft[num].guard.name, 0.2f);
			dogRightAnimation.CrossFade(caughtLeft[num].dog.name, 0.2f);
			character.animations.stumbleDeath = caughtLeft[num].avatar.name;
			num3 = caughtLeft[num].catchAvatarAnimationPlayOffset / 25f;
			previusAvatarCaughtLeft = character.animations.stumbleDeath;
		}
		else
		{
			guardAnimation.CrossFade(caughtRight[num2].guard.name, 0.2f);
			dogRightAnimation.CrossFade(caughtRight[num2].dog.name, 0.2f);
			character.animations.stumbleDeath = caughtRight[num2].avatar.name;
			num3 = caughtLeft[num2].catchAvatarAnimationPlayOffset / 25f;
			previusAvatarCaughtRight = character.animations.stumbleDeath;
		}
		character.characterAnimation[character.animations.stumbleDeath].weight = 0f;
		character.characterAnimation[character.animations.stumbleDeath].enabled = true;
		StartCoroutine(pTween.To(num3, delegate(float t)
		{
			for (int i = 0; i < enemies.Length; i++)
			{
				enemies[i].position = Vector3.Lerp(enemies[i].position, character.transform.position, t);
			}
		}));
		StartCoroutine(CatchPlayerAnimStarter(num3));
	}

	private IEnumerator CatchPlayerAnimStarter(float delay)
	{
		yield return new WaitForSeconds(delay);
		StartCoroutine(pTween.To(0.2f, delegate(float t)
		{
			character.characterAnimation[character.animations.stumbleDeath].weight = Mathf.Lerp(0f, 1f, t);
		}));
	}

	public void HitByTrainSequence()
	{
		base.GetComponent<AudioSource>().Stop();
		StartCoroutine(HitByTrainSequenceCoroutine());
	}

	public IEnumerator HitByTrainSequenceCoroutine()
	{
		GameStats.Instance.guardHitScreen++;
		float catchUpTime = 0.2f;
		yield return StartCoroutine(pTween.To(catchUpTime, delegate(float t)
		{
			for (int i = 0; i < enemies.Length; i++)
			{
				enemies[i].position = Vector3.Lerp(enemies[i].position, character.transform.position, t);
			}
		}));
		dogRightAnimation.Play("Dog_death_movingTrain");
		yield return new WaitForSeconds(0.4f);
		Vector3 charPos = characterTransform.position;
		StartCoroutine(pTween.To(1f, delegate(float t)
		{
			characterTransform.position = Vector3.Lerp(charPos, new Vector3(charPos.x, -5f, charPos.z), t);
		}));
		yield return new WaitForSeconds(0.2f);
		guardAnimation.Play("Guard_death_movingTrain");
	}

	public void ShowEnemies(bool vis)
	{
		isShowing = vis;
		Renderer[] array = enemyRenderers;
		foreach (Renderer renderer in array)
		{
			renderer.gameObject.active = vis;
		}
	}

	public void LateUpdate()
	{
		x.Target = character.transform.position.x;
		x.Update();
		lastGroundedSmooth = Mathf.SmoothDamp(lastGroundedSmooth, character.lastGroundedY, ref lastGroundedVelocity, lastGroundedSmoothTime);
		if (y > lastGroundedSmooth)
		{
			verticalSpeed -= gravity * Time.deltaTime;
		}
		y += verticalSpeed * Time.deltaTime;
		y = Mathf.Max(y, lastGroundedSmooth);
		Vector3 position = characterTransform.position - Vector3.forward * distanceToCharacter;
		position.y = y;
		position.x = x.Value;
		base.transform.position = position;
	}

	private void OnRoll()
	{
		StartCoroutine(RollCoroutine(distanceToCharacter / game.currentSpeed));
	}

	private IEnumerator RollCoroutine(float delay)
	{
		yield return new WaitForSeconds(delay);
		verticalSpeed = 0f - character.CalculateJumpVerticalSpeed();
	}

	private void OnJump()
	{
		Jump(distanceToCharacter / game.currentSpeed);
	}

	public void Jump(float delay)
	{
		if (distanceToCharacter <= distanceToCharacterMin)
		{
			Missions.Instance.PlayerDidThis(Missions.MissionTarget.GuardJump);
		}
		StartCoroutine(JumpCoroutine(delay));
	}

	private IEnumerator JumpCoroutine(float delay)
	{
		yield return new WaitForSeconds(delay);
		guardAnimation.Play("Guard_jump");
		guardAnimation.CrossFadeQueued("Guard_Run", 0.2f);
		dogRightAnimation.Play("Dog_jump");
		dogRightAnimation.CrossFadeQueued("Dog_Fast Run", 0.2f);
		verticalSpeed = character.CalculateJumpVerticalSpeed() * 0.7f;
	}
}
