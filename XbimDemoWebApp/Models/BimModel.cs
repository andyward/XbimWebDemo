using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XbimDemoWebApp.Models
{
    public class BimModel
    {
        public BimModel(string modelName, string wexBim)
        {
            WexBimFile = wexBim;
            ModelName = modelName;
        }

        public string WexBimFile { get; set; }
        public string ModelName { get; set; }
    }

    public class FederatedBimModel
    {
        public FederatedBimModel()
        {
            Models = new List<BimModel>();
        }

        public List<BimModel> Models { get; private set; }

        public string ModelName
        {
            get
            {
                return Models.Select(m=>m.ModelName)
                    .Aggregate((current, next) => current + " + " + next);
            }
        }
    }
}