using System;
using System.Collections.Generic;
using UnityEngine;

namespace Extra
{
	public class Wrapper
	{
		private static readonly string DEBUG_CLASS = "debug.Debug";

		private static readonly string CLASS_LANGUAGE = "debug.Language";

		private static readonly string METHOD_GET_LANGUAGE = "GetLanguageIndex";

		private static readonly int[] UPGRADE_PRICES = new int[6] { 0, 500, 1500, 2000, 2500, 3000 };

		private static GameObject mFriends = null;

		private static GameObject mFacebook = null;

		private static bool mShopPositionSet = false;

		private static int mLanguage = -1;

		public static int GetLanguageIndex()
		{
			try
			{
				if (mLanguage < 0)
				{
					mLanguage = DoJavaCallReturnInt(CLASS_LANGUAGE, METHOD_GET_LANGUAGE);
				}
				return mLanguage;
			}
			catch (Exception)
			{
				return 0;
			}
		}

		public static void MoveOffscreen(GameObject inObject)
		{
			try
			{
				if (inObject != null)
				{
					Vector3 position = inObject.transform.position;
					if (position.x > -100f)
					{
						inObject.transform.position = new Vector3(-200f, position.y, position.z);
					}
				}
			}
			catch (Exception)
			{
			}
		}

		public static void DumpCharacters()
		{
			try
			{
				Dictionary<Characters.CharacterType, Characters.Model> dictionary = new Dictionary<Characters.CharacterType, Characters.Model>();
				foreach (KeyValuePair<Characters.CharacterType, Characters.Model> characterDatum in Characters.characterData)
				{
					Characters.Model value;
					if (Characters.characterData.TryGetValue(characterDatum.Key, out value))
					{
						switch (characterDatum.Value.modelName)
						{
						case "Yutani":
							value.Price = 5;
							break;
						case "Spike":
							value.Price = 5;
							break;
						case "Fresh":
							value.Price = 5;
							break;
						case "Tricky":
							value.Price = 5;
							break;
						case "Frank":
							value.Price = 2500;
							break;
						case "Frizzy":
							value.Price = 4000;
							break;
						case "King":
							value.Price = 2500;
							break;
						case "Lucy":
							value.Price = 3000;
							break;
						case "Ninja":
							value.Price = 2500;
							break;
						case "Tagbot":
							value.Price = 1000;
							break;
						case "Tasha":
							value.Price = 3000;
							break;
						case "Zoe":
							value.Price = 2500;
							break;
						case "Brody":
							value.Price = 2000;
							break;
						case "Prince K":
							value.Price = 3000;
							break;
						case "Zombie Jake":
							value.Price = 4000;
							break;
						}
						dictionary.Add(characterDatum.Key, value);
					}
				}
				Characters.characterData = dictionary;
			}
			catch (Exception)
			{
			}
		}

		public static void DumpUpgrades()
		{
			try
			{
				Dictionary<PowerupType, Upgrade> dictionary = new Dictionary<PowerupType, Upgrade>();
				foreach (KeyValuePair<PowerupType, Upgrade> upgrade in Upgrades.upgrades)
				{
					Upgrade value;
					if (Upgrades.upgrades.TryGetValue(upgrade.Key, out value))
					{
						switch (upgrade.Key)
						{
						case PowerupType.jetpack:
						case PowerupType.supersneakers:
						case PowerupType.coinmagnet:
						case PowerupType.doubleMultiplier:
							value.pricesRaw = UPGRADE_PRICES;
							break;
						}
					}
					dictionary.Add(upgrade.Key, value);
				}
				Upgrades.upgrades = dictionary;
			}
			catch (Exception)
			{
			}
		}

		public static void DumpMissions()
		{
			try
			{
				if (GetLanguageIndex() <= 0)
				{
					return;
				}
				Dictionary<Missions.MissionType, MissionTemplate> dictionary = new Dictionary<Missions.MissionType, MissionTemplate>();
				foreach (KeyValuePair<Missions.MissionType, MissionTemplate> missionTemplate in Missions.missionTemplates)
				{
					MissionTemplate value;
					if (Missions.missionTemplates.TryGetValue(missionTemplate.Key, out value))
					{
						if (missionTemplate.Value.descriptionSingle != null)
						{
							value.descriptionSingle = "#M#" + Text.TranslateMission("" + missionTemplate.Key, value.descriptionSingle);
							value.description = "#M#" + Text.TranslateMission("" + missionTemplate.Key, value.description);
							value.ultraShortDescriptionSingle = "#M#" + Text.TranslateMission("" + missionTemplate.Key, value.ultraShortDescriptionSingle);
							value.ultraShortDescription = "#M#" + Text.TranslateMission("" + missionTemplate.Key, value.ultraShortDescription);
						}
						dictionary.Add(missionTemplate.Key, value);
					}
				}
				Missions.missionTemplates = dictionary;
			}
			catch (Exception)
			{
			}
		}

		public static string ProcessText(MonoBehaviour inMonoBehaviour, string inText)
		{
			try
			{
				if (inText == null || inText.Length == 0)
				{
					return inText;
				}
				if (inText.StartsWith("#M#"))
				{
					return inText.Substring(3);
				}
				return Text.Translate(inMonoBehaviour.gameObject.name, inText);
			}
			catch (Exception)
			{
				return inText;
			}
		}

		public static string GetTextSeeTrophyCollection()
		{
			switch (GetLanguageIndex())
			{
			case 1:
				return "Sehen Sie Ihre Sammlung im Trophaenmenu";
			case 2:
				return "Vea su coleccion en el menu de trofeos";
			case 3:
				return "Voir votre collection dans le menu trophee";
			case 4:
				return "Guarda la tua collezione nel menu trofeo";
			case 5:
				return "Veja a sua colecao no menu do trofeu";
			default:
				return "See your collection in the trophy menu";
			}
		}

		public static string GetTextCoins()
		{
			switch (GetLanguageIndex())
			{
			case 1:
				return " Munzen";
			case 2:
				return " Monedas";
			case 3:
				return " Pieces";
			case 4:
				return " Monete";
			case 5:
				return " Moedas";
			default:
				return " Coins";
			}
		}

		public static string GetTextCollectMore()
		{
			switch (GetLanguageIndex())
			{
			case 1:
				return "Sammle {0} mehr, um {1} zu entsperren";
			case 2:
				return "Recoge {0} más para desbloquear {1}";
			case 3:
				return "Collectez {0} autres pour debloquer {1}";
			case 4:
				return "Raccogli altri {0} per sbloccare {1}";
			case 5:
				return "Colete mais {0} para desbloquear {1}";
			default:
				return "Collect {0} more to unlock {1}";
			}
		}

		public static string GetTextYouUnlocked()
		{
			switch (GetLanguageIndex())
			{
			case 1:
				return "Gekauft ";
			case 2:
				return "Desbloqueaste ";
			case 3:
				return "Deverrouille ";
			case 4:
				return "Hai sbloccato ";
			case 5:
				return "Desbloqueado ";
			default:
				return "You Unlocked ";
			}
		}

		public static string GetTextNotEnough()
		{
			switch (GetLanguageIndex())
			{
			case 1:
				return "Nicht genug Münzen!";
			case 2:
				return "¡No hay suficientes monedas!";
			case 3:
				return "Pas assez de pièces!";
			case 4:
				return "Non abbastanza monete!";
			case 5:
				return "Não há moedas suficientes!";
			default:
				return "Not enough coins!";
			}
		}

		public static string GetTextYouNeedMoreCoins()
		{
			switch (GetLanguageIndex())
			{
			case 1:
				return "Sie benötigen {0} weitere Münzen, um Ihren Kauf zu vervollständigen!";
			case 2:
				return "¡Necesita {0} monedas más para completar su compra!";
			case 3:
				return "Vous avez besoin de {0} pièces supplémentaires pour compléter votre achat!";
			case 4:
				return "Hai bisogno di {0} monete per completare l'acquisto!";
			case 5:
				return "Você precisa de mais {0} moedas para completar sua compra!";
			default:
				return "You need {0} more Coins to complete your purchase!";
			}
		}

		public static string GetTextExitDialog()
		{
			switch (GetLanguageIndex())
			{
			case 1:
				return "Bist du sicher, dass du das Spiel beenden willst?";
			case 2:
				return "¿Está seguro que quiere salir del juego?";
			case 3:
				return "Êtes-vous sûr de vouloir quitter le jeu?";
			case 4:
				return "Sei sicuro di voler lasciare il gioco?";
			case 5:
				return "Tem certeza de que deseja sair do jogo?";
			default:
				return "Are you sure you want to quit the game?";
			}
		}

		public static string GetTextQuit()
		{
			switch (GetLanguageIndex())
			{
			case 1:
				return "Verlassen";
			case 2:
				return "Dejar";
			case 3:
				return "Quitter";
			case 4:
				return "Smettere";
			case 5:
				return "Sair";
			default:
				return "Quit";
			}
		}

		public static string GetTextReturn()
		{
			switch (GetLanguageIndex())
			{
			case 1:
				return "Rückkehr";
			case 2:
				return "Regreso";
			case 3:
				return "Retour";
			case 4:
				return "Ritorno";
			case 5:
				return "Retorna";
			default:
				return "Return";
			}
		}

		public static void DumpGameObject(MonoBehaviour inMonoBehaviour)
		{
			DumpGameObject(inMonoBehaviour.gameObject, null, null, "");
		}

		public static void DumpGameObject(string inLocation, MonoBehaviour inMonoBehaviour)
		{
			try
			{
				DumpGameObject(inLocation, inMonoBehaviour.gameObject);
			}
			catch (Exception)
			{
			}
		}

		public static void DumpGameObject(GameObject gameObject)
		{
			try
			{
				DumpGameObject(gameObject, null, null, "");
			}
			catch (Exception)
			{
			}
		}

		public static void DumpGameObject(string inLocation, GameObject gameObject)
		{
			try
			{
				DumpGameObject(gameObject, null, null, "");
			}
			catch (Exception)
			{
			}
		}

		private static bool CheckDumpGameObject(GameObject inChild, GameObject inParent, string inName, string inNameParent)
		{
			if (inChild.name.Equals(inName) && inParent.name.Equals(inNameParent) && inChild.active)
			{
				inChild.SetActiveRecursively(false);
				return true;
			}
			return false;
		}

		private static bool CheckDumpGameObject(GameObject inChild, GameObject inParent, GameObject inGrandParent, string inName, string inNameParent, string inNameGrandParent)
		{
			if (inChild.name.Equals(inName) && inParent.name.Equals(inNameParent) && inGrandParent.name.Equals(inNameGrandParent))
			{
				if (inChild.active)
				{
					inChild.SetActiveRecursively(false);
				}
				return true;
			}
			return false;
		}

		private static bool CheckGameObject(GameObject inChild, GameObject inParent, GameObject inGrandParent, string inName, string inNameParent, string inNameGrandParent)
		{
			if (inChild.name.Equals(inName) && inParent.name.Equals(inNameParent) && inGrandParent.name.Equals(inNameGrandParent))
			{
				return true;
			}
			return false;
		}

		private static Vector3 SetNegativeX(Vector3 inPosition)
		{
			if (inPosition.x > 0f)
			{
				return new Vector3(0f - inPosition.x, inPosition.y, inPosition.z);
			}
			return inPosition;
		}

		private static bool DumpGameObject(GameObject inChild, GameObject inParent, GameObject inGrandParent, string indent)
		{
			try
			{
				bool result = false;
				if (mFacebook != null && mFacebook.active)
				{
					float x = mFacebook.transform.position.x;
					if (x > -100f)
					{
						mFacebook.transform.position = new Vector3(x - 200f, mFacebook.transform.position.y, mFacebook.transform.position.z);
					}
				}
				if (mFriends != null && mFriends.active)
				{
					mFriends.SetActiveRecursively(false);
				}
				CheckDumpGameObject(inChild, inParent, "discount", "4CoinsButton");
				CheckDumpGameObject(inChild, inParent, "4CoinsButton", "ShopFooter");
				CheckDumpGameObject(inChild, inParent, "4CoinsButton", "ShopFooter(Clone)");
				CheckDumpGameObject(inChild, inParent, "3DoubleCoin", "GameoverUI(Clone)");
				CheckDumpGameObject(inChild, inParent, "2 Message", "4Grid");
				CheckDumpGameObject(inChild, inParent, "4 Invite Friends", "4Grid");
				if (CheckDumpGameObject(inChild, inParent, inGrandParent, "FriendsButton", "tween", "3Anchor - BottomLeft"))
				{
					result = true;
					mFriends = inGrandParent;
				}
				if (CheckDumpGameObject(inChild, inParent, "FacebookButton", "8Offline"))
				{
					result = true;
					mFacebook = inParent;
				}
				if (!mShopPositionSet && CheckGameObject(inChild, inParent, inGrandParent, "ShopButton", "tween", "4Anchor - BottomRight"))
				{
					mShopPositionSet = true;
					inChild.transform.position = SetNegativeX(inChild.transform.position);
					inParent.transform.position = SetNegativeX(inParent.transform.position);
					inGrandParent.transform.position = SetNegativeX(inGrandParent.transform.position);
				}
				foreach (Transform item in inChild.transform)
				{
					if (DumpGameObject(item.gameObject, inChild, inParent, "") && inChild.active)
					{
						inChild.SetActiveRecursively(false);
					}
				}
				return result;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public static void Debug(string inLine)
		{
		}

		public static void Debug(string inLine, int inValue)
		{
		}

		public static void Debug(string inLine, bool inValue)
		{
		}

		public static void Exception(string inLine)
		{
		}

		private static void DoJavaCall(string inMethod)
		{
			DoJavaCall(inMethod, new object[0]);
		}

		private static void DoJavaCall(string inMethod, int inValue)
		{
			DoJavaCall(inMethod, new object[1] { inValue });
		}

		private static int DoJavaCallReturnInt(string inClass, string inMethod)
		{
			AndroidJNI.AttachCurrentThread();
			AndroidJNI.PushLocalFrame(0);
			try
			{
				using (AndroidJavaObject androidJavaObject = new AndroidJavaClass(inClass))
				{
					int result = androidJavaObject.CallStatic<int>(inMethod, new object[0]);
					((IDisposable)androidJavaObject).Dispose();
					return result;
				}
			}
			catch (Exception)
			{
				Exception("EXCEPTION: DoJavaCall: " + inClass + "." + inMethod + "(...)");
			}
			finally
			{
				AndroidJNI.PopLocalFrame(IntPtr.Zero);
			}
			return 0;
		}

		private static void DoJavaCall(string inMethod, object[] inParameters)
		{
			DoJavaCall(DEBUG_CLASS, inMethod, inParameters);
		}

		private static void DoJavaCall(string inClass, string inMethod, object[] inParameters)
		{
			AndroidJNI.AttachCurrentThread();
			AndroidJNI.PushLocalFrame(0);
			try
			{
				using (AndroidJavaObject androidJavaObject = new AndroidJavaClass(inClass))
				{
					androidJavaObject.CallStatic(inMethod, inParameters);
					((IDisposable)androidJavaObject).Dispose();
				}
			}
			catch (Exception)
			{
				Exception("EXCEPTION: DoJavaCall: " + inClass + "." + inMethod + "(...)");
			}
			finally
			{
				AndroidJNI.PopLocalFrame(IntPtr.Zero);
			}
		}
	}
}
