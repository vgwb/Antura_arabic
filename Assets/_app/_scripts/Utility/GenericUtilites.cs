using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EA4S
{
    public static class GenericUtilities
    {

        /// <summary>
        /// sort an Enum by its names.. returns List;
        /// to be used like: var orderedEnumList = Sort<Sfx>();
        /// </summary>
        public static IOrderedEnumerable<TEnum> SortEnums<TEnum>()
        {
            // alternative: Enum.GetValues(typeof(TEnum)).Cast<TEnum>().OrderBy(v => v.ToString());
            return ((TEnum[])Enum.GetValues(typeof(TEnum))).OrderBy(v => v.ToString());
        }

        public static string ReverseText(string text)
        {
            var cArray = text.ToCharArray();
            var reverse = string.Empty;
            for (var i = cArray.Length - 1; i > -1; i--) {
                reverse += cArray[i];
            }
            return reverse;
        }

        #region DateTime
        private static DateTime TIME_START = new DateTime(1970, 1, 1, 0, 0, 0);
        public static int GetRelativeTimestampFromNow(int deltaDays)
        {
            var timeSpan = new TimeSpan(deltaDays, 0, 0, 0, 0);
            return GetTimestampForNow() + (int)timeSpan.TotalSeconds;
        }
        public static int GetTimestampForNow()
        {
            var timeSpan = (DateTime.UtcNow - TIME_START);
            return (int)timeSpan.TotalSeconds;
        }
        public static DateTime FromTimestamp(int timestamp)
        {
            var span = TimeSpan.FromSeconds(timestamp);
            return TIME_START + span;
        }
        public static TimeSpan GetTimeSpanBetween(int timestamp_from, int timestamp_to)
        {
            return FromTimestamp(timestamp_to) - FromTimestamp(timestamp_from);
        }

        #endregion

        #region Math
        public static float GetAverage(List<float> floatsList)
        {
            if (floatsList.Count < 1)
                return 0f;

            var average = 0f;

            foreach (var item in floatsList) {
                average += item;
            }

            return (average / floatsList.Count);
        }
        #endregion

        #region Colors

        public static Color GetColorFromString(string color)
        {
            Color drawingColor;
            switch (color) {
                case "blue":
                    drawingColor = Color.blue;
                    break;
                case "brown":
                    drawingColor = new Color(165f / 255f, 42f / 255f, 42f / 255f);
                    break;
                case "gold":
                    drawingColor = new Color(255f / 255f, 215f / 255f, 0);
                    break;
                case "green":
                    drawingColor = Color.green;
                    break;
                case "grey":
                    drawingColor = Color.grey;
                    break;
                case "orange":
                    drawingColor = new Color(255f / 255f, 165f / 255f, 0);
                    break;
                case "pink":
                    drawingColor = new Color(255f / 255f, 192f / 255f, 128f / 203f);
                    break;
                case "purple":
                    drawingColor = new Color(128f / 255f, 0, 128f / 255f);
                    break;
                case "red":
                    drawingColor = Color.red;
                    break;
                case "silver":
                    drawingColor = new Color(128f / 255f, 128f / 255f, 128f / 255f);
                    break;
                case "white":
                    drawingColor = Color.white;
                    break;
                case "yellow":
                    drawingColor = Color.yellow;
                    break;
                default:
                    drawingColor = Color.black;
                    break;

            }
            return drawingColor;
        }

        #endregion
    }
}
