using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

namespace XbimDemoWebApp.Models
{
    public static class XbimDiagnostics
    {
        public static XbimEnvironment GetData()
        {
            var xbimData = new XbimEnvironment();
            xbimData.Environment = string.Format("Running under [{0}] : 64bit OS: {2}\nProcess type:{1}",
                Environment.OSVersion.VersionString,
                Environment.Is64BitProcess ? "64-bit" : "32-bit",
                Environment.Is64BitOperatingSystem);

            // Force Geometry engine to load if not already. Interop will load correct Managed C++ DLL
            var engine = new Xbim.Geometry.Engine.Interop.XbimGeometryEngine();

            var ignoreList = new[] { "mscor", "system", "microsoft.", "vshost", "app_web" };
            var xBimAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(asm => !ignoreList.Any(ignore => asm.GetName().Name.ToLowerInvariant().Contains(ignore)))
                .Where(asm => asm.IsDynamic==false)
                .OrderBy(asm => asm.GetName().Name);

            
            foreach (var assembly in xBimAssemblies)
            {
                GetData(xbimData, assembly);
            }
            return xbimData;

        }

        private static void GetData(XbimEnvironment env, Assembly assembly)
        {
            var assemblyName = assembly.GetName();
            var fileVersion = GetFileVersion(assembly);
            env.Assemblies.Add(new XbimAsmVersion
                {
                    Assembly= assemblyName.Name,
                    Architecture = assemblyName.ProcessorArchitecture.ToString(),
                    AssemblyVersion = assemblyName.Version.ToString(),
                    FileVersion = fileVersion.FileVersion,
                    ProductVersion = fileVersion.ProductVersion,
                    CodeBase = assembly.CodeBase
                });
            
        }

        private static FileVersionInfo GetFileVersion(Assembly assembly)
        {
            return FileVersionInfo.GetVersionInfo(assembly.Location);

        }

    }
}