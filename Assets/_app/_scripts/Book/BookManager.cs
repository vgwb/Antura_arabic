using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Antura.Book
{
    public class BookManager : MonoBehaviour
    {
        public static BookManager I;

        const string RESOURCES_BOOK = "Prefabs/Book/Book";

        void Awake()
        {
            I = this;
        }

        public void OpenBook(BookArea area)
        {
            // TODO first check if Book is already isntatiated!
            GameObject instance = Instantiate(Resources.Load(RESOURCES_BOOK, typeof(GameObject))) as GameObject;

            Book.I.OpenArea(area);
        }
    }
}