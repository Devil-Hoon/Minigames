using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Explosion : MonoBehaviourPunCallbacks
{
    bool once = false;
    // Start is called before the first frame update
    void Start()
    {
        if (!once)
        {
            once = true;
            EffectSoundManager.instance.Explosion();
        }
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
