using Jigsaw_Puzzle_Generator_WPF.Controls;
using Microsoft.Win32;
using Svg;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Net.WebRequestMethods;
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
        //private Bitmap maskBitmap;

        private int correctPieces;
        private int totalPieces;

        private List<PuzzlePiece> puzzlePieces;

        private int paddedWidth;
        private int paddedHeight;
        private int pieceSize;

        private string pathData1 = "M 400 200 v -120 h -120 c 0 22 40 80 -40 80 s -40 -58 -40 -80 h -120 v 120 c -22 0 -80 -40 -80 40 s 58 40 80 40 v 120 h 120 c 0 -22 -40 -80 40 -80 s 40 58 40 80 h 120 v -120 c 22 0 80 40 80 -40 s -58 -40 -80 -40 Z";

        private string pathData2 = "M 400 200 v -120 h -120 c 0 -22 40 -80 -40 -80 s -40 58 -40 80 h -120 v 120 c 22 0 80 -40 80 40 s -58 40 -80 40 v 120 h 120 c 0 22 -40 80 40 80 s 40 -58 40 -80 h 120 v -120 c -22 0 -80 40 -80 -40 s 58 -40 80 -40 Z";

        Region region1;
        Region region2;

        private Random random = new();

        public MainWindow()
        {
            InitializeComponent();

            ShowImage(IMAGE_PATH);

            borderBitmap = new Bitmap(BORDER_PATH);
            borderBitmap.SetResolution(96, 96);

            //maskBitmap = new Bitmap(MASK_PATH);
            //maskBitmap.SetResolution(96, 96);

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
            puzzlePieces = new();
            int width = b.Width;
            int height = b.Height;
            
            pieceSize = 480;
            int pieceCenter = 320;
            int padding = (pieceSize - pieceCenter) / 2;
            paddedWidth = padding + width + padding;
            paddedHeight = padding + height + padding;
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
            BuildRegions();
            int count = 0;
            for (int i = 0; i < hPieceCount; i++)
            {
                for (int j = 0; j < vPieceCount; j++)
                {
                    int xOffset = pieceCenter * i;
                    int yOffset = pieceCenter * j;
                    Bitmap bitmap = paddedBitmap.Clone(new Rectangle(xOffset, yOffset, pieceSize, pieceSize), paddedBitmap.PixelFormat);

                    

                    Graphics g2 = Graphics.FromImage(bitmap);

                    if (count % 2 == 1)
                    {
                        g2.ExcludeClip(region1);
                    }
                    else
                    {
                        g2.ExcludeClip(region2);
                    }
                    
                    g2.Clear(Color.Transparent);
                    count++;

                    BitmapImage croppedBitmapImage = ConvertToBitmapImage(bitmap);
                    borderBitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    BitmapImage borderBitmapImage = ConvertToBitmapImage(borderBitmap);
                    //maskBitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    //BitmapImage maskBitmapImage = ConvertToBitmapImage(maskBitmap);
                    PuzzlePiece puzzlePiece = new PuzzlePiece(croppedBitmapImage,borderBitmapImage, xOffset - padding, yOffset - padding, pieceSize);

                    //Canvas.SetLeft(puzzlePiece, xOffset - padding);
                    //Canvas.SetLeft(puzzlePiece, 0);
                    Canvas.SetLeft(puzzlePiece, random.Next(paddedWidth - pieceSize));
                    //Canvas.SetTop(puzzlePiece, yOffset - padding);
                    //Canvas.SetTop(puzzlePiece, 0);
                    Canvas.SetTop(puzzlePiece, random.Next(paddedHeight - pieceSize));
                    puzzlePiece.SnapEventHandler += OnPieceSnap;
                    puzzlePieces.Add(puzzlePiece);
                    puzzleCanvas.Children.Add(puzzlePiece);
                }
            }
        }

        private void BuildRegions()
        {
            GraphicsPath graphicsPath1 = new();

            foreach (var segment in SvgPathBuilder.Parse(pathData1))
                segment.AddToPath(graphicsPath1);

            region1 = new Region(graphicsPath1);

            GraphicsPath graphicsPath2 = new();

            foreach (var segment in SvgPathBuilder.Parse(pathData2))
                segment.AddToPath(graphicsPath2);

            region2 = new Region(graphicsPath2);
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

        private void ResetPuzzleClick(object sender, RoutedEventArgs e)
        {
            if (puzzlePieces.Count > 0)
            {
                foreach (PuzzlePiece puzzlePiece in puzzlePieces)
                {
                    Canvas.SetLeft(puzzlePiece, random.Next(paddedWidth - pieceSize));
                    Canvas.SetTop(puzzlePiece, random.Next(paddedHeight - pieceSize));
                    puzzlePiece.Reactivate();
                    puzzlePiece.SnapEventHandler -= OnPieceSnap;
                    puzzlePiece.SnapEventHandler += OnPieceSnap;
                }

                correctPieces = 0;
                UpdateProgress();
            }
        }
    }
}
