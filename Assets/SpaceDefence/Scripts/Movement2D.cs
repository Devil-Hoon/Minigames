using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement2D : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 0.0f;
    [SerializeField]
    private Vector3 moveDirection = Vector3.zero;
    private float baseMoveSpeed;

    public bool isStay = false;

    public float movSpeed
	{
        set => moveSpeed = Mathf.Max(0, value);
        get => moveSpeed;
	}

    public float bMoveSpeed
	{
        set => baseMoveSpeed = Mathf.Max(0, value);
        get => baseMoveSpeed;
	}
	private void Awake()
	{
        baseMoveSpeed = moveSpeed;
	}
	// Update is called once per frame
	void Update()
    {
		
		transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    public void MoveTo(Vector3 direction)
	{
        moveDirection = direction;
		if (direction == Vector3.right)
		{
			transform.rotation = Quaternion.Euler(0.0f, 0.0f, -90.0f);
		}
		else if (direction == Vector3.left)
		{
			transform.rotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
		}
		else if (direction == Vector3.up)
		{
			transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
		}
		else
		{
			transform.rotation = Quaternion.Euler(0.0f, 0.0f, 180.0f);
		}
	}

    public void ResetMoveSpeed()
	{
        moveSpeed = baseMoveSpeed;
	}
}
