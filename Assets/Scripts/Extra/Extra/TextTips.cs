namespace Extra
{
	public class TextTips
	{
		public static string[][] mTips;

		public static void Init()
		{
			if (mTips == null)
			{
				mTips = new string[8][]
				{
					new string[6] { "Stay on top of trains to get a better view of what's coming", "Bleib oben auf Zugen um einen besseren Blick zu bekommen von dem, was kommt", "Fique no topo dos trens para obter uma melhor visao do que esta acontecendo", "Restez sur les trains pour obtenir une meilleure vue de ce qui se passe", "Rimani in cima ai treni per avere una visione migliore di quello che sta arrivando", "Fique no topo dos trens para obter uma melhor visao do que esta acontecendo" },
					new string[6] { "Swipe down in air to cancel jumps and master the Super Sneakers", "Wischen Sie in die Luft ab, um die Sprunge abzubrechen und die Super Sneakers zu beherrschen", "Deslize para baixo no ar para cancelar saltos e dominar o Super Sneakers", "Faites glisser vers le bas dans l'air pour annuler les sauts et maitriser les Super Sneakers", "Spostare verso il basso in aria per annullare i salti e master i Super Sneakers", "Deslize para baixo no ar para cancelar saltos e dominar o Super Sneakers" },
					new string[6] { "Upgrade your preferred powerups in the Shop to make them last longer", "Aktualisieren Sie Ihre Powerups im Shop, um sie langer dauern zu lassen", "Atualize seus powerups preferidos no Shop para torna-los mais longos", "Ameliorez vos powerups preferes dans le Shop pour les faire durer plus longtemps", "Aggiorna le preferenze preferite nel Negozio per farli durare piu a lungo", "Atualize seus powerups preferidos no Shop para torna-los mais longos" },
					new string[6] { "Complete missions to increase your Multiplier up to x30", "Komplette Missionen um deinen Multiplikator zu erhohen bis zu x30", "Complete missoes para aumentar o numero de multiplicador ate x30", "Remplissez les missions pour augmenter votre Multiplicateur jusqu'a x30", "Completa missioni per aumentare il tuo moltiplicatore fino a x30", "Complete missoes para aumentar o numero de multiplicador ate x30" },
					new string[6] { "Switch lanes in mid-air to dodge trains at the last minute", "Wechseln Sie die Bahnen in der Luft-Luft, um Zuge auszugleichen in der letzten Minute", "Mude as pistas no meio do ar para esquivar os trens no ultimo minuto", "Deplacez les allers-retards dans le milieu de l'air pour esquiver les trains a la derniere minute", "Passare le corsie a meta aria per schivare i treni all'ultimo minuto", "Mude as pistas no meio do ar para esquivar os trens no ultimo minuto" },
					new string[6] { "Double tap the screen to grab a Hoverboard", "Doppelklicken Sie auf den Bildschirm, um einen Hoverboard zu holen", "Toque duas vezes na tela para pegar um Hoverboard", "Appuyez deux fois sur l'ecran pour prendre un Hoverboard", "Toccare due volte sullo schermo per catturare un Hoverboard", "Toque duas vezes na tela para pegar um Hoverboard" },
					new string[6] { "Skip Missions in the Shop", "Uberspringen Sie Missionen im Shop", "Skip Missions in the Shop", "Passer les missions dans la boutique", "Salta missioni nel negozio", "Skip Missions in the Shop" },
					new string[6] { "Use the bouncy Super Sneakers to jump over trains", "Verwenden Sie die bouncy Super Sneakers, um uber Zuge zu springen", "Use o nulo Super Sneakers para saltar sobre trens", "Utilisez les Sneakers Sondes gonflables pour sauter au-dessus des trains", "Usa il Sneakers Super di saltare per saltare i treni", "Use o nulo Super Sneakers para saltar sobre trens" }
				};
			}
		}
	}
}
