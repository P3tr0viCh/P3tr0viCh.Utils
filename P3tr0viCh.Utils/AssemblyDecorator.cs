using System;
using System.Diagnostics;
using System.Reflection;

namespace P3tr0viCh.Utils
{
    public class AssemblyDecorator
    {
        private Assembly assembly;

        public AssemblyDecorator()
        {
            Assembly = Assembly.LoadFrom(Process.GetCurrentProcess().MainModule.FileName);
        }

        public AssemblyDecorator(Assembly assembly)
        {
            Assembly = assembly;
        }

        public Assembly Assembly
        {
            get => assembly;
            set
            {
                assembly = value;

                Version = assembly.GetName().Version;

                var assemblyFileVersion = (AssemblyFileVersionAttribute)assembly.GetCustomAttribute(typeof(AssemblyFileVersionAttribute));
                FileVersion = new Version(assemblyFileVersion.Version);

                var assemblyInformationalVersion = (AssemblyInformationalVersionAttribute)assembly.GetCustomAttribute(typeof(AssemblyInformationalVersionAttribute));
                InformationalVersion = assemblyInformationalVersion.InformationalVersion;

                BuildDate = new DateTime(2000, 1, 1).AddDays(Version.Build).AddSeconds(Version.MinorRevision * 2);

                var assemblyConfiguration = (AssemblyConfigurationAttribute)assembly.GetCustomAttribute(typeof(AssemblyConfigurationAttribute));
                IsDebug = "Debug".Equals(assemblyConfiguration.Configuration);

                var assemblyTitle = (AssemblyTitleAttribute)assembly.GetCustomAttribute(typeof(AssemblyTitleAttribute));
                Title = assemblyTitle.Title;

                var assemblyProduct = (AssemblyProductAttribute)assembly.GetCustomAttribute(typeof(AssemblyProductAttribute));
                Product = assemblyProduct.Product;

                var assemblyCopyright = (AssemblyCopyrightAttribute)assembly.GetCustomAttribute(typeof(AssemblyCopyrightAttribute));
                Copyright = assemblyCopyright.Copyright;
            }
        }

        public bool IsDebug { get; private set; }

        public Version Version { get; private set; }
        
        public Version FileVersion { get; private set; }

        public string InformationalVersion { get; private set; }

        public DateTime BuildDate { get; private set; }

        public string Title { get; private set; }

        public string Product { get; private set; }

        public string Copyright { get; private set; }

        [Obsolete]
        public string VersionString(bool full = true, bool withDebug = true)
        {
            var versionString = Version.ToString(full ? 4 : 2);

            if (withDebug && IsDebug)
            {
                versionString += " (debug build)";
            }

            return versionString;
        }

        public static string TitleVersionString() => new AssemblyDecorator().InformationalVersion;
    }
}