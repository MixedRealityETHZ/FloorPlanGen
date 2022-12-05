using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Model : MonoBehaviour
{
    private List<Node> listOfNodes = new List<Node>();
    private List<Link> listOfLinks = new List<Link>();
    private GameObject[] outline;
    private static int outlinePositionCount = 7;
    private Vector3[] boundaries = new Vector3[outlinePositionCount];
    private Vector3 outlineScale;

    // TODO: outline integration
    // Origin of the outline (to get positions of objects with respect to this origin)
    private Vector3 origin;

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
        outline = GameObject.FindGameObjectsWithTag("Outline");
        origin = outline[0].transform.position;
        outlineScale = outline[0].transform.localScale;
        outline[0].GetComponent<LineRenderer>().GetPositions(boundaries);
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
        origin = outline[0].transform.position;
    }

    private bool checkNodeIsInOutline(Node node)
    {
        Vector3 loc = node.getSphereBaseCoordinates() - origin;
        var x = loc[0];
        var z = loc[2];
        if (x > boundaries[0][0] & z > boundaries[0][2] & x < boundaries[2][0] * outlineScale[0] & z < boundaries[2][0] * outlineScale[2])
            return true;
        if (x > boundaries[0][0] & z > boundaries[2][2] * outlineScale[2] & x < boundaries[4][0] * outlineScale[0] & z < boundaries[4][2] * outlineScale[2])
            return true;
        return false;
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
            if(checkNodeIsInOutline(node))
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
        StartCoroutine(Wait(4f, button)); // Asynchronous request
    }

    public IEnumerator Wait(float delayInSecs, GameObject button)
    {
        yield return new WaitForSeconds(0.1f); // To keep the button clicking sound
        button.GetComponent<ButtonLoader>().startLoading();
        yield return new WaitForSeconds(delayInSecs);  // TODO: implement the request, remove waiting

        Debug.Log(exportGraphToJson());
        button.GetComponent<ButtonLoader>().stopLoading();
    }
}