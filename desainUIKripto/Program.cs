using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace desainUIKripto
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            form1 = new Form1();

            Application.Run(form1);
        }

        public static byte[] FlatImage(Bitmap image)
        {
            byte[] buffer = new byte[image.Width * image.Height * 3];

            int i = 0;
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    var piksel = image.GetPixel(x, y);

                    buffer[i] = piksel.R;
                    buffer[i + 1] = piksel.G;
                    buffer[i + 2] = piksel.B;
                    i += 3;
                }
            }

            return buffer;
        }

        public static Bitmap UnflatImage(byte[] buffer, int width, int height)
        {
            Bitmap image = new Bitmap(width, height);

            int i = 0;
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    image.SetPixel(x, y, Color.FromArgb(buffer[i], buffer[i + 1], buffer[i + 2]));
                    i += 3;
                }
            }

            return image;
        }

        public static Form1 form1;
    }
}
