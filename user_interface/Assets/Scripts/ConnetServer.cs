using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnetServer : MonoBehaviourPunCallbacks
{
    public Model model;
    
    void Start()
    {
        PhotonNetwork.NickName = GameSettingsSingleton.Instance.userName;
        Debug.Log("connect to server...");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("server connected");
        PhotonNetwork.JoinLobby();
        StartCoroutine(CreateJoinRoom(0.1f));
        //model.GetComponent<Model>().onServerStart();
    }

    public override void OnJoinedRoom()
    {
        _SetCameraOwnership();
        model.onServerStart();
    }

    private void _SetCameraOwnership()
    {
        CameraControl cameraControl = Camera.main.GetComponent<CameraControl>();
        cameraControl.SetOwnership2Client();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"server disconnected due to {cause}");
    }

    private IEnumerator CreateJoinRoom(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            if (PhotonNetwork.IsConnected && PhotonNetwork.InLobby)
            {
                RoomOptions options = new RoomOptions();
                options.MaxPlayers = 5;

                PhotonNetwork.JoinOrCreateRoom(GameSettingsSingleton.Instance.roomName, options, TypedLobby.Default);

                Debug.Log(string.Format("joined {0}", GameSettingsSingleton.Instance.roomName));

                break;
            }
        }
        
    }

}
