using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Blooding : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DestoryObject()
    {
        photonView.RPC("Destroy", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void Destroy()
    {
        Destroy(gameObject);
    }
}
