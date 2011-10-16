﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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


    }
}