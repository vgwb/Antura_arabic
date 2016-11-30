using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EA4S
{
    public class GraphJourney : MonoBehaviour
    {
        public BookGraph Graph;



        void Start()
        {

        }


        public void Show(List<Db.PlaySessionInfo> psinfo, List<Db.PlaySessionInfo> unlockedPlaySessionInfos)
        {
            // Show journey
            float[] journeyValues = unlockedPlaySessionInfos.ConvertAll(x => x.score).ToArray();
            //string[] journeyLabels = allPsInfo.ConvertAll(x => x.data.Id).ToArray();
            Graph.SetValues(unlockedPlaySessionInfos.Count, 1f, journeyValues);
        }
    }
}