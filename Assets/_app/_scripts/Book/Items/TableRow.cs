using EA4S.UI;
using UnityEngine;

namespace EA4S.Book
{
    public class TableRow : MonoBehaviour
    {
        public TextRender TxTitle;
        public TextRender TxTitleEn;
        public TextRender TxValue;
        public CompletionSlider slider;

        public void Init(string _titleEn, string _title, string _value)
        {
            TxTitle.text = _title;
            TxTitleEn.text = _titleEn;
            TxValue.SetText(_value);
        }

        public void InitSlider(string _titleEn, string _title, float _value, float _valueMax)
        {
            TxTitle.text = _title;
            TxTitleEn.text = _titleEn;
            slider.SetValue(_value, _valueMax);
            //TxValue.SetText(_value);
        }
    }
}