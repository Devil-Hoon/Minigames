using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.Demo.Cockpit.Forms;

public class Granade : MonoBehaviourPunCallbacks
{
    public Rigidbody2D rb2D;
    public PhotonView pView;
    int dir;
    bool isBoom;
    bool particle;
    bool once;
    float count;
    // Start is called before the first frame update
    void Start()
    {
        once = false;
        isBoom = false;
        count = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
		if (!once)
        {
            once = true;
            rb2D.velocity = Vector2.zero;
            rb2D.AddForce(Vector2.up * 120 + Vector2.right * 300 * dir);
        }


        count += Time.deltaTime;
		if (count > 2.0f)
		{
            isBoom = true;
			if (!particle)
            {
				if (PhotonNetwork.MasterClient.IsMasterClient)
                {
                    particle = true;
                    PhotonNetwork.Instantiate("Prefabs/Explosion", transform.position, Quaternion.identity);
                }
                pView.RPC("AnimOnce", RpcTarget.AllBuffered);
            }
            
        }
		if (count > 2.1f && isBoom)
		{
            count = 0.0f;
            pView.RPC("DestroyRPC", RpcTarget.AllBuffered);
        }
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
		if (isBoom)
		{
            if (!pView.IsMine && collision.tag == "Player" && collision.GetComponent<PhotonView>().IsMine)
            {
                collision.GetComponent<PlayerScript>().PlayerGranadeHit();
                isBoom = false;
                if (gameObject != null)
                {
                    pView.RPC("DestroyRPC", RpcTarget.AllBuffered);
                }
            }
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
    public void AnimOnce()
	{
        particle = true;
	}
}
