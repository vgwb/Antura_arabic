using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using EA4S.Core;
using EA4S.Database;
using EA4S.UI;

namespace EA4S.Core
{
	public class DebugButton : MonoBehaviour, IPointerClickHandler
	{
		public TextRender Title;
		private DebugPanel manager;
		private MiniGameInfo minigameInfo;
		
		public void Init(DebugPanel _manager, MiniGameInfo _MiniGameInfo)
		{
			manager = _manager;
			minigameInfo = _MiniGameInfo;
			Title.text = _MiniGameInfo.data.Title_En;
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			manager.LaunchMinigame(minigameInfo.data.Code);
		}
	}
}