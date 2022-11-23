using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using System;

public class SphereBehavior : MonoBehaviour
{
    private Model model;
    private Node node;
    private LineRenderer lr; // Transparent line that shows during Sphere manipulation 

    public float nearDistance;


    // Start is called before the first frame update
    void Start()
    {
        lr = gameObject.transform.GetChild(0).gameObject.GetComponent<LineRenderer>();
        node = gameObject.transform.parent.gameObject.GetComponent<Node>();
        model = GameObject.FindGameObjectsWithTag("Model")[0].GetComponent<Model>();
    }

    // Update is called once per frame
    void Update()
    {
        lr.SetPosition(0, node.getSphereBaseCoordinates());
        lr.SetPosition(1, gameObject.transform.position);
    }

    public void onManipulationStart(ManipulationEventData eventData)
    {
        lr.gameObject.SetActive(true);
    }

    public void onManipulationRelease(ManipulationEventData eventData)
    {
        float dist = 0.0f;
        var releasePosition = gameObject.transform.position;
        gameObject.transform.position = node.getSphereBaseCoordinates(); // move sphere back to where it was

        lr.gameObject.SetActive(false);

        var furnitures = GameObject.FindGameObjectsWithTag("LinkSphere");
        foreach (var furniture in furnitures)
        {
            var go = furniture.gameObject;
            Vector3 endPosition = go.transform.position;

            dist = Vector3.Distance(endPosition, releasePosition);
            if (dist < nearDistance)
            {
                Node furnitureNode = furniture.transform.parent.gameObject.GetComponent<Node>();
                if (node.id != furnitureNode.id)
                {
                    model.updateLink(node, furnitureNode);
                }
            }
        }
    }
}
