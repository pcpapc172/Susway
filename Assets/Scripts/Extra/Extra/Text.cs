using System;

namespace Extra
{
	public class Text
	{
		public static string Translate(string inLabel, string inText)
		{
			try
			{
				int languageIndex = Wrapper.GetLanguageIndex();
				if (languageIndex > 0)
				{
					TextGame.Init();
					int result;
					if (inText.Length <= 10 && int.TryParse(inText, out result))
					{
						return inText;
					}
					string text = inText.Replace('\n', '#').Replace('\r', '*');
					if (text[text.Length - 1] == '\u0003')
					{
						text = text.Substring(0, text.Length - 1);
					}
					Translation value;
					string outText;
					if (TextGame.mTranslation.TryGetValue(new TextKey(inLabel, text), out value))
					{
						inText = value.GetTranslation(languageIndex);
						inText = inText.Replace('#', '\n');
					}
					else if (TranslateStart(inLabel, text, languageIndex, out outText))
					{
						inText = outText;
					}
				}
			}
			catch (Exception)
			{
			}
			return inText;
		}

		public static string TranslateMission(string inLabel, string inText)
		{
			try
			{
				if (inText != null && inText.Length > 0)
				{
					TextMissions.Init();
					Translation value;
					if (TextMissions.mMissions.TryGetValue(new TextKey(inLabel, inText), out value))
					{
						inText = value.GetTranslation();
					}
				}
			}
			catch (Exception)
			{
			}
			return inText;
		}

		public static bool TranslateStart(string inLabel, string inText, int inLanguage, out string outText)
		{
			try
			{
				TextStart.Init();
				bool result = false;
				outText = null;
				for (int i = 0; i < TextStart.mStart.GetLength(0); i++)
				{
					if (inLabel.Equals(TextStart.mStart[i][0]))
					{
						outText = inText.Replace(TextStart.mStart[i][1], TextStart.mStart[i][inLanguage + 1]);
						result = true;
					}
				}
				return result;
			}
			catch (Exception)
			{
				outText = null;
				return false;
			}
		}

		public static string[] SetTips(string[] inTips)
		{
			try
			{
				TextTips.Init();
				string[] array = new string[TextTips.mTips.GetLength(0)];
				for (int i = 0; i < TextTips.mTips.GetLength(0); i++)
				{
					array[i] = TextTips.mTips[i][Wrapper.GetLanguageIndex()];
				}
				return array;
			}
			catch (Exception)
			{
				return inTips;
			}
		}
	}
}
