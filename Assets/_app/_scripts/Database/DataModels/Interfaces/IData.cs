namespace EA4S.Db
{
    /// <summary>
    /// Interface for a generic data element that can be handled by the application.
    /// </summary>
    public interface IData {

        // @note: all data needs a string ID, which will be used for direct access in dictionaries
        string GetId();

    }
}