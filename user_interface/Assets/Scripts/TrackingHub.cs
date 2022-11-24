using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using UnityEngine;

public class TrackingHub : MonoBehaviour
{
    private Dictionary<int, GameObject> UIs = new Dictionary<int, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        var tmpUIList = GameObject.FindGameObjectsWithTag("Node");
        foreach (var gameObject in tmpUIList)
        {
            UIs.Add(gameObject.GetComponent<Node>().id, gameObject);
        }
        UnityEngine.Debug.Log(UIs.Count);
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Transform childT in transform)
        {
            GameObject child = childT.gameObject;
            int childID = child.GetComponent<ModelTargetID>().id;
            GameObject UI;
            UIs.TryGetValue(childID, out UI);
            if (UI != null)
            {
                //Set position of the UI according to the model target
                Vector3 trackedPosition = child.transform.position;
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
