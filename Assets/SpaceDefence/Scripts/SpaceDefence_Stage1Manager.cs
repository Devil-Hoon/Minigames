using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using MySql.Data;
using MySql.Data.MySqlClient;
using TMPro;
[System.Serializable]
public struct RoundSystem
{
    public WaveSystem waveSystem;
    public EnemySpawner enemySpawner;
    public int waveIndex;

    public void ViewOff()
	{
        enemySpawner.gameObject.SetActive(false);
	}
    public void Initialize()
	{
        waveIndex = 0;
        waveSystem.SetWaveIndex(waveIndex);
        enemySpawner.gameObject.SetActive(true);
    }
}
public class SpaceDefence_Stage1Manager : MonoBehaviour
{
    [SerializeField]
    RoundSystem[] rounds;
    [SerializeField]
    Sprite[] roundImgs;
    [SerializeField]
    Sprite[] stageImgs;
    //public WaveSystem waveSystem;
    public GameObject towerCollection;
    public TowerSpawner towerSpawner;
    //public EnemySpawner enemySpawner;
    public TowerDataViewer towerDataViewer;
    public PlayerHP playerHP;
    public GameObject panelDefeat;
    public GameObject panelStageClear;
    public GameObject buildPanel;
    public GameObject readyToPlayImage;
    public GameObject readyToBuildImage;
    public GameObject waveClearImage;
    public GameObject roundImage;
    public TextTMPViewer tmpViewer;

    public Button waveStartBtn;
    public Button upgradeBtn;
    public Button sellBtn;

    private float readyTime;
    private float buildImageShowTime;
    private float clearImageShowTime;
    private float playImageShowTime;
    private float waveClearImageShowTime;
    private int round;

    private bool ready;
    private bool buildImageShowCheck;
    private bool buildEnd;
    private bool playImageShowCheck;
    private bool waveStart;
    private bool waveClear;
    private bool roundClear;
    private bool gameOver;


	private void OnApplicationQuit()
	{
        DBManager.LogOut();
	}
	// Start is called before the first frame update
	void Start()
    {
        BGSoundManager.instance.StopBGM();
        BGSoundManager.instance.PlaySpaceDefenceGameBGM();
		for (int i = 0; i < rounds.Length - 1; i++)
		{
            rounds[i + 1].ViewOff();
		}
        tmpViewer.waveSystem = rounds[0].waveSystem;
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
		if (!ready)
		{
            readyTime += Time.deltaTime;
            roundImage.GetComponent<Image>().sprite = roundImgs[0];
            roundImage.SetActive(true);
            if (readyTime > 2.0f)
			{
                readyTime = 0.0f;
                roundImage.SetActive(false);
                ready = true;
			}
		}
		else
		{
			if (!buildImageShowCheck)
			{
                buildImageShowTime += Time.deltaTime;
                readyToBuildImage.SetActive(true);

				if (buildImageShowTime > 1.5f)
				{
                    buildImageShowTime = 0.0f;
                    readyToBuildImage.SetActive(false);
                    buildImageShowCheck = true;
                    towerDataViewer.buildable = true;
                    towerSpawner.buildCancel = false;
                    buildPanel.SetActive(true);
                    towerSpawner.buildTimeCheck = true;
                    BGSoundManager.instance.StopBGM();
                    BGSoundManager.instance.PlaySpaceDefenceBuildBGM();

                }
			}
			else
			{
                if (buildEnd)
                {
                    if (!playImageShowCheck)
                    {
                        playImageShowTime += Time.deltaTime;

                        if (playImageShowTime > 1.5f)
                        {
                            readyToPlayImage.SetActive(false);
                            playImageShowTime = 0.0f;
                            playImageShowCheck = true;
                            waveStart = true;

                            rounds[round].waveSystem.StartStageWave(rounds[round].waveIndex);
                        }
                    }
                    else
                    {

                        if (!waveClear)
                        {
                            if (playerHP.currentHp <= 0)
                            {
                                rounds[round].enemySpawner.AllClear();
                                BGSoundManager.instance.StopBGM();
                                if (!gameOver)
                                {
                                    RankManager.RankManagerAllClear();
                                    RankManager.CheckRankingDataEmpty();
                                    RankManager.RankDBCheck();
                                    gameOver = true;
                                }
                                panelDefeat.SetActive(true);
                            }
                            else
                            {
                                if (rounds[round].waveSystem.currentEnemyCount == 0)
                                {
                                    waveClear = true;
                                    waveClearImage.SetActive(true);
                                }
                            }
                        }
                        else
                        {
                            waveClearImageShowTime += Time.deltaTime;
                            if (waveClearImageShowTime > 1.5f)
                            {
                                if (round < rounds.Length - 1)
                                {
                                    if (rounds[round].waveIndex < 4 && !roundClear)
                                    {
                                        waveClearImage.SetActive(false);
                                        waveClearImageShowTime = 0.0f;
                                        rounds[round].waveIndex++;
                                        rounds[round].waveSystem.SetWaveIndex(rounds[round].waveIndex);
                                        readyToPlayImage.GetComponent<Image>().sprite = stageImgs[rounds[round].waveIndex];
                                        
                                        waveStart = false;
                                        waveClear = false;
                                        playImageShowCheck = false;
                                        buildEnd = false;
                                        buildImageShowCheck = false;

                                        waveStartBtn.interactable = true;
                                        upgradeBtn.interactable = true;
                                        sellBtn.interactable = true;
                                    }
                                    else
                                    {
                                        if (!roundClear)
                                        {
                                            waveClearImage.SetActive(false);
                                            waveClearImageShowTime = 0.0f;
                                            roundImage.GetComponent<Image>().sprite = roundImgs[round + 1];
                                            roundImage.SetActive(true);
                                            roundClear = true;
                                        }

                                        if (clearImageShowTime < 2.0f)
                                        {
                                            clearImageShowTime += Time.deltaTime;
                                        }
                                        else
                                        {
                                            rounds[round].ViewOff();
                                            round++;
                                            rounds[round].Initialize();
                                            EnemySpawnerUpdate(rounds[round].enemySpawner);
                                            towerSpawner.enemySpawner = rounds[round].enemySpawner;
                                            tmpViewer.waveSystem = rounds[round].waveSystem;
                                            clearImageShowTime = 0.0f;
                                            roundImage.SetActive(false);
                                            roundClear = false;
                                            waveStart = false;
                                            waveClear = false;
                                            playImageShowCheck = false;
                                            buildEnd = false;
                                            buildImageShowCheck = false;
                                            waveStartBtn.interactable = true;
                                            upgradeBtn.interactable = true;
                                            sellBtn.interactable = true;
                                        }

                                    }
                                }
                                else
                                {
                                    if (rounds[round].waveIndex < 4 && !roundClear)
                                    {
                                        waveClearImage.SetActive(false);
                                        waveClearImageShowTime = 0.0f;
                                        rounds[round].waveIndex++;
                                        rounds[round].waveSystem.SetWaveIndex(rounds[round].waveIndex);
                                        readyToPlayImage.GetComponent<Image>().sprite = stageImgs[rounds[round].waveIndex];

                                        waveStart = false;
                                        waveClear = false;
                                        playImageShowCheck = false;
                                        buildEnd = false;
                                        buildImageShowCheck = false;

                                        waveStartBtn.interactable = true;
                                        upgradeBtn.interactable = true;
                                        sellBtn.interactable = true;

                                    }
                                    else if(rounds[round].waveIndex >= 4 && !roundClear)
                                    {
                                        waveClearImage.SetActive(false);
                                        panelStageClear.SetActive(true);
                                        RankManager.RankManagerAllClear();
                                        RankManager.CheckRankingDataEmpty();
                                        RankManager.RankDBCheck();
                                    }
                                }
                                

                            }


                        }
                    }
                }
			}
		}
    }
    private void Initialize()
    {
        round = 0;
        roundClear = false;
        gameOver = false;
        readyTime = 0.0f;
        buildImageShowTime = 0.0f;
        clearImageShowTime = 0.0f;
        playImageShowTime = 0.0f;
        rounds[round].Initialize();
        ready = false;
        buildImageShowCheck = false;
        buildEnd = false;
        waveStart = false;
        waveClear = false;
        playImageShowCheck = false;

        panelDefeat.SetActive(false);
        panelStageClear.SetActive(false);
        buildPanel.SetActive(false);
    }

    public void BackToMain()
	{
        SceneManager.LoadScene(15);
	}
    public void RankingToJson(JsonData jsonData)
    {
        for (int i = 0; i < RankManager.users.Length; i++)
        {
            JsonMembers data = new JsonMembers();
            data.RANK = i + 1;
            data.USER = RankManager.users[i];
            data.Score = RankManager.scores[i];
            jsonData.result.Add(data);
        }
    }
    public void WaveStart()
	{
        buildPanel.SetActive(false);
        readyToPlayImage.GetComponent<Image>().sprite = stageImgs[rounds[round].waveIndex];
        readyToPlayImage.SetActive(true);
        towerSpawner.buildCancel = true;
        towerDataViewer.buildable = false;
        towerSpawner.buildTimeCheck = false;
        buildEnd = true;
        BGSoundManager.instance.StopBGM();
        BGSoundManager.instance.PlaySpaceDefenceGameBGM();

        waveStartBtn.interactable = false;
        upgradeBtn.interactable = false;
        sellBtn.interactable = false;
        //rounds[round].waveSystem.StartStageWave(rounds[round].waveIndex);
    }
    private void EnemySpawnerUpdate(EnemySpawner enemySpawner)
	{
		if (towerCollection.transform.childCount == 0)
		{
            return;
		}

		for (int i = 0; i < towerCollection.transform.childCount; i++)
		{
            TowerWeapon tw = towerCollection.transform.GetChild(i).Find("Tower").GetComponent<TowerWeapon>();

            tw.SetEnemySpawenr(enemySpawner);
		}
	}
}
