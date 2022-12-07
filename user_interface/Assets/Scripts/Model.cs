using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEngine.UI.Image;

public class Model : MonoBehaviour
{
    private List<Node> listOfNodes = new List<Node>();
    private List<Link> listOfLinks = new List<Link>();

    // Origin of the outline (to get positions of objects with respect to this origin)
    private float originYAngle;
    private GameObject outline;
    private List<Vector3> outlinePoints;
    private Vector3 origin; //Point given by API as bottom left corner of the outline TODO: change to use the actual origin of outline sent by the API
    private List<Vector3> transformedOutlinePoints;
    private Vector3 transformedOrigin;

    public Material finalLinkMaterial;
    public float finalLinkWidth;

    class Link
    {
        public Node node1;
        public Node node2;
        public LineRenderer lineRenderer;
    }

    // Start is called before the first frame update
    void Start()
    {
        var list = GameObject.FindGameObjectsWithTag("Node");
        foreach (var gameObject in list)
        {
            var node = gameObject.GetComponent<Node>();
            listOfNodes.Add(node);
        }

        // Hide UserInterface
        GameObject.FindGameObjectsWithTag("UserInterface")[0].gameObject.SetActive(false);

        outline = GameObject.FindGameObjectsWithTag("Outline")[0];

        // TODO: change with API call to initialize boundary
        origin = new Vector3(0f, 0f, 0f);
        outlinePoints = new List<Vector3>()
        {
            new Vector3(0f, 0f, 0f),
            new Vector3(9.2f/20.0f, 0f, 0f),
            new Vector3(9.2f/20.0f, 0f, 6f/20.0f),
            new Vector3(14f/20.0f, 0f, 6f/20.0f),
            new Vector3(14f/20.0f, 0f, 18f/20.0f),
            new Vector3(0f, 0f, 18f/20.0f),
            new Vector3(0f, 0f, 0f),
        };
        createOutlineFromPoints(outlinePoints); // initialize content for the outline gameObject

    }

    public void onConfirmOutline()
    {
        Quaternion rotation = outline.gameObject.transform.GetChild(0).gameObject.transform.rotation;
        transformedOrigin = outline.transform.position; //ToDo: take ball position??
        transformedOutlinePoints = new List<Vector3>(new Vector3[outlinePoints.Count]);
        for (int i = 0; i < outlinePoints.Count; i += 1)
        {
            transformedOutlinePoints[i] = rotation * (outlinePoints[i] - origin) + transformedOrigin;
        }

        //Debug
        //outline.transform.position = new Vector3(0f, 0f, 0f); // moves outline to user field of vue
        //outline.transform.eulerAngles = new Vector3(0f, 0f, 0f);
        //outline.GetComponent<LineRenderer>().SetPositions(transformedOutlinePoints.ToArray());
    }

    // Update is called once per frame
    void Update()
    {
        // Create links from the list
        foreach (var link in listOfLinks)
        {
            link.lineRenderer.SetPosition(0, link.node1.getSphereBaseCoordinates()); //x,y and z position of the starting point of the line
            link.lineRenderer.SetPosition(1, link.node2.getSphereBaseCoordinates()); //x,y and z position of the end point of the line
        }

        // Update outline origin positions
        //transformedOrigin = outline.transform.position;
        originYAngle = outline.transform.localEulerAngles.y;
    }

    private void createOutlineFromPoints(List<Vector3> points)
    {
        outline.GetComponent<LineRenderer>().positionCount = points.Count;
        outline.GetComponent<LineRenderer>().SetPositions(points.ToArray());
        outline.gameObject.transform.GetChild(0).gameObject.transform.position = origin;
        outline.transform.position = new Vector3(0f, -0.2f, 1.0f); // moves outline to user field of vue
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

    // Export graph to JSON

    [System.Serializable]
    struct GraphExport
    {
        public List<NodeExport> Nodes;
        public List<EdgeExport> Edges;
    }

    [System.Serializable]
    struct EdgeExport
    {
        public int nodeA;
        public int nodeB;
    }

    public string exportGraphToJson()
    {
        var graph = new GraphExport();

        graph.Nodes = new List<NodeExport>();
        foreach (var node in listOfNodes)
        {
            if (node.inOutline())
                graph.Nodes.Add(node.getNode());
        }

        graph.Edges = new List<EdgeExport>();
        foreach (var link in listOfLinks)
        {
            var edge = new EdgeExport();
            edge.nodeA = link.node1.id;
            edge.nodeB = link.node2.id;
            graph.Edges.Add(edge);
        }
            
        string json = JsonUtility.ToJson(graph);
        return json;
    }

    // Send Mesh request to API
    public void generateFloorPlan()
    {
        GameObject button = GameObject.FindGameObjectsWithTag("GenerateButton")[0];
        StartCoroutine(Wait(1f, button)); // Asynchronous request
    }

    public IEnumerator Wait(float delayInSecs, GameObject button)
    {
        yield return new WaitForSeconds(0.1f); // To keep the button clicking sound
        button.GetComponent<ButtonLoader>().startLoading();
        yield return new WaitForSeconds(delayInSecs);  // TODO: implement the request, remove waiting

        Debug.Log(exportGraphToJson());
        button.GetComponent<ButtonLoader>().stopLoading();

        // Do the action, TODO: move elsewhere
        visualizeMeshFloorPlanLayer(2);
    }

    // Visualization of the floor plan

    public void visualizeMeshFloorPlanLayer(int layers) // layers=0 means to not show the floor plan
    {
        GameObject loadedObject = (GameObject) Instantiate(Resources.Load("01_house_slice01")); // TODO: change for API
        loadedObject.transform.position = transformedOrigin;
        loadedObject.transform.eulerAngles = new Vector3(0.00f, 180.0f + originYAngle, 0.00f); // TODO: 180 seems weird to need to do that
        loadedObject.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
    }
}