using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    //[SerializeField]
    //private GameObject enemyPrefab;
    [SerializeField]
    private GameObject enemyHPSliderPrefab;
    [SerializeField]
    private Transform canvasTransform;
    //[SerializeField]
    //private float spawnTime;
    [SerializeField]
    private Transform[] wayPoints;
    [SerializeField]
    private PlayerHP playerHP;
    [SerializeField]
    private PlayerGold playerGold;
    private Wave currentWave;
    private int currentEnemyCount;
    public List<Enemy> enemyList;

    public bool spawnable = true;

    public int curEnemyCount => currentEnemyCount;
    public int maxEnemyCount => currentWave.maxEnemyCount;

	private void Awake()
	{
        enemyList = new List<Enemy>();
        //StartCoroutine("SpawnEnemy");
	}

    public void StartWave(Wave wave)
	{
        currentWave = wave;
        currentEnemyCount = currentWave.maxEnemyCount;
        StartCoroutine("SpawnEnemy");
	}

    private IEnumerator SpawnEnemy()
    {
        int spawnEnemyCount = 0;
        while (spawnEnemyCount < currentWave.maxEnemyCount && spawnable)
		{
            int enemyIndex = 0;
            int random = Random.Range(0, 3);
			if (random > 0)
			{
                enemyIndex = currentWave.enemyPrefabs.Length - 1;
			}
			else
			{
                enemyIndex = Random.Range(0, currentWave.enemyPrefabs.Length - 1);
            }
            GameObject clone = Instantiate(currentWave.enemyPrefabs[enemyIndex]);
            Enemy enemy = clone.GetComponent<Enemy>();

            enemy.Setup(this, wayPoints);
            enemyList.Add(enemy);

            SpawnEnemyHPSlider(clone);

            spawnEnemyCount++;

            yield return new WaitForSeconds(currentWave.spawnTime);
		}
	}

    public void DestroyEnemy(EnemyDestroyType type,Enemy enemy, int gold, int score)
	{
		if (type == EnemyDestroyType.Arrive)
		{
            playerHP.TakeDamage(1);
		}
		else if (type == EnemyDestroyType.Kill)
		{
            playerGold.curGold += gold;
            DBManager.score += score;
		}
        currentEnemyCount--;
        enemyList.Remove(enemy);
        Destroy(enemy.gameObject);
	}

    public void SpawnEnemyHPSlider(GameObject enemy)
	{
        GameObject sliderClone = Instantiate(enemyHPSliderPrefab);

        sliderClone.transform.SetParent(canvasTransform);
        sliderClone.transform.localScale = Vector3.one;
        sliderClone.GetComponent<SliderPositionAutoSetter>().Setup(enemy.transform);
        sliderClone.GetComponent<EnemyHPViewer>().Setup(enemy.GetComponent<EnemyHP>());
	}

    public void AllClear()
	{
        spawnable = false;
		for (int i = 0; i < enemyList.Count;i++)
		{
            Destroy(enemyList[i].gameObject);
		}
        enemyList.Clear();
	}
}
