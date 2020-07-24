using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
//using System.Windows.Forms;
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
using ArcGIS.Desktop.Internal.Framework.Controls;
using ArcGIS.Desktop.Internal.Layouts.DockPanes.Panels;
using ArcGIS.Desktop.Mapping;
//using Microsoft.Win32;
using ComboBox = ArcGIS.Desktop.Framework.Contracts.ComboBox;
//using MessageBox = System.Windows.MessageBox;
using MessageBox = ArcGIS.Desktop.Framework.Dialogs.MessageBox;
//using System.Windows.Forms;

namespace LayerList
{
    /// <summary>
    /// Represents the ComboBox
    /// </summary>
    internal class ComboBox_LayerList : ComboBox
    {
        public Dictionary<string, string> layerNameAndPath = new Dictionary<string, string>();
        private bool _isInitialized;
       
        /// <summary>
        /// Combo Box constructor
        /// </summary>
        public ComboBox_LayerList()
        {
            // Testing ComboBox access. Sets this ComboBox to accessible
            //   variable in AddLayersToMaps static module. JBK - 2020.07.17
            AddLayersToMap addLayersToMap = AddLayersToMap.Current;
            if (addLayersToMap == null) return;
            addLayersToMap.ComboBox_LayerList = this;
            UpdateCombo();
        }

        /// <summary>
        /// Updates the combo box with all the items.
        /// </summary>

        public void UpdateCombo()  // Testing update combo. Changed to public method. JBK - 2020.07.17
        {
            // TODO – customize this method to populate the combobox with your desired items  
            if (_isInitialized)
                SelectedItem = ItemCollection.FirstOrDefault(); //set the default item in the comboBox


            if (!_isInitialized)
            {
                //Clear();
                ClearLists();
                //Add items to the combobox
                string FILE_NAME = @"\\iowa.gov.state.ia.us\DATA\DNR_GIS_Data\gis_tools\ArcGISProAddin\layerList.txt";
                //string FILE_NAME = @"C:\Work\GIS\data\shpList1.txt";

                if (File.Exists(FILE_NAME))
                {
                    readText(FILE_NAME);

                }
                else
                {
                    //var Result = MessageBox.Show(FILE_NAME + " cannot be found.", "Do you want to open another text file?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);
                    //MessageBox.Show(Result.ToString());
                    //if (Result == MessageBoxResult.Yes )
                    
                    MessageBox.Show(FILE_NAME + " cannot be found. Please use OpenTextFile button to search for your text file");
                    return; 
                   // OpenFile();
                }
                _isInitialized = true;
            }

            Enabled = true; //enables the ComboBox
            SelectedItem = ItemCollection.FirstOrDefault(); //set the default item in the comboBox
        }

        public void OpenFile()
        {
            string fileName = string.Empty;
            // System.Windows.Forms.OpenFileDialog OpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            // OpenFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            OpenItemDialog oid = new OpenItemDialog
            {
                // oid.BrowseFilter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

                Title = "Open a text file",
                Filter = ItemFilters.textFiles,
                MultiSelect = true
            };
            bool? ok = oid.ShowDialog();
            //System.Threading.Thread.Sleep(300);


            if (ok == true)
            {
                IEnumerable<Item> selected = oid.Items;

                fileName = selected.First().Path;
                //MessageBox.Show("LayerList is updated using "+fileName);

                //pass the text file path to Button1, so it can read the file too. 
                AddLayersToMap addLayersToMap = AddLayersToMap.Current;

                Button1 b1 = addLayersToMap.B1; //// get the instance of the current one, do not create a new Button1
                //Button1 b1 = new Button1(); 
                if (b1 != null)
                {
                    b1.FILE_NAME = fileName;
                    if (b1.Enabled == false)
                    {
                        b1.Enabled = true;
                    }
                }
                readText(fileName);
            }
            else
            {
                MessageBox.Show("No file selected");
                return; 
            }
            
        }
        public void readText(string FILE_NAME)
        {
            //this.Clear();
            ClearLists();
            if (FILE_NAME != "")
            {
                string[] lines = File.ReadAllLines(FILE_NAME);
                if (lines.ToList().Count == 0)
                {
                    MessageBox.Show(FILE_NAME + " is empty!");
                    return; 
                }
                foreach (string line in lines)
                {
                    if (line.Contains(","))
                    {
                        string[] content = line.Split(',');
                        string layerName = content[0].Trim();
                        //MessageBox.Show(layerName);
                        //Add(new ComboBoxItem(layerName));
                        layerNameAndPath[layerName] = content[1].Trim();
                    }
                    else
                    {                                            
                        string FName = Path.GetFileName(line.Trim());
                        if (Path.HasExtension(line.Trim()))
                        {
                            FName = Path.GetFileNameWithoutExtension(FName);
                            //Add(new ComboBoxItem(FName));
                            layerNameAndPath[FName] = line.Trim();
                            
                        }
                        else
                        {
                            if (FName.ToUpper() == "MAPSERVER")
                            {
                                string[] folders = line.Trim().Split('\\'); //(Path.DirectorySeparatorChar);
                                string serviceName = folders[folders.Length-1];
                                //Add(new ComboBoxItem(serviceName));
                                layerNameAndPath[serviceName] = line.Trim();
                            } else
                            {
                                MessageBox.Show(FName + " is not a map service, please verify.");
                                return; 
                            }
                        }
                                              
                    }
                }

                var list = layerNameAndPath.Keys.ToList();
                list.Sort();
                foreach (var key in list)
                {
                    if (key.ToUpper() == "LAYERNAME") Insert(0, new ComboBoxItem(key));
                    else Add(new ComboBoxItem(key));
                }
                MessageBox.Show(this.ItemCollection.Count() + " layers added to layer list from "+FILE_NAME);
                _isInitialized = true;
            }
            else
            {
                MessageBox.Show("No file to read");
                return;
            }
        }
        /// <summary>
        /// The on comboBox selection change event. 
        /// </summary>
        /// <param name="item">The newly selected combo box item</param>
        protected override void OnSelectionChange(ComboBoxItem item)
        {
            if (item == null)
                return;

            if (string.IsNullOrEmpty(item.Text))
                return;

            // TODO  Code behavior when selection changes.   
            AddLayersToMap addLayersToMap = AddLayersToMap.Current;

            Button1 btn = addLayersToMap.B1??new Button1();          

            if (item.Text.ToUpper() != "LAYERNAME")
            {
                if (File.Exists(layerNameAndPath[item.Text]) || layerNameAndPath[item.Text].Contains("MapServer"))  //make sure the path exists
                {
                    btn.AddLayer(layerNameAndPath[item.Text]); //get the path with dictionary key, then call AddLayer
                }
                else System.Windows.MessageBox.Show(string.Format("{0} doesn't exist or is not accessible", layerNameAndPath[item.Text]));              
            }


            //string FILE_NAME = @"C:\Work\GIS\data\shpList.txt";
            //string[] lines = File.ReadAllLines(FILE_NAME);//.Where(x =>!string.IsNullOrWhiteSpace(x));
            //foreach (string line in lines)
            //{
            //    string[] content = line.Split(',');
            //    //Add(new ComboBoxItem(content[0]));
            //    if (item.Text != "LayerName" && item.Text == content[0])
            //    {
            //        btn.AddLayer(content[1]);
            //    }
            //}

        }

        private void ClearLists()
        {
            Clear(); //clear the comboBox
            layerNameAndPath.Clear(); //clear the dictionary
        }

        //the OnUpdate method will run automatically when the comboBox is empty. 
        //protected override void OnUpdate()
        //{
        //    if (_isInitialized != true && SelectedItem == null)
        //    {
        //        OpenItemDialog oid = new OpenItemDialog
        //        {
        //            Title = "Open a text file.",
        //            Filter = ItemFilters.textFiles,
        //            MultiSelect = true
        //        };

        //        if (!oid.ShowDialog().Value)
        //        {
        //            return;
        //        }

        //        var item = oid.Items.First();
        //        readText(item.Path);

        //        SelectedItem = ItemCollection.FirstOrDefault(); //set the default item in the comboBox
        //    }
        //}

    }
}
