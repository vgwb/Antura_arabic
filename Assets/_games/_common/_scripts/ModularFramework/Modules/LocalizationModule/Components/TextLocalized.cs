
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace EA4S.Core
{


    [RequireComponent(typeof(Text))]
    public class TextLocalized : MonoBehaviour {

        public string LocalizedText_ID;
        Text LocalizedText;

        void OnEnable() {
            LocalizedText = GetComponent<Text>();
            // Remove UniRx refactoring request: now UpdateLable must be called manually.
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
