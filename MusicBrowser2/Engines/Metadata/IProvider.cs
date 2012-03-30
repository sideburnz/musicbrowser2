using System;
using MusicBrowser.Entities;

namespace MusicBrowser.Engines.Metadata
{
    public enum ProviderOutcome
    {
        Success,        // everything was as expected, data is available
        SystemError,    // something went wrong
        NoData,         // the query worked but there's no data or no new data
        InvalidInput    
    }

    public interface IProvider
    {
        ProviderOutcome Fetch(baseEntity dto);
        string FriendlyName();
        bool CompatibleWith(baseEntity dto);
        bool isStale(DateTime lastAccess);
    }
}
