using System;
using System.Reflection;

namespace P3tr0viCh.Utils
{
    public static class Misc
    {
        public static Assembly MainModuleAssembly()
        {
            return Assembly.LoadFrom(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
        }

        public static Version AssemblyVersion(Assembly assembly = null)
        {
            if (assembly == null)
            {
                assembly = MainModuleAssembly();
            }

            return assembly.GetName().Version;
        }

        public static bool AssemblyVersionIsDebug(Assembly assembly = null)
        {
            if (assembly == null)
            {
                assembly = MainModuleAssembly();
            }

            var assemblyConfiguration = (AssemblyConfigurationAttribute)assembly.GetCustomAttribute(typeof(AssemblyConfigurationAttribute));

            return "Debug".Equals(assemblyConfiguration.Configuration);
        }

        public static string AssemblyVersionToString(bool full = true, Assembly assembly = null)
        {
            if (assembly == null)
            {
                assembly = MainModuleAssembly();
            }

            var versionString = AssemblyVersion(assembly).ToString(full ? 4 : 2);

            if (AssemblyVersionIsDebug(assembly))
            {
                versionString += " (debug build)";
            }

            return versionString;
        }
    }
}