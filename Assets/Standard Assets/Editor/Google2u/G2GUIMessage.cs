// ----------------------------------------------
//     G2U: Google Spreadsheet Unity integration
//          Copyright © 2015 Litteratus
// ----------------------------------------------

namespace Google2u
{
    #region Using Directives

    using System;

    #endregion

    [Serializable]
    public class G2GUIMessage
    {
        public string Message;
        public GFGUIMessageType MessageType;

        public G2GUIMessage(GFGUIMessageType in_type, string in_message)
        {
            MessageType = in_type;
            Message = in_message;
        }
    }
}