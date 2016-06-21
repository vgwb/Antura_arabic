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
using System.Collections.Generic;
using ModularFramework.Core;

namespace ModularFramework.Components {
    /// <summary>
    /// Component to set active language.
    /// </summary>
    [RequireComponent(typeof(Dropdown))]
    public class AvailableLanguagesList : MonoBehaviour {
        /// <summary>
        /// If true, hide selector if there is only one active languages.
        /// </summary>
        public bool HideIfOnlyOneActLang = true;
        Dropdown ddl;

        void OnEnable() {
            ddl = GetComponent<Dropdown>();
            // if unnecessarily disable component
            if (HideIfOnlyOneActLang && GameManager.Instance.Localization.GetAllAvailableLanguages().Length <= 1) {
                this.gameObject.SetActive(false);
                return;
            }
            // OnValueChange reaction
            ddl.onValueChanged.AddListener(delegate {
                GameManager.Instance.Localization.SetActualLanguage(ddl.options[ddl.value].text);
            });

        }

        void OnDisable() {
            ddl.onValueChanged.RemoveAllListeners();
        }

        void Start() {
            ddl.ClearOptions();
            List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
            foreach (string lang in GameManager.Instance.Localization.GetAllAvailableLanguages()) {
                options.Add(new Dropdown.OptionData() { text = lang });
            }
            ddl.AddOptions(options);
        }
    }
}