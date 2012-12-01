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
using System.Windows.Threading;
using AForge.Neuro;
using AForge.Neuro.Learning;
using Emgu.CV.Structure;
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
		private const int SquareSize = (Radius*2 + 1)*(Radius*2 + 1);
		ActivationNetwork net = new ActivationNetwork((IActivationFunction)new SigmoidFunction(),
														SquareSize*3, SquareSize/9, SquareSize/81, 3);

		private IEnumerable<Tuple<double[], double[]>> GenerateInputOutput()
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
					int x = Radius + random.Next(originalImage.Width - 2*Radius - 1);
					int y = Radius + random.Next(originalImage.Height - 2*Radius - 40);

					var recognizedOut = GetRecognized(recognized, new Point(x, y), Radius);
					if (gettedSamplesCount % 2 == 0 && recognizedOut[0] + recognizedOut[1] == 0)
						continue;
					var input = GetInputData(originalImage, new Point(x, y), Radius);
					gettedSamplesCount++;
					yield return new Tuple<double[], double[]>(input, recognizedOut);
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
			for (int x = (int)point.X - radius; x <= point.X + radius; x++)
			{
				for (int y = (int)point.Y - radius; y <= point.Y + radius; y++)
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
			return result.Select(count => count/result.Sum()).ToArray();
		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{

			var learningTask = new Task(delegate
			{
				var learningData = GenerateInputOutput().Take(10).ToArray();
				BackPropagationLearning trainer = new BackPropagationLearning(net);

				double prErr = 10000000;
				// Ошибка сети
				double error = 100;
				// Сначала скорость обучения должна быть высока
				trainer.LearningRate = 100;
				// Обучаем сеть пока ошибка сети станет небольшой
				while (error > 0.001)
				{
					// Получаем ошибку сети
					error = trainer.RunEpoch(learningData.Select(tuple => tuple.Item1).ToArray(),
					                         learningData.Select(tuple => tuple.Item2).ToArray());
					// Если ошибка сети изменилась на небольшое значения, в сравнении ошибкой предыдущей эпохи

					Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate {label.Content = error;}));
					
					if (Math.Abs(error - prErr) < 0.000000001)
					{
						// Уменьшаем коэффициент скорости обучения на 2
						//trainer.LearningRate /= 1.45;
						if (trainer.LearningRate < 0.001)
							trainer.LearningRate = 0.001;
					}

					prErr = error;
				}
			});
			learningTask.Start();

		}
	}
}
