using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class TrackingHub : MonoBehaviour
{
    private Model model;

    private Dictionary<int, GameObject> UIs = new Dictionary<int, GameObject>();
    private Dictionary<int, GameObject> modelTargets = new Dictionary<int, GameObject>();
    private Dictionary<int, bool> previousTrackingStatus = new Dictionary<int, bool>();
    private bool manualTracking = false;
    private int trackedTargetID;

    //Camera
    private GameObject cameraUI;
    private GameObject cameraTarget;
    private GameObject cameraPuppet;
    private bool cameraPreviousTrackingStatus = false;

    // Start is called before the first frame update
    void Start()
    {
        model = GameObject.FindGameObjectsWithTag("Model")[0].GetComponent<Model>();

        //Furnitures UI
        var tmpUIList = GameObject.FindGameObjectsWithTag("Node");
        foreach (var gameObject in tmpUIList)
        {
            UIs.Add(gameObject.GetComponent<Node>().id, gameObject);
        }
        //Camera UI
        cameraUI = GameObject.FindGameObjectsWithTag("CameraUI")[0];
        //Camera Puppet
        cameraPuppet = GameObject.FindGameObjectsWithTag("CameraPuppet")[0];

        foreach (Transform childT in transform)
        {
            GameObject child = childT.gameObject;
            int childID = child.GetComponent<ModelTargetID>().id;
            if (childID >= 0)
            {
                modelTargets.Add(childID, child);
                previousTrackingStatus.Add(childID, false);
            }
            else
            {
                cameraTarget = child;
            }
        }

        System.Diagnostics.Debug.Assert(UIs.Count == modelTargets.Count);
        UnityEngine.Debug.Log("[TrackingHub] :" + modelTargets.Count + " furnitures has been initialized");
    }

    public void updateManualTracking(bool manualTracking)
    {
        if (this.manualTracking == manualTracking) return;

        this.manualTracking = manualTracking;

        UnityEngine.Debug.Log("[TrackingHub] ManualMode set to: " + this.manualTracking);

        foreach (KeyValuePair<int, GameObject> item in modelTargets)
        {
            int modelTargetID = item.Key;
            GameObject modelTarget = item.Value;

            modelTarget.GetComponent<ModelTargetBehaviour>().enabled = (!this.manualTracking);
            if (this.manualTracking)
            {
                GameObject UI;
                UIs.TryGetValue(modelTargetID, out UI);
                UI.GetComponent<Node>().updateTrackingStatus(false);
                previousTrackingStatus[modelTargetID] = false;
            }
        }
    }

    public void setTarget(int trackingID)
    {
        if (!manualTracking) return;

        UnityEngine.Debug.Log("[TrackingHub] ManualTarget set to: " + trackingID);

        trackedTargetID = trackingID;

        foreach (KeyValuePair<int, GameObject> item in modelTargets)
        {
            int modelTargetID = item.Key;
            GameObject modelTarget = item.Value;

            if (modelTargetID == trackingID)
            {
                modelTarget.GetComponent<ModelTargetBehaviour>().enabled = true;
            }
            else
            {
                modelTarget.GetComponent<ModelTargetBehaviour>().enabled = false;
                //Tell the UI that the model is longer tracked
                GameObject UI;
                UIs.TryGetValue(modelTargetID, out UI);
                UI.GetComponent<Node>().updateTrackingStatus(false);
                previousTrackingStatus[modelTargetID] = false;
            }
        }
    }

    bool modelTargetUpdateStatus(int modelTargetID, GameObject modelTarget, GameObject UI)
    {
        Vuforia.ModelTargetBehaviour modelTargetBehaviour = modelTarget.GetComponent<ModelTargetBehaviour>();

        //Check tracking status
        bool previousStatus;
        previousTrackingStatus.TryGetValue(modelTargetID, out previousStatus);
        bool currentTrackingStatus = (modelTargetBehaviour.TargetStatus.Status == Vuforia.Status.TRACKED);

        if (previousStatus != currentTrackingStatus)
        {
            UnityEngine.Debug.Log("[TrackingHub] ModelTarget: " + modelTargetID.ToString() + " tracking status is " + currentTrackingStatus.ToString());
            UI.GetComponent<Node>().updateTrackingStatus(currentTrackingStatus);
            previousTrackingStatus[modelTargetID] = currentTrackingStatus;
        }

        return currentTrackingStatus;
    }

    void modelTargetUpdatePosition(GameObject modelTarget, GameObject UI)
    {
        //Set position of the UI according to the model target
        Vector3 trackedPosition = modelTarget.transform.position;
        UI.transform.position = new Vector3(trackedPosition.x, trackedPosition.y, trackedPosition.z);
    }

    void modelTargetUpdateOrientation(GameObject modelTarget, GameObject UI)
    {
        //Set the orientation such that face the user i.e. the camera
        Vector3 currentPosition = UI.transform.position;
        Vector3 lookDir = currentPosition - Camera.main.transform.position; //Direction in this way as the will actually look at the back of the UI
        lookDir.y = 0.0f;
        Quaternion rot = Quaternion.LookRotation(lookDir, Vector3.up);
        UI.transform.rotation = rot;
    }

    void modelTargetUpdate(int modelTargetID, GameObject modelTarget)
    {
        GameObject UI;
        UIs.TryGetValue(modelTargetID, out UI);

        //Check tracking status
        bool isTracked = modelTargetUpdateStatus(modelTargetID, modelTarget, UI);

        //Update UI orientation and position
        //Set position of the UI according to the model target if tracked
        if (isTracked) modelTargetUpdatePosition(modelTarget, UI);
        //Set the orientation such that face the user i.e. the camera (after as position could have been updated)
        modelTargetUpdateOrientation(modelTarget, UI);
    }

    void cameraUpdate()
    {
        Vuforia.ModelTargetBehaviour modelTargetBehaviour = cameraTarget.GetComponent<ModelTargetBehaviour>();
        //Check tracking status
        bool currentTrackingStatus = (modelTargetBehaviour.TargetStatus.Status == Vuforia.Status.TRACKED);
        if (cameraPreviousTrackingStatus != currentTrackingStatus)
        {
            UnityEngine.Debug.Log("[TrackingHub] CameraTarget: tracking status is " + currentTrackingStatus.ToString());
            cameraUI.GetComponent<CameraUI>().updateTrackingStatus(currentTrackingStatus);
            cameraPreviousTrackingStatus = currentTrackingStatus;
        }

        //Update UI
        if (currentTrackingStatus)
        {
            modelTargetUpdatePosition(cameraTarget, cameraUI);

            //Update puppet
            Vector3 trackedPosition = cameraTarget.transform.position;
            trackedPosition.y = 0.0f;
            Vector3 originPosition = model.getTransformedOrigin();
            originPosition.y = 0.0f;
            Quaternion trackedRotation = cameraTarget.transform.rotation;

            cameraPuppet.transform.position = model.getOutileRotationInv() * (trackedPosition - originPosition);
            cameraPuppet.transform.rotation = model.getOutileRotationInv() * trackedRotation;
        }
        modelTargetUpdateOrientation(cameraTarget, cameraUI);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug
        //if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        //Switch to manual target
        if (Input.GetKeyDown(KeyCode.M))
        {
            bool track = (!manualTracking);
            updateManualTracking(track);
        }
        //Set the target
        //-Dinning table
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            setTarget(0);
        }
        //-Simple Bed B
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            setTarget(1);
        }
           
        //Furnitures tracking
        foreach (KeyValuePair<int, GameObject> item in modelTargets)
        {
            int modelTargetID = item.Key;
            GameObject modelTarget = item.Value;
            GameObject UI;
            UIs.TryGetValue(modelTargetID, out UI);

            if (manualTracking)
            {
                if (modelTargetID == trackedTargetID)
                {
                    modelTargetUpdate(modelTargetID, modelTarget);
                }
                else
                {
                    modelTargetUpdateOrientation(modelTarget, UI);
                }
            }
            else
            {
                modelTargetUpdate(modelTargetID, modelTarget);
            }
        }
        //Camera tracking
        cameraUpdate();
    }
}