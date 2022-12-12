using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettingsSingleton : MonoBehaviourPunCallbacks
{
    public static GameSettingsSingleton Instance;

    public string userName;
    public string roomName;
    [HideInInspector]
    public string graphJsonString;
    [HideInInspector]
    public string meshJsonString;
    [HideInInspector]
    public string boundaryJsonString;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            graphJsonString = "";
            meshJsonString = "";
            boundaryJsonString = "";
        }
        else
        {
            if (Instance == this) return;
            Destroy(Instance.gameObject);
            Instance = this;
            graphJsonString = "";
            meshJsonString = "";
            boundaryJsonString = "";
        }
    }
}
