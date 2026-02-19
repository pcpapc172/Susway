namespace Extra
{
	public struct Translation
	{
		public string mGerman;

		public string mFrench;

		public string mItalian;

		public string mSpanish;

		public string mPortuguese;

		public Translation(string inGerman, string inSpanish, string inFrench, string inItalian, string inPortuguese)
		{
			mGerman = inGerman;
			mSpanish = inSpanish;
			mFrench = inFrench;
			mItalian = inItalian;
			mPortuguese = inPortuguese;
		}

		public string GetTranslation()
		{
			return GetTranslation(Wrapper.GetLanguageIndex());
		}

		public string GetTranslation(int inLanguage)
		{
			switch (inLanguage)
			{
			case 1:
				return mGerman;
			case 2:
				return mSpanish;
			case 3:
				return mFrench;
			case 4:
				return mItalian;
			case 5:
				return mPortuguese;
			default:
				return "";
			}
		}
	}
}
