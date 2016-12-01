using UnityEngine;
using System.Collections;

namespace EA4S
{
    public class GraphMood : MonoBehaviour
    {
        public BookGraph Graph;

        public void OnEnable()
        {
            int nMoods = 10;
            var latestMoods = AppManager.I.Teacher.GetLastMoodData(nMoods);
            float[] moodValues = latestMoods.ConvertAll(x => x.MoodValue).ToArray();
            Graph.SetValues(nMoods, AppConstants.maximumMoodValue, moodValues);
        }
    }
}