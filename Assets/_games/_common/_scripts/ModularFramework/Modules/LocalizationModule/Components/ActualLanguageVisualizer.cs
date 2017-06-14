
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace EA4S.Core
{

    [RequireComponent(typeof(Text))]
    public class ActualLanguageVisualizer : MonoBehaviour {

        Text LanguageLable;

        void OnEnable() {
            LanguageLable = GetComponent<Text>();
            // Remove UniRx refactoring request: now UpdateLable must be called manually.
        }

        /// <summary>
        /// Called when property value changed.
        /// </summary>
        void UpdateLable(string valueToUpdate) {
            // Bind Logic here
            LanguageLable.text = valueToUpdate;
        }
    }
}
