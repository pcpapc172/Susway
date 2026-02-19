using System.Collections.Generic;
using Extra;

public class Characters
{
	public enum UnlockType
	{
		free = 0,
		tokens = 1,
		coins = 2
	}

	public enum CharacterType
	{
		slick = 0,
		tricky = 1,
		fresh = 2,
		spike = 3,
		yutani = 4,
		frank = 5,
		frizzy = 6,
		king = 7,
		lucy = 8,
		ninja = 9,
		tag = 10,
		tasha = 11,
		zoe = 12,
		brody = 13,
		princek = 14,
		zombiejake = 15
	}

	public struct Model
	{
		public string modelName;

		public int Price;

		public UnlockType unlockType;

		public string tokenName;

		public string tokenSprite2dName;

		public Missions.MissionTarget missionTargetKey;

		public string characterDescriptionPreBuy;

		public string characterDescriptionPostBuy;

		public string characterSeasonLimitedDescription;

		public bool isNewInThisUpdate;

		public PlayerInfo.Season characterSeason;
	}

	public static Dictionary<CharacterType, Model> characterData = new Dictionary<CharacterType, Model>
	{
		{
			CharacterType.slick,
			new Model
			{
				modelName = "Jake",
				characterDescriptionPreBuy = "This is a pre buy character description.",
				characterDescriptionPostBuy = "This is a post buy character description"
			}
		},
		{
			CharacterType.tricky,
			new Model
			{
				modelName = "Tricky",
				unlockType = UnlockType.tokens,
				tokenName = "Tricky's Hat",
				tokenSprite2dName = "trophy_tricky_token",
				Price = 3,
				characterDescriptionPreBuy = "This is a pre buy character description.",
				characterDescriptionPostBuy = "This is a post buy character description"
			}
		},
		{
			CharacterType.fresh,
			new Model
			{
				modelName = "Fresh",
				tokenName = "Fresh's Stereo",
				tokenSprite2dName = "trophy_fresh_token",
				unlockType = UnlockType.tokens,
				Price = 50,
				characterDescriptionPreBuy = "This is a pre buy character description.",
				characterDescriptionPostBuy = "This is a post buy character description"
			}
		},
		{
			CharacterType.spike,
			new Model
			{
				modelName = "Spike",
				unlockType = UnlockType.tokens,
				tokenName = "Spike's Guitar",
				tokenSprite2dName = "trophy_spike_token",
				Price = 200,
				characterDescriptionPreBuy = "This is a pre buy character description.",
				characterDescriptionPostBuy = "This is a post buy character description"
			}
		},
		{
			CharacterType.yutani,
			new Model
			{
				modelName = "Yutani",
				unlockType = UnlockType.tokens,
				tokenName = "Yutani's UFO",
				tokenSprite2dName = "trophy_yutani_token",
				Price = 500,
				missionTargetKey = Missions.MissionTarget.HaveYutani,
				characterDescriptionPreBuy = "This is a pre buy character description.",
				characterDescriptionPostBuy = "This is a post buy character description"
			}
		},
		{
			CharacterType.lucy,
			new Model
			{
				modelName = "Lucy",
				unlockType = UnlockType.coins,
				Price = 7000,
				missionTargetKey = Missions.MissionTarget.HaveLucy,
				characterDescriptionPreBuy = "This is a pre buy character description.",
				characterDescriptionPostBuy = "This is a post buy character description"
			}
		},
		{
			CharacterType.ninja,
			new Model
			{
				modelName = "Ninja",
				unlockType = UnlockType.coins,
				Price = 20000,
				missionTargetKey = Missions.MissionTarget.HaveNinja,
				characterDescriptionPreBuy = "This is a pre buy character description.",
				characterDescriptionPostBuy = "This is a post buy character description"
			}
		},
		{
			CharacterType.frank,
			new Model
			{
				modelName = "Frank",
				unlockType = UnlockType.coins,
				Price = 40000,
				missionTargetKey = Missions.MissionTarget.HaveFrank,
				characterDescriptionPreBuy = "This is a pre buy character description.",
				characterDescriptionPostBuy = "This is a post buy character description"
			}
		},
		{
			CharacterType.king,
			new Model
			{
				modelName = "King",
				unlockType = UnlockType.coins,
				Price = 80000,
				missionTargetKey = Missions.MissionTarget.HaveKing,
				characterDescriptionPreBuy = "This is a pre buy character description.",
				characterDescriptionPostBuy = "This is a post buy character description"
			}
		},
		{
			CharacterType.frizzy,
			new Model
			{
				modelName = "Frizzy",
				unlockType = UnlockType.coins,
				Price = 150000,
				missionTargetKey = Missions.MissionTarget.HaveFrizzy,
				characterDescriptionPreBuy = "This is a pre buy character description.",
				characterDescriptionPostBuy = "This is a post buy character description"
			}
		},
		{
			CharacterType.tag,
			new Model
			{
				modelName = "Tagbot",
				unlockType = UnlockType.coins,
				Price = 12000,
				missionTargetKey = Missions.MissionTarget.HaveTag,
				characterDescriptionPreBuy = "This is a pre buy character description.",
				characterDescriptionPostBuy = "This is a post buy character description"
			}
		},
		{
			CharacterType.tasha,
			new Model
			{
				modelName = "Tasha",
				unlockType = UnlockType.coins,
				Price = 30000,
				missionTargetKey = Missions.MissionTarget.HaveTasha,
				characterDescriptionPreBuy = "This is a pre buy character description.",
				characterDescriptionPostBuy = "This is a post buy character description"
			}
		},
		{
			CharacterType.zoe,
			new Model
			{
				modelName = "Zoe",
				unlockType = UnlockType.coins,
				Price = 120000,
				missionTargetKey = Missions.MissionTarget.HaveZoe,
				characterDescriptionPreBuy = "This is a pre buy character description.",
				characterDescriptionPostBuy = "This is a post buy character description"
			}
		},
		{
			CharacterType.brody,
			new Model
			{
				modelName = "Brody",
				unlockType = UnlockType.coins,
				Price = 350000,
				missionTargetKey = Missions.MissionTarget.HaveBrody,
				characterDescriptionPreBuy = "This is a pre buy character description.",
				characterDescriptionPostBuy = "This is a post buy character description"
			}
		},
		{
			CharacterType.princek,
			new Model
			{
				modelName = "Prince K",
				unlockType = UnlockType.coins,
				Price = 980000,
				missionTargetKey = Missions.MissionTarget.HavePrinceK,
				characterDescriptionPreBuy = "This is a pre buy character description.",
				characterDescriptionPostBuy = "This is a post buy character description"
			}
		},
		{
			CharacterType.zombiejake,
			new Model
			{
				modelName = "Zombie Jake",
				unlockType = UnlockType.coins,
				Price = 95000,
				characterDescriptionPreBuy = "Zombie Jake only appears during Halloween. Get him now.",
				characterDescriptionPostBuy = "This is a post buy character description",
				isNewInThisUpdate = true,
				characterSeason = PlayerInfo.Season.halloween,
				characterSeasonLimitedDescription = "Halloween Special"
			}
		}
	};

	public static List<CharacterType> characterOrder = new List<CharacterType>
	{
		CharacterType.yutani,
		CharacterType.spike,
		CharacterType.fresh,
		CharacterType.tricky,
		CharacterType.slick,
		CharacterType.zombiejake,
		CharacterType.frank,
		CharacterType.frizzy,
		CharacterType.king,
		CharacterType.lucy,
		CharacterType.ninja,
		CharacterType.tag,
		CharacterType.tasha,
		CharacterType.zoe,
		CharacterType.brody,
		CharacterType.princek
	};

	static Characters()
	{
		Wrapper.DumpCharacters();
	}
}
