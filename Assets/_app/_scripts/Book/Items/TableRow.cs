using UnityEngine;
using System.Collections;

namespace EA4S
{
    public class TableRow : MonoBehaviour
    {
        public TextRender TxTitle;
        public TextRender TxSubtitle;
        public TextRender TxValue;

        public void Init(string _title, string _value, string _subtitle = "")
        {
            TxTitle.setText(_title);
            TxSubtitle.setText(_subtitle);
            TxValue.setText(_value);
        }
    }
}