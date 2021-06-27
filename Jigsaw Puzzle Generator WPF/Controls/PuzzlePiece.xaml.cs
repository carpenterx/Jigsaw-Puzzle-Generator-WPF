using System;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Jigsaw_Puzzle_Generator_WPF.Controls
{
    /// <summary>
    /// Interaction logic for PuzzlePiece.xaml
    /// </summary>
    public partial class PuzzlePiece : UserControl
    {
        private Point _positionInBlock;
        private double dragStartX;
        private double dragStartY;
        private double destinationX;
        private double destinationY;
        private double precision = 10;
        private UIElement container;
        SoundPlayer player;
        public PuzzlePiece(BitmapImage pieceBitmap, BitmapImage maskBitmap, BitmapImage borderBitmap, double x, double y, SoundPlayer soundPlayer, int pieceSize)
        {
            InitializeComponent();

            puzzleImage.Width = pieceSize;
            puzzleImage.Height = pieceSize;
            borderImage.Width = pieceSize;
            borderImage.Height = pieceSize;

            borderImage.Source = borderBitmap;
            puzzleImage.Source = pieceBitmap;
            maskImage.ImageSource = maskBitmap;

            destinationX = x;
            destinationY = y;

            player = soundPlayer;

            container = VisualTreeHelper.GetParent(this) as UIElement;
        }

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // when the mouse is down, get the position within the current control. (so the control top/left doesn't move to the mouse position)
            _positionInBlock = Mouse.GetPosition(container);
            dragStartX = Canvas.GetLeft(this);
            dragStartY = Canvas.GetTop(this);

            // capture the mouse (so the mouse move events are still triggered (even when the mouse is not above the control)
            CaptureMouse();
        }

        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            // if the mouse is captured. you are moving it. (there is your 'real' boolean)
            if (IsMouseCaptured)
            {
                // get the position within the container
                Point mousePosition = e.GetPosition(container);

                // move the usercontrol.
                //RenderTransform = new TranslateTransform(mousePosition.X - _positionInBlock.X, mousePosition.Y - _positionInBlock.Y);
                Canvas.SetLeft(this, dragStartX + (mousePosition.X - _positionInBlock.X));
                Canvas.SetTop(this, dragStartY + (mousePosition.Y - _positionInBlock.Y));
            }
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Point mousePosition = e.GetPosition(container);
            if (Math.Abs(dragStartX + (mousePosition.X - _positionInBlock.X) - destinationX) < precision && Math.Abs(dragStartY + (mousePosition.Y - _positionInBlock.Y) - destinationY) < precision)
            {
                Canvas.SetLeft(this, destinationX);
                Canvas.SetTop(this, destinationY);
                this.MouseDown -= UserControl_MouseDown;
                this.MouseUp -= UserControl_MouseUp;
                this.MouseMove -= UserControl_MouseMove;
                this.IsHitTestVisible = false;
                //borderImage.Visibility = Visibility.Hidden;
                borderImage.Opacity = 0.2;
                player.Play();
            }
            // release this control.
            ReleaseMouseCapture();
        }
    }
}
