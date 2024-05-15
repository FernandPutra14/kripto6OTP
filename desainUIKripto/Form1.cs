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
                pictureBox4.Image = null;
            }

            var key = GenerateRandomKey(10);

            using (var dbContext = new AppDbContext())
            {
                while (true)
                {
                    var terpakai = dbContext.TabelKunciTerpakai.FirstOrDefault(k => k.Key == Key);
                    if (terpakai == null)
                        break;
                    key = GenerateRandomKey(10);
                }
            }

            Key = key;
        }

        public string GenerateRandomKey(int panjang)
        {
            if (panjang == 0) return "";

            var buffer = new byte[panjang];
            var random = new Random();

            for (var i = 0; i < buffer.Length; i++)
                buffer[i] = (byte)random.Next(0, 256);

            var key = Encoding.UTF8.GetString(buffer);

            return key;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Key))
            {
                using (var dbContext = new AppDbContext())
                {
                    var terpakai = dbContext.TabelKunciTerpakai.FirstOrDefault(k => k.Key == Key);
                    if (terpakai != null)
                        MessageBox.Show($"Kunci Sudah pernah digunakan untuk enkripsi pada {terpakai.WaktuDigunakan.ToShortDateString()}", "Kunci Terpakai", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                var otpKey = Key.Select(c =>
                {
                    return (byte)c;
                }).ToList();

                var length = Key.Length;

                var plainImage = new Bitmap(pictureBox1.Image);

                var plain = Program.FlatImage(plainImage);
                var cipher = new byte[plain.Length];

                for (int i = 0; i < length; i++)
                {
                    cipher[i] = (byte)((plain[i] + otpKey[i]) % 256);
                }

                for (int i = length; i < plain.Length; i++)
                {
                    otpKey.Add((byte)((otpKey[i - length] + otpKey[i - 1]) % 256));
                    cipher[i] = (byte)((plain[i] + otpKey[i]) % 256);
                }

                var cipherImage = Program.UnflatImage(cipher, plainImage.Width, plainImage.Height);
                var otpImage = Program.UnflatImage(otpKey.ToArray(), plainImage.Width, plainImage.Height);

                pictureBox3.Image = cipherImage;
                pictureBox4.Image = otpImage;

                using (var dbContext = new AppDbContext())
                {
                    var terpakai = new KunciTerpakai()
                    {
                        Id = dbContext.TabelKunciTerpakai.Count() + 1,
                        Key = Key,
                        WaktuDigunakan = DateTime.Now,
                    };

                    dbContext.TabelKunciTerpakai.Add(terpakai);
                    dbContext.SaveChanges();
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (pictureBox3.Image != null)
            {
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
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

        private void button4_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                var formHistogram = new FormHistogram();
                formHistogram.SetHistogram(new Bitmap(pictureBox1.Image));
                formHistogram.Show();

            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (pictureBox3.Image != null)
            {
                var formHistogram = new FormHistogram();
                formHistogram.SetHistogram(new Bitmap(pictureBox3.Image));
                formHistogram.Show();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (pictureBox4.Image != null)
            {
                var formHistogram = new FormHistogram();
                formHistogram.SetHistogram(new Bitmap(pictureBox4.Image));
                formHistogram.Show();
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
                Key = GenerateRandomKey(10);
        }
    }
}
