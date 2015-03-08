# FrozenSky
FrozenSky is an application framework including a DirectX based graphics engine which I'am using for most of my hobby projects. 
The base library is separated into the following projects:
 - FrozenSky
 - FrozenSky.Multimedia
 - FrozenSky.BuildTasks
 
All libraries are optimzed to work on most current Windows platforms containing Windows Tablet, Desktop and Server Systems and 
Windows Phone (starting 8.1). The libraries are separated in 
 - a "DESKTOP" version optimized for Windows.Forms and WPF 
 - and a "UNIVERSAL" version optimized for Windows Store and Windows Phone Store.
 
### Library FrozenSky
This one contains all base classes including resource loading/saving, helpers for the MVVM pattern, a main service container and 
much more. I put all methods/classes there which are not directly related to graphics processing.

### Library FrozenSky.Multimedia
This one is the most complex library in this project. It contains the graphics engine with the following features.
 - Full integration into Windows.Forms, WPF and WinRT
 - Heavy multithreading (all calculations and rendering is done in background threads)
 - Working with multiple graphics devices at once (dynamically configure the target device per view)
 - Working with multiple scenegraphs at once (dynamically configure the current scene per view)
 - And much more..
 
# Applications
For now, there are included the following applications.
 - RK 2048: A simple 3D-game which is running all most current windows platforms
 - WPF and Win.Forms Sample Containers
 
