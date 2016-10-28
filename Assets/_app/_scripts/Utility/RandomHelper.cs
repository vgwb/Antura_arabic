using System.Collections.Generic;
using System.Linq;
using System;

namespace EA4S
{
    public static class RandomHelper
    {
        private static readonly Random _random = new Random(DateTime.Now.Millisecond);
        public static T GetRandom<T>(this IList<T> list)
        {
            if (list.Count == 0)
            {
                throw new System.Exception("Cannot get a random element from the list as count is zero.");
            }
            return list[_random.Next(0, list.Count)];
        }

        public static T GetRandomEnum<T>()
        {
            var A = Enum.GetValues(typeof(T));
            var V = (T)A.GetValue(UnityEngine.Random.Range(0, A.Length));
            return V;
        }


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
                        //UnityEngine.Debug.Log("CHOSEN: " + chosenItem.ToString());
                        break;
                    }
                }
            }

            return chosenList;
        }

    }
}