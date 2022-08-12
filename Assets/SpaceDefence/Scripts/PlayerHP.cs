using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour
{
	[SerializeField]
	private Image imageScreen;
	[SerializeField]
	private int maxHP = 20;
	private int currentHP;

	public int maxHp => maxHP;
	public int currentHp => currentHP;

	private void Awake()
	{
		currentHP = maxHP;
	}

	public void TakeDamage(int damage)
	{
		currentHP -= damage;

		StopCoroutine("HitAlphaAnimation");
		StartCoroutine("HitAlphaAnimation");

		if (currentHP <= 0)
		{
			currentHP = 0;
		}
	}
	public int HPToScore()
	{
		if (currentHP > 0)
		{
			return currentHP * 1000;
		}
		return 0;
	}

	private IEnumerator HitAlphaAnimation()
	{
		Color color = imageScreen.color;
		color.a = 0.4f;
		imageScreen.color = color;

		while (color.a >= 0.0f)
		{
			color.a -= Time.deltaTime;
			imageScreen.color = color;
			yield return null;
		}
	}
}
