using System.Collections.Generic;
using System.Xml;
using System;

//*****************************************************************************
//
//  Basic standing data reader to remove hard coded lists in the application.
//  This data is read-only during runtime.
//
//*****************************************************************************

namespace MusicBrowser.Util
{
    class StandingData
    {
        public static IEnumerable<string> GetStandingData(string Category)
        {
            Logging.Logger.Debug("Attempting to loaded and read data on cat '" + Category + "' from standing data file");

            string fileName = Helper.AppFolder + "\\standingdata.xml";
            XmlDocument XMLDoc = new XmlDocument();
            string XpathString = string.Format("/standingdata/section[@name='{0}']/item", Category.ToLower());
            List<string> rval = new List<string>();

            // if the file doesn't exist, just continue
            if (!System.IO.File.Exists(fileName))
            {
                Exception MBex = new Exception("MusicBrowser.XML.StandingData - file not found: " + fileName);
                Logging.Logger.Error(MBex);
                throw (MBex);
            }
            try
            {
                // load the file
                XMLDoc.Load(fileName);
            }
            catch (Exception Ex)
            {
                Logging.Logger.Error(Ex);
                return null;
            }

            Logging.Logger.Debug("file opened, using XPath: " + XpathString);

            //  get the data
            foreach (XmlNode node in XMLDoc.SelectNodes(XpathString))
            {
                rval.Add(node.InnerText);
            }

            //Logging.Log("Loaded and read data on cat '" + Category + "' from standing data file");
            return rval;
        }

    }
}
