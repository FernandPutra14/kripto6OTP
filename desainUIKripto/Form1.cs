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
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var fileName = openFileDialog1.FileName;
                pictureBox1.Image = new Bitmap(fileName);
                pictureBox3.Image = null;
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
                var otpKey = Key.Select(c =>
                {
                    return (byte)c;
                }).ToList();

                var length = Key.Length;

                var plainImage = new Bitmap(pictureBox1.Image);

                var plain = Program.FlatImage(plainImage);
                var cipher = new byte[plain.Length];

                for(int i = 0; i < length; i++)
                {
                    cipher[i] = (byte)((plain[i] + otpKey[i]) % 256);
                }

                for (int i = length; i < plain.Length; i++)
                {
                    otpKey.Add((byte)((otpKey[i - length] + otpKey[i - 1]) % 256));
                    cipher[i] = (byte)((plain[i] + otpKey[i]) % 256);
                }

                var cipherImage = Program.UnflatImage(cipher, plainImage.Width, plainImage.Height);

                pictureBox3.Image = cipherImage;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (pictureBox3.Image != null)
            {
                if(saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    var fileName = saveFileDialog1.FileName;

                    var cipher = pictureBox3.Image;
                    cipher.Save(fileName);
                }
            }
        }

        private void label13_Click(object sender, EventArgs e)
        {
            this.Hide();
            var form2 = new Form2();
            form2.Show();
        }
    }
}
