using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EA4S
{
    public class GraphJourney : MonoBehaviour
    {
        public BookGraph Graph;

        public void Show(List<Db.PlaySessionInfo> allPsInfo, List<Db.PlaySessionInfo> unlockedPlaySessionInfos)
        {
            float[] journeyValues = allPsInfo.ConvertAll(x => x.score).ToArray();
            //string[] journeyLabels = allPsInfo.ConvertAll(x => x.data.Id).ToArray();
            Graph.SetValues(unlockedPlaySessionInfos.Count, 1f, journeyValues);
        }
    }
}