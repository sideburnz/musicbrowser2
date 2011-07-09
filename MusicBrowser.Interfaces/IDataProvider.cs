using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/******************************************************************************
 * 
 *  Contains all of the interfaces, enums and structs for DataProoviders
 * 
 * ***************************************************************************/


namespace MusicBrowser.Interfaces
{

    public enum  DataProviderOutcome
    {
        Success,        // everything was as expected, data is available
        SystemError,    // something went wrong, more info should be in the Errors collection
        NoData,         // the query worked but there's no data or no new data
        NotFound,       // the query worked but there item couldn't be found (e.g. an artist was looked for that doesn't exist)
        InvalidInput
    }

    /// <summary>
    /// DTO for passing data to and from data providers
    /// </summary>
    public struct DataProviderDTO
    {
        // out
        public DataProviderOutcome Outcome;
        public IList<string> Errors;

        // in and out
        public IDictionary<string, string> Parameters;
    }

    /// <summary>
    /// Provides a common interface for data providers to provide.
    /// </summary>
    public interface IDataProvider
    {
        /// <summary>
        /// Creates a blank DTO with fields expected for this provider
        /// </summary>
        /// <returns>Proforma DTO</returns>
        DataProviderDTO GetDTO();

        /// <summary>
        /// Populates the DTO with data
        /// </summary>
        /// <param name="input">DTO</param>
        /// <returns>Populated DTO</returns>
        DataProviderDTO Fetch(DataProviderDTO input);
    }
}
