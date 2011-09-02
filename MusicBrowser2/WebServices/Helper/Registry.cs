﻿using Microsoft.Win32;

namespace MusicBrowser.WebServices.Helper
{
    static class Registry
    {
        private const string subKey = "SOFTWARE\\JOOCER\\WebServices\\";

        public static string Read(string path, string key)
        {
            RegistryKey registry = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(subKey + path);
            if (registry == null)
            {
                return null;
            }
            try
            {
                return (string)registry.GetValue(key.ToUpper());
            }
            catch
            {
                return null;
            }
            finally
            {
                registry.Close();
            }
        }
    }
}