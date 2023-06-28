using System;
using System.Reflection;

namespace P3tr0viCh.Utils
{
    public static class Misc
    {
        public class AssemblyDecorator
        {
            public AssemblyDecorator()
            {
                Assembly = Assembly.LoadFrom(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
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

                    version = assembly.GetName().Version;

                    var assemblyConfiguration = (AssemblyConfigurationAttribute)assembly.GetCustomAttribute(typeof(AssemblyConfigurationAttribute));
                    isDebug = "Debug".Equals(assemblyConfiguration.Configuration);

                    var assemblyProduct = (AssemblyProductAttribute)assembly.GetCustomAttribute(typeof(AssemblyProductAttribute));
                    product = assemblyProduct.Product;

                    var assemblyCopyright = (AssemblyCopyrightAttribute)assembly.GetCustomAttribute(typeof(AssemblyCopyrightAttribute));
                    copyright = assemblyCopyright.Copyright;
                }
            }

            private bool isDebug;
            public bool IsDebug
            {
                get { return isDebug; }
            }

            private Version version;
            public Version Version
            {
                get
                {
                    return version;
                }
            }

            private string product;
            public string Product
            {
                get
                {
                    return product;
                }
            }

            private string copyright;
            public string Copyright
            {
                get
                {
                    return copyright;
                }
            }

            public string VersionString(bool full = true, bool withDebug = true)
            {
                var versionString = version.ToString(full ? 4 : 2);

                if (withDebug && isDebug)
                {
                    versionString += " (debug build)";
                }

                return versionString;
            }
        }
    }
}