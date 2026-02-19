using System.Collections.Generic;
using UnityEngine;

public class CharacterAudio : MonoBehaviour
{
	[SerializeField]
	private AudioClipInfo DodgeLeft;

	[SerializeField]
	public AudioClipInfo DodgeRight;

	[SerializeField]
	private AudioClipInfo H_Left;

	[SerializeField]
	private AudioClipInfo H_Right;

	[SerializeField]
	private AudioClipInfo StumbleSound;

	[SerializeField]
	private AudioClipInfo StumbleBushSound;

	[SerializeField]
	private AudioClipInfo StumbleSideSound;

	private Game game;

	private Character character;

	private Dictionary<Character.StumbleType, AudioClipInfo> stumbleClips;

	public void Awake()
	{
		game = Game.Instance;
		character = GetComponent<Character>();
		character.OnChangeTrack += HandleOnChangeTrack;
		character.OnStumble += HandleOnStumble;
		stumbleClips = new Dictionary<Character.StumbleType, AudioClipInfo>();
		stumbleClips.Add(Character.StumbleType.NORMAL, StumbleSound);
		stumbleClips.Add(Character.StumbleType.BUSH, StumbleBushSound);
		stumbleClips.Add(Character.StumbleType.SIDE, StumbleSideSound);
	}

	private void HandleOnChangeTrack(Character.OnChangeTrackDirection direction)
	{
		if (direction == Character.OnChangeTrackDirection.LEFT)
		{
			if (game.Modifiers.IsActive(game.Modifiers.Hoverboard))
			{
				So.Instance.playSound(H_Left);
			}
			else
			{
				So.Instance.playSound(DodgeLeft);
			}
		}
		else if (game.Modifiers.IsActive(game.Modifiers.Hoverboard))
		{
			So.Instance.playSound(H_Right);
		}
		else
		{
			So.Instance.playSound(DodgeRight);
		}
	}

	private void HandleOnStumble(Character.StumbleType stumbleType)
	{
		So.Instance.playSound(stumbleClips[stumbleType]);
	}
}
