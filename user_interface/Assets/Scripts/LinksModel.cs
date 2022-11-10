using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinksModel : MonoBehaviour
{
    private List<LineRenderer> allLineRenderers = new List<LineRenderer>();

    public List<LineRenderer> getLines()
    {
        return allLineRenderers;
    }

    public void addLine(LineRenderer lineRenderer)
    {
        allLineRenderers.Add(lineRenderer);
    }

    public void removeLine(LineRenderer lineRenderer)
    {
        allLineRenderers.Remove(lineRenderer);
        Destroy(lineRenderer.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
