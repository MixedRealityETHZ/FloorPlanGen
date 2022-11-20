using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NameTextBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string objectName = gameObject.transform.parent.gameObject.GetComponent<Node>().getObjectName();

        TextMeshPro mText = GetComponent<TextMeshPro>();
        mText.SetText(UppercaseFirst(objectName));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    string UppercaseFirst(string s)
    {
        if (string.IsNullOrEmpty(s))
        {
            return string.Empty;
        }
        return char.ToUpper(s[0]) + s.Substring(1);
    }
}
