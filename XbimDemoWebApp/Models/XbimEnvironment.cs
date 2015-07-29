using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XbimDemoWebApp.Models
{
    public class XbimEnvironment
    {
        public XbimEnvironment()
        {
            Assemblies = new List<XbimAsmVersion>();
        }
        public string Environment { get; set; }
        public List<XbimAsmVersion> Assemblies { get; set; }
    }

    public class XbimAsmVersion
    {
        public string Assembly { get; set; }
        public string Architecture { get; set; }
        public string AssemblyVersion { get; set; }
        public string ProductVersion { get; set; }
        public string FileVersion { get; set; }
        public string CodeBase { get; set; }
    }
}