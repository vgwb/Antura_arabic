namespace SRDebugger.Services
{
    using System.Collections.Generic;

    public interface ISystemInfo
    {
        string Title { get; }
        object Value { get; }

        /// <summary>
        /// If system info is private, do not include in analytics or bug reports.
        /// </summary>
        bool IsPrivate { get; }
    }

    public interface ISystemInformationService
    {
        /// <summary>
        /// Get an IEnumerable with the available data categories for this system
        /// </summary>
        IEnumerable<string> GetCategories();

        /// <summary>
        /// Get a list of information for a category
        /// </summary>
        /// <param name="category">Category name to fetch (get a list of these from GetCategories())</param>
        /// <returns></returns>
        IList<ISystemInfo> GetInfo(string category);

        /// <summary>
        /// Generate a report from all available system data (useful for sending with bug reports)
        /// </summary>
        /// <param name="includePrivate">Set to true to include identifying private information (usually you don't want this)</param>
        /// <returns>The generated report</returns>
        Dictionary<string, Dictionary<string, object>> CreateReport(bool includePrivate = false);
    }
}
