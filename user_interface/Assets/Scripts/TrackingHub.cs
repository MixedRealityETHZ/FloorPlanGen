using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using UnityEngine;
using Vuforia;

public class TrackingHub : MonoBehaviour
{
    private Dictionary<int, GameObject> UIs = new Dictionary<int, GameObject>();
    private Dictionary<int, GameObject> modelTargets = new Dictionary<int, GameObject>();
    private Dictionary<int, bool> previousTrackingStatus = new Dictionary<int, bool>();

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

    // Update is called once per frame
    void Update()
    {
        foreach (KeyValuePair<int, GameObject> item in modelTargets)
        {
            int modelTargetID = item.Key;
            GameObject modelTarget = item.Value;
            Vuforia.ModelTargetBehaviour modelTargetBehaviour = modelTarget.GetComponent<ModelTargetBehaviour>();
            GameObject UI;
            UIs.TryGetValue(modelTargetID, out UI);

            //Check tracking status
            bool previousStatus;
            previousTrackingStatus.TryGetValue(modelTargetID, out previousStatus);
            bool currentTrackingStatus = (modelTargetBehaviour.TargetStatus.Status == Vuforia.Status.TRACKED);
            if (previousStatus != currentTrackingStatus)
            {
                UnityEngine.Debug.Log("ModelTarget: " + modelTargetID.ToString() + " tracking status is " + currentTrackingStatus.ToString());
                //ToDo: Prevenir UI
                previousTrackingStatus[modelTargetID] = currentTrackingStatus;
            }

            //Update UI orientation and position
            if (UI != null)
            {
                //Set position of the UI according to the model target
                Vector3 trackedPosition = modelTarget.transform.position;
                UI.transform.position = new Vector3(trackedPosition.x, trackedPosition.y + 0.02f, trackedPosition.z); //ToDo: Coller le y sur une surface

                //Set the orientation such that face the user i.e. the camera
                Vector3 lookDir = trackedPosition - Camera.main.transform.position; //Direction in this way as the will actually look at the back of the UI
                lookDir.y = 0.0f;
                Quaternion rot = Quaternion.LookRotation(lookDir, Vector3.up);
                UI.transform.rotation = rot;
            }
        }
    }
}
