namespace Extra
{
	public class TextStart
	{
		public static string[][] mStart;

		public static void Init()
		{
			if (mStart == null)
			{
				mStart = new string[7][]
				{
					new string[7] { "3ProgressText", "   DONE", " GESCHEHEN", " Hecho", " FAIT", " FATTO", " FEITO" },
					new string[7] { "1mainSetLabel", "MISSION SET Number ", "MISSION SET Nummer ", "MISION SET Numero ", "MISSION SET Numero ", "MISSIONE SET ", "MISSAO SET Numero " },
					new string[7] { "2Title", "SKIP MISSION ", "MISSION ", "OMITIR MISION ", "SAUTER MISSION ", "MISSIONE ", "MISSAO SKIP " },
					new string[7] { "3Desc", "Skip your current mission ", "Uberspringen Sie Ihre aktuelle Mission ", "Omitir la mision actual ", "Passer votre mission actuelle ", "Salta la tua missione attuale ", "Evite sua missao atual " },
					new string[7] { "4Timer", "Time: ", "Zeit: ", "Tiempo: ", "Heure: ", "Ora: ", "Hora: " },
					new string[7] { "HaveAmount", "You have: ", "Sie haben: ", "Usted tiene: ", "Vous avez: ", "Hai: ", "Voce tem: " },
					new string[7] { "bigLabel", " Coins", " Munzen", " monedas", " Pieces", " Monete", " Moedas" }
				};
			}
		}
	}
}
