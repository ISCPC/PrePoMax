using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CaeGlobals;
using System.IO;
using CaeMesh;

namespace PrePoMax
{
    [Serializable]
    public class SettingsContainer
    {
        // Variables                                                                                                                
        [NonSerialized] private Dictionary<string, vtkControl.vtkMaxColorSpectrum> _colorSpectrums;
        //
        private GeneralSettings _general;
        private GraphicsSettings _graphics;
        private ColorSettings _color;
        private AnnotationSettings _annotations;
        private MeshingSettings _meshing;
        private PreSettings _pre;
        private CalculixSettings _calculix;
        private PostSettings _post;
        private LegendSettings _legend;
        private StatusBlockSettings _statusBlock;


        // Properties                                                                                                               
        public GeneralSettings General { get { return _general; } set { _general = value; } }
        public GraphicsSettings Graphics { get { return _graphics; } set { _graphics = value; } }
        public ColorSettings Color { get { return _color; } set { _color = value; } }
        public AnnotationSettings Annotations { get { return _annotations; } set { _annotations = value; } }
        public MeshingSettings Meshing { get { return _meshing; } set { _meshing = value; } }
        public PreSettings Pre { get { return _pre; } set { _pre = value; } }
        public CalculixSettings Calculix { get { return _calculix; } set { _calculix = value; } }
        public PostSettings Post { get { return _post; } set { _post = value; } }       
        public LegendSettings Legend { get { return _legend; } set { _legend = value; } }
        public StatusBlockSettings StatusBlock { get { return _statusBlock; } set { _statusBlock = value; } }


        // Constructors                                                                                                             
        public SettingsContainer()
        {
            Initialize();
        }
        public SettingsContainer(Dictionary<string, ISettings> items)
        {
            Initialize();
            FromDictionary(items);
        }


        // Methods                                                                                                                  
        public void Initialize()
        {
            _colorSpectrums = new Dictionary<string, vtkControl.vtkMaxColorSpectrum>();
            //
            _general = new GeneralSettings();
            _graphics = new GraphicsSettings();
            _color = new ColorSettings();
            _annotations = new AnnotationSettings();
            _meshing = new MeshingSettings();
            _pre = new PreSettings();
            _calculix = new CalculixSettings();
            _post = new PostSettings();
            _legend = new LegendSettings();
            _statusBlock = new StatusBlockSettings();
        }
        public void Reset()
        {
            _general.Reset();
            _graphics.Reset();
            _color.Reset();
            _annotations.Reset();
            _meshing.Reset();
            _pre.Reset();
            _calculix.Reset();
            _post.Reset();
            _legend.Reset();
            _statusBlock.Reset();
        }
        public void ClearColorSpectrums()
        {
            _colorSpectrums.Clear();
            _legend.ColorSpectrum.MinMaxType = vtkControl.vtkColorSpectrumMinMaxType.Automatic;
        }
        public void Set(SettingsContainer settingsContainer, ViewGeometryModelResults currentView,
                        CaeResults.FieldData currentFieldData)
        {
            Clone(settingsContainer);
            //
            if (currentView == ViewGeometryModelResults.Results && currentFieldData != null)
            {
                string key = currentFieldData.Name + "_" + currentFieldData.Component;
                // Save individual Min/Max setting
                if (_legend.ColorSpectrum.MinMaxType == vtkControl.vtkColorSpectrumMinMaxType.Manual)
                {
                    if (_colorSpectrums.ContainsKey(key)) _colorSpectrums[key] = _legend.ColorSpectrum.DeepClone();
                    else _colorSpectrums.Add(key, _legend.ColorSpectrum.DeepClone());
                }
                // Remove individual Min/Max setting
                else _colorSpectrums.Remove(key);
            }
            //
            _legend.ColorSpectrum.MinMaxType = vtkControl.vtkColorSpectrumMinMaxType.Automatic;
        }
        public SettingsContainer Get(ViewGeometryModelResults currentView, CaeResults.FieldData currentFieldData)
        {
            SettingsContainer clone = new SettingsContainer();
            clone.Clone(this);
            //
            if (currentView == ViewGeometryModelResults.Results && currentFieldData != null)
            {
                vtkControl.vtkMaxColorSpectrum colorSpectrum;
                string key = currentFieldData.Name + "_" + currentFieldData.Component;
                // Apply individual Min/Max setting if it exists
                if (_colorSpectrums.TryGetValue(key, out colorSpectrum) &&
                    colorSpectrum.MinMaxType == vtkControl.vtkColorSpectrumMinMaxType.Manual)
                {
                    clone._legend.ColorSpectrum.SetMinMax(colorSpectrum);
                }
            }
            //
            return clone;
        }
        private void Clone(SettingsContainer settingsContainer)
        {
            SettingsContainer clone = settingsContainer.DeepClone();
            //
            _general = clone._general;
            _graphics = clone._graphics;
            _color = clone._color;
            _annotations = clone._annotations;
            _meshing = clone._meshing;
            _pre = clone._pre;
            _calculix = clone._calculix;
            _post = clone._post;
            _legend = clone._legend;
            _statusBlock = clone._statusBlock;
        }
        public Dictionary<string, ISettings> ToDictionary()
        {
            Dictionary<string, ISettings> items = new Dictionary<string, ISettings>();
            items.Add(Globals.GeneralSettingsName, _general);
            items.Add(Globals.GraphicsSettingsName, _graphics);
            items.Add(Globals.ColorSettingsName, _color);
            items.Add(Globals.AnnotationSettingsName, _annotations);
            items.Add(Globals.MeshingSettingsName, _meshing);
            items.Add(Globals.PreSettingsName, _pre);
            items.Add(Globals.CalculixSettingsName, _calculix);
            items.Add(Globals.PostSettingsName, _post);
            items.Add(Globals.LegendSettingsName, _legend);
            items.Add(Globals.StatusBlockSettingsName, _statusBlock);
            return items;
        }
        public void FromDictionary(Dictionary<string, ISettings> items)
        {
            try
            {
                _general = (GeneralSettings)items[Globals.GeneralSettingsName];
                _graphics = (GraphicsSettings)items[Globals.GraphicsSettingsName];
                _color = (ColorSettings)items[Globals.ColorSettingsName];
                _annotations = (AnnotationSettings)items[Globals.AnnotationSettingsName];
                _meshing = (MeshingSettings)items[Globals.MeshingSettingsName];
                _pre = (PreSettings)items[Globals.PreSettingsName];
                _calculix = (CalculixSettings)items[Globals.CalculixSettingsName];
                _post = (PostSettings)items[Globals.PostSettingsName];
                _legend = (LegendSettings)items[Globals.LegendSettingsName];
                _statusBlock = (StatusBlockSettings)items[Globals.StatusBlockSettingsName];
            }
            catch
            {

            }
        }
        public void SaveToFile()
        {
            SaveToFile(Path.Combine(System.Windows.Forms.Application.StartupPath, Globals.SettingsFileName));
        }
        public void SaveToFile(string fileName)
        {
            // Use a temporary file to save the data and copy it at the end
            string tmpFileName = Tools.GetNonExistentRandomFileName(Path.GetDirectoryName(fileName), ".tmp");
            ToDictionary().DumpToFile(tmpFileName);
            File.Copy(tmpFileName, fileName, true);
            File.Delete(tmpFileName);
        }
        public void LoadFromFile()
        {
            try
            {
                Initialize();
                //
                string fileName = Path.Combine(System.Windows.Forms.Application.StartupPath, Globals.SettingsFileName);
                if (File.Exists(fileName))
                {
                    var t = Task.Run(() => LoadFromFile(fileName));
                    t.Wait();
                }
                // Reset the color limits
                ClearColorSpectrums();
            }
            catch
            {
                Initialize();
            }
        }
        private void LoadFromFile(string fileName)
        {            
            Dictionary<string, ISettings> items = CaeGlobals.Tools.LoadDumpFromFile<Dictionary<string, ISettings>>(fileName);
            FromDictionary(items);
        }
        //
        public string GetWorkDirectory()
        {
            string lastFileName = _general.LastFileName;
            if (_calculix.UsePmxFolderAsWorkDirectory && lastFileName != null && File.Exists(lastFileName) &&
                Path.GetExtension(lastFileName) == ".pmx")
            {
                return Path.GetDirectoryName(lastFileName);
            }
            else return _calculix.WorkDirectory;
        }
      
    }
}
