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
    static class StandingData
    {
        public static IEnumerable<string> GetStandingData(string category)
        {
            string fileName = Helper.AppFolder + "\\standingdata.xml";
            XmlDocument xmlDoc = new XmlDocument();
            string xpathString = string.Format("/standingdata/section[@name='{0}']/item", category.ToLower());
            List<string> rval = new List<string>();

            // if the file doesn't exist, just continue
            if (!System.IO.File.Exists(fileName))
            {
                Exception mbEx = new Exception("MusicBrowser.XML.StandingData - file not found: " + fileName);
                Logging.LoggerFactory.Error(mbEx);
                throw (mbEx);
            }
            try
            {
                // load the file
                xmlDoc.Load(fileName);
            }
            catch (Exception ex)
            {
                Logging.LoggerFactory.Error(ex);
                return null;
            }

            //  get the data
            foreach (XmlNode node in xmlDoc.SelectNodes(xpathString))
            {
                rval.Add(node.InnerText);
            }

            //Logging.Log("Loaded and read data on cat '" + Category + "' from standing data file");
            return rval;
        }

    }
}
