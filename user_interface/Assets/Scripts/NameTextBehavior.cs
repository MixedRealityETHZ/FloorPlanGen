using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NameTextBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string objectName = gameObject.transform.parent.gameObject.GetComponent<Node>().getObjectDisplayName();

        TextMeshPro mText = GetComponent<TextMeshPro>();
        mText.SetText(objectName);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
