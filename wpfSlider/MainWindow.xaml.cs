using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Resources;
using System.IO;
using Microsoft.Win32;

namespace wpfSlider
{

    public partial class MainWindow : Window
    {
        private bool _isDown = false;
        private Point _startPoint = new Point(0.0, 0.0);
        private int imgIndex = 0;

        Cursor OpenHandCursor { get; set; }
        Cursor ClosedHandCursor { get; set; }

        public List<Image> imgList { get; set; } = new List<Image>();
        public MainWindow()
        {
            InitializeComponent();

            OpenHandCursor = new Cursor((Application.GetResourceStream(new Uri(Properties.Resources.openHand, UriKind.RelativeOrAbsolute))).Stream);
            ClosedHandCursor = new Cursor((Application.GetResourceStream(new Uri(Properties.Resources.closedHand, UriKind.RelativeOrAbsolute))).Stream);

            foreach (string s in Resources.Keys)
            {
                imgList.Add(Resources[s] as Image);
            }

            PreviewMouseDown += MainWindow_PreviewMouseDown;
            PreviewMouseUp += MainWindow_PreviewMouseUp;
            PreviewMouseMove += MainWindow_PreviewMouseMove;

        }

        private void MainWindow_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_isDown)
            {
                Border b = LogicalTreeHelper.FindLogicalNode(this, "bBorder") as Border;
                Image img0 = (b.Child as Image);
                double d = (Mouse.GetPosition(this).X - _startPoint.X);
                if ((Math.Abs(d) > 0.5 * SystemParameters.MinimumHorizontalDragDistance) && (Math.Abs(d) < 5.0 * SystemParameters.MinimumHorizontalDragDistance))
                {
                    img0.ForceCursor = true;
                    img0.Cursor = ClosedHandCursor;
                }
                else if (((Math.Abs(d) >= 5.0 * SystemParameters.MinimumHorizontalDragDistance)))
                {
                    if (d > 0.0)
                    {
                        imgIndex = (imgIndex == (imgList.Count - 1)) ? 0 : imgIndex + 1;
                    }
                    else
                    {
                        imgIndex = (imgIndex == 0) ? (imgList.Count - 1) : imgIndex - 1;
                    }

                    img0.ForceCursor = false;
                    img0.Cursor = null;

                    Image img1 = imgList[imgIndex];
                    b.Child = img1;

                    _startPoint = e.GetPosition(this);

                    _isDown = false;
                }
            }
            return;
        }

        private void MainWindow_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Border b = LogicalTreeHelper.FindLogicalNode(this, "bBorder") as Border;

            Image img = b.Child as Image;
            img.Cursor = null;
            img.ForceCursor = false;

            _startPoint = e.GetPosition(this);
            _isDown = false;

            return;
        }

        private void MainWindow_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

            if (!(e.OriginalSource is System.Windows.Controls.Image)) return;

            Border b = LogicalTreeHelper.FindLogicalNode(this, "bBorder") as Border;

            Image img = b.Child as Image;
            img.ForceCursor = true;

            img.Cursor = OpenHandCursor;
            e.Handled = true;
            _startPoint = Mouse.GetPosition(this);
            _isDown = true;

            return;
        }

    }
}
