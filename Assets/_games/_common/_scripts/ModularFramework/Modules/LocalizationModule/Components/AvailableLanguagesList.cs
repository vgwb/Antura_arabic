
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
namespace EA4S.Core
{

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
            if (HideIfOnlyOneActLang && EA4S.AppManager.Instance.Localization.GetAllAvailableLanguages().Length <= 1) {
                this.gameObject.SetActive(false);
                return;
            }
            // OnValueChange reaction
            ddl.onValueChanged.AddListener(delegate {
                EA4S.AppManager.Instance.Localization.SetActualLanguage(ddl.options[ddl.value].text);
            });

        }

        void OnDisable() {
            ddl.onValueChanged.RemoveAllListeners();
        }

        void Start() {
            ddl.ClearOptions();
            List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
            foreach (string lang in EA4S.AppManager.Instance.Localization.GetAllAvailableLanguages()) {
                options.Add(new Dropdown.OptionData() { text = lang });
            }
            ddl.AddOptions(options);
        }
    }
}