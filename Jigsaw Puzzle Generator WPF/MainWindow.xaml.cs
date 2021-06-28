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
        //private const string BORDER_PATH = @"C:\Users\jorda\Desktop\puzzle border new.png";
        //private const string MASK_PATH = @"C:\Users\jorda\Desktop\puzzle mask new.png";
        private const string BORDER_CORNER_PATH = @"C:\Users\jorda\Desktop\puzzle border corner.png";
        private const string BORDER_CURVE_PATH = @"C:\Users\jorda\Desktop\puzzle border curved.png";
        private const string BORDER_INSIDE_PATH = @"C:\Users\jorda\Desktop\puzzle border inside.png";
        private const string BORDER_LINE_PATH = @"C:\Users\jorda\Desktop\puzzle border straight.png";
        private const string SOUND_PATH = @"C:\Users\jorda\Desktop\click2.wav";
        private SoundPlayer soundPlayer = new SoundPlayer(SOUND_PATH);
        private Bitmap b;
        private Bitmap borderBitmap;
        //private Bitmap maskBitmap;

        private Bitmap borderCornerBitmap;
        private Bitmap borderCurveBitmap;
        private Bitmap borderInsideBitmap;
        private Bitmap borderLineBitmap;

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

            //borderBitmap = new Bitmap(BORDER_PATH);
            //borderBitmap.SetResolution(96, 96);

            borderCornerBitmap = new Bitmap(BORDER_CORNER_PATH);
            borderCornerBitmap.SetResolution(96, 96);

            borderCurveBitmap = new Bitmap(BORDER_CURVE_PATH);
            borderCurveBitmap.SetResolution(96, 96);

            borderInsideBitmap = new Bitmap(BORDER_INSIDE_PATH);
            borderInsideBitmap.SetResolution(96, 96);

            borderLineBitmap = new Bitmap(BORDER_LINE_PATH);
            borderLineBitmap.SetResolution(96, 96);

            BuildBorderBitmap();

            //maskBitmap = new Bitmap(MASK_PATH);
            //maskBitmap.SetResolution(96, 96);

            soundPlayer.Load();
        }

        private void BuildBorderBitmap()
        {
            borderBitmap = new Bitmap(480, 480);
            borderBitmap.SetResolution(96, 96);
            Graphics g = Graphics.FromImage(borderBitmap);

            g.DrawImageUnscaled(borderCornerBitmap, 0, 0);

            g.DrawImageUnscaled(borderInsideBitmap, 165, 0);

            borderCornerBitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
            g.DrawImageUnscaled(borderCornerBitmap, 315, 0);

            borderCurveBitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
            g.DrawImageUnscaled(borderCurveBitmap, 315, 165);

            borderCornerBitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
            g.DrawImageUnscaled(borderCornerBitmap, 315, 315);

            borderInsideBitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
            g.DrawImageUnscaled(borderInsideBitmap, 165, 315);

            borderCornerBitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
            g.DrawImageUnscaled(borderCornerBitmap, 0, 315);

            borderCurveBitmap.RotateFlip(RotateFlipType.RotateNoneFlipX);
            g.DrawImageUnscaled(borderCurveBitmap, 0, 165);
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

                    BitmapImage croppedBitmapImage = ConvertToBitmapImage(bitmap);
                    borderBitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    BitmapImage borderBitmapImage = ConvertToBitmapImage(borderBitmap);
                    //maskBitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    //BitmapImage maskBitmapImage = ConvertToBitmapImage(maskBitmap);
                    PuzzlePiece puzzlePiece = new PuzzlePiece(croppedBitmapImage,borderBitmapImage, xOffset - padding, yOffset - padding, pieceSize, count + 1, totalPieces + 1);

                    //Canvas.SetLeft(puzzlePiece, xOffset - padding);
                    Canvas.SetLeft(puzzlePiece, random.Next(paddedWidth - pieceSize));
                    //Canvas.SetTop(puzzlePiece, yOffset - padding);
                    Canvas.SetTop(puzzlePiece, random.Next(paddedHeight - pieceSize));
                    Canvas.SetZIndex(puzzlePiece, count + 1);
                    puzzlePiece.SnapEventHandler += OnPieceSnap;
                    puzzlePieces.Add(puzzlePiece);
                    puzzleCanvas.Children.Add(puzzlePiece);

                    count++;
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
