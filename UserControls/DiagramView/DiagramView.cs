using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CaeGlobals;
using ZedGraph;

namespace UserControls
{
    public partial class FrmDiagramView : Form
    {
        // Variables                                                                                                                
        private double[] _x;
        private double[][] _y;
        private double[] _xAvg;
        private double[][] _yAvg;
        private Color[] colors = new Color[] { Color.Red, Color.Green, Color.Blue, Color.Orange, Color.Violet, Color.Black };
        private int _initialHeight;


        // Properties                                                                                                               
        public string XAxisTitle { get; set; }
        public string YAxisTitle { get; set; }
        public string[] CurveNames { get; set; }
        public bool StartPlotAtZero { get; set; }


        // Constructor                                                                                                              
        public FrmDiagramView()
        {
            InitializeComponent();
            //
            XAxisTitle = "X axis";
            YAxisTitle = "Data";
            StartPlotAtZero = false;
        }


        // Event handlers                                                                                                           
        private void DiagramView_Load(object sender, EventArgs e)
        {
            _initialHeight = Height;
            UpdateData();
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            Hide();
        }
        private void DiagramView_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }
        private void FrmDiagramView_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible) ResetSizeAndLocation();
        }


        // Methods                                                                                                                  
        public void ResetSizeAndLocation()
        {
            if (Owner != null)
            {
                Height = Math.Max(Owner.Height, _initialHeight);
                Width = (int)(Height * 4.0 / 3.0);
                //
                var myScreen = Screen.FromControl(Owner);
                int screenWidth = myScreen.Bounds.Width;
                int center = Owner.Left + Owner.Width / 2;
                if (center < screenWidth / 2)
                {
                    Point location = Owner.Location.DeepClone();
                    location.X += (Owner.Width - 15 + 3);
                    Location = location;
                }
                else
                {
                    Point location = Owner.Location.DeepClone();
                    location.X -= (Width - 15 + 3);
                    Location = location;
                }
            }
        }
        public void SetData(double[] xData, double[][] yData)
        {
            _x = xData;
            _y = yData;
            //
            UpdateData();
        }
        public void GetData(out double[] xData, out double[][] yData)
        {
            UpdateData();
            //
            xData = _xAvg.ToArray();
            yData = _yAvg.ToArray();
        }
        private void UpdateData()
        {
            try
            {
                if (_x == null) return;
                //
                ApplyFilter();
                PlotData();
            }
            catch { }
        }
        private void ApplyFilter()
        {
            // Make a copy
            _xAvg = _x.ToArray();
            _yAvg = new double[_y.Length][];
            for (int i = 0; i < _y.Length; i++) _yAvg[i] = _y[i].ToArray();
        }
        private void PlotData()
        {
            zedGraph.GraphPane.CurveList.Clear();
            ZedGraph.LineItem newCurve;
            string name;
            for (int i = 0; i < _yAvg.Length; i++)
            {
                if (CurveNames != null && CurveNames.Length > i) name = CurveNames[i];
                else name = i.ToString();
                //
                newCurve = zedGraph.GraphPane.AddCurve(name, _xAvg, _yAvg[i], colors[i % colors.Length], SymbolType.Circle);
                newCurve.Line.IsAntiAlias = true;
                newCurve.Symbol.IsVisible = true;
            }
            zedGraph.GraphPane.Title.IsVisible = false;
            zedGraph.GraphPane.Legend.Position = LegendPos.TopCenter;
            zedGraph.GraphPane.Legend.IsVisible = (CurveNames != null && CurveNames.Length > 1);
            zedGraph.GraphPane.XAxis.Title.Text = XAxisTitle;
            zedGraph.GraphPane.YAxis.Title.Text = YAxisTitle;
            // Add 0,0 point
            if (StartPlotAtZero)
            { 
            zedGraph.GraphPane.XAxis.Scale.Min = 0;
            zedGraph.GraphPane.YAxis.Scale.Min = 0;
            }
            //zedGraph.RestoreScale(zedGraph.GraphPane);
            zedGraph.AxisChange();
            zedGraph.RestoreScale(zedGraph.GraphPane);
            zedGraph.ZoomOut(zedGraph.GraphPane);
        }

        
    }
}
