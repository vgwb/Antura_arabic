using System.Collections.Generic;
using System.Linq;

namespace EA4S
{
    public static class RandomHelper
    {

        public static List<T> RouletteSelectNonRepeating<T>(List<T> fromList, List<float> weightsList, int numberToSelect)
        {
            if (numberToSelect > fromList.Count)
            {
                throw new System.Exception("Cannot select more than available with a non-repeating selection");
            }

            List<T> chosenList = new List<T>();

            if (numberToSelect == fromList.Count) { 
                chosenList.AddRange(fromList);
                return chosenList;
            }

            for (int choice_index=0; choice_index < numberToSelect; choice_index++)
            {
                float totalWeight = weightsList.Sum();
                float choiceValue = UnityEngine.Random.value * totalWeight;
                float cumulativeWeight = 0;
                for (int element_index = 0; element_index < fromList.Count; element_index++)
                {
                    cumulativeWeight += weightsList[element_index];
                    if (choiceValue <= cumulativeWeight)
                    {
                        T chosenItem = fromList[element_index];
                        fromList.RemoveAt(element_index);
                        weightsList.RemoveAt(element_index);
                        chosenList.Add(chosenItem);
                        break;
                    }
                }
            }

            return chosenList;
        }

    }
}