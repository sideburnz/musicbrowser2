using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

//from http://buyan.wordpress.com/2010/01/17/making-external-assemblies-well-known-in-mcml/

namespace MusicBrowser
{
    public static class AssemblyResolver
    {
        public static void MakeCustomAssembliesWellknown()
        {
            AttachEventHandler();
        }

        private static Assembly ResolveAssembly(object sender, ResolveEventArgs args)
        {
            Assembly assembly = null;
            string nameToResolve = GetAssemblyNameOnly(args.Name);

            try
            {
                assembly = GetAssemblyFromGac(nameToResolve);
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed resolving assembly: " + nameToResolve);
            }

            return assembly;
        }

        private static void AttachEventHandler()
        {
            if (!_handlerAttached)
            {
                AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;

                _handlerAttached = true;
            }
        }
        private static string GetAssemblyNameOnly(string name)
        {
            string[] details = name.Split(',');
            return details.Length > 0 ? details[0] : name;
        }

        private static Assembly GetAssemblyFromGac(string name)
        {
            try
            {
                string gacRoot = Environment.ExpandEnvironmentVariables(@"%WINDIR%\assembly");
                string[] gacDirectories = Directory.GetDirectories(gacRoot);

                // GAC, GAC_32. GAC_MSIL, etc.
                foreach (string directory in gacDirectories)
                {
                    string[] assemblyDirectories = Directory.GetDirectories(directory, name);

                    // i.e. System.Xml
                    foreach (string assemblyDirectory in assemblyDirectories)
                    {
                        string[] versionKeyDirectories = Directory.GetDirectories(assemblyDirectory);

                        //i.e 1.0.0.0_ef2c1abcc5f37ec4
                        foreach (string sub in versionKeyDirectories)
                        {
                            string[] files = Directory.GetFiles(sub, name + ".dll");

                            foreach (string file in files)
                            {
                                if (file.Contains(name))
                                    return Assembly.LoadFile(file);
                            }
                        }
                    }
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        #region Fields

        private static bool _handlerAttached;
 
        #endregion
    }
}
