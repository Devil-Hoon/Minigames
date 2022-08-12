using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHPViewer : MonoBehaviour
{
	private EnemyHP enemyHP;
	private Slider hpSlider;

	public void Setup(EnemyHP enemyHP)
	{
		this.enemyHP = enemyHP;
		hpSlider = GetComponent<Slider>();
	}

	public void Update()
	{
		hpSlider.value = enemyHP.currentHp / enemyHP.maxHp;
	}
}
