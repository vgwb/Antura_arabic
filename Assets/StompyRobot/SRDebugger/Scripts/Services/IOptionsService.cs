namespace SRDebugger.Services
{
    using System.Collections.Generic;
    using Internal;

    public interface IOptionsService
    {
        ICollection<OptionDefinition> Options { get; }

        /// <summary>
        /// Scan <paramref name="obj" /> for options add them to the Options collection
        /// </summary>
        /// <param name="obj">Object to scan for options</param>
        void Scan(object obj);
    }
}
