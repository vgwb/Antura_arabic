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
using System;
using System.Collections.Generic;

namespace ModularFramework.Helpers {

    public static class GenericHelper {

        #region extensions

        /// <summary>
        /// Return random element of list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_thisList"></param>
        /// <returns></returns>
        public static T GetRandomElement<T>(this List<T> _thisList) {
            return _thisList[new Random().Next(_thisList.Count)];
        }

        #endregion

    }
}