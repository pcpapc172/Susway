using System.Collections;

namespace Prime31
{
	public static class MiniJsonExtensions
	{
		public static ArrayList arrayListFromJson(this string json)
		{
			return MiniJSON.jsonDecode(json) as ArrayList;
		}
	}
}
