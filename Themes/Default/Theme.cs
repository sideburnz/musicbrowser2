using System;
using System.Collections.Generic;
using System.Text;
using MusicBrowser.Engines.Themes;
using System.IO;
using System.Xml;
using System.Reflection;

namespace MusicBrowser.Engines.Themes
{
    public class Theme : ITheme
    {
        public string HomePage()
        {
            return "resx://Default/Default.Resources/pageFolder";
        }
    }
}
