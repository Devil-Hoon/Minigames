using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Newtonsoft.Json;
using MySql.Data;
using MySql.Data.MySqlClient;
public enum LEVEL_TWO
{
	ONE = 7,
	TWO = 9,
	THREE = 11,
	FOUR = 13,
	FIVE = 15,
}
public class CardMemoryGame_Stage2Manager : MonoBehaviour
{
	public Camera mainCam;
	[Header("Canvas")]
	public GameObject dataSaveCanvas;

	[Header("Button")]
	public GameObject restartBtn;
	public GameObject backToMainBtn;
	public GameObject dataSave;

	[Header("Card Prefabs")]
	public GameObject[] allCards;
	public GameObject[] playCards;

	[Header("Stage Level")]
	public GameObject[] stage2Levels;

	[Header("Round Image")]
	public GameObject stageClear;
	public GameObject stageFail;
	public GameObject[] stage2Rounds;

	[Header("Score")]
	public Text userScore;


	public Transform[] cardPos;
	public GameObject timeBar;
	public int[] randomMake;
	private int maxCard;

	private int allCardsNum;
	private LEVEL_TWO level;

	private bool cardSetEnd;
	private bool gameStart;
	private bool gameFailed;
	private int clickCount;
	private bool twoCardFront;
	private int cIndex1, cIndex2;
	private bool cardAllTurn;
	private bool once;
	private bool gameAllClear;
	private bool clearEffect;
	private float waitTime;
	private float viewTime;
	private float time;
	private int saveIndexMem;
	private int saveIndex;
	private bool saveComp;
	private bool save;
	private bool clearComp;
	private bool timeTempScore;
	private float timeScore;
	private float tempTime;
	private bool tempTimeCheck;

	private bool checkTime;

	private int beforeRank;
	private int rank;

	private bool gameSave;
	private bool gameNoSave;

	private void OnApplicationQuit()
	{
		DBManager.LogOut();
	}
	// Start is called before the first frame update
	void Start()
	{
		level = LEVEL_TWO.ONE;
		GameInit(level);
	}

	// Update is called once per frame
	void Update()
	{
		UserInfoSet();
		if (cardSetEnd)
		{
			if (gameStart)
			{
				UserInfoSet();
				if (!gameFailed)
				{
					if (GameEnd())
					{
						if (!gameAllClear)
						{
							if (!timeTempScore)
							{
								if (!tempTimeCheck)
								{
									tempTime = time;
									tempTimeCheck = true;
								}

								if (tempTime < 0.0f)
								{
									checkTime = true;
									tempTime = 0.0f;
								}
								if (tempTime < timeScore - 1.0f)
								{
									if (timeScore == float.MaxValue)
									{
										timeScore = tempTime;
									}
									else
									{
										timeScore = tempTime;
										DBManager.score += 100;
									}
								}
								tempTime -= Time.deltaTime * 10.0f;
								timeBar.GetComponent<TimeBar>().SetTimeBarValue(tempTime);
								
						
								if (checkTime)
								{
									timeTempScore = true;
								}
								
							}
							else
							{
								if (!clearEffect)
								{
									clearEffect = true;
									EffectSoundManager.instance.Stop();
									BGSoundManager.instance.StopBGM();
									EffectSoundManager.instance.CardMemoryGameStageClear();
								}
								stageClear.SetActive(true);
								waitTime -= Time.deltaTime;
								if (waitTime < 0.0f)
								{
									waitTime = 0.0f;
									if (level != LEVEL_TWO.FIVE)
									{
										SetLevel();
										GameInit(level);
									}
									else if (level == LEVEL_TWO.FIVE)
									{
										gameAllClear = true;
									}
								}
							}
							
						}
						else
						{
							BGSoundManager.instance.StopBGM();
							EffectSoundManager.instance.Stop();
							DBManager.stageNum = StageNum.THREE;
							SceneManager.LoadScene(9);
						}
						
					}
					else
					{
						time -= Time.deltaTime;
						if (time < 0.0f)
						{
							time = 0.0f;
							gameFailed = true;
							BGSoundManager.instance.StopBGM();
							EffectSoundManager.instance.Stop();
							EffectSoundManager.instance.CardMemoryGameStageFail();
							stageFail.SetActive(true);
							dataSaveCanvas.SetActive(true);
						}
						else
						{
							timeBar.GetComponent<TimeBar>().SetTimeBarValue(time);
							CardSelect();
							CheckCardSame();
						}
					}
				}
				else
				{
					if (gameSave && !gameNoSave)
					{
						if (!save)
						{
							if (!RankManager.CheckRankingDataEmpty())
							{
								dataSave.SetActive(true);
								RankManager.RankManagerAllClear();
							}
							else
							{
								save = true;
							}
						}
						else
						{
							dataSave.SetActive(false);
							restartBtn.SetActive(true);
							backToMainBtn.SetActive(true);
						}
					}
					else if (gameNoSave && !gameSave)
					{
						restartBtn.SetActive(true);
						backToMainBtn.SetActive(true);
					}
					


				}
			}
			else
			{
				if (!cardAllTurn)
				{
					cardAllTurn = true;
					CardsAllTurn();
				}
				if (!CheckCardAllBack())
				{
					viewTime -= Time.deltaTime;
				}

				if (viewTime <= 0.0f)
				{
					viewTime = 0.0f;
					if (!CheckCardAllBack())
					{
						CardsAllTurn();
					}
					else
					{
						gameStart = true;
						timeBar.SetActive(true);
						EffectSoundManager.instance.CardMemoryGameCountDown();
					}
				}
			}
			

		}
		else
		{
			cardSetEnd = CheckCardPosIsReady();
		}
	}

	public void RoundImageInit()
	{
		for (int i = 0; i < stage2Rounds.Length; i++)
		{
			stage2Rounds[i].SetActive(false);
		}
	}

	public bool CardIsRotate()
	{
		bool rot = false;
		for (int i = 0; i < playCards.Length; i++)
		{
			if (playCards[i].GetComponent<CardInfo>().isRotate)
			{
				rot = true;
				break;
			}
		}
		return rot;
	}	
	public void CardsAllTurn()
	{
		for (int i = 0; i < playCards.Length; i++)
		{
			if (!playCards[i].GetComponent<CardInfo>().isRotate)
			{
				playCards[i].GetComponent<CardInfo>().TurnCard();
			}
		}
	}
	public void SetLevel()
	{
		switch (level)
		{
			case LEVEL_TWO.ONE:
				level = LEVEL_TWO.TWO;
				break;
			case LEVEL_TWO.TWO:
				level = LEVEL_TWO.THREE;
				break;
			case LEVEL_TWO.THREE:
				level = LEVEL_TWO.FOUR;
				break;
			case LEVEL_TWO.FOUR:
				level = LEVEL_TWO.FIVE;
				break;
			case LEVEL_TWO.FIVE:
				break;
			default:
				break;
		}
	}

	public void Restart()
	{
		DBManager.score = 0;
		SceneManager.LoadScene(5);
	}

	public void BackToMain()
	{
		SceneManager.LoadScene(4);
	}
	public void GameInit(LEVEL_TWO level)
	{
		switch (level)
		{
			case LEVEL_TWO.ONE:
				BGSoundManager.instance.StopBGM();
				BGSoundManager.instance.PlayCardMemoryGameStage2BGM1();
				break;
			case LEVEL_TWO.TWO:
				BGSoundManager.instance.StopBGM();
				BGSoundManager.instance.PlayCardMemoryGameStage2BGM2();
				break;
			case LEVEL_TWO.THREE:
				BGSoundManager.instance.StopBGM();
				BGSoundManager.instance.PlayCardMemoryGameStage2BGM1();
				break;
			case LEVEL_TWO.FOUR:
				BGSoundManager.instance.StopBGM();
				BGSoundManager.instance.PlayCardMemoryGameStage2BGM2();
				break;
			case LEVEL_TWO.FIVE:
				BGSoundManager.instance.StopBGM();
				BGSoundManager.instance.PlayCardMemoryGameStage2BGM1();
				break;
			default:
				break;
		}
		tempTimeCheck = false;
		checkTime = false;
	    timeScore = float.MaxValue;
		timeTempScore = false;
		beforeRank = -1;
		rank = 0;
		saveIndex = 0;
		saveIndexMem = -1;
		clearComp = false;
		save = false;
		saveComp = false;
		clearEffect = false;
		dataSaveCanvas.SetActive(false);
		restartBtn.SetActive(false);
		backToMainBtn.SetActive(false);
		dataSave.SetActive(false);
		gameFailed = false;
		waitTime = 3.0f;
		viewTime = 2.0f;
		cardAllTurn = false;
		gameStart = false;
		time = 40.0f;
		stageFail.SetActive(false);
		stageClear.SetActive(false);
		timeBar.GetComponent<TimeBar>().SetTimeBarMaxRange(time);
		timeBar.SetActive(false);
		once = false;
		cIndex1 = 0;
		cIndex2 = 0;
		twoCardFront = false;
		clickCount = 2;
		cardSetEnd = false;
		maxCard = (int)level;
		playCards = new GameObject[maxCard * 2];
		allCardsNum = 52;
		randomMake = new int[maxCard];
		gameSave = false;
		gameNoSave = false;
		RoundImageInit();
		switch (level)
		{
			case LEVEL_TWO.ONE:
				stage2Rounds[0].SetActive(true);
				GetPoses(stage2Levels[0]);
				break;
			case LEVEL_TWO.TWO:
				stage2Rounds[1].SetActive(true);
				GetPoses(stage2Levels[1]);
				break;
			case LEVEL_TWO.THREE:
				stage2Rounds[2].SetActive(true);
				GetPoses(stage2Levels[2]);
				break;
			case LEVEL_TWO.FOUR:
				stage2Rounds[3].SetActive(true);
				GetPoses(stage2Levels[3]);
				break;
			case LEVEL_TWO.FIVE:
				stage2Rounds[4].SetActive(true);
				GetPoses(stage2Levels[4]);
				break;
			default:
				break;
		}

		MakeRandomNum();
		MakePlayCard();
		CardMoveToReadyPos();
	}

	public void GetPoses(GameObject obj)
	{
		int count = obj.transform.childCount;
		cardPos = new Transform[count];
		for (int i = 0; i < count; i++)
		{
			cardPos[i] = obj.transform.GetChild(i);
		}
	}
	public void MakeRandomNum()
	{
		if (maxCard < allCardsNum + 1)
		{
			int[] list = Enumerable.Range(0, allCardsNum).ToArray();

			int randRange = allCardsNum - 1;

			for (int i = 0; i < maxCard; i++)
			{
				int idx = Random.Range(0, randRange);
				randomMake[i] = list[idx];
				list[idx] = list[randRange--];

			}
		}
		else
		{
			int[] list = Enumerable.Range(0, allCardsNum).ToArray();

			int randRange = allCardsNum - 1;

			for (int i = 0; i < allCardsNum; i++)
			{
				int idx = Random.Range(0, randRange);
				randomMake[i] = list[idx];
				list[idx] = list[randRange--];
			}
			list = Enumerable.Range(0, allCardsNum).ToArray();

			randRange = allCardsNum - 1;
			for (int i = 0; i < maxCard - allCardsNum; i++)
			{
				int idx = Random.Range(0, randRange);
				randomMake[allCardsNum + i] = list[idx];
				list[idx] = list[randRange--];
			}
		}
	}
	public void UserInfoSet()
	{
		userScore.text = "Score : " + (DBManager.score <= 0 ? "0" : string.Format("{0:#,###}", DBManager.score));

	}
	public void MakePlayCard()
	{
		int[] list = Enumerable.Range(0, maxCard * 2).ToArray();
		int randRange = maxCard * 2 - 1;
		for (int i = 0; i < cardPos.Length; i++)
		{
			int idx = Random.Range(0, randRange);
			playCards[list[idx]] = Instantiate(allCards[randomMake[i % maxCard]]);
			playCards[list[idx]].transform.position = Vector3.zero;
			list[idx] = list[randRange--];
		}

		for (int i = 0; i < cardPos.Length; i++)
		{
			playCards[i].GetComponent<CardInfo>().cIndex = i;
			playCards[i].GetComponent<CardInfo>().targetPos = cardPos[i];
		}
	}

	public void CardMoveToReadyPos()
	{
		for (int i = 0; i < cardPos.Length; i++)
		{
			playCards[i].GetComponent<CardInfo>().isMove = true;
		}
	}

	public bool CheckCardAllBack()
	{
		bool ready = true;
		for (int i = 0; i < playCards.Length; i++)
		{
			if (playCards[i].GetComponent<CardInfo>().isFront)
			{
				ready = false;
				break;
			}
		}
		return ready;
	}

	public bool CheckCardPosIsReady()
	{
		bool ready = true;

		for (int i = 0; i < cardPos.Length; i++)
		{
			if (playCards[i].GetComponent<CardInfo>().isMove)
			{
				ready = false;
				break;
			}
		}

		return ready;
	}

	public void CardSelect()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Vector3 mPos = Input.mousePosition;
			mPos = mainCam.ScreenToWorldPoint(mPos);

			RaycastHit2D hit = Physics2D.Raycast(mPos, mainCam.transform.forward, 15.0f);
			if (hit && clickCount > 0)
			{
				if (!hit.transform.GetComponent<CardInfo>().isFront && !hit.transform.GetComponent<CardInfo>().isRotate)
				{
					hit.transform.GetComponent<CardInfo>().TurnCard();
					if (clickCount == 2)
					{
						cIndex1 = hit.transform.GetComponent<CardInfo>().cIndex;
					}
					else
					{
						cIndex2 = hit.transform.GetComponent<CardInfo>().cIndex;
					}
					clickCount--;
				}
			}
		}
	}

	public void CheckCardSame()
	{
		if (clickCount == 0 && !twoCardFront)
		{
			if (playCards[cIndex1].GetComponent<CardInfo>().isFront && playCards[cIndex2].GetComponent<CardInfo>().isFront)
			{
				twoCardFront = true;
			}

		}
		else if (clickCount == 0 && twoCardFront)
		{
			if (playCards[cIndex1].GetComponent<CardInfo>().cName == playCards[cIndex2].GetComponent<CardInfo>().cName)
			{
				EffectSoundManager.instance.CardMemoryGameCardSame();
				DBManager.score += 100;
				twoCardFront = false;
				Destroy(playCards[cIndex1]);
				playCards[cIndex1] = null;
				Destroy(playCards[cIndex2]);
				playCards[cIndex2] = null;
				clickCount = 2;
			}
			else
			{
				if (!once)
				{
					once = true;
					playCards[cIndex1].GetComponent<CardInfo>().TurnCard();
					playCards[cIndex2].GetComponent<CardInfo>().TurnCard();
				}

				if (!playCards[cIndex1].GetComponent<CardInfo>().isFront && !playCards[cIndex2].GetComponent<CardInfo>().isFront)
				{
					twoCardFront = false;
					clickCount = 2;
					once = false;
				}
			}

		}
	}

	public void DataSaveYes()
	{
		gameSave = true;
		dataSaveCanvas.SetActive(false);
	}

	public void DataSaveNo()
	{
		gameNoSave = true;
		dataSaveCanvas.SetActive(false);
	}

	public bool GameEnd()
	{
		bool end = true;
		for (int i = 0; i < playCards.Length; i++)
		{
			if (playCards[i] != null)
			{
				end = false;
				break;
			}
		}
		return end;
	}
}
