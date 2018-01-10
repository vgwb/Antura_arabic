using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Antura.Book
{
    public class BookManager : MonoBehaviour
    {
        public static BookManager I;

        void Awake()
        {
            I = this;
        }

        public void OpenBook(BookArea area)
        {
            GameObject instance = Instantiate(Resources.Load("Prefabs/Book/Book", typeof(GameObject))) as GameObject;

            Book.I.OpenArea(area);
        }
    }
}