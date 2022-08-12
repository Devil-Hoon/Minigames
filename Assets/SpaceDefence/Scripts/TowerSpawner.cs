using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject towerObjectCollection;
    [SerializeField]
    private TowerTemplate[] towerTemplate;
    [SerializeField]
    public EnemySpawner enemySpawner;
    [SerializeField]
    private PlayerGold playerGold;
    [SerializeField]
    private SystemTextViewer systemTextViewer;
    private bool isOnTowerButton = false;
    private GameObject followTowerClone = null;
    private int towerType;

    public bool buildCancel = false;
    public bool buildTimeCheck = false;

    public GameObject followTower => followTowerClone;

	private void Update()
	{
		if (buildTimeCheck)
		{
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                ReadyToSpawnTower(0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                ReadyToSpawnTower(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                ReadyToSpawnTower(2);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                ReadyToSpawnTower(3);
            }
        }
    }

	public void ReadyToSpawnTower(int type)
	{
        towerType = type;
		if (isOnTowerButton)
		{
            return;
		}
		if (towerTemplate[towerType].weapon[0].cost > playerGold.curGold)
		{
            systemTextViewer.PrintText(SystemType.Money);
            return;
		}
        isOnTowerButton = true;
        followTowerClone = Instantiate(towerTemplate[towerType].followTowerPrefab);
        StartCoroutine("OnTowerCancelSystem");
	}
    public void SpawnTower(Transform tileTansform)
	{
		if (!isOnTowerButton)
		{
            return;
		}
        Tile tile = tileTansform.GetComponent<Tile>();

		if (tile.isBuildTower)
		{
            systemTextViewer.PrintText(SystemType.Build);
            return;
		}

		if (towerObjectCollection.transform.childCount > 0)
        { 
            for (int i = 0; i < towerObjectCollection.transform.childCount; i++)
            {
				if ((tileTansform.localPosition.x == towerObjectCollection.transform.GetChild(i).localPosition.x)
                    && (tileTansform.localPosition.y == towerObjectCollection.transform.GetChild(i).localPosition.y))
				{
                    systemTextViewer.PrintText(SystemType.Build);
                    return;
				}
            }
        }
        
        isOnTowerButton = false;
        tile.isBuildTower = true;
        playerGold.curGold -= towerTemplate[towerType].weapon[0].cost;
        Vector3 position = tileTansform.position + Vector3.back;
        GameObject clone = Instantiate(towerTemplate[towerType].towerPrefabs, position, Quaternion.identity);
        clone.transform.SetParent(towerObjectCollection.transform);
        clone.GetComponentInChildren<TowerWeapon>().Setup(this, enemySpawner, playerGold, tile);
        OnBuffAllTowers();
        Destroy(followTowerClone);
        StopCoroutine("OnTowerCancelSystem");
	}

    private IEnumerator OnTowerCancelSystem()
	{
		while (true)
		{
			if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1) || buildCancel)
			{
                isOnTowerButton = false;
                Destroy(followTowerClone);
                break;
			}

            yield return null;
		}
	}

    public void OnBuffAllTowers()
	{
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");
		for (int i = 0; i < towers.Length; i++)
		{
            TowerWeapon weapon = towers[i].GetComponent<TowerWeapon>();

			if (weapon.wType == WeaponType.Buff)
			{
                weapon.OnBuffAroundTower();
			}
		}
	}
}
