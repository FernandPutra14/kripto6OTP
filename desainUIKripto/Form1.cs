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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public string Key { get => textBox1.Text; set => textBox1.Text = value; }

        private void button1_Click(object sender, EventArgs e)
        {
            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var fileName = openFileDialog1.FileName;
                pictureBox1.Image = new Bitmap(fileName);
            }

            var buffer = new byte[8];
            var random = new Random();
            random.NextBytes(buffer);
            Key = Encoding.ASCII.GetString(buffer);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Key))
            {
                var otpKey = Key;
                var length = otpKey.Length;
                var plainImage = pictureBox1.Image;

                byte[] plain = new byte[plainImage.Width * plainImage.Height * 3];
            }
        }
    }
}
