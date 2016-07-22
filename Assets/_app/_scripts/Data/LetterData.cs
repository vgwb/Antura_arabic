using Google2u;

namespace EA4S {

    public class LetterData {
        public string Key;
        public string Isolated_Unicode;
        public string Isolated;
        public string Initial_Unicode;
        public string Medial_Unicode;
        public string Final_Unicode;

        public LetterData(string _keyRow, lettersRow _letRow) {
            Key = _keyRow;
            Isolated = _letRow._isolated;
            Isolated_Unicode = _letRow._unicode;
            Initial_Unicode = _letRow._initial_unicode;
            Medial_Unicode = _letRow._medial_unicode;
            Final_Unicode = _letRow._final_unicode;
        }
    }
}