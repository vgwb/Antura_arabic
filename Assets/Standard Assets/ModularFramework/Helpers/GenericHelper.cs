/* --------------------------------------------------------------
*   Indie Contruction : Modular Framework for Unity
*   Copyright(c) 2016 Indie Construction / Paolo Bragonzi
*   All rights reserved. 
*   For any information refer to http://www.indieconstruction.com
*   
*   This library is free software; you can redistribute it and/or
*   modify it under the terms of the GNU Lesser General Public
*   License as published by the Free Software Foundation; either
*   version 3.0 of the License, or(at your option) any later version.
*   
*   This library is distributed in the hope that it will be useful,
*   but WITHOUT ANY WARRANTY; without even the implied warranty of
*   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
*   Lesser General Public License for more details.
*   
*   You should have received a copy of the GNU Lesser General Public
*   License along with this library.
* -------------------------------------------------------------- */
using UnityEngine;
using System.Collections.Generic;

namespace ModularFramework.Helpers {

    public static class GenericHelper {

        /// <summary>
        /// Return random float value around _value parameter + or - _variation.
        /// </summary>
        /// <param name="_value"></param>
        /// <param name="_variation"></param>
        /// <returns></returns>
        public static float GetValueWithRandomVariation(float _value, float _variation) {
            return Random.Range(_value - _variation, _value + _variation);
        }

        #region extensions

        /// <summary>
        /// Return random element of list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_thisList"></param>
        /// <returns></returns>
        public static T GetRandomElement<T>(this List<T> _thisList) {
            return _thisList[Random.Range(0, _thisList.Count)];
        }

        private static System.Random rng = new System.Random();

        public static void Shuffle<T>(this IList<T> list) {
            int n = list.Count;
            while (n > 1) {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        #endregion

    }
}