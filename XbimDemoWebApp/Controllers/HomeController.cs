using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Xbim.IO;
using Xbim.ModelGeometry.Scene;
using XbimDemoWebApp.Models;
using XbimGeometry.Interfaces;

namespace XbimDemoWebApp.Controllers
{
    public class HomeController : Controller
    {
        private const string DataHome = "~/App_Data/";

        public ActionResult Index()
        {
            var diagnostics = XbimDiagnostics.GetData();
            return View(diagnostics);
        }

        public ActionResult ViewModel(string name)
        {
            if (!IsValidFile(name, ".ifc"))
            {
                return View("Error");
            }
            var model = BuildModel(name);

            return View(model);
        }

        public ActionResult ViewFederation(IEnumerable<string> names)
        {
            var federation = new FederatedBimModel();

            foreach (var modelName in names)
            {
                if (!IsValidFile(modelName, ".ifc"))
                {
                    return View("Error");
                }
                federation.Models.Add(BuildModel(modelName));
            }

            return View(federation);
        }

        public ActionResult GeometryFile(string wexBim)
        {
            // Quick and dirty. We're acting as a proxy - you'd not normally get a file straight from disk without sanitising the input like this. 
            // Ideally you'd use a sythentic key and make authR checks
            if (IsValidFile(wexBim, ".wexbim"))
            {
                var fileName = Path.Combine("~/App_Data/", wexBim);
                return File(fileName, "application/octet-stream", wexBim);
            }

            return View("Error");

        }

        private string DataDir
        {
            get
            {
                return Server.MapPath(DataHome);
            }
        }

        private BimModel BuildModel(string modelName, bool refresh=false)
        {

            var ifcModel = Path.Combine(DataDir, modelName);

            string geometryFileName = Path.ChangeExtension(ifcModel, ".wexbim");
            string xbimFileName = Path.ChangeExtension(ifcModel, ".xbim");

            if(refresh)
            {
                Clean(xbimFileName, geometryFileName);
            }

            using (var model = new XbimModel())
            {
                if (!Cached(xbimFileName))
                {
                    //Parse IFC to xbim
                    model.CreateFrom(ifcModel, xbimFileName, null, true);
                }
                // Generate Geometry
                BuildGeometry(model, geometryFileName);
            }

            return new BimModel(modelName, Path.GetFileName(geometryFileName));

        }

        // In the data folder = Cached
        private bool Cached(string filename)
        {
            return System.IO.File.Exists(filename);
        }

        // Sanitise the input paths. Not production code
        private bool IsValidFile(string file, string expectedExtension)
        {
            if (file.Contains(Path.DirectorySeparatorChar) || file.Contains(Path.VolumeSeparatorChar))
            {
                return false;
            }
            if (Path.GetExtension(file).ToLowerInvariant() != expectedExtension)
            {
                return false;
            }
            var filePath = Path.Combine(DataDir, file);

            // final sanity check - we should only be in data directory
            if (!filePath.StartsWith(DataDir))
            {
                return false;
            }
            return System.IO.File.Exists(filePath);
        }


        // NOTE: I don't recommend you do this under a web server due to the use of unmanaged code and the fact the operation is very expensive in terms of resources
        private void BuildGeometry(XbimModel model, string geometryFileName)
        {
            if (Cached(geometryFileName))
            {
                return;
            }
            var m3DModelContext = new Xbim3DModelContext(model);
            m3DModelContext.CreateContext(XbimGeometryType.PolyhedronBinary);

            ExportGeometryData(geometryFileName, m3DModelContext);
        }

        private static void ExportGeometryData(string geometryFileName, Xbim3DModelContext m3DModelContext)
        {
            using (var geometryStream = new FileStream(geometryFileName, FileMode.Create))
            {
                using (var bw = new BinaryWriter(geometryStream))
                {
                    m3DModelContext.Write(bw);

                    bw.Close();
                    geometryStream.Close();
                }
            }
        }

        // Tidies after prior runs - so we run every time
        private void Clean(string xbimFileName, string geometryFileName)
        {
            TryDelete(xbimFileName);
            TryDelete(geometryFileName);
        }

        private static void TryDelete(string xbimFileName)
        {
            try
            {
                if (System.IO.File.Exists(xbimFileName))
                {
                    System.IO.File.Delete(xbimFileName);
                }
            }
            catch
            {/* Ignore */}
        }
    }
}