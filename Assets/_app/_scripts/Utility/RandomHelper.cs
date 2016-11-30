using System;
using System.Linq;
using System.Collections.Generic;

namespace EA4S
{
    public static class RandomHelper
    {

        private static readonly Random _random = new Random(DateTime.Now.Millisecond);
        public static T GetRandom<T>(this IList<T> list)
        {
            if (list.Count == 0) {
                throw new System.Exception("Cannot get a random element from the list as count is zero.");
            }
            return list[_random.Next(0, list.Count)];
        }

        public static T GetRandomParams<T>(params T[] ids)
        {
            return ids[_random.Next(0, ids.Length)];
        }

        public static T GetRandomEnum<T>()
        {
            var A = Enum.GetValues(typeof(T));
            var V = (T)A.GetValue(UnityEngine.Random.Range(0, A.Length));
            return V;
        }

        /// <summary>
        /// Extension to Shuffle a list and return a new one
        /// </summary>
        public static List<T> Shuffle<T>(this List<T> fromList)
        {
            return new List<T>(fromList.OrderBy(a => UnityEngine.Random.value));
        }

        public static List<T> RouletteSelectNonRepeating<T>(List<T> fromList, int numberToSelect)
        {
            if (numberToSelect > fromList.Count) {
                throw new System.Exception("Cannot select more than available with a non-repeating selection");
            }

            var chosenList = new List<T>();

            if (numberToSelect == fromList.Count) {
                chosenList.AddRange(fromList);
                return chosenList;
            }

            for (var choice_index = 0; choice_index < numberToSelect; choice_index++) {
                var element_index = UnityEngine.Random.Range(0, fromList.Count);
                var chosenItem = fromList[element_index];
                fromList.RemoveAt(element_index);
                chosenList.Add(chosenItem);
            }

            return chosenList;
        }

        public static List<T> RouletteSelectNonRepeating<T>(List<T> fromList, List<float> weightsList, int numberToSelect)
        {
            if (numberToSelect > fromList.Count) {
                throw new System.Exception("Cannot select more than available with a non-repeating selection");
            }

            var chosenList = new List<T>();

            if (numberToSelect == fromList.Count) {
                chosenList.AddRange(fromList);
                chosenList = chosenList.Shuffle();
                return chosenList;
            }

            for (var choice_index = 0; choice_index < numberToSelect; choice_index++) {
                var totalWeight = weightsList.Sum();
                var choiceValue = UnityEngine.Random.value * totalWeight;
                float cumulativeWeight = 0;
                for (var element_index = 0; element_index < fromList.Count; element_index++) {
                    cumulativeWeight += weightsList[element_index];
                    if (choiceValue <= cumulativeWeight) {
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

        public static List<T> RandomSelect<T>(this List<T> all_list, int maxNumberToSelect, bool forceMaxNumber = false)
        {
            if (maxNumberToSelect == 0) {
                return new List<T>();
            }

            if (all_list.Count == 0) {
                throw new System.Exception("The list has zero elements to select from.");
            }

            if (!forceMaxNumber && all_list.Count < maxNumberToSelect) {
                maxNumberToSelect = all_list.Count;

            }

            return RouletteSelectNonRepeating<T>(all_list, maxNumberToSelect);
        }

        public static T RandomSelectOne<T>(this List<T> all_list)
        {
            if (all_list.Count == 0) {
                throw new System.Exception("The list has zero elements to select from.");
            }

            return RouletteSelectNonRepeating<T>(all_list, 1)[0];
        }
    }
}