using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class TrackingHub : MonoBehaviour
{
    private Dictionary<int, GameObject> UIs = new Dictionary<int, GameObject>();
    private Dictionary<int, GameObject> modelTargets = new Dictionary<int, GameObject>();
    private Dictionary<int, bool> previousTrackingStatus = new Dictionary<int, bool>();
    private bool manualTracking = false;
    private int trackedTargetID;

    // Start is called before the first frame update
    void Start()
    {
        var tmpUIList = GameObject.FindGameObjectsWithTag("Node");
        foreach (var gameObject in tmpUIList)
        {
            UIs.Add(gameObject.GetComponent<Node>().id, gameObject);
        }

        foreach (Transform childT in transform)
        {
            GameObject child = childT.gameObject;
            int childID = child.GetComponent<ModelTargetID>().id;
            modelTargets.Add(childID, child);
            previousTrackingStatus.Add(childID, false);
        }

        System.Diagnostics.Debug.Assert(UIs.Count == modelTargets.Count);
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

    void modelTargetUpdateStatus(int modelTargetID, GameObject modelTarget, GameObject UI)
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
    }

    void modelTargetUpdatePosition(GameObject modelTarget, GameObject UI)
    {
        //Set position of the UI according to the model target
        Vector3 trackedPosition = modelTarget.transform.position;
        UI.transform.position = new Vector3(trackedPosition.x, trackedPosition.y + 0.02f, trackedPosition.z);
    }

    void modelTargetUpdateOrientation(GameObject modelTarget, GameObject UI)
    {
        //Set the orientation such that face the user i.e. the camera
        Vector3 trackedPosition = modelTarget.transform.position;
        Vector3 lookDir = trackedPosition - Camera.main.transform.position; //Direction in this way as the will actually look at the back of the UI
        lookDir.y = 0.0f;
        Quaternion rot = Quaternion.LookRotation(lookDir, Vector3.up);
        UI.transform.rotation = rot;
    }

    void modelTargetUpdate(int modelTargetID, GameObject modelTarget)
    {
        Vuforia.ModelTargetBehaviour modelTargetBehaviour = modelTarget.GetComponent<ModelTargetBehaviour>();
        GameObject UI;
        UIs.TryGetValue(modelTargetID, out UI);

        //Check tracking status
        modelTargetUpdateStatus(modelTargetID, modelTarget, UI);

        //Update UI orientation and position
        //Set position of the UI according to the model target
        modelTargetUpdatePosition(modelTarget, UI);

        //Set the orientation such that face the user i.e. the camera
        modelTargetUpdateOrientation(modelTarget, UI);
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
    }
}