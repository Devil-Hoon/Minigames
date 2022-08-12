using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardInfo : MonoBehaviour
{
    public Transform targetPos;
    public string cName;
    public int cIndex;
    public bool isFront;
    public bool isRotate;
    public bool isMove;

    public float turnSpeed;
    // Start is called before the first frame update
    void Start()
    {
        transform.eulerAngles = new Vector3(0, 180, 0);
    }

    // Update is called once per frame
    void Update()
    {
		if (isMove)
		{
            transform.position = Vector3.MoveTowards(transform.position, targetPos.position, Time.deltaTime * 3.0f);
			if (transform.position == targetPos.position)
			{
                isMove = false;
			}
		}
		else
		{
            if (isRotate && !isFront)
            {
                Vector3 rot = transform.eulerAngles;
                rot.y += Time.deltaTime * turnSpeed;
                transform.eulerAngles = rot;
                if (transform.eulerAngles.y > 0 && transform.eulerAngles.y < 180)
                {
                    rot = Vector3.zero;
                    transform.eulerAngles = rot;
                    isRotate = false;
                    isFront = true;
                }
            }
            else if (isRotate && isFront)
            {
                Vector3 rot = transform.eulerAngles;
                rot.y += Time.deltaTime * (turnSpeed * 1.5f);
                transform.eulerAngles = rot;
                if (transform.eulerAngles.y > 180)
                {
                    rot = new Vector3(0, 180, 0);
                    transform.eulerAngles = rot;
                    isRotate = false;
                    isFront = false;
                }
            }
        }
		
    }

    public void TurnCard()
	{
        isRotate = true;
	}
}
