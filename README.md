# SeeingSharp
SeeingSharp is an application framework including a DirectX based graphics engine which I'am using for most of my hobby projects. 
The base library is separated into the following projects:
 - SeeingSharp
 - SeeingSharp.Multimedia
 - SeeingSharp.BuildTasks
 
All libraries are optimized to work on most current Windows platforms containing Windows tablets (also ARM based), Desktop and Server Systems and Windows Phone (starting 8.1). The libraries are separated in 
 - a "DESKTOP" version optimized for Windows.Forms and WPF.
 - a "UNIVERSAL" version optimized for Windows Store and Windows Phone Store.

You can download the current stable version of Seeing# using Nuget:
https://www.nuget.org/packages?q=SeeingSharp

### Library SeeingSharp
This one contains all base classes including resource loading/saving, helpers for the MVVM pattern, a main service container and much more. I put all methods/classes there which are not directly related to graphics processing.

### Library SeeingSharp.Multimedia
This one is the most complex library in this project. It contains the graphics engine with the following features.
 - Full integration into Windows.Forms, WPF and WinRT
 - Heavy multithreading (all calculations and rendering is done in background threads)
 - Working with multiple graphics devices at once (dynamically configure the target device per view)
 - Working with multiple scenegraphs at once (dynamically configure the current scene per view)
 - Flexible postprocessing mechanism
 - Support for all Direct3D 11 Hardware (Featurelevel 9.1 up to 11)
 - Support for software rendering using WARP technology
 - Integration of Direct2D directly into the 3D render process (works also on Windows 7 platform)
 - Integration of Media Foundation to enable VideoTextures (read video files) and VideoCapturing (write video files)
 - And much more..
 
# Applications
For now, there are included the following applications.
 - RK 2048: A simple 3D-game which is running all most current windows platforms
 - RK Kinect Lounge: A photo viewer which can be controlled using the Kinect technology
 - RK Rocket: A simple 2D shooting game (uses Direct2D)
 - WPF and Win.Forms Sample Containers
 
