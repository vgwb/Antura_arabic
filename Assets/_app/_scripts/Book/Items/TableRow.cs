using EA4S.UI;
using UnityEngine;

namespace EA4S.PlayerBook
{
    public class TableRow : MonoBehaviour
    {
        public TextRender TxTitle;
        public TextRender TxSubtitle;
        public TextRender TxValue;

        public void Init(string _title, string _value, string _subtitle = "")
        {
            TxTitle.SetText(_title);
            TxSubtitle.SetText(_subtitle);
            TxValue.SetText(_value);
        }
    }
}