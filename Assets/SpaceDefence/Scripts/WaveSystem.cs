using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSystem : MonoBehaviour
{
	[SerializeField]
	private Wave[] waves;
	[SerializeField]
	private EnemySpawner enemySpawner;
	private int currentWaveIndex = -1;

	public int currentWave => currentWaveIndex + 1;
	public int maxWave => waves.Length;
	public int currentEnemyCount => enemySpawner.curEnemyCount;

	public void SetWaveIndex(int index)
	{
		currentWaveIndex = index;
	}
	public void StartStageWave(int index)
	{
		if (index < waves.Length)
		{
			enemySpawner.StartWave(waves[index]);
		}
	}

	public void StartWave()
	{
		if (enemySpawner.enemyList.Count == 0 && currentWaveIndex < waves.Length - 1)
		{
			currentWaveIndex++;
			enemySpawner.StartWave(waves[currentWaveIndex]);
		}
	}
}

[Serializable]
public struct Wave
{
	public float spawnTime;
	public int maxEnemyCount;
	public GameObject[] enemyPrefabs;
}
