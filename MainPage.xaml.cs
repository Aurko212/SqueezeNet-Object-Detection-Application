using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.AI.MachineLearning;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SqueezeApp2
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // All the required fields declaration 
        private SqueezeNetModel modelGen;
        private SqueezeNetInput image = new SqueezeNetInput();
        private SqueezeNetOutput results;
        private StorageFile selectedStorageFile;
        private string label = "";
        private float probability = 0;
        private Helper helper = new Helper();

        public MainPage()
        {
            this.InitializeComponent();
            loadModel();
        }
        private async Task loadModel()
        {
            // Get an access the ONNX model and save it in memory.
            StorageFile modelFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Assets/SqueezeNet.onnx"));
            // Instantiate the model. 
            modelGen = await SqueezeNetModel.CreateFromStreamAsync(modelFile);
        }
        // Waiting for a click event to select a file 
        private async void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            label = "";
            if (!await getImage())
            {
                return;
            }
            // After the click event happened and an input selected, begin the model execution. 
            // Bind the model input
            await imageBind();
            // Model evaluation
            await evaluate();
            // Extract the results
            extractResult();
            // Display the results  
            await displayResult();
        }

        private async Task<bool> getImage()
        {
            try
            {
                // Trigger file picker to select an image file
                FileOpenPicker fileOpenPicker = new FileOpenPicker();
                fileOpenPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                fileOpenPicker.FileTypeFilter.Add(".jpg");
                fileOpenPicker.FileTypeFilter.Add(".png");
                fileOpenPicker.ViewMode = PickerViewMode.Thumbnail;
                selectedStorageFile = await fileOpenPicker.PickSingleFileAsync();
                if (selectedStorageFile == null)
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        // A method to convert and bide the input image. 
        private async Task imageBind()
        {
            UIPreviewImage.Source = null;
            try
            {
                SoftwareBitmap softwareBitmap;
                using (IRandomAccessStream stream = await selectedStorageFile.OpenAsync(FileAccessMode.Read))
                {
                    // Create the decoder from the stream
                    BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
                    // Get the SoftwareBitmap representation of the file in BGRA8 format
                    softwareBitmap = await decoder.GetSoftwareBitmapAsync();
                    softwareBitmap = SoftwareBitmap.Convert(softwareBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
                }
                // Display the image 
                SoftwareBitmapSource imageSource = new SoftwareBitmapSource();
                await imageSource.SetBitmapAsync(softwareBitmap);
                UIPreviewImage.Source = imageSource;

                // Encapsulate the image within a VideoFrame to be bound and evaluated
                VideoFrame inputImage = VideoFrame.CreateWithSoftwareBitmap(softwareBitmap);
                // Resize the image size to 32x32  
                inputImage = await helper.CropAndDisplayInputImageAsync(inputImage);
                // Bind the model input with image 
                ImageFeatureValue imageTensor = ImageFeatureValue.CreateFromVideoFrame(inputImage);
                image.data_0 = imageTensor;

            }
            catch (Exception e)
            {
            }
        }

        private async Task evaluate()
        {
            results = await modelGen.EvaluateAsync(image);
        }
        // A method to extract output from the model 
        private void extractResult()
        {
            // Retrieve the results of evaluation
            var resultTensor = results.softmaxout_1 as TensorFloat;
            // convert the result to vector format
            var resultVector = resultTensor.GetAsVectorView();

            List<(int index, float probability)> indexedResults = new List<(int, float)>();
            for (int i = 0; i < resultVector.Count; i++)
            {
                indexedResults.Add((index: i, probability: resultVector.ElementAt(i)));
            }
            indexedResults.Sort((a, b) =>
            {
                if (a.probability < b.probability)
                    {
                        return 1;
                    }
                if (a.probability > b.probability)
                    {
                        return -1;
                    }
                else
                    {
                        return 0;
                    }
            });

            for (int i = 0; i < 5; i++)
            {
                label += $"\n\"{indexedResults[i].index}\" with confidence of { indexedResults[i].probability}%";
            }

        }
        private async Task displayResult()
        {
            displayOutput.Text = label;
        }
    }

}