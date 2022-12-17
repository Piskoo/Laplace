using System;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.IO;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Runtime.InteropServices;
using System.Diagnostics;
using LaplaceCSharp;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Laplace
{
    public partial class MainWindow : Window
    {

        [DllImport("C:\\Studia\\ja\\sem5\\Laplace\\x64\\Debug\\LaplaceASM.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void processImage(byte[] original, byte[] processed, int[] filter, int chunk, int stride);

        LaplaceC laplaceC = new LaplaceC();

        Bitmap originalImage;
        Bitmap processedImage;
        Stopwatch sw = new Stopwatch();

        int[][] FilterTypes;

        public MainWindow()
        {
            originalImage = new Bitmap(1, 1);
            processedImage = new Bitmap(1, 1);
            InitializeComponent();
            CreateFilters();
            testAction();
        }

        private void Laplacianfilter()
        {
            processedImage = (Bitmap)originalImage.Clone();
            var inputData = originalImage.LockBits(new Rectangle(0, 0, originalImage.Width, originalImage.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var outputData = processedImage.LockBits(new Rectangle(0, 0, originalImage.Width, originalImage.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            int height = originalImage.Height;
            int width = originalImage.Width;
            int stride = inputData.Stride;
            byte[] input = new byte[stride * height];
            byte[] output = new byte[stride * height];
            Marshal.Copy(inputData.Scan0, input, 0, stride * height);
            Marshal.Copy(outputData.Scan0, output, 0, stride * height);
            int bpp = 4;
            int[] filter = FilterTypes[FilterType.SelectedIndex];

            ParallelOptions options = new ParallelOptions { MaxDegreeOfParallelism = (int)ThreadSlider.Value };
            sw.Restart();
            //divine into smaller pieces
            var listOfChunks = new List<int>();
            for (int i = stride; i < stride * (height - 1); i += stride)//for full stride
            {
                for (int j = bpp; j != stride - bpp; j += bpp) // for 1px
                {
                    listOfChunks.Add(i + j);
                }
            }
            //var listOfPixels = new List<byte[]>();
            //for (int i = stride; i < stride * (height - 1); i += stride)
            //{
            //    for (int j = bpp; j != stride - bpp; j += bpp)
            //    {
            //        listOfPixels.Add(new byte[]
            //        {
            //            input[i+j-stride-bpp],
            //            input[i+j-stride],
            //            input[i+j-stride+bpp],
            //            input[i+j-bpp],
            //            input[i+j],
            //            input[i+j+bpp],
            //            input[i+j+stride-bpp],
            //            input[i+j+stride],
            //            input[i+j+stride+bpp]
            //        });
            //    }
            //}
            //processing
            if (CSharp.IsChecked != null && CSharp.IsChecked == true)
            {
                sw.Start();
                Parallel.ForEach(listOfChunks, options, chunk =>
                {
                    laplaceC.processImage(input, output, filter, stride, bpp, chunk);
                });
                sw.Stop();
            }
            //if (CSharp.IsChecked != null && CSharp.IsChecked == true)
            //{
            //    sw.Start();
            //    Parallel.ForEach(listOfPixels, options, pixel =>
            //    {
            //        laplaceC.processImage(pixel, output, filter);
            //    });
            //    sw.Stop();
            //}
            else if (ASM.IsChecked != null && ASM.IsChecked == true)
            {
                sw.Start();
                Parallel.ForEach(listOfChunks, options, chunk =>
                {
                    processImage(input, output, filter, chunk, stride);
                });
                sw.Stop();
            }
            Marshal.Copy(input, 0, inputData.Scan0, stride * height);
            Marshal.Copy(output, 0, outputData.Scan0, stride * height);
            originalImage.UnlockBits(inputData);
            processedImage.UnlockBits(outputData);
        }

        private void CreateFilters()
        {
            FilterTypes = new int[6][];
            FilterTypes[0] = new int[] { 0, -1, 0, -1, 4, -1, 0, -1, 0 };
            FilterTypes[1] = new int[] { -1, -1, -1, -1, 8, -1, -1, -1, -1 };
            FilterTypes[2] = new int[] { 1, -2, 1, -2, 4, -2, 1, -2, 1 };
            FilterTypes[3] = new int[] { -1, 0, -1, 0, 4, 0, -1, 0, -1 };
            FilterTypes[4] = new int[] { 0, -1, 0, 0, 2, 0, 0, -1, 0 };
            FilterTypes[5] = new int[] { 0, 0, 0, -1, 2, -1, 0, 0, 0 };
        }


        private void RunClick(object sender, RoutedEventArgs e)
        {
            
            processedImage = (Bitmap)originalImage.Clone();
            Laplacianfilter();
            Timer.Text = sw.Elapsed.TotalMilliseconds.ToString()+" ms";
            SaveButton.IsEnabled = true;
            ProcessedImage.Source = BitmapToImageSource(processedImage);

        }
        private void SaveClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG Files (*.png)|*.png|JPEG Files (*.jpeg)|*.jpeg|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            Nullable<bool> result = saveFileDialog.ShowDialog();
            if (result == true)
            {
                processedImage.Save(saveFileDialog.FileName);
            }
        }

        private void LoadClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (.jpg,.jpeg,.png)|*.jpg;*.jpeg;*.png|All Files (.)|*.";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            Nullable<bool> result = openFileDialog.ShowDialog();
            if (result == true)
            {
                string filename = openFileDialog.FileName;
                Bitmap newBitmap = (Bitmap)Bitmap.FromFile(filename);
                Bitmap bmp = newBitmap.Clone(new Rectangle(0, 0, newBitmap.Width, newBitmap.Height), PixelFormat.Format32bppArgb);
                originalImage = bmp;
                OriginalImage.Source = new BitmapImage(new Uri(filename));
                RunButton.IsEnabled = true;
            }
        }

        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                return bitmapimage;
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void testAction()
        {
            string filename = "C:\\Users\\Pisko\\Desktop\\anime.png";
            Bitmap newBitmap = (Bitmap)Bitmap.FromFile(filename);
            Bitmap bmp = newBitmap.Clone(new Rectangle(0, 0, newBitmap.Width, newBitmap.Height), PixelFormat.Format32bppArgb);
            originalImage = bmp;
            OriginalImage.Source = new BitmapImage(new Uri(filename));
            RunButton.IsEnabled = true;
            processedImage = (Bitmap)originalImage.Clone();
            Laplacianfilter();
            Timer.Text = sw.Elapsed.TotalMilliseconds.ToString() + " ms";
            SaveButton.IsEnabled = true;
            ProcessedImage.Source = BitmapToImageSource(processedImage);
        }
    }
}
