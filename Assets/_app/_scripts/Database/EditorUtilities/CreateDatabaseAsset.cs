#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace EA4S.Db
{
    public class CreateDatabaseAsset
    {
        [MenuItem("Assets/Create/Database")]
        public static void CreateAsset()
        {
            CustomAssetUtility.CreateAsset<Database>();
        }
    }
}
#endif