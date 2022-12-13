using System.Collections.Generic;
using UnityEngine;

public class CameraUI : MonoBehaviour
{
    private Model model;
    private bool trackingStatus;

    private Vector2 location;
    private float rotation;

    private Vector3 referencePosition;
    private GameObject referenceObj;
    private GameObject trackingIndicator;
    private Transform target;

    // Start is called before the first frame update
    void Start()
    {
        model = GameObject.FindGameObjectsWithTag("Model")[0].GetComponent<Model>();
        referenceObj = gameObject.transform.Find("Center").gameObject; // Center is the reference object for position and rotation
        trackingIndicator = gameObject.transform.Find("TrackingIndicator").gameObject;
        target = trackingIndicator.transform.GetChild(0).gameObject.transform.GetChild(0);

        updateTrackingStatus(false);
    }

    // Update is called once per frame
    void Update()
    {
        referencePosition = referenceObj.transform.position;
        setTrackingIndicatorColor();
    }

    public bool inOutline()
    {
        return PointInPolygon(referencePosition, model.getTransformedOutlinePoints());
        // TODO: referencePosition is the diamond, so it move with the user -> find a fix point
    }

    /// <summary>
    /// Determines if a point is inside of a polygon on the XZ plane, the y value is not used
    /// </summary>
    /// <param name="testPoint">The point to test</param>
    /// <param name="vertices">The vertices that make up the bounds of the polygon</param>
    /// <returns>True if the point is inside the polygon, false otherwise</returns>
    public static bool PointInPolygon(Vector3 testPoint, List<Vector3> vertices)
    {
        // Sanity check - not enough bounds vertices = nothing to be inside of
        if (vertices.Count < 3)
            return false;

        // Check how many lines this test point collides with going in one direction
        // Odd = Inside, Even = Outside
        var collisions = 0;
        var vertexCounter = 0;
        var startPoint = vertices[vertices.Count - 1];

        // We recenter the test point around the origin to simplify the math a bit
        startPoint.x -= testPoint.x;
        startPoint.z -= testPoint.z;

        var currentSide = false;
        if (!MathUtility.ApproximatelyZero(startPoint.z))
        {
            currentSide = startPoint.z < 0f;
        }
        else
        {
            // We need a definitive side of the horizontal axis to start with (since we need to know when we
            // cross it), so we go backwards through the vertices until we find one that does not lie on the horizontal
            for (var i = vertices.Count - 2; i >= 0; --i)
            {
                var vertZ = vertices[i].z;
                vertZ -= testPoint.z;
                if (!MathUtility.ApproximatelyZero(vertZ))
                {
                    currentSide = vertZ < 0f;
                    break;
                }
            }
        }

        while (vertexCounter < vertices.Count)
        {
            var endPoint = vertices[vertexCounter];
            endPoint.x -= testPoint.x;
            endPoint.z -= testPoint.z;

            var startToEnd = endPoint - startPoint;
            var edgeSqrMagnitude = startToEnd.sqrMagnitude;
            if (MathUtility.ApproximatelyZero(startToEnd.x * endPoint.z - startToEnd.z * endPoint.x) &&
                startPoint.sqrMagnitude <= edgeSqrMagnitude && endPoint.sqrMagnitude <= edgeSqrMagnitude)
            {
                // This line goes through the start point, which means the point is on an edge of the polygon
                return true;
            }

            // Ignore lines that end at the horizontal axis
            if (!MathUtility.ApproximatelyZero(endPoint.z))
            {
                var nextSide = endPoint.z < 0f;
                if (nextSide != currentSide)
                {
                    currentSide = nextSide;

                    // If we've crossed the horizontal, check if the origin is to the left of the line
                    if ((startPoint.x * endPoint.z - startPoint.z * endPoint.x) / -(startPoint.z - endPoint.z) > 0)
                        collisions++;
                }
            }

            startPoint = endPoint;
            vertexCounter++;
        }

        return collisions % 2 > 0;
    }

    // Getters



    //public NodeExport getNode()
    //{
    //    // Position
    //    location[0] = referencePosition[0]; // TODO: change to get location relative to boundary curve plane
    //    location[1] = referencePosition[2];

    //    // Rotation
    //    rotation = referenceObj.transform.eulerAngles[1]; // TODO: change to get rotation relative to boundary curve plane

    //    NodeExport node = new NodeExport();
    //    node.ID = id;
    //    node.objectname = objectName;
    //    node.location = location;
    //    node.rotation = rotation;
    //    node.size = size;

    //    string json = JsonUtility.ToJson(node);
    //    return node;
    //}

    public Vector3 getSphereBaseCoordinates()
    {
        return referencePosition;
    }

    public void updateTrackingStatus(bool trackingStatus)
    {
        this.trackingStatus = trackingStatus;

        gameObject.SetActive(true);
        trackingIndicator.GetComponent<CustomProgressIndicatorObjectDisplay>().rotationActive = trackingStatus;
    }

    public void setTrackingIndicatorColor()
    {
        if (trackingStatus)
        {
            if (inOutline())
            {
                target.GetComponent<Renderer>().material.color = new Color(0.16f, 1.0f, 0.0f); // Green
            }
            else
            {
                target.GetComponent<Renderer>().material.color = new Color(1.0f, 0.0f, 0.0f); // Red
            }
        }
        else
        {
            if (inOutline())
            {
                target.GetComponent<Renderer>().material.color = new Color(0.0f, 0.576f, 1.0f); // Blue
            }
            else
            {
                target.GetComponent<Renderer>().material.color = new Color(1.0f, 0.0f, 0.0f); // Red
            }
        }
    }
}