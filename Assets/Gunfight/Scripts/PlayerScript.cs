using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PlayerScript : MonoBehaviourPunCallbacks, IPunObservable
{
    
    public CapsuleCollider2D bodyCollider;
    public Rigidbody2D rigid2D;
    public Animator animator;
    public PhotonView pView;
    public Transform bulletPos;
    public Transform granadePos;
    public float moveSpeed;
    public float jumpPower;
    public int EnemyScore;
    public bool isEnd;
    public bool isStart;
    public SpriteRenderer head;
    public SpriteRenderer body;
    public SpriteRenderer leg1;
    public SpriteRenderer leg2;
    
    private bool wallCheck;
    private float chargeTime;
	private bool isGround;
    private bool isBottom;
    private bool isDown;
    private bool isCharging;
    private int granadeCount;
    private bool granadeHit;
    
    //private Collider2D[] cols;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(transform.localScale);
        }
        else
        {
            transform.position = (Vector3)stream.ReceiveNext();
            transform.rotation = (Quaternion)stream.ReceiveNext();
            transform.localScale = (Vector3)stream.ReceiveNext();
        }
    }
    void Awake()
	{

	}
	// Start is called before the first frame update
	void Start()
    {
        granadeCount = 10;
        isGround = false;
        wallCheck = false;
        isEnd = false;
        isStart = false;
        isDown = false;
        isBottom = false;
    }

    // Update is called once per frame
    void Update()
    {
		if (pView.IsMine && !isEnd && isStart)
		{
			if (!granadeHit)
			{
                float axis = Input.GetAxisRaw("Horizontal");

                rigid2D.velocity = new Vector2(moveSpeed * axis, rigid2D.velocity.y);
                
                photonView.RPC("CharacterFlip", RpcTarget.AllBuffered, axis);
                if (axis < 0)
                {
                    animator.SetBool("IsMove", true);
                }
                else if (axis > 0)
                {
                    animator.SetBool("IsMove", true);
                }
                else
                {
                    animator.SetBool("IsMove", false);
                }

                if (Input.GetButtonDown("Jump") && (isGround||isBottom))
                {
                    pView.RPC("PlayerJump", RpcTarget.All);
                }

                if (Input.GetButtonDown("Down") && isGround && !isDown && !isBottom)
                {
                    pView.RPC("PlayerDown", RpcTarget.All);
                }


                float rot = transform.rotation.y;

                if (Input.GetButtonDown("Fire1"))
                {
					if (PhotonNetwork.LocalPlayer.IsLocal)
					{
                        PhotonNetwork.Instantiate("Prefabs/Bullet", bulletPos.position, transform.rotation.y == 0 ? new Quaternion(0, 0, 0, 0) : new Quaternion(0, 180, 0, 0))
                           .GetComponent<PhotonView>().RPC("DirRPC", RpcTarget.All, rot < 0 ? 1 : -1);
                        animator.SetTrigger("IsFire");
                        
                    }
                    
                    
                }
                if (Input.GetButtonDown("Fire2")&&isCharging)
                {
                    if (PhotonNetwork.LocalPlayer.IsLocal)
                    {
                        PhotonNetwork.Instantiate("Prefabs/Laser", bulletPos.position, Quaternion.identity)
                        .GetComponent<PhotonView>().RPC("DirRPC", RpcTarget.All, rot > 0 ? 1 : -1);
                        animator.SetTrigger("IsFire");
                        chargeTime = 0.0f;
                        isCharging = false;
                    }
                }
                if (Input.GetButtonDown("Fire3") && granadeCount > 0)
                {
                    if (PhotonNetwork.LocalPlayer.IsLocal)
                    {
                        PhotonNetwork.Instantiate("Prefabs/Granade", granadePos.position, Quaternion.identity)
                        .GetComponent<PhotonView>().RPC("DirRPC", RpcTarget.All, rot > 0 ? 1 : -1);
                        animator.SetTrigger("IsGranade");
                        granadeCount--;
                    }
                }
                if (!isCharging)
                {
                    if (PhotonNetwork.LocalPlayer.IsLocal)
                    {
                        chargeTime += Time.deltaTime;
                        if (chargeTime > 1.0f)
                        {
                            isCharging = true;
                        }
                    }
                }
            }
            
        }   
    }

    public void PlayerHit()
	{
        if (PhotonNetwork.LocalPlayer.IsLocal)
        {
            EnemyScore += 10000;
        }
	}

    public void PlayerLaserHit()
    {
        if (PhotonNetwork.LocalPlayer.IsLocal)
        {
            EnemyScore += 100000;
        }
	}
    public void PlayerGranadeHit()
    {
        if (PhotonNetwork.LocalPlayer.IsLocal)
        {
            PhotonNetwork.Instantiate("Prefabs/Blood", transform.position, Quaternion.identity);
            pView.RPC("PlayerGranadeHitMove", RpcTarget.All);
            EnemyScore += 1000000;
        }
    }
    public void OnTriggerEnter2D(Collider2D collision)
	{

        ContactPoint2D[] contacts = new ContactPoint2D[10];
        collision.GetContacts(contacts);
        Vector2 contactPos = contacts[0].point;
		if (collision.gameObject.layer == 10)
		{
            bodyCollider.isTrigger = false;
            wallCheck = true;
		}
		if (collision.gameObject.layer == 9 && rigid2D.velocity.y <= 0 && !isDown)
        {
            rigid2D.velocity = Vector2.zero;
            bodyCollider.isTrigger = false;
			if (contactPos.y < transform.position.y)
			{
                isGround = true;
            }
		}
		if (collision.gameObject.layer == 12 && rigid2D.velocity.y <= 0)
		{
            rigid2D.velocity = Vector2.zero;
            bodyCollider.isTrigger = false;
            if (contactPos.y < transform.position.y)
            {
                isBottom = true;
            }
        }
	}

	public void OnCollisionStay2D(Collision2D collision)
	{
        if (collision.gameObject.layer == 10)
        {
            bodyCollider.isTrigger = false;
            wallCheck = true;
        }
        
    }

    public void OnTriggerStay2D(Collider2D collision)
	{

        if (collision.gameObject.layer == 10)
        {
            bodyCollider.isTrigger = false;
            wallCheck = true;
        }
        if (collision.gameObject.layer == 9 && rigid2D.velocity.y <= 0 && !isDown)
        {
            rigid2D.velocity = Vector2.zero;
            bodyCollider.isTrigger = false;
            isGround = true;
            granadeHit = false;
        }
        if (collision.gameObject.layer == 12 && rigid2D.velocity.y <= 0)
        {
            rigid2D.velocity = Vector2.zero;
            bodyCollider.isTrigger = false;
            isBottom = true;
            granadeHit = false;
        }
    }

	public void OnTriggerExit2D(Collider2D collision)
	{
        if (collision.gameObject.layer == 10)
        {
            wallCheck = false;
        }
		if (collision.gameObject.layer == 9 && isDown)
		{
            isDown = false;
		}
    }

	public void OnCollisionExit2D(Collision2D collision)
	{
		if (collision.gameObject.layer == 10)
		{
			wallCheck = false;
		}
        if (collision.gameObject.layer == 9)
        {
            isGround = false;
        }
		if (collision.gameObject.layer == 12)
		{
            isBottom = false;
		}
    }

    [PunRPC]
    public void CharacterFlip(float axis)
    {
        if (axis < 0)
        {
            transform.rotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
        }
        else if (axis > 0)
        {
            transform.rotation = new Quaternion(0.0f, 180.0f, 0.0f, 0.0f);
        }
    }
    [PunRPC]
    public void PlayerGranadeHitMove()
    {
        isGround = false;
        granadeHit = true;
        if (!wallCheck)
        {
            bodyCollider.isTrigger = true;
        }
        rigid2D.velocity = Vector2.zero;
        rigid2D.AddForce(Vector2.up * 200);
    }
    [PunRPC]
    public void PlayerJump()
    {
        isGround = false;
        isBottom = false;
        if (!wallCheck)
        {
            bodyCollider.isTrigger = true;
        }
        rigid2D.velocity = Vector2.zero;
        rigid2D.AddForce(Vector2.up * jumpPower);
    }
    [PunRPC]
    public void PlayerDown()
    {
        isGround = false;
        if (!wallCheck)
        {
            bodyCollider.isTrigger = true;
        }
        rigid2D.velocity = Vector2.zero;
        rigid2D.AddForce(Vector2.down * jumpPower);
        
        isDown = true;
    }


    [PunRPC]
    public void SetBlack()
	{
        head.color = Color.gray;
        body.color = Color.gray;
        leg1.color = Color.gray;
        leg2.color = Color.gray;
	}

    [PunRPC]
    public void SetWhite()
    {

        head.color = Color.white;
        body.color = Color.white;
        leg1.color = Color.white;
        leg2.color = Color.white;
    }
}
