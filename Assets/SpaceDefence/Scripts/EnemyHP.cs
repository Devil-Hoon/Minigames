using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHP : MonoBehaviour
{
	[SerializeField]
	private float maxHP;
	private float currentHP;
	private bool isDie = false;
	private Enemy enemy;
	private SpriteRenderer spriteRenderer;

	public float maxHp => maxHP;
	public float currentHp => currentHP;

	private void Awake()
	{
		currentHP = maxHP;
		enemy = GetComponent<Enemy>();
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	public void TakeDamage(float damage)
	{
		if (isDie)
		{
			return;
		}

		currentHP -= damage;

		StopCoroutine("HitAlphaAnimation");
		StartCoroutine("HitAlphaAnimation");

		if (currentHP <= 0)
		{
			isDie = true;
			enemy.OnDie(EnemyDestroyType.Kill);
			EffectSoundManager.instance.SpaceDefenceEnemyDestroy();
		}
	}

	private IEnumerator HitAlphaAnimation()
	{
		Color color = spriteRenderer.color;

		color.a = 0.4f;
		spriteRenderer.color = color;

		yield return new WaitForSeconds(0.05f);

		color.a = 1.0f;
		spriteRenderer.color = color;
	}
}
