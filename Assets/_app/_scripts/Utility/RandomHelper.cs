using System.Collections.Generic;
using System.Linq;

namespace EA4S
{
    public static class RandomHelper
    {
        public static List<T> RouletteSelectNonRepeating<T>(List<T> fromList, int numberToSelect)
        {
            if (numberToSelect > fromList.Count)
            {
                throw new System.Exception("Cannot select more than available with a non-repeating selection");
            }

            var chosenList = new List<T>();

            if (numberToSelect == fromList.Count)
            {
                chosenList.AddRange(fromList);
                return chosenList;
            }

            for (var choice_index = 0; choice_index < numberToSelect; choice_index++)
            {
                var element_index = UnityEngine.Random.Range(0, fromList.Count);
                var chosenItem = fromList[element_index];
                fromList.RemoveAt(element_index);
                chosenList.Add(chosenItem);
            }

            return chosenList;
        }

        public static List<T> RouletteSelectNonRepeating<T>(List<T> fromList, List<float> weightsList, int numberToSelect)
        {
            if (numberToSelect > fromList.Count)
            {
                throw new System.Exception("Cannot select more than available with a non-repeating selection");
            }

            var chosenList = new List<T>();

            if (numberToSelect == fromList.Count) { 
                chosenList.AddRange(fromList);
                return chosenList;
            }

            for (var choice_index=0; choice_index < numberToSelect; choice_index++)
            {
                var totalWeight = weightsList.Sum();
                var choiceValue = UnityEngine.Random.value * totalWeight;
                float cumulativeWeight = 0;
                for (var element_index = 0; element_index < fromList.Count; element_index++)
                {
                    cumulativeWeight += weightsList[element_index];
                    if (choiceValue <= cumulativeWeight)
                    {
                        var chosenItem = fromList[element_index];
                        fromList.RemoveAt(element_index);
                        weightsList.RemoveAt(element_index);
                        chosenList.Add(chosenItem);
                        break;
                    }
                }
            }

            return chosenList;
        }

        public static List<T> RandomSelect<T>(this List<T>  all_list, int numberToSelect)
        {
            return RouletteSelectNonRepeating<T>(all_list, numberToSelect);
        }

        public static T RandomSelectOne<T>(this List<T> all_list)
        {
            return RouletteSelectNonRepeating<T>(all_list, 1)[0];
        }
    }
}