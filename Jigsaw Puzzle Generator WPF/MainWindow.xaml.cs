using Jigsaw_Puzzle_Generator_WPF.Controls;
using Microsoft.Win32;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Rectangle = System.Drawing.Rectangle;

namespace Jigsaw_Puzzle_Generator_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string IMAGE_PATH = @"C:\Users\jorda\Desktop\pexels-julia-volk-5273517.jpg";
        private Bitmap b;

        public MainWindow()
        {
            InitializeComponent();

            ShowImage(IMAGE_PATH);
        }

        private void BrowseToImage(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".jpg"; // Default file extension
            dlg.Filter = "JPG documents (.jpg)|*.jpg"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                ShowImage(dlg.FileName);
            }
        }

        private void ShowImage(string imagePath)
        {
            b = new Bitmap(imagePath);
            b.SetResolution(96, 96);
            BitmapImage bitmapImage = ConvertToBitmapImage(b);
            puzzleCanvas.Width = bitmapImage.PixelWidth;
            puzzleCanvas.Height = bitmapImage.PixelHeight;

            image.Source = bitmapImage;
            image.Width = bitmapImage.PixelWidth;
            image.Height = bitmapImage.PixelHeight;
        }

        private BitmapImage ConvertToBitmapImage(Bitmap bitmap)
        {
            MemoryStream memoryStream = new();
            bitmap.Save(memoryStream, ImageFormat.Png);
            BitmapImage bitmapImage = new();
            bitmapImage.BeginInit();
            memoryStream.Seek(0, SeekOrigin.Begin);
            bitmapImage.StreamSource = memoryStream;
            bitmapImage.EndInit();

            return bitmapImage;
        }

        private void AddPieceClick(object sender, RoutedEventArgs e)
        {
            Bitmap bitmap = b.Clone(new Rectangle(300, 150, 200, 400), b.PixelFormat);
            BitmapImage croppedBitmapImage = ConvertToBitmapImage(bitmap);
            PuzzlePiece puzzlePiece = new PuzzlePiece(croppedBitmapImage);

            Canvas.SetLeft(puzzlePiece, 300);
            Canvas.SetTop(puzzlePiece, 150);
            puzzleCanvas.Children.Add(puzzlePiece);
        }
    }
}
