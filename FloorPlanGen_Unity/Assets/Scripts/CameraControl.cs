using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    // Start is called before the first frame update
    public void SetOwnership2Client()
    {
        PhotonView pv = GetComponent<PhotonView>();
        if (pv.IsMine) return;

        if (PhotonNetwork.NickName == "client")
        {
            Debug.Log("request ownership");
            pv.RequestOwnership();
        }
    }
}
