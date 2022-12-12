using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;
using Dummiesman;
using System;

public class SendReceive : MonoBehaviourPun
{
    private PhotonView _photonView;

    private void OnEnable()
    {
        _photonView = GetComponent<PhotonView>();
    }

    public void sendBoundaryRequestClient()
    {
        //Debug.Log("sendBoundaryRequest");
        int boundaryID = 0; // get this from UI
        _SendBoundrayRequest2Server(boundaryID);

    }

    public void sendGraphClient()
    {
        //Debug.Log("sendGraph");
        _SendGraph2Server();

    }

    private void _SendBoundrayRequest2Server(int boundaryID)
    {
        _photonView.RPC("PunRPC_sendBoundaryRequest", RpcTarget.Others, boundaryID);
        Debug.Log("send out boundary request");
    }

    [PunRPC]
    private void PunRPC_sendBoundaryRequest(int boundaryID)
    {
        //if server send me back the boundary to client
        if (PhotonNetwork.NickName == "server")
        {
            _SendBoundary2Client(boundaryID);
        }
    }
    private void _SendBoundary2Client(int boundaryID)
    {
        string path = Application.dataPath + $"/Resources/boundary{boundaryID}.json"; //make it boundaryID
        
        if (!string.IsNullOrEmpty(path))
        {
            string jsonString = File.ReadAllText(path);
            _photonView.RPC("PunRPC_sendBoundary", RpcTarget.All, jsonString); //max length 32k
            Debug.Log("send out boundary");
        }
        
    }
    
    [PunRPC]
    private void PunRPC_sendBoundary(string jsonString)
    {
        GameSettingsSingleton.Instance.boundaryJsonString = jsonString;
        if (PhotonNetwork.NickName == "client")
        {
            //write boundary
            string path = Application.dataPath + "/Resources/boundary.json";
            File.WriteAllText(path, GameSettingsSingleton.Instance.boundaryJsonString);
        }
    }
    private void _SendGraph2Server()
    {
        //TODO update jsonString from tracked objects
        string path = Application.dataPath + "/Resources/graph.json";
        string jsonString = File.ReadAllText(path);
        // send graph to both and store in instance
        _photonView.RPC("PunRPC_sendGraph", RpcTarget.All, jsonString); //max length 32k
        Debug.Log("send out graph with json: " + jsonString);
    }

    [PunRPC]
    private void PunRPC_sendGraph(string jsonString)
    {
        GameSettingsSingleton.Instance.graphJsonString = jsonString;
        if (PhotonNetwork.NickName == "server")
        {
            //write json
            string path = Application.dataPath + "/Resources/graph.json";
            File.WriteAllText(path, GameSettingsSingleton.Instance.graphJsonString);

            //Send mesh to client when we have all information on graph
            sendMeshServer();
        }

    }

    public void sendMeshServer()
    {
        Debug.Log("Server spawning mesh");
        _SpawnMesh();
        // you can use the FileSystemWatcher to detect file changes
        //TODO we will need to change it to account for the fact that the mesh is not yet generated or will be overwritten

        _SendMeshData2Client();
    }

    private void _SendMeshData2Client()
    {
        //TODO generate mesh from rhino

        // split string into chunks
        string path = Application.dataPath + "/Resources/mesh.obj";
        string objString = File.ReadAllText(path);

        int chunkSize = 32000;
        int stringLength = objString.Length;
        List<string> objStringList = new List<string>();
        for (int i = 0; i < stringLength; i += chunkSize)
        {
            if (i + chunkSize > stringLength) chunkSize = stringLength - i;
            objStringList.Add(objString.Substring(i, chunkSize));
        }
        string[] objStringArray = objStringList.ToArray();

        Debug.Log($"string length: {stringLength}");
        Debug.Log($"string array length: {objStringArray.Length}");
        _photonView.RPC("PunPRC_sendMeshBuddle", RpcTarget.All, objStringArray);
    }

    [PunRPC]
    private void PunPRC_sendMeshBuddle(string[] objStringArray)
    {
        StartCoroutine(_sendMeshBuddle(objStringArray, 0.5f));
    }

    private IEnumerator _sendMeshBuddle(string[] objStringArray, float delay)
    {
        for(int i=0; i < objStringArray.Length; i++)
        {
            GameSettingsSingleton.Instance.meshJsonString += objStringArray[i];
            yield return new WaitForSeconds(delay);
            Debug.Log($"get {i} package");
        }
        Debug.Log($"received string with length of {GameSettingsSingleton.Instance.meshJsonString.Length}");

        // received all
        _SpawnMeshFromString(GameSettingsSingleton.Instance.meshJsonString);
    }

    private void _SpawnMeshFromString(string objString)
    {
        if (GameObject.Find("/mesh"))
        {
            Debug.Log("found mesh");
            Destroy(GameObject.Find("/mesh"));
        }

        var textStream = new MemoryStream(Encoding.UTF8.GetBytes(objString));
        var loadedObj = new OBJLoader().Load(textStream);
        loadedObj.name = "mesh";

        //Debug.Log("mesh pos " + loadedObj.transform.position);

        //loadedObj.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);

        //so the whole function can be deleted
        GameSettingsSingleton.Instance.meshJsonString = "";
        GameObject.FindGameObjectsWithTag("Model")[0].GetComponent<Model>().finishMeshGeneration(loadedObj);

    }

    private void _SpawnMesh()
    {
        if (GameObject.Find("/mesh"))
        {
            Destroy(GameObject.Find("/mesh"));
        }

        GameObject go = Resources.Load("mesh") as GameObject;
        GameObject meshGo = Instantiate(go, Vector3.zero, Quaternion.identity);
        meshGo.name = "mesh";
        //Debug.Log("Mesh " + meshGo.name + " spawned");
    }

    [PunRPC]
    private void PunRPC_SetNickName(string name)
    {
        gameObject.name = name;
    }
}
