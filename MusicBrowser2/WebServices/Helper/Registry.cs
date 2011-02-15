using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace MusicBrowser.WebServices.Helper
{
    static class Registry
    {
        private static string subKey = "SOFTWARE\\JOOCER\\WebServices\\";

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

        public static void Write(string path, string key, string value)
        {
            RegistryKey registry = Microsoft.Win32.Registry.LocalMachine.CreateSubKey(subKey + path);
            if (registry != null)
            {
                registry.SetValue(key, value);
                registry.Close();
            }
        }
    }
}