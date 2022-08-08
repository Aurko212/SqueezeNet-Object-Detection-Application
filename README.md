# SqueezeNet Object Recognition

Using a pretrained SqueezeNet ONNX model, I created an application that detects the most predominant object in an image. 

Uses the [Windows.AI.MachineLearning API](https://docs.microsoft.com/en-us/uwp/api/windows.ai.machinelearning?view=winrt-22621/) to load a model, binding an input image to an output tensor, and evaluate it.

## Requirements to run

- [Visual Studio 2019](https://docs.microsoft.com/en-us/visualstudio/releases/2019/release-notes) 
     
- [minimum version Windows 10, version 1809](https://www.microsoft.com/en-us/software-download/windows10) (10.0; build 17763)
     
- [Windows SDK for build 17763](https://developer.microsoft.com/en-us/windows/downloads/sdk-archive/)
     
- [Windows ML Code Generator (mlgen) Visual Studio extension](https://docs.microsoft.com/en-us/windows/ai/windows-ml/mlgen)


## Stretch Goals/ Where to improve

- Integrate the microsoft effects package into the app
-        add filters in real time
-  Connect the webcam to the app for live input
-  add densenet, mobilenet, resnet and squeezenet to app
-         use to create edge detection in webcam footage
-  Add OpenCV


  
