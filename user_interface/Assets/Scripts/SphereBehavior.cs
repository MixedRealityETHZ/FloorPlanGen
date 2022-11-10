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
    private Vector3[] positions;
    private LinksModel linksModel;

    public Material finalLinkMaterial;
    public GameObject linkInformation;
    public float nearDistance;
    public float finalLinkWidth;


    // Start is called before the first frame update
    void Start()
    {
        lr = connectionLine.GetComponent<LineRenderer>();
        linksModel = linkInformation.GetComponent<LinksModel>();
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
        float dist = 0.0f;
        var releasePosition = gameObject.transform.position;

        gameObject.transform.position = originalPosition; // move sphere back to where it was
        // hasMoved = false;

        var furnitures = GameObject.FindGameObjectsWithTag("LinkSphere");
        foreach (var furniture in furnitures)
        {
            var go = furniture.gameObject;
            Vector3 endPosition = go.transform.position;

            dist = Vector3.Distance(endPosition, releasePosition); //(float)Math.Sqrt(Math.Pow(endPosition[0] - releasePosition[0], 2) + Math.Pow(endPosition[1] - releasePosition[1], 2));
            if (dist < nearDistance)
            {
                // Debug.Log($"Furniture origin: {originalPosition}");
                // Debug.Log($"Furniture found: {endPosition}");

                // check for existing lines
                bool createLine = true;
                List<LineRenderer> allLines = linksModel.getLines();
                foreach (var line in allLines)
                {
                    positions = new Vector3[2];
                    line.GetPositions(positions);
                    if (positions == null)
                    {
                        continue;
                    }
                    if ((positions[0] == originalPosition && positions[1] == endPosition) || (positions[1] == originalPosition && positions[0] == endPosition))
                    {
                        // Debug.Log("Deleting line");
                        linksModel.removeLine(line);
                        createLine = false;
                        break;
                    }
                }
                if (createLine)
                {
                    // Create a new line
                    var lineRenderer = new GameObject("Line").AddComponent<LineRenderer>();
                    lineRenderer.material = finalLinkMaterial;
                    lineRenderer.startWidth = 0.01f;
                    lineRenderer.endWidth = 0.01f;
                    lineRenderer.positionCount = 2;
                    lineRenderer.useWorldSpace = true;

                    //For drawing line in the world space, provide the x,y,z values
                    lineRenderer.SetPosition(0, originalPosition); //x,y and z position of the starting point of the line
                    lineRenderer.SetPosition(1, endPosition); //x,y and z position of the end point of the line
                    linksModel.addLine(lineRenderer);
                }
            }

        }
        // eventData.Use();
    }
}
