//----------------------------------------------
//    Google2u: Google Doc Unity integration
//         Copyright © 2015 Litteratus
//
//        This file has been auto-generated
//              Do not manually edit
//----------------------------------------------

using UnityEngine;
using System.Globalization;

namespace Google2u
{
    [System.Serializable]
    public class wordsRow : IGoogle2uRow
    {
        public string _id;
        public string _kind;
        public string _category;
        public string _stage;
        public string _english;
        public string _word;
        public string _letters;
        public string _transliteration;
        public string _difficulty_level;
        public string _number_of_letter;
        public string _group;

        public wordsRow(string __id, string __kind, string __category, string __stage, string __english, string __word, string __letters, string __transliteration, string __difficulty_level, string __number_of_letter, string __group) {
            _id = __id;
            _kind = __kind.Trim();
            _category = __category.Trim();
            _stage = __stage.Trim();
            _english = __english.Trim();
            _word = __word.Trim();
            _letters = __letters.Trim();
            _transliteration = __transliteration.Trim();
            _difficulty_level = __difficulty_level.Trim();
            _number_of_letter = __number_of_letter.Trim();
            _group = __group.Trim();
        }

        public int Length { get { return 10; } }

        public string this [int i]
        {
            get {
                return GetStringDataByIndex(i);
            }
        }

        public string GetStringDataByIndex(int index) {
            string ret = System.String.Empty;
            switch (index) {
                case 0:
                    ret = _kind.ToString();
                    break;
                case 1:
                    ret = _category.ToString();
                    break;
                case 2:
                    ret = _stage.ToString();
                    break;
                case 3:
                    ret = _english.ToString();
                    break;
                case 4:
                    ret = _word.ToString();
                    break;
                case 5:
                    ret = _letters.ToString();
                    break;
                case 6:
                    ret = _transliteration.ToString();
                    break;
                case 7:
                    ret = _difficulty_level.ToString();
                    break;
                case 8:
                    ret = _number_of_letter.ToString();
                    break;
                case 9:
                    ret = _group.ToString();
                    break;
            }

            return ret;
        }

        public string GetStringData(string colID) {
            var ret = System.String.Empty;
            switch (colID) {
                case "kind":
                    ret = _kind.ToString();
                    break;
                case "category":
                    ret = _category.ToString();
                    break;
                case "stage":
                    ret = _stage.ToString();
                    break;
                case "english":
                    ret = _english.ToString();
                    break;
                case "word":
                    ret = _word.ToString();
                    break;
                case "letters":
                    ret = _letters.ToString();
                    break;
                case "transliteration":
                    ret = _transliteration.ToString();
                    break;
                case "difficulty_level":
                    ret = _difficulty_level.ToString();
                    break;
                case "number of letter":
                    ret = _number_of_letter.ToString();
                    break;
                case "group":
                    ret = _group.ToString();
                    break;
            }

            return ret;
        }

        public override string ToString() {
            string ret = System.String.Empty;
            ret += "{" + "kind" + " : " + _kind.ToString() + "} ";
            ret += "{" + "category" + " : " + _category.ToString() + "} ";
            ret += "{" + "stage" + " : " + _stage.ToString() + "} ";
            ret += "{" + "english" + " : " + _english.ToString() + "} ";
            ret += "{" + "word" + " : " + _word.ToString() + "} ";
            ret += "{" + "letters" + " : " + _letters.ToString() + "} ";
            ret += "{" + "transliteration" + " : " + _transliteration.ToString() + "} ";
            ret += "{" + "difficulty_level" + " : " + _difficulty_level.ToString() + "} ";
            ret += "{" + "number of letter" + " : " + _number_of_letter.ToString() + "} ";
            ret += "{" + "group" + " : " + _group.ToString() + "} ";
            return ret;
        }
    }

    public sealed class words : IGoogle2uDB
    {
        public enum rowIds {
            father,
            mother,
            brother,
            sister,
            uncle_father,
            aunt_father,
            uncle_mother,
            aunt_mother,
            grandmother,
            grandfather,
            mouth,
            tooth,
            eye,
            nose,
            ear,
            head,
            hand,
            foot,
            belly,
            delicious,
            water,
            bread,
            cheese,
            yogurt,
            sugar,
            tea,
            rice,
            banana,
            berry,
            apricot,
            flower,
            tree,
            rain,
            snow,
            cold,
            hot,
            dog,
            cat,
            mouse,
            bear,
            girl,
            boy,
            kid,
            man,
            woman,
            grandson,
            granddaughter,
            day,
            month,
            year,
            hour,
            second,
            hair,
            face,
            tongue,
            chest,
            back,
            hot2,
            cold2,
            oil,
            vegetables,
            food,
            onion,
            garlic,
            cucumber,
            potato,
            tomato,
            cherry,
            grape,
            fig,
            plum,
            apple,
            pencil,
            paper,
            copybook,
            book,
            tiger,
            eagle,
            whale,
            lion}

        ;

        public string[] rowNames =
        {
            "father", "mother", "brother", "sister", "uncle_father", "aunt_father", "uncle_mother", "aunt_mother", "grandmother", "grandfather", "mouth", "tooth", "eye", "nose", "ear", "head", "hand", "foot", "belly"
			, "delicious", "water", "bread", "cheese", "yogurt", "sugar", "tea", "rice", "banana", "berry", "apricot", "flower", "tree", "rain", "snow", "cold", "hot", "dog", "cat", "mouse"
			, "bear", "girl", "boy", "kid", "man", "woman", "grandson", "granddaughter", "day", "month", "year", "hour", "second", "hair", "face", "tongue", "chest", "back", "hot2", "cold2"
			, "oil", "vegetables", "food", "onion", "garlic", "cucumber", "potato", "tomato", "cherry", "grape", "fig", "plum", "apple", "pencil", "paper", "copybook", "book", "tiger", "eagle", "whale"
			, "lion"
        };
        public System.Collections.Generic.List<wordsRow> Rows = new System.Collections.Generic.List<wordsRow>();

        public static words Instance
        {
            get { return Nestedwords.instance; }
        }

        private class Nestedwords
        {
            static Nestedwords() {
            }

            internal static readonly words instance = new words();
        }

        private words() {
            Rows.Add(new wordsRow("father", "noun", "family_members", "1", "Father", "بابا", "بـ ا بـ ا", "baba", "1", "4", ""));
            Rows.Add(new wordsRow("mother", "noun", "family_members", "1", "Mother", "ماما", "مـ ا مـ ا", "mama", "1", "4", ""));
            Rows.Add(new wordsRow("brother", "noun", "family_members", "1", "Brother", "أخ", "أ خ", "akh", "1", "2", ""));
            Rows.Add(new wordsRow("sister", "noun", "family_members", "1", "Sister", "أخت", "أ خـ ـت", "ukht", "1", "3", ""));
            Rows.Add(new wordsRow("uncle_father", "noun", "family_members", "1", "Uncle (from father’s side)", "عم", "عـ ـم", "am", "1", "2", "A"));
            Rows.Add(new wordsRow("aunt_father", "noun", "family_members", "1", "Aunt (from father’s side)", "عمة", "عـ ـمـ ـة", "amma", "1", "3", "A"));
            Rows.Add(new wordsRow("uncle_mother", "noun", "family_members", "1", "Uncle (from mother’s side)", "خال", "خـ ا ل", "khal", "1", "3", "B"));
            Rows.Add(new wordsRow("aunt_mother", "noun", "family_members", "1", "Aunt (from mother’s side)", "خالة", "خـ ا لـ ـة", "khala", "2", "4", "B"));
            Rows.Add(new wordsRow("grandmother", "noun", "family_members", "1", "Grandmother", "نانا", "نـ ا نـ ا", "nana", "1", "4", ""));
            Rows.Add(new wordsRow("grandfather", "noun", "family_members", "1", "Grandfather", "جدو", "جـ ـد و", "jeddo", "1", "3", ""));
            Rows.Add(new wordsRow("mouth", "noun", "body_parts", "1", "Mouth", "فم", "فـ ـم", "fam", "1", "2", ""));
            Rows.Add(new wordsRow("tooth", "noun", "body_parts", "1", "Tooth", "سن", "سـ ـن", "sin", "1", "2", ""));
            Rows.Add(new wordsRow("eye", "noun", "body_parts", "1", "Eye", "عين", "عـ ـيـ ـن", "ain", "1", "3", ""));
            Rows.Add(new wordsRow("nose", "noun", "body_parts", "1", "Nose", "أنف", "أ نـ ـف", "anf", "1", "3", ""));
            Rows.Add(new wordsRow("ear", "noun", "body_parts", "1", "Ear", "ادن", "ا د ن", "Eden", "1", "3", ""));
            Rows.Add(new wordsRow("head", "noun", "body_parts", "1", "Head", "راس", "ر ا س", "ras", "1", "3", ""));
            Rows.Add(new wordsRow("hand", "noun", "body_parts", "1", "Hand", "يد", "يـ ـد", "yad", "1", "2", ""));
            Rows.Add(new wordsRow("foot", "noun", "body_parts", "1", "Foot", "رجل", "ر جـ ـل", "rijl", "1", "3", ""));
            Rows.Add(new wordsRow("belly", "noun", "body_parts", "1", "Belly", "بطن", "بـ ـطـ ـن", "batn", "1", "3", ""));
            Rows.Add(new wordsRow("delicious", "noun", "food", "1", "Delicious", "حلو", "حـ ـلـ ـو", "helu", "1", "3", ""));
            Rows.Add(new wordsRow("water", "noun", "food", "1", "Water", "مي", "مـ ـي", "mai", "1", "2", ""));
            Rows.Add(new wordsRow("bread", "noun", "food", "1", "Bread", "خبز", "خـ ـبـ ـز", "khubz", "1", "3", ""));
            Rows.Add(new wordsRow("cheese", "noun", "food", "1", "Cheese", "جبن", "جـ ـبـ ـن", "jubn", "1", "3", ""));
            Rows.Add(new wordsRow("yogurt", "noun", "food", "1", "Yogurt", "لبن", "لـ ـبـ ـن", "laban", "1", "3", ""));
            Rows.Add(new wordsRow("sugar", "noun", "food", "1", "Sugar", "سكر", "سـ ـكـ ـر", "sukkar", "1", "3", ""));
            Rows.Add(new wordsRow("tea", "noun", "food", "1", "Tea", "شاي", "شـ ـا ي", "shai", "1", "3", ""));
            Rows.Add(new wordsRow("rice", "noun", "food", "1", "Rice", "رز", "ر ز", "ruz", "1", "2", ""));
            Rows.Add(new wordsRow("banana", "noun", "fruits", "1", "Banana", "موز", "مـ ـو ز", "mawz", "1", "3", ""));
            Rows.Add(new wordsRow("berry", "noun", "fruits", "1", "Berry", "توت", "تـ ـو ت", "tut", "1", "3", ""));
            Rows.Add(new wordsRow("apricot", "noun", "fruits", "1", "Apricot", "مشمش", "مـ ـشـ ـمـ ـش", "mishmosh", "2", "4", ""));
            Rows.Add(new wordsRow("flower", "noun", "nature", "1", "Flower", "ورد", "و ر د", "ward", "1", "3", ""));
            Rows.Add(new wordsRow("tree", "noun", "nature", "1", "Tree", "شجر", "شـ ـجـ ـر", "shajar", "1", "3", ""));
            Rows.Add(new wordsRow("rain", "noun", "nature", "1", "Rain", "مطر", "مـ ـطـ ـر", "matar", "1", "3", ""));
            Rows.Add(new wordsRow("snow", "noun", "nature", "1", "Snow", "تلج", "تـ ـلـ ـج", "talj", "1", "3", ""));
            Rows.Add(new wordsRow("cold", "adjective", "nature", "1", "Cold", "برد", "بـ ـر د", "bard", "1", "3", ""));
            Rows.Add(new wordsRow("hot", "adjective", "nature", "1", "Hot", "شوب", "شـ ـو ب", "shob", "1", "3", ""));
            Rows.Add(new wordsRow("dog", "noun", "animals", "1", "Dog", "كلب", "كـ ـلـ ـب", "kalb", "1", "3", ""));
            Rows.Add(new wordsRow("cat", "noun", "animals", "1", "Cat", "هر", "هـ ـر", "hir", "1", "2", ""));
            Rows.Add(new wordsRow("mouse", "noun", "animals", "1", "Mouse", "فار", "فـ ـا ر", "far", "1", "3", ""));
            Rows.Add(new wordsRow("bear", "noun", "animals", "1", "Bear", "دب", "د ب", "dub", "1", "2", ""));
            Rows.Add(new wordsRow("girl", "noun", "family_members", "2", "Girl", "بنت", "بـ ـنـ ـت", "bint", "", "3", ""));
            Rows.Add(new wordsRow("boy", "noun", "family_members", "2", "Boy", "صبي", "صـ ـبـ ـي", "Sabi", "", "3", ""));
            Rows.Add(new wordsRow("kid", "noun", "family_members", "2", "Kid", "ولد", "و لـ ـد", "walad", "", "3", ""));
            Rows.Add(new wordsRow("man", "noun", "family_members", "2", "Man", "رجل", "ر جـ ـل", "rajol", "", "3", ""));
            Rows.Add(new wordsRow("woman", "noun", "family_members", "2", "Woman", "مرأة", "مـ ـر أ ة", "mara", "", "4", ""));
            Rows.Add(new wordsRow("grandson", "noun", "family_members", "2", "Grandson", "حفيد", "حـ ـفـ ـيـ ـد", "hafid", "", "4", ""));
            Rows.Add(new wordsRow("granddaughter", "noun", "family_members", "2", "Granddaughter", "حفيدة", "حـ ـفـ ـيـ ـد ة", "hafida", "", "5", ""));
            Rows.Add(new wordsRow("day", "noun", "time", "2", "Day", "يوم", "يـ ـو م", "yawm", "", "3", ""));
            Rows.Add(new wordsRow("month", "noun", "time", "2", "Month", "شهر", "شـ ـهـ ـر", "shahr", "", "3", ""));
            Rows.Add(new wordsRow("year", "noun", "time", "2", "Year", "عام", "عـ ـا م", "a’am", "", "3", ""));
            Rows.Add(new wordsRow("hour", "noun", "time", "2", "Hour", "ساعة", "سـ ـا عـ ـة", "sa’a", "", "4", ""));
            Rows.Add(new wordsRow("second", "noun", "time", "2", "Second", "ثانية", "ثـ ـا نـ ـيـ ـة", "thaniya", "", "5", ""));
            Rows.Add(new wordsRow("hair", "noun", "body_parts", "2", "Hair", "شعر", "شـ ـعـ ـر", "sha’ar", "", "3", ""));
            Rows.Add(new wordsRow("face", "noun", "body_parts", "2", "Face", "وجه", "و جـ ـه", "wajeh", "", "3", ""));
            Rows.Add(new wordsRow("tongue", "noun", "body_parts", "2", "Tongue", "لسان", "لـ ـسـ ـا ن", "lisan", "", "4", ""));
            Rows.Add(new wordsRow("chest", "noun", "body_parts", "2", "Chest", "صدر", "صـ ـد ر", "Sadr", "", "3", ""));
            Rows.Add(new wordsRow("back", "noun", "body_parts", "2", "Back", "ظهر", "ظـ ـهـ ـر", "DHahr", "", "3", ""));
            Rows.Add(new wordsRow("hot2", "noun", "food", "2", "Hot (warm, not spicy)", "سخن", "سـ ـخـ ـن", "sekhen", "", "3", ""));
            Rows.Add(new wordsRow("cold2", "noun", "food", "2", "Cold", "بارد", "بـ ـا ر د", "bared", "", "4", ""));
            Rows.Add(new wordsRow("oil", "noun", "food", "2", "Oil", "زيت", "ز يـ ـت", "zayt", "", "3", ""));
            Rows.Add(new wordsRow("vegetables", "noun", "food", "2", "Vegetables", "خضر", "خـ ـضـ ـر", "khuDar", "", "3", ""));
            Rows.Add(new wordsRow("food", "noun", "food", "2", "Food", "أكل", "أ كـ ـل", "akl", "", "3", ""));
            Rows.Add(new wordsRow("onion", "noun", "vegetables", "2", "Onion", "بصل", "بـ ـصـ ـل", "baSal", "", "3", ""));
            Rows.Add(new wordsRow("garlic", "noun", "vegetables", "2", "Garlic", "ثوم", "ثـ ـو م", "thuum", "", "3", ""));
            Rows.Add(new wordsRow("cucumber", "noun", "vegetables", "2", "Cucumber", "خيار", "خـ ـيـ ـا ر", "khiyar", "", "4", ""));
            Rows.Add(new wordsRow("potato", "noun", "vegetables", "2", "Potato", "بطاطا", "بـ ـطـ ـا طـ ـا", "baTaTa", "", "5", ""));
            Rows.Add(new wordsRow("tomato", "noun", "vegetables", "2", "Tomato", "بندورة", "بـ ـنـ ـد و ر ة", "banadora", "", "5", ""));
            Rows.Add(new wordsRow("cherry", "noun", "fruits", "2", "Cherry", "كرز", "كـ ـر ز", "karaz", "", "3", ""));
            Rows.Add(new wordsRow("grape", "noun", "fruits", "2", "Grape", "عنب", "عـ ـنـ ـب", "e’nab", "", "3", ""));
            Rows.Add(new wordsRow("fig", "noun", "fruits", "2", "Fig", "تين", "تـ ـيـ ـن", "tiin", "", "3", ""));
            Rows.Add(new wordsRow("plum", "noun", "fruits", "2", "Plum", "خوخ", "خـ ـو خ", "khawkh", "", "3", ""));
            Rows.Add(new wordsRow("apple", "noun", "fruits", "2", "Apple", "تفاح", "تـ ـفـ ـا ح", "tuffah", "", "4", ""));
            Rows.Add(new wordsRow("pencil", "noun", "things", "2", "Pencil", "قلم", "قـ ـلـ ـم", "qalam", "", "3", ""));
            Rows.Add(new wordsRow("paper", "noun", "things", "2", "Paper", "ورق", "و ر ق", "waraq", "", "3", ""));
            Rows.Add(new wordsRow("copybook", "noun", "things", "2", "Copybook", "دفتر", "د فـ ـتـ ـر", "daftar", "", "4", ""));
            Rows.Add(new wordsRow("book", "noun", "things", "2", "Book", "كتاب", "كـ ـتـ ـا ب", "kitaa", "", "4", ""));
            Rows.Add(new wordsRow("tiger", "noun", "animals", "2", "Tiger", "نمر", "نـ ـمـ ـر", "nemer", "", "3", ""));
            Rows.Add(new wordsRow("eagle", "noun", "animals", "2", "Eagle", "نسر", "نـ ـسـ ـر", "neser", "", "3", ""));
            Rows.Add(new wordsRow("whale", "noun", "animals", "2", "Whale", "حوت", "حـ ـو ت", "huut", "", "3", ""));
            Rows.Add(new wordsRow("lion", "noun", "animals", "2", "Lion", "أسد", "أ سـ ـد", "asad", "", "3", ""));
        }

        public IGoogle2uRow GetGenRow(string in_RowString) {
            IGoogle2uRow ret = null;
            try {
                ret = Rows[(int)System.Enum.Parse(typeof(rowIds), in_RowString)];
            }
            catch (System.ArgumentException) {
                Debug.LogError(in_RowString + " is not a member of the rowIds enumeration.");
            }
            return ret;
        }

        public IGoogle2uRow GetGenRow(rowIds in_RowID) {
            IGoogle2uRow ret = null;
            try {
                ret = Rows[(int)in_RowID];
            }
            catch (System.Collections.Generic.KeyNotFoundException ex) {
                Debug.LogError(in_RowID + " not found: " + ex.Message);
            }
            return ret;
        }

        public wordsRow GetRow(rowIds in_RowID) {
            wordsRow ret = null;
            try {
                ret = Rows[(int)in_RowID];
            }
            catch (System.Collections.Generic.KeyNotFoundException ex) {
                Debug.LogError(in_RowID + " not found: " + ex.Message);
            }
            return ret;
        }

        public wordsRow GetRow(string in_RowString) {
            wordsRow ret = null;
            try {
                ret = Rows[(int)System.Enum.Parse(typeof(rowIds), in_RowString)];
            }
            catch (System.ArgumentException) {
                Debug.LogError(in_RowString + " is not a member of the rowIds enumeration.");
            }
            return ret;
        }

    }

}
