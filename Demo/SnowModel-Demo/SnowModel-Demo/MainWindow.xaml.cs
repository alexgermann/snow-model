using SnowModel_Demo.Models;
using SnowModel_Demo.Services;
using SnowModel_Demo.Utilities;
using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace SnowModel_Demo
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		readonly WatsonServiceCall _watsonServiceCall;
		readonly KerasServiceCall _kerasServiceCall;


		public MainWindow()
		{
			InitializeComponent();
			CreateImageList();
			
			_watsonServiceCall = new WatsonServiceCall();
			_kerasServiceCall = new KerasServiceCall();
		}

		void CreateImageList()
		{
            // Cameras we trained on
            webcamSelector.Items.Add(new CameraListItem() { DisplayName = "I80/US189 Interchange", URI = "https://wyoroad.info/highway/webcameras/I80US189Interchange/I80US189InterchangeRoadSurface.jpg" });
            webcamSelector.Items.Add(new CameraListItem() { DisplayName = "I80 Evanston POE", URI = "https://wyoroad.info/highway/webcameras/I80EvanstonPOE/I80EvanstonPOERoadSurface.jpg" });
            webcamSelector.Items.Add(new CameraListItem() { DisplayName = "I80 Evanston", URI = "https://wyoroad.info/highway/webcameras/I80Evanston/RoadSurface.jpg" });
            webcamSelector.Items.Add(new CameraListItem() { DisplayName = "I80 Rock Springs West", URI = "https://wyoroad.info/highway/webcameras/I80RockSpringsWest/RoadSurface.jpg" });
            webcamSelector.Items.Add(new CameraListItem() { DisplayName = "I80 - Laramie West", URI = "https://www.wyoroad.info/highway/webcameras/I80LaramieWest/RoadSurface.jpg" });
            webcamSelector.Items.Add(new CameraListItem() { DisplayName = "I80 - Laramie", URI = "https://wyoroad.info/highway/webcameras/I80Laramie/RoadSurface.jpg" });
            webcamSelector.Items.Add(new CameraListItem() { DisplayName = "I80 Walcott", URI = "https://wyoroad.info/highway/webcameras/I80Walcott/RoadSurface.jpg" });
            webcamSelector.Items.Add(new CameraListItem() { DisplayName = "I80 Sinclair", URI = "https://wyoroad.info/highway/webcameras/I80Sinclair/RoadSurface.jpg" });
            webcamSelector.Items.Add(new CameraListItem() { DisplayName = "I80 Point of Rocks", URI = "https://wyoroad.info/highway/webcameras/I80PointOfRocks/RoadSurface.jpg" });

            // Cameras we did not train on
            webcamSelector.Items.Add(new CameraListItem() { DisplayName = "US30 - Bosler Jct", URI = "https://www.wyoroad.info/Highway/webcameras/US30BoslerJct/US30BoslerJctRoadSurface.jpg" });
            webcamSelector.Items.Add(new CameraListItem() { DisplayName = "US287 - Pumpkin Vine",	URI = "https://www.wyoroad.info/Highway/webcameras/US287PumpkinVine/US287PumpkinVineLuminaire.jpg" });
			webcamSelector.Items.Add(new CameraListItem() { DisplayName = "US287 - Midway",		URI = "https://www.wyoroad.info/Highway/webcameras/US287Midway/US287MidwayRoadSurface.jpg" });
			webcamSelector.Items.Add(new CameraListItem() { DisplayName = "I80 - Strouss Hill", URI = "https://www.wyoroad.info/highway/webcameras/I80StroussHill/I80StroussHillRoadSurface.jpg" });

			webcamSelector.Items.Add(new CameraListItem() { DisplayName = "I80 - Summit Bridge", URI = "https://www.wyoroad.info/Highway/webcameras/I80Summit/I80SummitBridge.jpg" });
			webcamSelector.Items.Add(new CameraListItem() { DisplayName = "I80 - Summit East", URI = "https://www.wyoroad.info/Highway/webcameras/I80SummitEast/RoadSurface.jpg" });
			webcamSelector.Items.Add(new CameraListItem() { DisplayName = "I80 - Buford East", URI = "https://www.wyoroad.info/Highway/webcameras/I80BufordEast/RoadSurface.jpg" });
		}

		void ImagePanel_Drop(object sender, DragEventArgs e)
		{
			WatsonResults.Visibility = Visibility.Collapsed;
			KerasLoader.Visibility = Visibility.Collapsed;
			var files = (string[]) e.Data.GetData(DataFormats.FileDrop);
			if (files.Length != 1)
			{
				SetErrorMessage("Only one photo can be uploaded at a time. Please try again.");
				return;
			}
			if (!IsImageFile(files[0]))
			{
				SetErrorMessage("Only image files (such as .jpg or .png) can be uploaded. Please try again.");
				return;
			}

			var image = SetFileName(files[0]);
			CallWatsonService(image, files[0]);
			CallKerasService(image);
		}


		async void CallWatsonService(BitmapImage image, string fileName)
		{
			WatsonLoader.Visibility = Visibility.Visible;
			var returnedClass = await _watsonServiceCall.GetClassification(ImageUtilities.ToByteArray(image), fileName);

			WatsonResults.Visibility = Visibility.Visible;
			WatsonClass.Text = returnedClass.@class;
			WatsonScore.Text = returnedClass.score.ToString();

			WatsonLoader.Visibility = Visibility.Collapsed;
		}


		async void CallKerasService(BitmapImage image)
		{
			KerasLoader.Visibility = Visibility.Visible;

			var returnedClass = await _kerasServiceCall.GetClassification(ImageUtilities.ToBase64(image));

			KerasResults.Visibility = Visibility.Visible;
			KerasClass.Text = returnedClass.@class;
			KerasScore.Text = returnedClass.score.ToString();

			KerasLoader.Visibility = Visibility.Collapsed;
		}

		
		bool IsImageFile(string filePath)
		{
			try
			{
				Image.FromFile(filePath).Dispose();
				return true;
			}
			catch (OutOfMemoryException)
			{
				return false;
			}
		}


		BitmapImage SetFileName(string fileName)
		{
			FileName.FontWeight = FontWeights.Normal;
			FileName.Foreground = System.Windows.Media.Brushes.Black;
			FileName.Text = Path.GetFileName(fileName);
			var image = new BitmapImage(new Uri(fileName));
			FileImage.Source = image;
			FileProcessingResults.Visibility = Visibility.Visible;

			return image;
		}


		void SetErrorMessage(string message)
		{
			FileName.FontWeight = FontWeights.Bold;
			FileName.Foreground = System.Windows.Media.Brushes.Red;
			FileName.Text = message;
			FileProcessingResults.Visibility = Visibility.Hidden;
			WatsonResults.Visibility = Visibility.Collapsed;
		}


		private void WebcamSelector_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			try
			{
				WatsonResults.Visibility = Visibility.Collapsed;
				KerasLoader.Visibility = Visibility.Collapsed;

				var imageURI = ((CameraListItem)e.AddedItems[0]).URI;
				System.Net.WebRequest request =
				System.Net.WebRequest.Create(imageURI);
				System.Net.WebResponse response = request.GetResponse();
				Stream responseStream =
					response.GetResponseStream();
				var image = ImageUtilities.ToBitmapImage(new Bitmap(responseStream));

				CallWatsonService(image, "test.jpg");
				CallKerasService(image);

				FileName.FontWeight = FontWeights.Normal;
				FileName.Foreground = System.Windows.Media.Brushes.Black;
				FileName.Text = Path.GetFileName(imageURI);
				FileImage.Source = image;
				FileProcessingResults.Visibility = Visibility.Visible;
			}
			catch (Exception exp)
			{
				// Keeps Demo running!!!
			}
		}
	}
}
