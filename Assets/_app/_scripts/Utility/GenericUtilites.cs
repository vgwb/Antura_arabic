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

        #region Layers

        public static int LayerMaskToIndex(LayerMask _mask)
        {
            int layerIndex = 0;
            int layer = _mask.value;
            while(layer > 1) {
                layer = layer >> 1;
                layerIndex++;
            }
            return layerIndex;
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

        // Taken from here: http://answers.unity3d.com/questions/812240/convert-hex-int-to-colorcolor32.html
        public static Color HexToColor(string _hex)
        {
            _hex = _hex.Replace ("0x", "");
            _hex = _hex.Replace ("#", "");
            byte a = 255;
            byte r = byte.Parse(_hex.Substring(0,2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(_hex.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(_hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
            // Only use alpha if the string has enough characters
            if(_hex.Length == 8) a = byte.Parse(_hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
            return new Color32(r,g,b,a);
        }

        // Taken from here: http://wiki.unity3d.com/index.php?title=HexConverter
        public static string ColorToHex(Color32 _color, bool _addHashPrefix = false)
        {
	        string hex = _color.r.ToString("X2") + _color.g.ToString("X2") + _color.b.ToString("X2");
	        return _addHashPrefix ? "#" + hex : hex;
        }

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
