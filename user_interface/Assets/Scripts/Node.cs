using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;

public class Node : MonoBehaviour // attached to FurnitureUI
{
    public int id; // user-specified (TODO: auto-generate?)
    public string objectName; // user-specified
    public string displayName; // user-specified
    private Vector2 location;
    private float rotation;
    private float size;

    private float maxSize = 40;
    private Vector3 referencePosition;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        GameObject referenceObj = gameObject.transform.GetChild(3).gameObject; // SphereBase is the reference object for position and rotation

        // Position
        referencePosition = referenceObj.transform.position;
        location[0] = referencePosition[0]; // TODO: change to get location relative to boundary curve plane
        location[1] = referencePosition[2];
        
        // Rotation
        rotation = referenceObj.transform.eulerAngles[1]; // TODO: change to get rotation relative to boundary curve plane

        // Debug.Log(this);
    }

    public void updateObjectSize(SliderEventData eventData)
    {
        // Call this function when the slider value changes
        this.size = eventData.NewValue * maxSize;
    }


    // Getters



    public NodeExport getNode()
    {
        NodeExport node = new NodeExport();
        node.ID = id;
        node.objectname = objectName;
        node.location = location;
        node.rotation = rotation;
        node.size = size;

        string json = JsonUtility.ToJson(node);
        return node;
    }

    public float getObjectSize()
    {
        return size;
    }

    public Vector3 getSphereBaseCoordinates()
    {
        return referencePosition;
    }

    public string getObjectDisplayName()
    {
        return displayName;
    }
}

[System.Serializable]
public struct NodeExport
{
    public int ID;
    public string objectname;
    public Vector2 location;
    public float rotation;
    public float size;
}
