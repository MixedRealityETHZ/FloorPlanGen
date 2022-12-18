# SET UP

You need to be on Windows for this to run.

## A. Unity
1. Unity version: `2022.1.20f1`, please switch to the one that is recommended for Hololens `2020.3.40f1`.
2. Go to https://assetstore.unity.com, log in to your account and add `PUN 2 FREE` (https://assetstore.unity.com/packages/tools/network/pun-2-free-119922) to your assets.
3. In the Unity Editor with your project open, open the Package Manager and switch to 'My Assets'. Find `PUN 2 FREE` and download it. Then import it.
4. Download the unity project from the GitHub repo.
5. open `ClientServer.scene`.  This is the 3D visualizer.
6. In the GameSetting GameObject: set Username field to: client (on the Hololens), server (on the API machine)

7. In the SendReceive.cs you have the framework for the following:
   1. Client2Server: get boundary by id (1,2 or 3) - the boundary json string will be in `GameSettingsSingleton.Instance.boundaryJsonString = jsonString;`
   2. Client2Server: send the graph - the client will save the graph, the server will save it as graph.json in its Resources folder
   3. Server2Client: send mesh to client


8. update camera position: The GameObject TrackedCamera has an Id and an observer (Photon Transfrom View). So in the client link your tracked 3d printed camera object to this Tracked Camera. It will be mirrored in the server.


## B. Remote environment with API

To save you time, we provide you with remote access to one of our workstations. Use TeamViewer and the account I sent you per email to log in.

All needed files should be in the folder `MR Course 2022` on the desktop.

Clone the Unity project from the repo there as well.

Use only this folder to save files.
   
## B. Python (Skip for now)

1. install miniconda in case you don't have it: https://docs.conda.io/projects/conda/en/latest/user-guide/install/windows.html
2. update conda by navigating to the anaconda directory in the Miniconda shell and Run `conda update conda`
3. download folder `python` from gitrepo
4. create a new conda environment with:

		conda create -n genmodel python=3.9 tensorflow==2.6.0 py7zr

5. unzip the ML model

		python unzip_model.py


## C. Rhino (skip for now)
1. install `Rhino 7 for Windows - One-Time Evaluation` version 90 days from here: https://www.rhino3d.com/download/
2. install `lunchbox` and `TT Toolbox` as follows. Open Rhino 7. Go to Tools->Package Manager...
3. install and `Pufferfish` from https://www.food4rhino.com
3. open Rhino once.

# RUN

On the machine acting as server:
1. run  `ClientServer.scene` in the editor
<s>
2. open a Miniconda shell and navigate to the `python` folder.
3. activate conda env `genmodel` and run `A.py`~~

 		conda activate genmodel
		python a.py 
</s>

4. open Rhino 7. Open file: `modelgen_dummy.3dm`
5. open Grasshopper by typing `grasshopper` then `Enter` in Rhino
5. in grasshopper: File->Open: `modelgen_dummy.gh`
6. In grasshopper, choose the paths for the graph.json file and the where to save the OBJ files.


# Test
<s>
request boundary by sending id

request model by sending graph

update camera position
</s>