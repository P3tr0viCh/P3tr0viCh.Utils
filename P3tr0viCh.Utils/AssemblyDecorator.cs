using System;
using System.Diagnostics;
using System.Reflection;

namespace P3tr0viCh.Utils
{
    public class AssemblyDecorator
    {
        public AssemblyDecorator()
        {
            Assembly = Assembly.LoadFrom(Process.GetCurrentProcess().MainModule.FileName);
        }

        public AssemblyDecorator(Assembly assembly)
        {
            Assembly = assembly;
        }

        private Assembly assembly;

        public Assembly Assembly
        {
            get
            {
                return assembly;
            }
            set
            {
                assembly = value;

                Version = assembly.GetName().Version;

                var assemblyConfiguration = (AssemblyConfigurationAttribute)assembly.GetCustomAttribute(typeof(AssemblyConfigurationAttribute));
                IsDebug = "Debug".Equals(assemblyConfiguration.Configuration);

                var assemblyProduct = (AssemblyProductAttribute)assembly.GetCustomAttribute(typeof(AssemblyProductAttribute));
                Product = assemblyProduct.Product;

                var assemblyCopyright = (AssemblyCopyrightAttribute)assembly.GetCustomAttribute(typeof(AssemblyCopyrightAttribute));
                Copyright = assemblyCopyright.Copyright;
            }
        }

        public bool IsDebug { get; private set; }

        public Version Version { get; private set; }

        public string Product { get; private set; }

        public string Copyright { get; private set; }

        public string VersionString(bool full = true, bool withDebug = true)
        {
            var versionString = Version.ToString(full ? 4 : 2);

            if (withDebug && IsDebug)
            {
                versionString += " (debug build)";
            }

            return versionString;
        }
    }
}