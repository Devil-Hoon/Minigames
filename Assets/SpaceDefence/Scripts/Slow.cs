using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slow : MonoBehaviour
{
	public TowerWeapon towerWeapon;

	private void OnTriggerStay2D(Collider2D collision)
	{
		if (!collision.CompareTag("Enemy"))
		{
			return;
		}

		Movement2D movement2D = collision.GetComponent<Movement2D>();
		movement2D.movSpeed = movement2D.bMoveSpeed - movement2D.bMoveSpeed * towerWeapon.slow;
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (!collision.CompareTag("Enemy"))
		{
			return;
		}
		collision.GetComponent<Movement2D>().ResetMoveSpeed();
	}
}
