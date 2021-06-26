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
        private double startX;
        private double startY;
        private UIElement container;
        public PuzzlePiece(BitmapImage pieceBitmap, BitmapImage maskBitmap, BitmapImage borderBitmap)
        {
            InitializeComponent();

            borderImage.Source = borderBitmap;
            puzzleImage.Source = pieceBitmap;
            maskImage.ImageSource = maskBitmap;

            container = VisualTreeHelper.GetParent(this) as UIElement;
        }

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // when the mouse is down, get the position within the current control. (so the control top/left doesn't move to the mouse position)
            _positionInBlock = Mouse.GetPosition(container);
            startX = Canvas.GetLeft(this);
            startY = Canvas.GetTop(this);

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
                Canvas.SetLeft(this, startX + (mousePosition.X - _positionInBlock.X));
                Canvas.SetTop(this, startY + (mousePosition.Y - _positionInBlock.Y));
            }
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // release this control.
            ReleaseMouseCapture();
        }
    }
}
