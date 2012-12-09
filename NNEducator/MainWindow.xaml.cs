using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;
using AForge.Neuro;
using AForge.Neuro.Learning;
using Emgu.CV.Structure;
using Emgu.CV.WPF;
using auto;
using Emgu.CV;

namespace NNEducator
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}


		const int Radius = 5;
		const int Step = 3;
		private const int SquareSize = (Radius*2 + 1)*(Radius*2 + 1);
		ActivationNetwork net = new ActivationNetwork((IActivationFunction)new SigmoidFunction(1),
														SquareSize * 3, 20, 1);

		private IEnumerable<Tuple<double[], double[], Image<Hls, double>>> GenerateInputOutput()
		{
			ImageFileGallery rats = new ImageFileGallery("rats");
			ImageFileGallery recognizedRats = new ImageFileGallery("marksrats");

			var random = new Random();

			for (int i = 0; i < rats.Count(); i++)
			{
				int gettedSamplesCount = 0;

				var originalImage = rats.GetImageHls();
				var recognized = recognizedRats.GetImage();

				while (gettedSamplesCount < 40)
				{
					int x = Radius*Step + random.Next(originalImage.Width - 2*Radius*Step - 1);
					int y = Radius*Step + random.Next(originalImage.Height - 2*Radius*Step - 40);

					var recognizedOut = GetRecognized(recognized, new Point(x, y), Radius);
					if (gettedSamplesCount % 2 == 0 && recognizedOut[0] < 1)
						continue;
					var input = GetInputData(originalImage, new Point(x, y), Radius);
					gettedSamplesCount++;
					var cropRectangle = new System.Drawing.Rectangle(x - Radius*Step, y - Radius*Step, 2*Radius*Step, 2*Radius*Step);
					var result = originalImage.Copy(cropRectangle);
					//input = gettedSamplesCount % 2 == 0 ? new double[input.Length].Select(a => 0.0).ToArray() : new double[input.Length].Select(a => 1.0).ToArray();
					yield return new Tuple<double[], double[], Image<Hls, double>>(input, recognizedOut, result);
				}
				rats.MoveRight();
				recognizedRats.MoveRight();
			}
		}

		private double[] GetInputData(Image<Hls, Double> image, Point point, int radius)
		{
			List<double> result = new List<double>();
			for (int x = (int)point.X - radius; x <= point.X + radius; x++)
			{
				for (int y = (int) point.Y - radius; y <= point.Y + radius; y++)
				{
					var color = image[y, x];
					result.AddRange(new[] {color.Hue/180, color.Lightness/255, color.Satuation/255});
				}
			}
			return result.ToArray();
		}

		private double[] GetRecognized(Image<Bgr, byte> image, Point point, int radius)
		{
			double[] result = new double[3];
			for (int x = (int)point.X - radius * Step; x <= point.X + radius * Step; x += Step)
			{
				for (int y = (int)point.Y - radius * Step; y <= point.Y + radius * Step; y += Step)
				{
					var color = image[y, x];
					if (color.Red == 255)
						result[0] += 1;
					else if (color.Blue == 255)
						result[1] += 1;
					else
						result[2] += 1;
				}
			}
			return new[]{result[0] / result.Sum() < 0.25 ? 0.0 : 1};
		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			var learningTask = new Task(delegate
			{
				var learningData = GenerateInputOutput().Take(10);
				BackPropagationLearning trainer = new BackPropagationLearning(net);



				double prErr = 10000000;
				// Ошибка сети
				double error = 100;
				// Сначала скорость обучения должна быть высока
				trainer.LearningRate = 100;
				int iteration = 0;
				// Обучаем сеть пока ошибка сети станет небольшой
				while (error > 0.001)
				{
					iteration++;
					// Получаем ошибку сети
					var che = false;
					Dispatcher.Invoke(DispatcherPriority.Normal,
						                new Action(
							                delegate
							                { if (ShowResultsCheckbox.IsChecked != null) che = ShowResultsCheckbox.IsChecked.Value; }));
					if (!che)
					{
						error = trainer.RunEpoch(learningData.Select(i => i.Item1).ToArray(), learningData.Select(i => i.Item2).ToArray());
						Dispatcher.Invoke(DispatcherPriority.Normal,
						                  new Action(delegate { label.Content = error + "\nIteration: " + iteration; }));
					}
					else
					{
						Dispatcher.Invoke(DispatcherPriority.Normal,
											new Action(delegate { label.Content = "See?"; }));
						foreach (var data in learningData)
						{
							Dispatcher.Invoke(DispatcherPriority.Normal,
							                  new Action(
								                  delegate
								                  { if (ShowResultsCheckbox.IsChecked != null) che = ShowResultsCheckbox.IsChecked.Value; }));
							if (!che)
								break;
							// Если ошибка сети изменилась на небольшое значения, в сравнении ошибкой предыдущей эпохи
							Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
							{
								_answerLable.Content = String.Join(", ", data.Item2.Select(d => d.ToString("N2")));
								_learnedLable.Content = String.Join(", ", net.Compute(data.Item1).Select(d => d.ToString("N2")));
								im.Source = BitmapSourceConvert.ToBitmapSource(data.Item3);
								im.UpdateLayout();
							}));
							Thread.Sleep(1000);
						}
					}
					if (Math.Abs(error - prErr) < 0.000000001)
					{
						// Уменьшаем коэффициент скорости обучения на 2
						trainer.LearningRate /= 2;
						if (trainer.LearningRate < 0.001)
							trainer.LearningRate = 0.001;
					}

					prErr = error;
					learningData = GenerateInputOutput().Take(10);
				}
			});
			learningTask.Start();

		}
	}
}
