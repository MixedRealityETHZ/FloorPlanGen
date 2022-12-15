using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using System.Runtime.CompilerServices;
using System;
using UnityEngine.PlayerLoop;

public class Node : MonoBehaviour // attached to FurnitureUI
{
    public int id; // user-specified
    public string objectName; // user-specified
    public string displayName; // user-specified

    private Model model;
    private bool trackingStatus;

    // private Vector2 location;
    // private float rotation;
    private float size;

    private float maxSize = 40;
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

    public void updateObjectSize(SliderEventData eventData)
    {
        // Call this function when the slider value changes
        this.size = eventData.NewValue * maxSize;
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



    public NodeExport getNode()
    {
        // Position
        Vector3 relativePosition = model.getTrackingHub().getNodeRelativePosition(id);

        NodeExport node = new NodeExport();
        node.id = id;
        node.name = objectName;
        node.position = new Vector3Json(relativePosition.x*20f, relativePosition.z*20f, relativePosition.y*20f); // *20 to scale to original size
        node.angle = model.getTrackingHub().getNodeRelativeRotation(id);
        node.area = size;

        string json = JsonUtility.ToJson(node);
        return node;
    }

    public float getObjectSize()
    {
        return size;
    }

    public Vector3 getSphereBaseCoordinates()
    {
        return trackingIndicator.transform.position;
    }

    public string getObjectDisplayName()
    {
        return displayName;
    }

    public void updateTrackingStatus(bool trackingStatus)
    {
        this.trackingStatus = trackingStatus;

        if (trackingStatus) gameObject.SetActive(true);
        trackingIndicator.GetComponent<CustomProgressIndicatorObjectDisplay>().rotationActive = trackingStatus;
    }

    public void setTrackingIndicatorColor()
    {
        if (trackingStatus)
        {
            if (inOutline())
            {
                target.GetComponent<Renderer>().material.color = new Color(0.16f, 1.0f, 0.0f); // Green
            } else
            {
                target.GetComponent<Renderer>().material.color = new Color(1.0f, 0.0f, 0.0f); // Red
            }
        } else
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

[System.Serializable]
public struct NodeExport
{
    public float area;
    public Vector3Json position;
    public float angle;
    public string name;
    public int id;
}

[System.Serializable]
public struct Vector3Json
{
    public float z;
    public float y;
    public float x;

    public Vector3Json(float x, float y, float z)
    {
        this.x = x; this.y = y; this.z = z;
    }
}

public static class MathUtility
{
    // constants used in approximate equality checks
    internal static readonly float EpsilonScaled = Mathf.Epsilon * 8;

    /// <summary>
    /// A faster drop-in replacement for Mathf.Approximately(a, b).
    /// Compares two floating point values and returns true if they are similar.
    /// As an optimization, this method does not take into account the magnitude of the values it is comparing.
    /// This method may not provide the same results as Mathf.Approximately for extremely large values
    /// </summary>
    /// <param name="a">The first float being compared</param>
    /// <param name="b">The second float being compared</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Approximately(float a, float b)
    {
        var d = b - a;
        var absDiff = d >= 0f ? d : -d;
        return absDiff < EpsilonScaled;
    }

    /// <summary>
    /// A slightly faster way to do Approximately(a, 0f).
    /// </summary>
    /// <param name="a">The floating point value to compare with 0</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ApproximatelyZero(float a)
    {
        return (a >= 0f ? a : -a) < EpsilonScaled;
    }

    /// <summary>
    /// Constrain a value between a minimum and a maximum
    /// </summary>
    /// <param name="input">The input number</param>
    /// <param name="min">The minimum output</param>
    /// <param name="max">The maximum output</param>
    /// <returns>The <paramref name="input"/> number, clamped between <paramref name="min"/> and <paramref name="max"/> </returns>
    public static double Clamp(double input, double min, double max)
    {
        if (input > max)
            return max;

        return input < min ? min : input;
    }

    /// <summary>
    /// Finds the shortest angle distance between two angle values
    /// </summary>
    /// <param name="start">The start value</param>
    /// <param name="end">The end value</param>
    /// <param name="halfMax">Half of the max angle</param>
    /// <param name="max">The max angle value</param>
    /// <returns>The angle distance between start and end</returns>
    public static double ShortestAngleDistance(double start, double end, double halfMax, double max)
    {
        var angleDelta = end - start;
        var angleSign = Math.Sign(angleDelta);

        angleDelta = Math.Abs(angleDelta) % max;
        if (angleDelta > halfMax)
            angleDelta = -(max - angleDelta);

        return angleDelta * angleSign;
    }

    /// <summary>
    /// Finds the shortest angle distance between two angle values
    /// </summary>
    /// <param name="start">The start value</param>
    /// <param name="end">The end value</param>
    /// <param name="halfMax">Half of the max angle</param>
    /// <param name="max">The max angle value</param>
    /// <returns>The angle distance between start and end</returns>
    public static float ShortestAngleDistance(float start, float end, float halfMax, float max)
    {
        var angleDelta = end - start;
        var angleSign = Mathf.Sign(angleDelta);

        angleDelta = Math.Abs(angleDelta) % max;
        if (angleDelta > halfMax)
            angleDelta = -(max - angleDelta);

        return angleDelta * angleSign;
    }

    /// <summary>
    /// Is the float value infinity or NaN?
    /// </summary>
    /// <param name="value">The float value</param>
    /// <returns>True if the value is infinity or NaN (not a number), otherwise false</returns>
    public static bool IsUndefined(this float value)
    {
        return float.IsInfinity(value) || float.IsNaN(value);
    }

    /// <summary>
    /// Checks if a vector is aligned with one of the axis vectors
    /// </summary>
    /// <param name="v"> The vector </param>
    /// <returns>True if the vector is aligned with any axis, otherwise false</returns>
    public static bool IsAxisAligned(this Vector3 v)
    {
        return ApproximatelyZero(v.x * v.y) && ApproximatelyZero(v.y * v.z) && ApproximatelyZero(v.z * v.x);
    }

    /// <summary>
    /// Check if a value is a positive power of two
    /// </summary>
    /// <param name="value">The value to check</param>
    /// <returns>True if the value is a positive power of two, false otherwise</returns>
    public static bool IsPositivePowerOfTwo(int value)
    {
        return value > 0 && (value & (value - 1)) == 0;
    }

    /// <summary>
    /// Return the index of the first flag bit set to true
    /// </summary>
    /// <param name="value">The flags value to check</param>
    /// <returns>The index of the first active flag</returns>
    public static int FirstActiveFlagIndex(int value)
    {
        if (value == 0)
            return 0;

        const int bits = sizeof(int) * 8;
        for (var i = 0; i < bits; i++)
            if ((value & 1 << i) != 0)
                return i;

        return 0;
    }
}