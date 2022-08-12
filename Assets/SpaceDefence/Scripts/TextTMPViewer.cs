using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextTMPViewer : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI textPlayerHP;
	[SerializeField]
	private TextMeshProUGUI textPlayerGold;
	[SerializeField]
	private TextMeshProUGUI textWave;
	[SerializeField]
	private TextMeshProUGUI textEnemyCount;
	[SerializeField]
	private TextMeshProUGUI textPlayerScore;
	[SerializeField]
	private PlayerHP playerHP;
	[SerializeField]
	private PlayerGold playerGold;
	[SerializeField]
	public WaveSystem waveSystem;
	[SerializeField]
	private EnemySpawner enemySpawner;

	private void Update()
	{
		textPlayerHP.text = playerHP.currentHp + "/" + playerHP.maxHp;
		textPlayerGold.text = playerGold.curGold.ToString();
		if (waveSystem != null)
		{
			textWave.text = waveSystem.currentWave + "/" + waveSystem.maxWave;
		}
		else
		{
			textWave.text = 0 + "/" + 0;
		}
		textEnemyCount.text = enemySpawner.curEnemyCount + "/" + enemySpawner.maxEnemyCount;
		textPlayerScore.text = DBManager.score <= 0 ? "0" : string.Format("{0:#,###}", DBManager.score);
	}
}
