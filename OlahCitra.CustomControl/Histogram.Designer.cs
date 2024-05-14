namespace OlahCitra.CustomControl
{
    partial class Histogram
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.chartHistogram = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chartNormalizedHistogram = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.labelHistogram = new System.Windows.Forms.Label();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartHistogram)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartNormalizedHistogram)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 23);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.chartHistogram);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.chartNormalizedHistogram);
            this.splitContainer1.Size = new System.Drawing.Size(593, 431);
            this.splitContainer1.SplitterDistance = 293;
            this.splitContainer1.TabIndex = 4;
            // 
            // chartHistogram
            // 
            chartArea1.AxisX.Maximum = 255D;
            chartArea1.AxisX.Minimum = 0D;
            chartArea1.AxisY.ScaleBreakStyle.Spacing = 1D;
            chartArea1.Name = "ChartArea1";
            this.chartHistogram.ChartAreas.Add(chartArea1);
            this.chartHistogram.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.Name = "Legend1";
            this.chartHistogram.Legends.Add(legend1);
            this.chartHistogram.Location = new System.Drawing.Point(0, 0);
            this.chartHistogram.Name = "chartHistogram";
            series1.ChartArea = "ChartArea1";
            series1.CustomProperties = "PixelPointWidth=5";
            series1.LabelToolTip = "Gray Level #VALX{N0}, Frekuensi : #VAL{N0}\\n";
            series1.Legend = "Legend1";
            series1.Name = "Histogram";
            series1.ToolTip = "X : #VALX{N0}, Y : #VAL{N0}";
            this.chartHistogram.Series.Add(series1);
            this.chartHistogram.Size = new System.Drawing.Size(293, 431);
            this.chartHistogram.TabIndex = 0;
            this.chartHistogram.Text = "chart1";
            // 
            // chartNormalizedHistogram
            // 
            chartArea2.AxisX.Maximum = 255D;
            chartArea2.AxisX.Minimum = 0D;
            chartArea2.AxisY.Maximum = 1D;
            chartArea2.AxisY.Minimum = 0D;
            chartArea2.Name = "ChartArea1";
            this.chartNormalizedHistogram.ChartAreas.Add(chartArea2);
            this.chartNormalizedHistogram.Dock = System.Windows.Forms.DockStyle.Fill;
            legend2.Name = "Legend1";
            this.chartNormalizedHistogram.Legends.Add(legend2);
            this.chartNormalizedHistogram.Location = new System.Drawing.Point(0, 0);
            this.chartNormalizedHistogram.Name = "chartNormalizedHistogram";
            series2.ChartArea = "ChartArea1";
            series2.Color = System.Drawing.Color.Lime;
            series2.LabelToolTip = "X: #VALX{N0}, Y: #VAL{N3}";
            series2.Legend = "Legend1";
            series2.Name = "Normalized Histogram";
            series2.ToolTip = "X: #VALX{N0}, Y: #VAL{N3}";
            this.chartNormalizedHistogram.Series.Add(series2);
            this.chartNormalizedHistogram.Size = new System.Drawing.Size(296, 431);
            this.chartNormalizedHistogram.TabIndex = 0;
            this.chartNormalizedHistogram.Text = "chart1";
            // 
            // labelHistogram
            // 
            this.labelHistogram.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelHistogram.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.labelHistogram.Location = new System.Drawing.Point(0, 0);
            this.labelHistogram.Name = "labelHistogram";
            this.labelHistogram.Size = new System.Drawing.Size(593, 23);
            this.labelHistogram.TabIndex = 3;
            this.labelHistogram.Text = "labelHistogram";
            this.labelHistogram.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Histogram
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.labelHistogram);
            this.Name = "Histogram";
            this.Size = new System.Drawing.Size(593, 454);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chartHistogram)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartNormalizedHistogram)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartHistogram;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartNormalizedHistogram;
        private System.Windows.Forms.Label labelHistogram;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
    }
}
