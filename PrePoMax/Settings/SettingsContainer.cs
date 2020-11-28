using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;
using System.IO;
using DynamicTypeDescriptor;
using System.Drawing;

namespace PrePoMax
{
    [Serializable]
    public class SettingsContainer
    {
        // Variables                                                                                                                
        private GeneralSettings _general;
        private GraphicsSettings _graphics;
        private ColorSettings _color;
        private PreSettings _pre;
        private CalculixSettings _calculix;
        private PostSettings _post;
        private LegendSettings _legend;
        private StatusBlockSettings _statusBlock;


        // Properties                                                                                                               
        public GeneralSettings General { get { return _general; } set { _general = value; } }
        public GraphicsSettings Graphics { get { return _graphics; } set { _graphics = value; } }
        public ColorSettings Color { get { return _color; } set { _color = value; } }
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
            _general = new GeneralSettings();
            _graphics = new GraphicsSettings();
            _color = new ColorSettings();
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
            _pre.Reset();
            _calculix.Reset();
            _post.Reset();
            _legend.Reset();
            _statusBlock.Reset();
        }
        public Dictionary<string, ISettings> ToDictionary()
        {
            Dictionary<string, ISettings> items = new Dictionary<string, ISettings>();
            items.Add(Globals.GeneralSettingsName, _general);
            items.Add(Globals.GraphicsSettingsName, _graphics);
            items.Add(Globals.ColorSettingsName, _color);
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
        public void SaveToFile(string fileName)
        {
            ToDictionary().DumpToFile(fileName);
        }
        public void LoadFromFile()
        {
            string fileName = Path.Combine(System.Windows.Forms.Application.StartupPath, Globals.SettingsFileName);
            if (File.Exists(fileName))
            {
                var t = Task.Run(() => LoadFromFile(fileName));
                t.Wait();
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
