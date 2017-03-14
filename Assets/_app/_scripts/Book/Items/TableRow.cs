using EA4S.UI;
using UnityEngine;

namespace EA4S.Book
{
    public class TableRow : MonoBehaviour
    {
        public TextRender TxTitle;
        public TextRender TxTitleEn;
        public TextRender TxValue;

        public void Init(string _titleEn, string _title, string _value)
        {
            TxTitle.SetText(_title);
            TxTitleEn.SetText(_titleEn);
            TxValue.SetText(_value);
        }
    }
}