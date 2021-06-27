using Jigsaw_Puzzle_Generator_WPF.Controls;
using Microsoft.Win32;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
        private const string BORDER_PATH = @"C:\Users\jorda\Desktop\puzzle border new.png";
        private const string MASK_PATH = @"C:\Users\jorda\Desktop\puzzle mask new.png";
        private const string SOUND_PATH = @"C:\Users\jorda\Desktop\click2.wav";
        private SoundPlayer soundPlayer = new SoundPlayer(SOUND_PATH);
        private Bitmap b;
        private Bitmap borderBitmap;
        private Bitmap maskBitmap;

        private int correctPieces;
        private int totalPieces;

        private Random random = new();

        public MainWindow()
        {
            InitializeComponent();

            ShowImage(IMAGE_PATH);

            borderBitmap = new Bitmap(BORDER_PATH);
            borderBitmap.SetResolution(96, 96);

            maskBitmap = new Bitmap(MASK_PATH);
            maskBitmap.SetResolution(96, 96);

            soundPlayer.Load();
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
            GeneratePieces();
        }

        private void GeneratePieces()
        {
            int width = b.Width;
            int height = b.Height;
            
            int pieceSize = 480;
            int pieceCenter = 320;
            int padding = (pieceSize - pieceCenter) / 2;
            int paddedWidth = padding + width + padding;
            int paddedHeight = padding + height + padding;
            int hPieceCount = width / pieceCenter;
            int vPieceCount = height / pieceCenter;

            correctPieces = 0;
            totalPieces = hPieceCount * vPieceCount;
            UpdateProgress();

            Bitmap paddedBitmap = new Bitmap(paddedWidth, paddedHeight);
            paddedBitmap.SetResolution(96, 96);
            Graphics g = Graphics.FromImage(paddedBitmap);
            g.Clear(Color.White);
            g.DrawImageUnscaled(b, padding, padding);
            //paddedBitmap.Save(@"C:\Users\jorda\Desktop\padded.png", ImageFormat.Png);
            
            for (int i = 0; i < hPieceCount; i++)
            {
                for (int j = 0; j < vPieceCount; j++)
                {
                    int xOffset = pieceCenter * i;
                    int yOffset = pieceCenter * j;
                    Bitmap bitmap = paddedBitmap.Clone(new Rectangle(xOffset, yOffset, pieceSize, pieceSize), paddedBitmap.PixelFormat);
                    BitmapImage croppedBitmapImage = ConvertToBitmapImage(bitmap);
                    borderBitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    BitmapImage borderBitmapImage = ConvertToBitmapImage(borderBitmap);
                    maskBitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    BitmapImage maskBitmapImage = ConvertToBitmapImage(maskBitmap);
                    PuzzlePiece puzzlePiece = new PuzzlePiece(croppedBitmapImage, maskBitmapImage,borderBitmapImage, xOffset - padding, yOffset - padding, pieceSize);

                    //Canvas.SetLeft(puzzlePiece, xOffset - padding);
                    //Canvas.SetLeft(puzzlePiece, 0);
                    Canvas.SetLeft(puzzlePiece, random.Next(paddedWidth - pieceSize));
                    //Canvas.SetTop(puzzlePiece, yOffset - padding);
                    //Canvas.SetTop(puzzlePiece, 0);
                    Canvas.SetTop(puzzlePiece, random.Next(paddedHeight - pieceSize));
                    puzzlePiece.SnapEventHandler += OnPieceSnap;
                    puzzleCanvas.Children.Add(puzzlePiece);
                }
            }
        }
        private void OnPieceSnap(object sender, bool e)
        {
            soundPlayer.Play();
            (sender as PuzzlePiece).SnapEventHandler -= OnPieceSnap;
            correctPieces++;
            UpdateProgress();
        }

        private void UpdateProgress()
        {
            piecesTxt.Text = $"{correctPieces} pieces out of {totalPieces}";
        }
    }
}
