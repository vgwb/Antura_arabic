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
using UnityEngine.UI;
using System.Collections;
using ModularFramework.Core;
using UniRx;

namespace ModularFramework.Components {

    [RequireComponent(typeof(Text))]
    public class TextLocalized : MonoBehaviour {

        public string LocalizedText_ID;
        Text LocalizedText;

        void OnEnable() {
            LocalizedText = GetComponent<Text>();
            /// <summary>
            /// Monitoring property value changes.
            /// </summary>
            GameManager.Instance.Localization.ObserveEveryValueChanged(x => x.ActualLanguage).Subscribe(_ => {
                UpdateLable(GameManager.Instance.Localization.GetLocalizedString(LocalizedText_ID));
            }).AddTo(this);
        }

        /// <summary>
        /// Called when property value changed.
        /// </summary>
        void UpdateLable(string valueToUpdate) {
            // Bind Logic here
            LocalizedText.text = valueToUpdate;
        }
    }
}
