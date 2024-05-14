using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace desainUIKripto
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        public string Key { get => textBox1.Text; set => textBox1.Text = value; }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var fileName = openFileDialog1.FileName;
                pictureBox1.Image = new Bitmap(fileName);
                pictureBox3.Image = null;
            }
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            Program.form1.Show();
        }

        private void label13_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(Key))
            {
                var otpKey = Key.Select(c =>
                {
                    return (byte)c;
                }).ToList();

                var length = Key.Length;

                var cipherImage = new Bitmap(pictureBox1.Image);
                var cipher = Program.FlatImage(cipherImage);
                var plain = new byte[cipher.Length];

                for (int i = 0; i < length; i++)
                {
                    plain[i] = (byte)((cipher[i] - otpKey[i]) % 256);
                }

                for (int i = length; i < cipher.Length; i++)
                {
                    otpKey.Add((byte)((otpKey[i - length] + otpKey[i - 1]) % 256));
                    plain[i] = (byte)((cipher[i] - otpKey[i]) % 256);
                }

                var plainImage = Program.UnflatImage(plain, cipherImage.Width, cipherImage.Height);

                pictureBox3.Image = plainImage;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (pictureBox3.Image != null)
            {
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    var fileName = saveFileDialog1.FileName;

                    var plain = pictureBox3.Image;
                    plain.Save(fileName);
                }
            }
        }
    }
}
