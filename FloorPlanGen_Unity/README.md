# FloorPlanGen Unity (HoloLens App)

## Install the necessary tools
Follow [these instructions](https://learn.microsoft.com/en-us/windows/mixed-reality/develop/install-the-tools?tabs=unity) to install the necessary tools to develop for HoloLens.

Then install Unity 2020.3 LTS, and [set up MRTK](https://learn.microsoft.com/en-us/training/modules/learn-mrtk-tutorials/1-5-exercise-configure-resources) for this Unity project.

## Steps to load the project on Unity
* Open this folder on Unity and ignore the compilation errors.
* Open the scene `FloorPlanGen_Scene.unity`.
* Add the following packages:
  * [Vuforia](https://developer.vuforia.com/downloads/sdk)
  * [Photon/PUN 2 FREE](https://assetstore.unity.com/packages/tools/network/pun-2-free-119922)
  * [Newtonsoft JSON](https://github.com/needle-mirror/com.unity.nuget.newtonsoft-json)
* Close and reopen the project.

## Build for HoloLens
* Change the build settings to UWP, HoloLens and ARM64, then build.
* Open the solution file in Visual Studio.
* Connect your HoloLens to your computer and select it as the target device (more details at [the end of this tutorial](https://learn.microsoft.com/en-us/training/modules/learn-mrtk-tutorials/1-7-exercise-hand-interaction-with-objectmanipulator)).
* Click "Start without debugging".
