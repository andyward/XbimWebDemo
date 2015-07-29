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
        public ActionResult Index()
        {
            var diagnostics = XbimDiagnostics.GetData();
            return View(diagnostics);
        }

        public ActionResult Open()
        {
            var ifcModel = Server.MapPath(@"~/App_Data/OneWallTwoWindows.ifc");

            string geometryFileName = Path.ChangeExtension(ifcModel, ".wexbim");
            string xbimFileName = Path.ChangeExtension(ifcModel, ".xbim");

            Clean(xbimFileName, geometryFileName);

            using(var model = new XbimModel())
            {
                //Parse
                model.CreateFrom(ifcModel, xbimFileName, null, true);


                BuildGeometry(model, geometryFileName);

                ViewBag.XbimFile = xbimFileName;
                ViewBag.GeomFile = geometryFileName;

                

            }

            return View();
        }

        // NOTE: I don't recommend you do this under a web server due to the use of unmanaged code and the fact the operation is very expensive in terms of resources
        private static void BuildGeometry(XbimModel model, string geometryFileName = "")
        {
            Console.WriteLine("Generating Geometry...");
            var m3DModelContext = new Xbim3DModelContext(model);
            m3DModelContext.CreateContext(XbimGeometryType.PolyhedronBinary);

            if (!String.IsNullOrEmpty(geometryFileName))
            {
                ExportGeometryData(geometryFileName, m3DModelContext);
            }
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
            try
            {
                if (System.IO.File.Exists(xbimFileName))
                {
                    System.IO.File.Delete(xbimFileName);
                }
                if (System.IO.File.Exists(geometryFileName))
                {
                    System.IO.File.Delete(geometryFileName);
                }
            }
            catch
            {/* Ignore */}

        }
    }
}