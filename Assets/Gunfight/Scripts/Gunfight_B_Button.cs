using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client;

public class B_Button : MonoBehaviourPun
{
    public void BackToMain()
	{
        PhotonNetwork.LeaveRoom();
	}
}
