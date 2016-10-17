using UnityEngine;
using System.Collections;

/**
 * here we can put all static general config, to be used with EA4S.Config.Current.name_of_your_var;
 * 
 **/

namespace EA4S
{
    public class Config
    {
        public static readonly Config Current = new Config();


#if UNITY_EDITOR

        public readonly string Url_Github_Repository = "https://github.com/vgwb/EA4S_Antura_U3D";
        public readonly string Url_Trello = "https://trello.com/b/ltLndaQI/ea4s-beta";

#endif
    }
}
