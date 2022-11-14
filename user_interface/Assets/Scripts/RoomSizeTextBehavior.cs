using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;


public class RoomSizeTextBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float size = gameObject.transform.parent.gameObject.GetComponent<Node>().getObjectSize();

        TextMeshPro mText = GetComponent<TextMeshPro>();
        var text = size.ToString("0.0") + "m²";
        mText.SetText(text);
    }
}
