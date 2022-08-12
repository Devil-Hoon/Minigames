using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Laser : MonoBehaviourPunCallbacks
{
    public PhotonView pView;
    int dir;
    bool once = false;
    bool isDmg = false;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 1.5f);
        if (!once)
        {
            once = true;
            EffectSoundManager.instance.Laser();
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.right * 15 * Time.deltaTime * dir);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Map" || collision.tag == "EndMap")
        {
            pView.RPC("DestroyRPC", RpcTarget.AllBuffered);
        }

        if (!pView.IsMine && collision.tag == "Player" && collision.GetComponent<PhotonView>().IsMine && !isDmg)
        {
            collision.GetComponent<PlayerScript>().PlayerLaserHit();
            ContactPoint2D[] colls = new ContactPoint2D[10];
            collision.GetContacts(colls);
            pView.RPC("DamageOnce", RpcTarget.AllBuffered);
            PhotonNetwork.Instantiate("Prefabs/Blood", transform.position, Quaternion.identity);
            pView.RPC("DestroyRPC", RpcTarget.AllBuffered);
        }
    }


    [PunRPC]
    public void DirRPC(int dir)
    {
        this.dir = dir;
    }

    [PunRPC]
    public void DestroyRPC()
    {
        Destroy(gameObject);
    }

    [PunRPC]
    public void DamageOnce()
	{
        isDmg = true;
	}
}
