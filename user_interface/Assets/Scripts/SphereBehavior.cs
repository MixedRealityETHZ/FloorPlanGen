using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using System;

public class SphereBehavior : MonoBehaviour
{
    private Vector3 originalPosition;
    public GameObject connectionLine;
    private LineRenderer lr;
    private bool hasMoved = false;
    private List<LineRenderer> allLineRenderers = new List<LineRenderer>();
    private Vector3[] positions;

    public Material finalLinkMaterial;


    // Start is called before the first frame update
    void Start()
    {
        lr = connectionLine.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (hasMoved)
        {
            lr.SetPosition(0, originalPosition);
            lr.SetPosition(1, gameObject.transform.position);
        }
    }

    public void onManipulationStart(ManipulationEventData eventData)
    {
        originalPosition = gameObject.transform.position; // save original position
        hasMoved = true;
    }

    public void onManipulationRelease(ManipulationEventData eventData)
    {
        var releasePosition = gameObject.transform.position;

        gameObject.transform.position = originalPosition; // move sphere back to where it was
        // hasMoved = false;

        var furnitures = GameObject.FindGameObjectsWithTag("LinkSphere");
        foreach (var furniture in furnitures)
        {
            var go = furniture.gameObject;
            Vector3 point1 = go.transform.position;
            float dist = (float)Math.Sqrt(Math.Pow(point1[0] - releasePosition[0], 2) + Math.Pow(point1[1] - releasePosition[1], 2)
                + Math.Pow(point1[2] - releasePosition[2], 2));

            dist = (float)Math.Sqrt(Math.Pow(point1[0] - releasePosition[0], 2) + Math.Pow(point1[1] - releasePosition[1], 2));
            if (dist < 0.2f)
            {
                Debug.Log($"Furniture found at {point1}");
                // check for existing lines
                bool createLine = true;
                foreach (var line in allLineRenderers)
                {
                    positions = new Vector3[2];
                    line.GetPositions(positions);
                    if (positions == null)
                    {
                        continue;
                    }
                    if ((positions[0] == originalPosition && positions[1] == point1) || (positions[1] == originalPosition && positions[1] == point1))
                    {
                        line.SetVertexCount(0);
                        createLine = false;
                        break;
                    }
                }
                if (createLine)
                {
                    // Create a new line
                    var lineRenderer = new GameObject("Line").AddComponent<LineRenderer>();
                    // lineRenderer.startColor = Color.cyan;
                    // lineRenderer.endColor = Color.cyan;
                    lineRenderer.material = finalLinkMaterial;
                    lineRenderer.startWidth = 0.01f;
                    lineRenderer.endWidth = 0.01f;
                    lineRenderer.positionCount = 2;
                    lineRenderer.useWorldSpace = true;

                    //For drawing line in the world space, provide the x,y,z values
                    lineRenderer.SetPosition(0, originalPosition); //x,y and z position of the starting point of the line
                    lineRenderer.SetPosition(1, point1); //x,y and z position of the end point of the line
                    allLineRenderers.Add(lineRenderer);
                }
            }

        }
        // eventData.Use();
    }
}
