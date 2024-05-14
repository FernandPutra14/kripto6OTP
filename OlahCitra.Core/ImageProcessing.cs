using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Ocl;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using static System.Net.Mime.MediaTypeNames;

namespace OlahCitra.Core
{
    public static class ImageProcessing
    {
        public static Bitmap MakeGreyScale(Bitmap original)
        {
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);

            for (int i = 0; i < original.Width; i++)
                for (int j = 0; j < original.Height; j++)
                {
                    var pixel = original.GetPixel(i, j);
                    var avg = (pixel.R + pixel.B + pixel.G) / 3;
                    newBitmap.SetPixel(i, j, Color.FromArgb(avg, avg, avg));
                }

            return newBitmap;
        }

        public static Bitmap GrayLevelTransformation(Bitmap original, Func<int, int> transform)
        {
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);

            for (int i = 0; i < original.Width; i++)
            {
                for (int j = 0; j < original.Height; j++)
                {
                    var pixel = original.GetPixel(i, j);
                    var newGrayScale = Math.Min(transform(pixel.R), 255);
                    newGrayScale = Math.Max(newGrayScale, 0);

                    newBitmap.SetPixel(i, j, Color.FromArgb(pixel.A, newGrayScale, newGrayScale, newGrayScale));
                }
            }

            return newBitmap;
        }

        public static Bitmap RGBTransformation(Bitmap original, Func<Color, Color> transform)
        {
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);

            for (int i = 0; i < original.Width; i++)
            {
                for (int j = 0; j < original.Height; j++)
                {
                    var pixel = original.GetPixel(i, j);
                    var newPixel = transform(pixel);
                    newBitmap.SetPixel(i, j, newPixel);
                }
            }

            return newBitmap;
        }

        public static int MaxGrayLevel(Bitmap img)
        {
            int max = 0;

            for (int i = 0; i < img.Width; i++)
                for (int j = 0; j < img.Height; j++)
                    max = Math.Max(max, img.GetPixel(i, j).R);

            return max;
        }

        public static int MinGrayLevel(Bitmap img)
        {
            int min = int.MaxValue;

            for (int i = 0; i < img.Width; i++)
                for (int j = 0; j < img.Height; j++)
                    min = Math.Min(min, img.GetPixel(i, j).R);

            return min;
        }

        public static double AvgGreyLevel(Bitmap img)
        {
            double total = 0;
            double jumlahPixel = img.Width * img.Height;

            for (int i = 0; i < img.Width; i++)
                for (int j = 0; j < img.Height; j++)
                {
                    var pixel = img.GetPixel(i, j);
                    total += pixel.R;
                }

            return total / jumlahPixel;
        }

        public static ImageStatistics MakeStatistic(Bitmap img)
        {
            return new ImageStatistics
            {
                MininimumGrayLevel = MinGrayLevel(img),
                MaximumGrayLevel = MaxGrayLevel(img),
                AverageGrayLevel = AvgGreyLevel(img)
            };
        }

        public static int[] MakeGrayScaleHistogram(Bitmap img)
        {
            int[] histogram = new int[256];

            for (int i = 0; i < img.Width; i++)
                for (int j = 0; j < img.Height; j++)
                {
                    var pixel = img.GetPixel(i, j);
                    histogram[(pixel.R + pixel.G + pixel.B) / 3]++;
                }

            //using (var image = img.ToImage<Bgr, byte>())
            //using (var gray = new UMat())
            //using (var result = new Mat())
            //using (var vou = new VectorOfUMat())
            //{
            //    CvInvoke.CvtColor(image, gray, ColorConversion.Bgr2Gray);
            //    vou.Push(gray);
            //    CvInvoke.CalcHist(vou, new int[] { 0 }, null, result, new int[] { 256 }, new float[] { 0, 256 }, false);
            //    var hist = (float[,])result.GetData();

            //    for (int i = 0; i < hist.GetLength(0); i++)
            //        histogram[i] = (int)hist[i, 0];
            //}

            return histogram;
        }

        public static (Bitmap red, Bitmap green, Bitmap blue) SplitToRGB(Bitmap img)
        {
            var redImage = new Bitmap(img.Width, img.Height);
            var greenImage = new Bitmap(img.Width, img.Height);
            var blueImage = new Bitmap(img.Width, img.Height);

            for (int i = 0; i < img.Width; i++)
            {
                for (int j = 0; j < img.Height; j++)
                {
                    var originalPixel = img.GetPixel(i, j);
                    redImage.SetPixel(i, j, Color.FromArgb(originalPixel.A, originalPixel.R, 0, 0));
                    greenImage.SetPixel(i, j, Color.FromArgb(originalPixel.A, 0, originalPixel.G, 0));
                    blueImage.SetPixel(i, j, Color.FromArgb(originalPixel.A, 0, 0, originalPixel.B));
                }
            }

            return (redImage, greenImage, blueImage);
        }

        public static (int[] histogramR, int[] histogramG, int[] histogramB) MakeRGBHistogram(Bitmap img)
        {
            var histogramR = new int[256];
            var histogramG = new int[256];
            var histogramB = new int[256];

            for (int i = 0; i < img.Width; i++)
            {
                for (int j = 0; j < img.Height; j++)
                {
                    var pixel = img.GetPixel(i, j);
                    histogramR[pixel.R]++;
                    histogramG[pixel.G]++;
                    histogramB[pixel.B]++;
                }
            }

            return (histogramR, histogramG, histogramB);
        }

        public static int OtsuThresholding(Bitmap bitmap)
        {
            var histogram = MakeGrayScaleHistogram(bitmap);

            var total = histogram.Sum();

            var sum = 0f;
            for (int i = 1; i < histogram.Length; i++) sum += i * histogram[i];

            var sumB = 0f;
            var wB = 0;
            var wF = 0;

            var varMax = 0f;
            var threshold = 0;

            for (int i = 1; i < histogram.Length; i++)
            {
                wB += histogram[i];

                if (wB == 0) continue;

                wF = total - wB;

                if (wF == 0) break;

                sumB += i * histogram[i];

                var mB = sumB / wB;
                var mF = (sum - sumB) / wF;

                float varBetween = (float)wB * (float)wF * (mB - mF) * (mB - mF);

                if (varBetween > varMax)
                {
                    varMax = varBetween;
                    threshold = i;
                }
            }

            return threshold;
        }

        public static Bitmap OtsuCv(Bitmap original)
        {
            var outImage = new Bitmap(original.Width, original.Height);

            using (var image = original.ToImage<Bgr, byte>())
            using (var imageGray = image.Convert<Gray, byte>())
            using (var result = new Mat())
            {
                CvInvoke.Threshold(imageGray, result, 0, 255, ThresholdType.Otsu | ThresholdType.Binary);

                outImage = result.ToBitmap();
            }

            return outImage;
        }

        public static Bitmap KMeansSegmentation(
            Bitmap image,
            int k = 3)
        {
            Matrix<float> data = new Matrix<float>(image.Width * image.Height, 3);
            var labels = new VectorOfInt();

            int matrixIndex = 0;
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    var pixel = image.GetPixel(x, y);
                    data[matrixIndex, 0] = (float)pixel.R;
                    data[matrixIndex, 1] = (float)pixel.G;
                    data[matrixIndex, 2] = (float)pixel.B;
                    matrixIndex++;
                }
            }

            CvInvoke.Kmeans(data, k, labels, new MCvTermCriteria(Math.Pow(10, -10)), 8, KMeansInitType.PPCenters);

            //Pelabelan
            var random = new Random();
            var colorLabel = new List<Color>();
            for (int i = 0; i < k; i++)
            {
                var randR = random.Next(0, 256);
                var randG = random.Next(0, 256);
                var randB = random.Next(0, 256);
                colorLabel.Add(Color.FromArgb(randR, randG, randB));
            }

            var outImage = new Bitmap(image.Width, image.Height);
            var labelsIndex = 0;
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    outImage.SetPixel(x, y, colorLabel[labels[labelsIndex]]);
                    labelsIndex++;
                }
            }

            return outImage;
        }

        public static Bitmap KMeansSegmentationCielab(
            Bitmap image,
            int k = 3)
        {
            Image<Bgr, Byte> imageBgr = image.ToImage<Bgr, Byte>();
            Image<Lab, Byte> imageLab = imageBgr.Convert<Lab, Byte>();

            Matrix<float> data = new Matrix<float>(image.Width * image.Height, 3);
            var labels = new VectorOfInt();

            int matrixIndex = 0;
            for (int x = 0; x < imageLab.Width; x++)
            {
                for (int y = 0; y < imageLab.Height; y++)
                {
                    var pixel = imageLab[y, x];
                    data[matrixIndex, 0] = (float)pixel.X;
                    data[matrixIndex, 1] = (float)pixel.Y;
                    data[matrixIndex, 2] = (float)pixel.Z;
                    matrixIndex++;
                }
            }

            CvInvoke.Kmeans(data, k, labels, new MCvTermCriteria(Math.Pow(10, -10)), 8, KMeansInitType.PPCenters);

            //Pelabelan
            var random = new Random();
            var colorLabel = new List<Color>();
            for (int i = 0; i < k; i++)
            {
                var randR = random.Next(0, 256);
                var randG = random.Next(0, 256);
                var randB = random.Next(0, 256);
                colorLabel.Add(Color.FromArgb(randR, randG, randB));
            }

            var outImage = new Bitmap(image.Width, image.Height);
            var labelsIndex = 0;
            for (int x = 0; x < imageLab.Width; x++)
            {
                for (int y = 0; y < imageLab.Height; y++)
                {
                    outImage.SetPixel(x, y, colorLabel[labels[labelsIndex]]);
                    labelsIndex++;
                }
            }

            return outImage;
        }

        public static Bitmap Dilation(Bitmap input, int kernelSize)
        {
            Bitmap outImage = new Bitmap(input.Width, input.Height);

            using (UMat image = input.ToImage<Gray, Byte>().ToUMat())
            {
                var kernel = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(kernelSize, kernelSize), new Point(1, 1));

                CvInvoke.MorphologyEx(image,
                                      image,
                                      MorphOp.Dilate,
                                      kernel,
                                      new Point(-1, -1),
                                      1,
                                      BorderType.Default,
                                      new MCvScalar());

                outImage = image.ToBitmap();
            }

            return outImage;
        }

        public static Bitmap Erosion(Bitmap input, int kernelSize)
        {
            Bitmap outImage = new Bitmap(input.Width, input.Height);

            using (UMat image = input.ToImage<Gray, Byte>().ToUMat())
            {
                var kernel = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(10, 10), new Point(1, 1));

                CvInvoke.MorphologyEx(image,
                                      image,
                                      MorphOp.Erode,
                                      kernel,
                                      new Point(-1, -1),
                                      1,
                                      BorderType.Default,
                                      new MCvScalar());

                outImage = image.ToBitmap();
            }

            return outImage;
        }

        public static Bitmap Opening(Bitmap input, int kernelSize)
        {
            Bitmap outImage = new Bitmap(input.Width, input.Height);

            using (UMat image = input.ToImage<Gray, Byte>().ToUMat())
            {
                var kernel = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(kernelSize, kernelSize), new Point(1, 1));

                CvInvoke.MorphologyEx(image,
                                      image,
                                      MorphOp.Open,
                                      kernel,
                                      new Point(-1, -1),
                                      1,
                                      BorderType.Default,
                                      new MCvScalar());

                outImage = image.ToBitmap();
            }

            return outImage;
        }

        public static Bitmap Closing(Bitmap input, int kernelSize)
        {
            Bitmap outImage = new Bitmap(input.Width, input.Height);

            using (UMat image = input.ToImage<Gray, Byte>().ToUMat())
            {
                var kernel = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(kernelSize, kernelSize), new Point(1, 1));

                CvInvoke.MorphologyEx(image,
                                      image,
                                      MorphOp.Close,
                                      kernel,
                                      new Point(-1, -1),
                                      1,
                                      BorderType.Default,
                                      new MCvScalar());

                outImage = image.ToBitmap();
            }

            return outImage;
        }

        public static Bitmap WatershedSegmentation(Bitmap input)
        {
            Image<Bgr, Byte> image = input.ToImage<Bgr, byte>();
            using (UMat imageGray = new UMat())
            using (UMat imageClear = new UMat())
            using (UMat sureBg = new UMat())
            using (UMat sureFg = new UMat())
            using (UMat unknown = new UMat())
            using (UMat markers = new UMat())
            {

                //Konversi ke grayscale
                CvInvoke.CvtColor(image, imageGray, ColorConversion.Bgr2Gray);
                CvInvoke.Threshold(imageGray, imageClear, 0, 255, ThresholdType.Otsu);

                //Kernel
                var kernel = CvInvoke.GetStructuringElement(ElementShape.Ellipse, new Size(8, 8), new Point(-1, -1));

                //Removing noise
                CvInvoke.MorphologyEx(imageClear, imageClear, MorphOp.Open, kernel, new Point(-1, -1), 1, BorderType.Default, new MCvScalar());
                CvInvoke.MorphologyEx(imageClear, imageClear, MorphOp.Close, kernel, new Point(-1, -1), 1, BorderType.Default, new MCvScalar());

                //Get sure background
                CvInvoke.MorphologyEx(imageClear, sureBg, MorphOp.Dilate, kernel, new Point(-1, -1), 1, BorderType.Default, new MCvScalar());

                //Get sure foreground
                CvInvoke.MorphologyEx(imageClear, sureFg, MorphOp.Erode, kernel, new Point(-1, -1), 1, BorderType.Default, new MCvScalar());

                //Get unknown
                CvInvoke.Subtract(sureBg, sureFg, unknown);

                CvInvoke.Imshow("SureFg", sureFg);
                CvInvoke.Imshow("SureBg", sureBg);
                CvInvoke.Imshow("Unknown", unknown);

                //Get Markers
                CvInvoke.ConnectedComponents(sureFg, markers);

                Matrix<int> markersMat = new Matrix<int>(markers.Size);
                markers.CopyTo(markersMat);
                markersMat += 1;

                Matrix<byte> unknownMat = new Matrix<byte>(unknown.Size);
                unknown.CopyTo(unknownMat);

                for (int x = 0; x < unknownMat.Rows; x++)
                    for (int y = 0; y < unknownMat.Cols; y++)
                        if (unknownMat[x, y] == 255) markersMat[x, y] = 0;

                CvInvoke.Watershed(image, markersMat);

                for (int x = 0; x < markersMat.Rows; x++)
                    for (int y = 0; y < markersMat.Cols; y++)
                        if (markersMat[x, y] == -1) image[x, y] = new Bgr(0, 0, 255);

                input = image.ToBitmap<Bgr, Byte>();
            }

            return input;
        }

        public static Bitmap AdaptMeanThreshold(Bitmap input, double c)
        {
            var image = input.ToImage<Bgr, byte>();
            UMat imageGray = new UMat();
            CvInvoke.CvtColor(image, imageGray, ColorConversion.Bgr2Gray);
            CvInvoke.AdaptiveThreshold(imageGray, imageGray, 255, AdaptiveThresholdType.GaussianC, ThresholdType.Binary, 3, c);

            var outImage = imageGray.ToBitmap();
            image.Dispose();

            return outImage;
        }

        public static Bitmap KMeansSegmentationHsv(
            Bitmap image,
            int k = 3)
        {
            Image<Bgr, Byte> imageBgr = image.ToImage<Bgr, Byte>();
            Image<Hsv, Byte> imageHsv = new Image<Hsv, byte>(imageBgr.Width, imageBgr.Height);
            CvInvoke.CvtColor(imageBgr, imageHsv, ColorConversion.Bgr2Hsv);

            Matrix<float> data = new Matrix<float>(image.Width * image.Height, 3);
            var labels = new VectorOfInt();

            int matrixIndex = 0;
            for (int x = 0; x < imageHsv.Width; x++)
            {
                for (int y = 0; y < imageHsv.Height; y++)
                {
                    var pixel = imageHsv[y, x];
                    data[matrixIndex, 0] = (float)pixel.Hue;
                    data[matrixIndex, 1] = (float)pixel.Satuation;
                    data[matrixIndex, 2] = (float)pixel.Value;
                    matrixIndex++;
                }
            }

            CvInvoke.Kmeans(data, k, labels, new MCvTermCriteria(Math.Pow(10, -10)), 8, KMeansInitType.PPCenters);

            //Pelabelan
            var random = new Random();
            var colorLabel = new List<Color>();
            for (int i = 0; i < k; i++)
            {
                var randR = random.Next(0, 256);
                var randG = random.Next(0, 256);
                var randB = random.Next(0, 256);
                colorLabel.Add(Color.FromArgb(randR, randG, randB));
            }

            var outImage = new Bitmap(image.Width, image.Height);
            var labelsIndex = 0;
            for (int x = 0; x < imageHsv.Width; x++)
            {
                for (int y = 0; y < imageHsv.Height; y++)
                {
                    outImage.SetPixel(x, y, colorLabel[labels[labelsIndex]]);
                    labelsIndex++;
                }
            }

            return outImage;
        }

        public static Bitmap SharpeningLaplacian(Bitmap original)
        {
            Bitmap outImage = new Bitmap(original.Width, original.Height);
            using (Image<Bgr, byte> image = original.ToImage<Bgr, byte>())
            using (UMat laplacian = new UMat(image.Size, DepthType.Cv32F, 3))
            using (UMat img32F = new UMat())
            using (UMat sharp = new UMat())
            {
                var kernel = new ConvolutionKernelF(new float[,]
                {
                    { -1, -1, -1 },
                    { -1, 8, -1 },
                    { -1, 1, -1 }
                });

                CvInvoke.Filter2D(image, laplacian, kernel, new Point(-1, -1));

                image.Mat.ConvertTo(img32F, DepthType.Cv32F);

                CvInvoke.AbsDiff(img32F, laplacian, sharp);

                sharp.ConvertTo(sharp, DepthType.Cv8U);

                outImage = sharp.ToBitmap();
            }

            return outImage;
        }

        public static Bitmap GaussianBlur(Bitmap original)
        {
            Bitmap outImage = new Bitmap(original.Width, original.Height);
            using (Image<Bgr, byte> image = original.ToImage<Bgr, byte>())
            using (UMat blurred = new UMat())
            {
                CvInvoke.GaussianBlur(image, blurred, new Size(5, 5), 0);
                outImage = blurred.ToBitmap();
            }

            return outImage;
        }

        public static Bitmap ColorCorrection(Bitmap input, int deltaR, int deltaG, int deltaB)
        {
            Bitmap outImage = new Bitmap(input.Width, input.Height);
            using (Image<Bgr, byte> image = input.ToImage<Bgr, byte>().Clone())
            using (Image<Bgr, byte> result = new Image<Bgr, byte>(image.Width, image.Height))
            {
                for (int x = 0; x < image.Width; x++)
                {
                    for (int y = 0; y < image.Height; y++)
                    {
                        var pixel = image[y, x];
                        var newPixel = new Bgr(
                            (int)(((pixel.Blue - deltaB) * (255 - deltaB)) / 255),
                            (int)(((pixel.Green - deltaG) * (255 - deltaG)) / 255),
                            (int)(((pixel.Red - deltaR) * (255 - deltaR)) / 255)
                        );
                        result[y, x] = newPixel;
                    }
                }

                outImage = result.ToBitmap();
            }

            return outImage;
        }

        public static Bitmap GreenSegmentation(Bitmap original)
        {
            Bitmap outImage = new Bitmap(original.Width, original.Height);

            using (Image<Bgr, byte> image = original.ToImage<Bgr, byte>())
            using (UMat imageHsv = new UMat())
            using (UMat mask = new UMat())
            using (UMat result = new UMat())
            {
                //CvInvoke.GaussianBlur(image, image, new Size(5, 5), 0);
                CvInvoke.CvtColor(image, imageHsv, ColorConversion.Bgr2Hsv);

                CvInvoke.InRange(imageHsv,
                                 new ScalarArray(new MCvScalar(35, 30, 30)),
                                 new ScalarArray(new MCvScalar(80, 255, 160)),
                                 mask);



                var kernel = CvInvoke.GetStructuringElement(ElementShape.Ellipse, new Size(5, 5), new Point(-1, -1));
                //CvInvoke.MorphologyEx(mask, mask, MorphOp.Close, kernel, new Point(-1, -1), 2, BorderType.Default, new MCvScalar());
                CvInvoke.BitwiseNot(mask, mask);

                CvInvoke.BitwiseAnd(image, image, result, mask);

                outImage = result.ToBitmap();
            }

            return outImage;
        }

        public static Bitmap Sobel(Bitmap original)
        {
            Bitmap outImage = new Bitmap(original.Width, original.Height);

            using (Image<Bgr, byte> image = original.ToImage<Bgr, byte>())
            using (var imageGray = image.Convert<Gray, byte>())
            using (UMat blur = new UMat())
            using (UMat sobel = new UMat())
            using (UMat result = new UMat())
            {
                CvInvoke.GaussianBlur(imageGray, blur, new Size(3, 3), 0);
                CvInvoke.Sobel(blur, sobel, DepthType.Cv64F, 1, 1);
                CvInvoke.ConvertScaleAbs(sobel, result, 1, 0);
                CvInvoke.Threshold(result, result, 0, 255, ThresholdType.Otsu);

                outImage = result.ToBitmap();
            }

            return outImage;
        }

        public static Bitmap Canny(Bitmap original)
        {
            Bitmap outImage = new Bitmap(original.Width, original.Height);

            using (Image<Bgr, byte> image = original.ToImage<Bgr, byte>())
            using (UMat blur = new UMat())
            using (UMat canny = new UMat())
            {
                CvInvoke.GaussianBlur(image, blur, new Size(3, 3), 0);
                CvInvoke.Canny(blur, canny, 70, 140);

                outImage = canny.ToBitmap();
            }

            return outImage;
        }

        public static Dictionary<string, Bitmap> PreProcessing(Bitmap original)
        {
            var dictionary = new Dictionary<string, Bitmap>();

            var outImage = OtsuCv(original);
            dictionary.Add("Otsu", outImage);

            outImage = Opening(outImage, 5);
            dictionary.Add("Opening", outImage);

            outImage = Closing(outImage, 5);
            dictionary.Add("Closing", outImage);

            var greenMask = GreenSegmentation(original);
            dictionary.Add("Green Mask", greenMask);

            var edgeMask = Canny(original);
            dictionary.Add("Edge Mask", edgeMask);

            using (var image = outImage.ToImage<Gray, byte>())
            using (var green = greenMask.ToImage<Gray, byte>())
            using (var edge = edgeMask.ToImage<Gray, byte>())
            {
                CvInvoke.BitwiseNot(green, green);

                CvInvoke.BitwiseAnd(image, green, image);
                dictionary.Add("Closing AND Green Mask", image.ToBitmap());

                CvInvoke.Dilate(edge, edge, null, new Point(-1, -1), 1, BorderType.Default, new MCvScalar());
                CvInvoke.BitwiseNot(edge, edge, green);

                CvInvoke.BitwiseAnd(image, edge, image);
                dictionary.Add("Closing AND Edge Mask", image.ToBitmap());

                var kernel = CvInvoke.GetStructuringElement(ElementShape.Cross, new Size(3, 3), new Point(-1, -1));
                CvInvoke.MorphologyEx(image, image, MorphOp.Close, kernel, new Point(-1, -1), 1, BorderType.Default, new MCvScalar());
                dictionary.Add("Closing2", image.ToBitmap());

                outImage = image.ToBitmap();
            }

            dictionary.Add("Result", outImage);

            return dictionary;
        }

        public static Bitmap HSVColorSegmentation(Bitmap input, int[] low, int[] high)
        {
            Bitmap outImage = new Bitmap(input.Width, input.Height);

            using (var image = input.ToImage<Bgr, byte>().Clone())
            using (var imageHsv = image.Convert<Hsv, int>())
            using (var mask = new UMat())
            using (var result = new UMat())
            {
                var lowHue = new ScalarArray(new MCvScalar(low[0], low[1], low[2]));
                var highHue = new ScalarArray(new MCvScalar(high[0], high[1], high[2]));
                CvInvoke.InRange(imageHsv, lowHue, highHue, mask);
                CvInvoke.BitwiseAnd(image, image, result, mask);
                outImage = result.ToBitmap();
            }

            return outImage;
        }

        public static Bitmap HSVColorSegmentation(Bitmap input, int hue, int hueRange)
        {
            var lowHue = new int[] { Math.Max(hue - hueRange, 0), 30, 30 };
            var highHue = new int[] { Math.Min(hue + hueRange, 255), 255, 255 };
            Bitmap outImage = HSVColorSegmentation(input, lowHue, highHue);

            return outImage;
        }

        public static (Bitmap, List<List<Point>>) BlobDetection(Bitmap input, int minJmlPiksel)
        {
            var outImage = new Bitmap(input.Width, input.Height);
            var listBlob = new List<List<Point>>();

            using (var image = input.ToImage<Bgr, byte>())
            using (var imageGray = image.Convert<Gray, byte>())
            using (var markers = new Mat())
            using (var result = new Matrix<int>(image.Rows, image.Cols))
            using (var resultImage = new Image<Bgr, byte>(image.Width, image.Height))
            {
                var jumlah = CvInvoke.ConnectedComponents(imageGray, markers, LineType.FourConnected) - 1;
                var blob = new List<List<Point>>();

                for (int i = 0; i < jumlah; i++)
                    blob.Add(new List<Point>());

                markers.CopyTo(result);
                markers.ConvertTo(markers, DepthType.Cv8U, 100);

                for (int i = 0; i < result.Rows; i++)
                {
                    for (int j = 0; j < result.Cols; j++)
                    {
                        var label = result[i, j];

                        if (label != 0)
                            blob[label - 1].Add(new Point(j, i));
                    }
                }

                for (int i = 0; i < jumlah; i++)
                {
                    if (blob[i].Count >= minJmlPiksel)
                        listBlob.Add(blob[i]);
                }

                //Pelabelan
                var random = new Random();
                var colorLabel = new List<Color>();
                for (int i = 0; i < listBlob.Count; i++)
                {
                    var randR = random.Next(0, 256);
                    var randG = random.Next(0, 256);
                    var randB = random.Next(0, 256);
                    colorLabel.Add(Color.FromArgb(randR, randG, randB));
                }

                for (int i = 0; i < listBlob.Count; i++)
                    foreach (var point in listBlob[i])
                    {
                        resultImage[point.Y, point.X] = new Bgr(colorLabel[i].B, colorLabel[i].G, colorLabel[i].R);
                    }

                outImage = resultImage.ToBitmap();
            }

            return (outImage, listBlob);
        }

        //Skala : luas/piksel
        public static Bitmap HitungLuas(Bitmap original, List<List<Point>> blobs, double skala)
        {
            var outImage = new Bitmap(original.Width, original.Height);

            List<double> luas = new List<double>();
            List<Point> centers = new List<Point>();

            foreach (var blob in blobs)
            {
                luas.Add((double)blob.Count * skala);

                var minPoint = new Point(int.MaxValue, int.MaxValue);
                var maxPoint = new Point(0, 0);

                //foreach (var point in blob)
                //{
                //    if (point.X < minPoint.X)
                //        minPoint.X = point.X;

                //    if (point.X > maxPoint.X)
                //        maxPoint.X = point.X;

                //    if (point.Y < minPoint.Y)
                //        minPoint.Y = point.Y;

                //    if (point.Y > maxPoint.Y)
                //        maxPoint.Y = point.Y;
                //}

                //var width = minPoint.X - maxPoint.X;
                //var height = maxPoint.Y - maxPoint.Y;

                //var center = new Point(minPoint.X + width / 2, minPoint.Y + height / 2);

                var center = new Point(0, 0);

                foreach (var point in blob)
                {
                    center.X += point.X;    
                    center.Y += point.Y;    
                }

                center.X = center.X / blob.Count;
                center.Y = center.Y / blob.Count;

                centers.Add(center);
            }

            var blobImage = new Bitmap(original.Width, original.Height);

            using (var g = Graphics.FromImage(blobImage))
            {
                g.Clear(Color.FromArgb(0, 0, 0));
            }

            foreach (var blob in blobs)
                foreach (var point in blob)
                    blobImage.SetPixel(point.X, point.Y, Color.FromArgb(255, 255, 255));

            using (var blobImageGray = blobImage.ToImage<Gray, byte>())
            using (var countur = new Matrix<byte>(blobImageGray.Rows, blobImageGray.Cols))
            using (var result = original.ToImage<Bgr, byte>())
            {
                CvInvoke.Canny(blobImageGray, countur, 60, 90);

                for (var x = 0; x < countur.Cols; x++)
                    for(var y = 0; y < countur.Rows; y++)
                    {
                        if (countur[y, x] > 0)
                            result[y, x] = new Bgr(255, 0, 0);
                    }

                for (var i = 0; i < centers.Count; i++)
                {
                    var origin = centers[i];

                    if((result.Width - origin.X) <= 200)
                    {
                        origin.X -= 230;
                    }

                    CvInvoke.PutText(result, $"Luas : {(int)luas[i]} m2", origin, FontFace.HersheyPlain, 2, new MCvScalar(0, 0, 0), 3);
                }
                
                outImage = result.ToBitmap();
            }
            
            return outImage;
        }
    }
}