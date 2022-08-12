using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Bullet : MonoBehaviourPunCallbacks
{
    public PhotonView pView;
    int dir;
    bool once = false;
    bool isDmg = false;
    bool isMove = true;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 1.5f);
		if (!once)
		{
            once = true;
            EffectSoundManager.instance.GunShot();
        }
    }

    // Update is called once per frame
    void Update()
    {
		if (isMove)
		{
            transform.Translate(Vector3.right * 15 * Time.deltaTime * dir);
        }
    }


	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.tag == "Map" || collision.tag == "EndMap")
		{
            if (gameObject != null)
            {
                pView.RPC("DestroyRPC", RpcTarget.AllBuffered);
            }
        }

		if (!pView.IsMine && collision.tag == "Player" && collision.GetComponent<PhotonView>().IsMine && !isDmg)
		{
			if (isMove)
			{
                isMove = false;
                isDmg = false;
                ContactPoint2D[] colls = new ContactPoint2D[10];
                collision.GetContacts(colls);
                PhotonNetwork.Instantiate("Prefabs/Blood", transform.position, Quaternion.identity);
                collision.GetComponent<PlayerScript>().PlayerHit();
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
    public void MoveFalse()
	{
        isMove = false;
	}
}
