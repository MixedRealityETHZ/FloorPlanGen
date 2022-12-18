using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VisualizeData : MonoBehaviour
{
    void Update()
    {
        GetComponent<TextMeshProUGUI>().text = string.Format(
            "Graph Data: {0}\nMesh Data: {1}",
            GameSettingsSingleton.Instance.graphJsonString,
            GameSettingsSingleton.Instance.meshJsonString
        );
    }

    private void _SpawnMesh()
    {
        GameObject go = Resources.Load("mesh") as GameObject;
        Instantiate(go, Vector3.zero, Quaternion.identity);
    }

}
