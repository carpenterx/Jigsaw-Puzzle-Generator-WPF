﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Jigsaw_Puzzle_Generator_WPF.Controls
{
    /// <summary>
    /// Interaction logic for PuzzlePiece.xaml
    /// </summary>
    public partial class PuzzlePiece : UserControl
    {
        private Point _positionInBlock;
        public PuzzlePiece()
        {
            InitializeComponent();
        }

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // when the mouse is down, get the position within the current control. (so the control top/left doesn't move to the mouse position)
            _positionInBlock = Mouse.GetPosition(this);

            // capture the mouse (so the mouse move events are still triggered (even when the mouse is not above the control)
            this.CaptureMouse();
        }

        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            // if the mouse is captured. you are moving it. (there is your 'real' boolean)
            if (this.IsMouseCaptured)
            {
                // get the parent container
                var container = VisualTreeHelper.GetParent(this) as UIElement;

                // get the position within the container
                var mousePosition = e.GetPosition(container);

                // move the usercontrol.
                this.RenderTransform = new TranslateTransform(mousePosition.X - _positionInBlock.X, mousePosition.Y - _positionInBlock.Y);
            }
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // release this control.
            this.ReleaseMouseCapture();
        }
    }
}
