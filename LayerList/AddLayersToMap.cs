using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Threading.Tasks;
using ArcGIS.Core.CIM;
using ArcGIS.Core.Data;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Catalog;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Extensions;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using System.IO;

namespace LayerList
{
    internal class AddLayersToMap : Module
    {
        private static AddLayersToMap _this = null;
       
        // Testing ComboBox access through this static module.JBK - 2020.07.17
        public ComboBox_LayerList ComboBox_LayerList { get; set; } = null;
        public Button1 B1 { get; set; } = null;
        //public string FILE_NAME = string.Empty;

        /// <summary>
        /// Retrieve the singleton instance to this module here
        /// </summary>
        public static AddLayersToMap Current
        {
            get
            {
                //return _this ?? (_this = (AddLayersToMap)FrameworkApplication.FindModule("LayerList_AddLayersToMap"));
                return _this ?? (_this = (AddLayersToMap)FrameworkApplication.FindModule("LayerList_Module"));
            }
        }

        

        #region Overrides
        /// <summary>
        /// Called by Framework when ArcGIS Pro is closing
        /// </summary>
        /// <returns>False to prevent Pro from closing, otherwise True</returns>
        protected override bool CanUnload()
        {
            //TODO - add your business logic
            //return false to ~cancel~ Application close
            return true;
        }

        #endregion Overrides

    }
}
