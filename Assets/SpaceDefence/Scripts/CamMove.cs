using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMove : MonoBehaviour
{
    private Camera cam;
    [SerializeField]
    private float maxX, maxY;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        cam.transform.position = Vector3.zero + Vector3.back * 10;
        maxX = 0.0f;
        maxY = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("CamHorizontal");
        float y = Input.GetAxis("CamVertical");

        float moveX = 0.0f, moveY = 0.0f;

		if (cam.transform.position.x >= 0 && cam.transform.position.x <= maxX)
		{
			moveX = x * Time.deltaTime * 300;
			if (moveX + cam.transform.position.x < 0)
			{
                moveX = 0 - cam.transform.position.x;
			}
			else if (moveX + cam.transform.position.x > maxX)
			{
                moveX = maxX - cam.transform.position.x;
			}
		}

		if (cam.transform.position.y >= 0 && cam.transform.position.y <= maxY)
		{
			moveY = y * Time.deltaTime * 300;
            if (moveY + cam.transform.position.y < 0)
            {
                moveY = 0 - cam.transform.position.y;
            }
            else if (moveY + cam.transform.position.y > maxY)
            {
                moveY = maxY - cam.transform.position.y;
            }
        }
		cam.transform.position = new Vector3(cam.transform.position.x + moveX, cam.transform.position.y + moveY, cam.transform.position.z);
    }

    public void SetMoveMax(float x, float y)
	{
        maxX = x;
        maxY = y;
	}
}
