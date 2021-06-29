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
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Image = System.Windows.Controls.Image;
using Rectangle = System.Drawing.Rectangle;

namespace Jigsaw_Puzzle_Generator_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string IMAGE_NAME = "pexels-julia-volk-5273517.jpg";
        private const string BORDER_CORNER_NAME = "puzzle border corner.png";
        private const string BORDER_CURVE_NAME = "puzzle border curved.png";
        private const string BORDER_INSIDE_NAME = "puzzle border inside.png";
        private const string BORDER_LINE_NAME = "puzzle border straight.png";
        private const string SOUND_NAME = "click2.wav";
        private SoundPlayer soundPlayer;
        //private Bitmap b;
        private Bitmap borderBitmap;
        private Bitmap rotatedBorderBitmap;
        private Bitmap currentBorderBitmap;

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

        //private string pathData1 = "M 400 200 v -120 h -120 c 0 22 40 78 -40 78 s -40 -58 -40 -78 h -120 v 120 c -22 0 -78 -40 -78 40 s 58 40 78 40 v 120 h 120 c 0 -22 -40 -78 40 -78 s 40 58 40 78 h 120 v -120 c 22 0 78 40 78 -40 s -58 -40 -78 -40 Z";

        private string pathData1 = "M 200 100 v -60 h -60 c 0 11 20 39 -20 39 s -20 -29 -20 -39 h -60 v 60 c -11 0 -39 -20 -39 20 s 29 20 39 20 v 60 h 60 c 0 -11 -20 -39 20 -39 s 20 29 20 39 h 60 v -60 c 11 0 39 20 39 -20 s -29 -20 -39 -20 Z -78 -40 Z";

        //private string pathData2 = "M 400 200 v -120 h -120 c 0 -22 40 -78 -40 -78 s -40 58 -40 78 h -120 v 120 c 22 0 78 -40 78 40 s -58 40 -78 40 v 120 h 120 c 0 22 -40 78 40 78 s 40 -58 40 -78 h 120 v -120 c -22 0 -78 40 -78 -40 s 58 -40 78 -40 Z";

        private string pathData2 = "M 200 100 v -60 h -60 c 0 -11 20 -39 -20 -39 s -20 29 -20 39 h -60 v 60 c 11 0 39 -20 39 20 s -29 20 -39 20 v 60 h 60 c 0 11 -20 39 20 39 s 20 -29 20 -39 h 60 v -60 c -11 0 -39 20 -39 -20 s 29 -20 39 -20 Z";

        Region region1;
        Region region2;

        private Random random = new();

        private static readonly string APP_RESOURCES_NAMESPACE = "Jigsaw_Puzzle_Generator_WPF.Resources.";

        public MainWindow()
        {
            InitializeComponent();

            soundPlayer = new SoundPlayer(GetResourceStream(SOUND_NAME));

            //borderCornerBitmap = new Bitmap(BORDER_CORNER_PATH);
            borderCornerBitmap = new Bitmap(GetResourceStream(BORDER_CORNER_NAME));
            borderCornerBitmap.SetResolution(96, 96);

            borderCurveBitmap = new Bitmap(GetResourceStream(BORDER_CURVE_NAME));
            borderCurveBitmap.SetResolution(96, 96);

            borderInsideBitmap = new Bitmap(GetResourceStream(BORDER_INSIDE_NAME));
            borderInsideBitmap.SetResolution(96, 96);

            borderLineBitmap = new Bitmap(GetResourceStream(BORDER_LINE_NAME));
            borderLineBitmap.SetResolution(96, 96);

            BuildBorderBitmap();

            soundPlayer.Load();

            GeneratePieces(GetBitmap(GetResourceStream(IMAGE_NAME)));
        }

        private Stream GetResourceStream(string resourceName)
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream(APP_RESOURCES_NAMESPACE + resourceName);
        }

        private Bitmap GetBitmap(string filePath)
        {
            return new Bitmap(filePath);
        }

        private Bitmap GetBitmap(Stream stream)
        {
            return new Bitmap(stream);
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

            rotatedBorderBitmap = borderBitmap.Clone(new Rectangle(0, 0, borderBitmap.Width, borderBitmap.Height), borderBitmap.PixelFormat);
            rotatedBorderBitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
        }

        private void BrowseToImage(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".jpg"; // Default file extension
            dlg.Filter = "Image Files(*.Bmp;*.Jpg;*.Png)|*.Bmp;*.Jpg;*.Png";

            piecesTxt.Text = "";

            if (dlg.ShowDialog() == true)
            {
                GeneratePieces(GetBitmap(dlg.FileName));
            }
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

        /*private void StartClick(object sender, RoutedEventArgs e)
        {
            GeneratePieces();
        }*/

        private void GeneratePieces(Bitmap b)
        {
            //Bitmap b = new Bitmap(imagePath);
            b.SetResolution(96, 96);
           
            puzzlePieces = new();
            int width = b.Width;
            int height = b.Height;
            
            pieceSize = 240;
            int pieceCenter = 160;
            
            int hPieceCount = width / pieceCenter;
            int vPieceCount = height / pieceCenter;
            if (width % pieceCenter != 0)
            {
                hPieceCount++;
            }
            if (height % pieceCenter != 0)
            {
                vPieceCount++;
            }

            correctPieces = 0;
            totalPieces = hPieceCount * vPieceCount;
            UpdateProgress();

            int basePadding = (pieceSize - pieceCenter) / 2;
            int leftPadding = ((hPieceCount * pieceCenter) - width) / 2;
            int rightPadding = (hPieceCount * pieceCenter) - width - leftPadding;
            int topPadding = ((vPieceCount * pieceCenter) - height) / 2;
            int bottomPadding = (vPieceCount * pieceCenter) - height - topPadding;
            paddedWidth = basePadding + leftPadding + width + rightPadding + basePadding + pieceSize + pieceSize;
            paddedHeight = basePadding + topPadding + height + bottomPadding + basePadding;

            Bitmap paddedBitmap = new Bitmap(paddedWidth, paddedHeight);
            paddedBitmap.SetResolution(96, 96);
            Graphics g = Graphics.FromImage(paddedBitmap);
            g.Clear(Color.White);
            g.DrawImageUnscaled(b, basePadding + leftPadding, basePadding + topPadding);
            //paddedBitmap.Save(@"C:\Users\jorda\Desktop\padded.png", ImageFormat.Png);

            BitmapImage paddedBitmapImage = ConvertToBitmapImage(paddedBitmap);
            puzzleCanvas.Children.Clear();
            puzzleCanvas.Width = paddedBitmapImage.PixelWidth;
            puzzleCanvas.Height = paddedBitmapImage.PixelHeight;

            //<Image x:Name="image" Stretch="None" Opacity="0.7"/>
            Image image = new();
            image.Stretch = System.Windows.Media.Stretch.None;
            image.Opacity = 0.7;
            image.Source = paddedBitmapImage;
            image.Width = paddedBitmapImage.PixelWidth;
            image.Height = paddedBitmapImage.PixelHeight;
            puzzleCanvas.Children.Add(image);

            BuildRegions();
            int count = 1;
            currentBorderBitmap = borderBitmap;
            for (int j = 0; j < vPieceCount; j++)
            {
                for (int i = 0; i < hPieceCount; i++)
                {
                    int xOffset = pieceCenter * i;
                    int yOffset = pieceCenter * j;
                    Bitmap bitmap = paddedBitmap.Clone(new Rectangle(xOffset, yOffset, pieceSize, pieceSize), paddedBitmap.PixelFormat);

                    Graphics g2 = Graphics.FromImage(bitmap);

                    if ((j + i) % 2 == 1)
                    {
                        g2.ExcludeClip(region1);
                        currentBorderBitmap = borderBitmap;
                    }
                    else
                    {
                        g2.ExcludeClip(region2);
                        currentBorderBitmap = rotatedBorderBitmap;
                    }
                    
                    g2.Clear(Color.Transparent);

                    BitmapImage croppedBitmapImage = ConvertToBitmapImage(bitmap);
                    BitmapImage borderBitmapImage = ConvertToBitmapImage(currentBorderBitmap);

                    PuzzlePiece puzzlePiece = new PuzzlePiece(croppedBitmapImage,borderBitmapImage, xOffset, yOffset, pieceSize, count, totalPieces + 1);

                    //Canvas.SetLeft(puzzlePiece, xOffset);
                    //Canvas.SetLeft(puzzlePiece, random.Next(paddedWidth - pieceSize));
                    Canvas.SetLeft(puzzlePiece, random.Next(paddedWidth - pieceSize - pieceSize, paddedWidth - pieceSize));
                    //Canvas.SetTop(puzzlePiece, yOffset);
                    Canvas.SetTop(puzzlePiece, random.Next(paddedHeight - pieceSize));
                    Canvas.SetZIndex(puzzlePiece, count);
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
            piecesTxt.Text = $"{correctPieces}/{totalPieces} pieces";
        }

        private void ResetPuzzleClick(object sender, RoutedEventArgs e)
        {
            if (puzzlePieces.Count > 0)
            {
                foreach (PuzzlePiece puzzlePiece in puzzlePieces)
                {
                    Canvas.SetLeft(puzzlePiece, random.Next(paddedWidth - pieceSize - pieceSize, paddedWidth - pieceSize));
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
