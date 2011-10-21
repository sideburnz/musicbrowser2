using MusicBrowser.Engines.Logging;
using MusicBrowser.Util;

namespace MusicBrowser.Models
{
    public class ConfigModel : BaseModel
    {
        private Config _config = Config.GetInstance();

        private const string CACHE_ENGINE = "CacheEngine";

        public bool UseCache
        {
            get 
            { 
                return (_config.GetStringSetting(CACHE_ENGINE).ToLower() == "filesystem"); 
            }
            set 
            {
                Logger.Debug(CACHE_ENGINE + " " + value);
                if (value)
                {
                    _config.SetSetting(CACHE_ENGINE, "FileSystem");
                }
                else
                {
                    _config.SetSetting(CACHE_ENGINE, "None");
                }
            }
        }

        public void SetUseCache(bool value) 
        {
            Logger.Debug("HIT " + value);
        
            UseCache = value; 
        }


    }
}
