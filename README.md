# SeeingSharp
### Screenshots
![alt tag](Misc/WikiImages/SeeingSharp_Samples.png)

### Common information
SeeingSharp is a library including a DirectX based graphics engine which I'am using for most of my hobby projects. 
The base library is separated into the following projects:
 - SeeingSharp
 - SeeingSharp.Multimedia
 - SeeingSharp.BuildTasks
 
All libraries are optimized to work on most current Windows platforms containing Windows tablets, desktop and server systems, XBox One and Windows Phone 10 (8 or 8.1 is not supported anymore). The libraries are separated in 
 - a "DESKTOP" version optimized for Windows.Forms and WPF.
 - a "UNIVERSAL" version optimized for Windows Store (UWP) (for tablet, phone and XBox One)

You can download the current stable version of Seeing# using Nuget:
https://www.nuget.org/packages?q=SeeingSharp

### Sample apps
 - Windows.Forms: http://www.rolandk.de/files/frozensky.samples/winforms/setup.exe
 - WPF: http://www.rolandk.de/files/frozensky.samples/wpf/setup.exe
 - Windows Store: https://www.microsoft.com/de-de/store/p/seeing-samples/9nblggh1rj07

### Tutorials
See separated GitHub project 
https://github.com/RolandKoenig/SeeingSharp.Tutorials

### Features
 - Full integration into Windows.Forms, WPF and WinRT
 - Heavy multithreading (all calculations and rendering is done in background threads)
 - Working with multiple graphics devices at once (dynamically configure the target device per view)
 - Working with multiple scenegraphs at once (dynamically configure the current scene per view)
 - Flexible postprocessing mechanism
 - Support for all Direct3D 11 Hardware (Featurelevel 9.1 up to 11)
 - Support for software rendering using WARP technology
 - Integration of Direct2D directly into the 3D render process (works also on Windows 7 platform)
 - Integration of Media Foundation to enable VideoTextures (read video files) and VideoCapturing (write video files)
 - Import external 3D models 
 - Build custom 3D models by code
 - And much more..

### Libraries
 - SeeingSharp: This one contains all base classes including resource loading/saving, helpers for the MVVM pattern, a main service container and much more. I put all methods/classes there which are not directly related to graphics processing.
 - SeeingSharp.Multimedia: This one is the most complex library in this project. It contains the graphics engine, classes for video/sound, ...
