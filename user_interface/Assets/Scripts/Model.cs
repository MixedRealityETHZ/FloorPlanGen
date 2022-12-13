using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using Microsoft.MixedReality.Toolkit;

public class Model : MonoBehaviour
{
    public SendReceive sendReceive;

    private List<Node> listOfNodes = new List<Node>();
    private List<Link> listOfLinks = new List<Link>();

    // Origin of the outline (to get positions of objects with respect to this origin)
    private List<List<float>> jsonArray;
    private TrackingHub trackingHub;
    private float originYAngle = 0f;
    private GameObject outline;
    private List<Vector3> outlinePoints;
    private Vector3 origin; //Point given by API as bottom left corner of the outline TODO: change to use the actual origin of outline sent by the API
    private List<Vector3> transformedOutlinePoints;
    private Vector3 transformedOrigin;
    private Quaternion outlineRotation;
    private Quaternion outlineRotationInv;

    // Mesh Slices
    public int layerNumber; // Default layer number
    private List<GameObject> slices = new List<GameObject>();
    private GameObject generationButton;

    public Material finalLinkMaterial;
    public float finalLinkWidth;
    public Vector3 initialOutlinePosition;

    class Link
    {
        public Node node1;
        public Node node2;
        public LineRenderer lineRenderer;
    }

    public void onServerStart()
    {
        sendReceive.sendBoundaryRequestClient();
    }

    public void onBoundaryReceive()
    {
        string path = Application.dataPath + $"/Resources/boundary.json";
        if (!string.IsNullOrEmpty(path))
        {
            string jsonString = File.ReadAllText(path);
            //Debug.Log("json " + jsonString);

            jsonArray = JsonConvert.DeserializeObject<List<List<float>>>(jsonString);

            outlinePoints = new List<Vector3>();
            origin = new Vector3(0f, 0f, 0f);

            foreach (var point in jsonArray)
            {
                outlinePoints.Add(new Vector3(point[0] / 20f, point[2] / 20f, point[1] / 20f));
            }

            createOutlineFromPoints(outlinePoints); // initialize content for the outline gameObject
            outline.SetActive(true);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        var list = GameObject.FindGameObjectsWithTag("Node");
        foreach (var gameObject in list)
        {
            var node = gameObject.GetComponent<Node>();
            listOfNodes.Add(node);
            gameObject.SetActive(false); // Furniture UI is not visible until it is tracked a first time
        }

        // Hide UserInterface
        GameObject.FindGameObjectsWithTag("UserInterface")[0].gameObject.SetActive(false);

        outline = GameObject.FindGameObjectsWithTag("Outline")[0];
        outline.SetActive(false);
    }
    

    public void onConfirmOutline()
    {
        // TODO: is it accurate to use the outline for this, and not the handle that is on the origin?
        outlineRotation = outline.transform.rotation;
        outlineRotationInv = Quaternion.Inverse(outlineRotation);
        transformedOrigin = outline.transform.position;
        transformedOutlinePoints = new List<Vector3>(new Vector3[outlinePoints.Count]);
        for (int i = 0; i < outlinePoints.Count; i += 1)
        {
            transformedOutlinePoints[i] = outlineRotation * (outlinePoints[i] - origin) + transformedOrigin;
        }
        originYAngle = outline.transform.localEulerAngles.y;
    }

    // Update is called once per frame
    void Update()
    {
        //check if we have outline otherwise wait for server 
        if (!outlinePoints.IsNull() && outlinePoints.Count != 0)
        {
            // Create links from the list
            foreach (var link in listOfLinks)
            {
                link.lineRenderer.SetPosition(0, link.node1.getSphereBaseCoordinates()); //x,y and z position of the starting point of the line
                link.lineRenderer.SetPosition(1, link.node2.getSphereBaseCoordinates()); //x,y and z position of the end point of the line
            }
        }
        
    }

    private void createOutlineFromPoints(List<Vector3> points)
    {
        outline.GetComponent<LineRenderer>().positionCount = points.Count;
        outline.GetComponent<LineRenderer>().SetPositions(points.ToArray());
        outline.gameObject.transform.GetChild(0).gameObject.transform.position = origin;
        moveOutlineInFrontOfUser();
    }

    public void moveOutlineInFrontOfUser()
    {
        outline.transform.position = initialOutlinePosition; // moves outline to user field of vue
        outline.transform.eulerAngles = new Vector3(0f,0f,0f);
        outline.transform.GetChild(0).transform.localPosition = new Vector3(0f, 0f, 0f); // Also move the handle
    }

    public void updateLayerNumber(SliderEventData eventData)
    {
        // Call this function when the slider value changes
        layerNumber = (int)(eventData.NewValue * 4);
        visualizeMeshFloorPlanLayer(layerNumber);
    }

    public void updateLink(Node node1, Node node2)
    {
        bool addLink = true;

        foreach (var link in listOfLinks) // check if link already exists, if yes, remove it
        {
            if ((link.node1 == node1 && link.node2 == node2) || (link.node1 == node2 && link.node2 == node1)) {
                Destroy(link.lineRenderer.gameObject);
                listOfLinks.Remove(link);
                addLink = false;
                break;
            }
        }

        // Check if both nodes are within the outline
        /*if(!checkNodeIsInOutline(node1) | !checkNodeIsInOutline(node2))
        {
            addLink = false;
        }*/
            
        if (addLink) // if link does not exist, add it
        {
            Link newLink = new Link();
            newLink.node1 = node1;
            newLink.node2 = node2;

            // Create a new line
            newLink.lineRenderer = new GameObject("Line_"+node1.objectName+"_"+node2.objectName).AddComponent<LineRenderer>();
            newLink.lineRenderer.material = finalLinkMaterial;
            newLink.lineRenderer.startWidth = finalLinkWidth;
            newLink.lineRenderer.endWidth = finalLinkWidth;
            newLink.lineRenderer.positionCount = 2;
            newLink.lineRenderer.useWorldSpace = true;

            listOfLinks.Add(newLink);
        }
    }

    // Getters

    public List<Vector3> getTransformedOutlinePoints()
    {
        return transformedOutlinePoints;
    }

    public Quaternion getOutlineRotation()
    {
        return outlineRotation;
    }

    public Quaternion getOutlineRotationInv()
    {
        return outlineRotationInv;
    }

    public Vector3 getTransformedOrigin()
    {
        return transformedOrigin;
    }

    public TrackingHub getTrackingHub()
    {
       return trackingHub;
    }

    public void setTrackingHub(TrackingHub trackingHub)
    {
        this.trackingHub = trackingHub;
    }

    // Export graph to JSON

    [System.Serializable]
    struct GraphExport
    {
        public List<NodeExport> nodes;
        public List<List<int>> edges;
        public List<List<float>> boundary;
    }

    public string exportGraphToJson()
    {
        var graph = new GraphExport();

        Debug.Log(jsonArray);
        graph.boundary = jsonArray;

        graph.nodes = new List<NodeExport>();
        foreach (var node in listOfNodes)
        {
            if (node.inOutline())
                graph.nodes.Add(node.getNode());
        }

        graph.edges = new List<List<int>>();
        foreach (var link in listOfLinks)
        {
            var edge = new List<int>();
            edge.Add(link.node1.id);
            edge.Add(link.node2.id);
            if (link.node1.inOutline() && link.node2.inOutline())
                graph.edges.Add(edge);
        }

        Debug.Log(graph.boundary);
        Debug.Log(graph.edges);

        // string json = JsonUtility.ToJson(graph);
        string json = JsonConvert.SerializeObject(graph);
        Debug.Log(json);
        return json;
    }
    
    //called by the UI in HandButtonController
    public void startMeshGeneration(GameObject button)
    {
        generationButton = button;
        generationButton.GetComponent<ButtonLoader>().startLoading();

        string jsonString = exportGraphToJson();
        //write jSon on disk
        GameSettingsSingleton.Instance.graphJsonString = jsonString;
        string path = Application.dataPath + "/Resources/graph.json";
        File.WriteAllText(path, GameSettingsSingleton.Instance.graphJsonString);

        Debug.Log("Client sends the graph to server");
        sendReceive.sendGraphClient();
    }

    public void finishMeshGeneration(GameObject loadedObj)
    {
        //so that we only load one mesh/slices each time
        foreach (var item in slices)
        {
            Destroy(item);
        }

        slices.Clear();
        Debug.Log("Displaying mesh on client");
        createMeshObjects(loadedObj);
        generationButton.GetComponent<ButtonLoader>().stopLoading();
    }

    public void createMeshObjects(GameObject loadedObj)
    {
        //TODO use slices instead of mesh
        slices.Add(loadedObj);
        slices.Add(new GameObject());
        slices.Add(new GameObject());
        slices.Add(new GameObject());
        //slices.Add((GameObject)Instantiate(Resources.Load("01_house_slice01")));
        //slices.Add((GameObject)Instantiate(Resources.Load("01_house_slice02")));
        //slices.Add((GameObject)Instantiate(Resources.Load("01_house_slice03")));
        //slices.Add((GameObject)Instantiate(Resources.Load("01_house_slice04")));

        foreach (var slice in slices) {
            slice.transform.position = transformedOrigin;
            slice.transform.eulerAngles = new Vector3(0.00f, 180f + originYAngle, 0.00f); // TODO: 180 seems weird to need to do that
            slice.transform.localScale = new Vector3(-0.05f, 0.05f, 0.05f);
            slice.SetActive(false);
        }
        Debug.Log("mesh pos " + loadedObj.transform.position);

        foreach (var obje in FindObjectsOfType(typeof(GameObject)) as GameObject[])
        {
            if (obje.name == "mesh")
            {
                Debug.Log(obje.name);
                Debug.Log(obje.transform.position);
            }
        }
        visualizeMeshFloorPlanLayer(layerNumber);
    }


    // Visualization of the floor plan

    public void visualizeMeshFloorPlanLayer(int layers) // layers=0 means to not show the floor plan
    {
        if (layers <= slices.Count) {
            for (int i = 0; i < layers; i++)
            {
                slices[i].SetActive(true);
            }
            for (int i = 0; i < slices.Count - layers; i++)
            {
                slices[slices.Count - 1 - i].SetActive(false);
            }
        }
    }
}