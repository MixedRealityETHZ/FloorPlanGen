using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class JsonVisualizerBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Model model = GameObject.FindGameObjectsWithTag("Model")[0].GetComponent<Model>();
        TextMeshPro mText = GetComponent<TextMeshPro>();
        var text = model.exportGraphToJson();
        mText.SetText(text);
    }
}
