using OlahCitra.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OlahCitra.CustomControl
{
    public partial class Histogram : UserControl
    {
        public Bitmap Image
        {
            set
            {
                _histogram = Task.Run(() => ImageProcessing.MakeGrayScaleHistogram(value)).Result;
                UpdateUI();
            }
        }

        [Browsable(true)]
        public int[] HistogramArray 
        { 
            get => _histogram; 
            set 
            {
                if(value.Length != 256)
                    throw new ArgumentException("Histogram length must be 256", nameof(value));

                _histogram = value;
                UpdateUI();
            } 
        }

        private int[] _histogram = new int[256];

        private void UpdateUI()
        {
            double jumlahPixel = _histogram.Sum();
            var normalizedHistogram = _histogram.Select(i => i / jumlahPixel).ToArray();
            var maxHistogram = _histogram.Max();

            chartHistogram.Series["Histogram"].Points.Clear();
            chartNormalizedHistogram.Series["Normalized Histogram"].Points.Clear();

            chartHistogram.ChartAreas[0].Axes[1].Maximum = maxHistogram == 0 ? maxHistogram + 1 : maxHistogram;
            chartHistogram.ChartAreas[0].Axes[1].Minimum = 0;

            for (int i = 0; i < _histogram.Length; i++)
            {
                chartHistogram.Series["Histogram"].Points.AddXY(i, _histogram[i]);
                chartNormalizedHistogram.Series["Normalized Histogram"].Points.AddXY(i, normalizedHistogram[i]);
            }
        }

        [Browsable(true)]
        public string Title { set => labelHistogram.Text = value; get => labelHistogram.Text; }

        public Histogram()
        {
            InitializeComponent();
        }
    }
}
