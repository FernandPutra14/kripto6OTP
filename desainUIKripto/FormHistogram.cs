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

namespace desainUIKripto
{
    public partial class FormHistogram : Form
    {
        public FormHistogram()
        {
            InitializeComponent();
        }

        public void SetHistogram(Bitmap bitmap)
        {
            (var histogramRed, var histogramGreen, var histogramBlue) = ImageProcessing.MakeRGBHistogram(bitmap);
            var histogram = ImageProcessing.MakeGrayScaleHistogram(bitmap);

            histogramR.HistogramArray = histogramRed;
            histogramG.HistogramArray = histogramGreen;
            histogramB.HistogramArray = histogramBlue;
            histogramGrayScale.HistogramArray = histogram;
        }
    }
}
