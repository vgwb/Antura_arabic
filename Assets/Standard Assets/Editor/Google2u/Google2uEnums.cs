// ----------------------------------------------
//     G2U: Google Spreadsheet Unity integration
//          Copyright © 2015 Litteratus
// ----------------------------------------------

namespace Google2u
{
    public enum QueryStatus
    {
        Idle,
        Uninitialized,
        Querying,
        QueryComplete
    }

    public enum GFCommand
    {
        DoLogin,
        DoLogout,
        WaitForLogin,
        RetrieveWorkbooks,
        WaitForRetrievingWorkbooks,
        RetrieveManualWorkbooks,
        WaitForRetrievingManualWorkbooks,
        ManualWorkbooksRetrievalComplete,
        DoUpload,
        WaitingForUpload,
        UploadComplete,
        AssetDatabaseRefresh
    }

    public enum ExportType
    {
        ObjectDatabase,
        StaticDatabase,
        CSV,
        XML,
        JSON,
        NGUI,
        DoNotExport
    }

    public enum SupportedType
    {
        Void,
        String,
        Int,
        Float,
        Bool,
        Byte,
        Vector2,
        Vector3,
        Color,
        Color32,
        Quaternion,
        FloatArray,
        IntArray,
        BoolArray,
        ByteArray,
        StringArray,
        Vector2Array,
        Vector3Array,
        ColorArray,
        Color32Array,
        QuaternionArray,
        GameObject,
        Unrecognized
    }

    public enum GFGUIMessageType
    {
        // Errors
        InvalidLogin,
        InvalidEditorDirectory,
        InvalidUploadDirectory
        // Warnings

        // Info
    }

    public enum CellType
    {
        ColumnHeader,
        RowHeader,
        Type,
        Value,
        Null
    }
}