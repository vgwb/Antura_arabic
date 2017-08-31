using System.Collections.Generic;
using UnityEngine;

namespace Antura.AnturaSpace
{
    public class ShopState
    {
        public List<string> unlockedDecorationsIDs = new List<string>();

        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }

        public static ShopState CreateFromJson(string jsonData)
        {
            var shopState = JsonUtility.FromJson<ShopState>(jsonData);
            if (shopState == null) shopState = new ShopState();
            return shopState;
        }

    }
}