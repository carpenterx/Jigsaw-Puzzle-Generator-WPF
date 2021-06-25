using Jigsaw_Puzzle_Generator_WPF.Controls;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Jigsaw_Puzzle_Generator_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string IMAGE_PATH = @"C:\Users\jorda\Desktop\pexels-julia-volk-5273517.jpg";

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
            BitmapImage bitmapImage = new BitmapImage(new Uri(imagePath, UriKind.Absolute));
            puzzleCanvas.Width = bitmapImage.PixelWidth;
            puzzleCanvas.Height = bitmapImage.PixelHeight;

            image.Source = bitmapImage;
            image.Width = bitmapImage.PixelWidth;
            image.Height = bitmapImage.PixelHeight;
        }

        private void AddPieceClick(object sender, RoutedEventArgs e)
        {
            puzzleCanvas.Children.Add(new PuzzlePiece());
        }
    }
}
