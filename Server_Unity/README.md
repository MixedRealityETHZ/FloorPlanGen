# Server Unity

Start by installing Unity 2022.1.20f1 on your Windows server machine.

## Steps to load the project on Unity
* Go to https://assetstore.unity.com, log in to your account and add [`PUN 2 FREE`](https://assetstore.unity.com/packages/tools/network/pun-2-free-119922) to your assets.
* In the Unity Editor with this project open, open the Package Manager and switch to 'My Assets'. Find `PUN 2 FREE` and download it. Then import it.
* Open the scene `ClientServer.unity`. 
* Close and reopen the project.

## Generation
The generation of the model is currently done in Rhino and Grasshopper.

* Open Rhino 7. Open the file [`modelgen_dummy.3dm`](../Generation/modelgen_dummy.3dm).
* Open Grasshopper by typing `grasshopper` then `Enter` in Rhino.
* In Grasshopper, open the file [`modelgen_dummy.gh`](../Generation/modelgen_dummy.gh).
* In grasshopper, choose the following paths:
  * For the `graph.json` file, choose `Server_Unity/Assets/Resources/graph.json`.
  * For the folder to save the OBJ files, choose `Server_Unity/Assets/Resources/`.

The outline that will be loaded in the HoloLens app and considered by the generation process is the file located at `Server_Unity/Assets/Resources/boundary.json`. It can be modified to any valid outline before running the server.

## Run

Once everything is set up, just click the play button in the Unity Editor to run the scene.

Then, the HoloLens app should be able to automatically connect to the server and load the outline. When the HoloLens will ask for the generated mesh, it will send a graph that will be placed in `Server_Unity/Assets/Resources/graph.json`. The generation process is watching this file and will generate the mesh when it is updated. This new mesh will be placed in `Server_Unity/Assets/Resources/`, and the server will wait for 3 seconds before sending the new mesh to the HoloLens app (which should be enough for the mesh generation process).