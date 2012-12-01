using System;
using System.Collections.Generic;
using System.IO;
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
using auto;
using Path = System.Windows.Shapes.Path;

namespace Razmetka
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public static ImageFileGallery images = new ImageFileGallery("rats");

		private static WriteableBitmap _wb =
			BitmapFactory.ConvertToPbgra32Format(new WriteableBitmap(640, 480, 96, 96, PixelFormats.Bgra32, null));

		public MainWindow()
		{
			InitializeComponent();
			UpdateImage();
			mask.Source = _wb;
		}

		private void UpdateImage()
		{
			var logo = new BitmapImage();
			logo.BeginInit();
			logo.CacheOption = BitmapCacheOption.OnLoad;
			logo.UriSource = new Uri(images.GetUri(), UriKind.Relative);
			logo.EndInit();
			im.Source = logo;
			im.UpdateLayout();
		}

		private DrawMode _dm;
		private bool drawing = false;

		private void DrawRazmetka(Point clickPosition)
		{
			if (_dm == DrawMode.NotRat || _dm == DrawMode.Rat)
				_wb.FillEllipseCentered((int)clickPosition.X, (int)clickPosition.Y, 5, 5, _dm == DrawMode.Rat ? Color.FromArgb(255, 255, 0, 0) : Color.FromArgb(255, 0, 0, 255));
			else
				_wb.FillEllipseCentered((int)clickPosition.X, (int)clickPosition.Y, 5, 5, Color.FromArgb(0, 0, 0, 0));
		}

		private void Grid_PreviewMouseMove_1(object sender, MouseEventArgs e)
		{
			var clickPosition = e.GetPosition(Application.Current.MainWindow);
			clickPosition.X = clickPosition.X*(640/im.ActualWidth) ;
			clickPosition.Y = clickPosition.Y*(480/im.ActualHeight);
			if (drawing)
				DrawRazmetka(clickPosition);
		}

		private void Grid_PreviewMouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
		{
			drawing = true;
		}

		private void Grid_PreviewMouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
		{
			drawing = false;
		}

		private void Window_PreviewKeyDown_2(object sender, KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.D1:
					_dm = DrawMode.Rat;
					break;
				case Key.D2:
					_dm = DrawMode.NotRat;
					break;
				case Key.D3:
					_dm = DrawMode.Erase;
					break;
				case Key.Left:
					SaveMark();
					images.MoveLeft();
					break;
				case Key.Right:
					SaveMark();
					images.MoveRight();
					break;
			}
			UpdateImage();
		}

		private void SaveMark()
		{
			var filename = "marks" + images.GetUri() + ".png";
			using (FileStream stream5 = new FileStream(filename, FileMode.Create))
			{
				PngBitmapEncoder encoder5 = new PngBitmapEncoder();
				encoder5.Frames.Add(BitmapFrame.Create(_wb));
				encoder5.Save(stream5);
				stream5.Close();
			}
			_wb.Clear();
		}

		private void Grid_PreviewMouseDown_1(object sender, MouseButtonEventArgs e)
		{
			var clickPosition = e.GetPosition(Application.Current.MainWindow);
			clickPosition.X = clickPosition.X * (640 / im.ActualWidth);
			clickPosition.Y = clickPosition.Y * (480 / im.ActualHeight);
			DrawRazmetka(clickPosition);
		}

		private void Grid_MouseLeave_1(object sender, MouseEventArgs e)
		{
			drawing = false;
		}
	}

	internal enum DrawMode
	{
		Rat,
		NotRat,
		Erase
	}
}
