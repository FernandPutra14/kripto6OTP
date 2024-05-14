using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OlahCitra.Core;

namespace OlahCitra.CustomControl
{
    public partial class ImageStatistics : UserControl
    {
        private OlahCitra.Core.ImageStatistics _imageStatistics;

        public ImageStatistics()
        {
            InitializeComponent();
        }

        [Browsable(true)]
        public string Title
        {
            set => labelStatistics.Text = value;
            get => labelStatistics.Text;
        }

        public OlahCitra.Core.ImageStatistics Statistics
        {
            set
            {
                _imageStatistics = value;
                UpdateUI();
            }
            get => _imageStatistics;
        }

        private void UpdateUI()
        {
            if (_imageStatistics != null)
            {
                progressBarMin.Value = _imageStatistics.MininimumGrayLevel;
                labelCurrentMin.Text = _imageStatistics.MininimumGrayLevel.ToString();

                progressBarMax.Value = _imageStatistics.MaximumGrayLevel;
                labelCurrentMax.Text = _imageStatistics.MaximumGrayLevel.ToString();

                progressBarAvg.Value = (int)_imageStatistics.AverageGrayLevel;
                labelCurrentAvg.Text = _imageStatistics.AverageGrayLevel.ToString("F3");
            }
        }
    }
}
