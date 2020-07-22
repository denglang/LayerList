using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using ArcGIS.Core.CIM;
using ArcGIS.Core.Data;
using ArcGIS.Core.Data.UtilityNetwork.Trace;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Catalog;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Extensions;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Internal.Mapping;
using ArcGIS.Desktop.Mapping;

namespace LayerList
{
    internal class Button1 : Button
    {
        public string FILE_NAME = string.Empty;
        protected override void OnClick()
        {
           AddLayersToMap addLayersToMap = AddLayersToMap.Current;
            addLayersToMap.B1 = this;

            ComboBox_LayerList cmbox = addLayersToMap.ComboBox_LayerList;
            // List<string> items = addLayersToMap.ComboBox_LayerList.ItemCollection;
            Dictionary<string, string> dict = cmbox.layerNameAndPath;

            if (cmbox.ItemCollection.Count > 0)
            {
                //IEnumerable<Item> items = cmbox.ItemCollection;
                foreach (var item in cmbox.ItemCollection)
                {
                    //Debug.Print(item.GetType());
                    //MessageBox.Show(item.ToString());
                    if(item.ToString() != "LayerName")
                    {
                        if (File.Exists(dict[item.ToString()]))
                        {
                            AddLayer(dict[item.ToString()]);
                        }                        
                    }                    
                }
            }
            
            else GetLayers();
        }

        public void GetLayers()
        {
            //string FILE_NAME = @"W:\Ames\LayerFiles\coa_LAYERS_list_W.txt"; 
            if (FILE_NAME == "") { 
                FILE_NAME = @"C:\Work\GIS\data\shpList1.txt";
            }
            //MapView mv = MapView.Active;
            //Map map = mv.Map;
            if (File.Exists(FILE_NAME))
            {
                string[] lines = File.ReadAllLines(FILE_NAME);//.Where(x =>!string.IsNullOrWhiteSpace(x));
                foreach (string line in lines)
                {                   
                    if (line.EndsWith(".lyr") || line.EndsWith(".shp"))
                    {
                        //Layer newLayer = LayerFactory.Instance.CreateLayer(new Uri(line), map);
                        //string[] path = line.Split(',');
                        if (line.Length >= 5 && line.Contains(","))
                        {
                            if (line.Split(',')[0]!= "LayerName")
                            {
                                string filePath = line.Split(',')[1].Trim();
                                if (File.Exists(filePath))
                                {
                                    AddLayer(filePath);
                                }
                                else MessageBox.Show(string.Format("{0} doesn't exist or is not accessible", filePath));
                            }                                                          
                        }
                        else
                        {
                            string fPath = line.Trim();
                            //FName = Path.GetFileNameWithoutExtension(FName);
                            if (File.Exists(fPath))
                            {
                                AddLayer(fPath);
                            } 
                            else MessageBox.Show(string.Format("{0} doesn't exist or is not accessible", fPath));                                                                            
                        }
                    }
                    
                    //else
                    //{
                    //    MessageBox.Show(line + " is not a valid file, cannot add to map. ");
                    //    //return; 
                    //}
                }
            }
            else
            {
                MessageBox.Show(FILE_NAME + " is not accessible");
                this.Enabled = false;
                return; 
                //string uriShp = @"C:\Work\GIS\data\states.shp";
                //Layer lyr = LayerFactory.Instance.CreateLayer(new Uri(uriShp), map);
            }
        }

        public Task<Layer> AddLayer(string uri)
        {
            return QueuedTask.Run(() =>
            {
                Map map = MapView.Active.Map;
                return LayerFactory.Instance.CreateLayer(new Uri(uri), map);
            });
        }
    }
}
