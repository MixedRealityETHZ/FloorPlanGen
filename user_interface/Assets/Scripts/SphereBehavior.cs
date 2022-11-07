using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;

public class SphereBehavior : MonoBehaviour
{
    private Vector3 originalPosition;
    public GameObject connectionLine;
    private LineRenderer lr;
    private bool hasMoved = false;

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
            Debug.Log("OP: " + originalPosition);
            lr.SetPosition(0, originalPosition);
            Debug.Log("NEW: " + gameObject.transform.position);
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
        gameObject.transform.position = originalPosition; // move sphere back to where it was
    }
}
