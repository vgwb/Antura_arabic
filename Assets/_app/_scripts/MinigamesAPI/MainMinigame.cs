using System.Collections;
using System.Collections.Generic;

using EA4S.Database;

namespace EA4S.Core
{
	public class MainMiniGame
	{
		public string id;
		public List<MiniGameInfo> variations;

		public string GetIconResourcePath()
		{
			return variations[0].data.GetIconResourcePath();
		}

		public MiniGameCode GetFirstVariationMiniGameCode()
		{
			return variations[0].data.Code;
		}
	}
}