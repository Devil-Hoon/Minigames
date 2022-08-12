using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using MySql.Data.MySqlClient;
using MySql.Data;
public class CardMemoryGame_MainManager : MonoBehaviour
{

	[Header("RankinPanel")]
	public GameObject rankingPanel;
	public GameObject[] names;
	public GameObject[] scores;

	[Header("ExplainPanel")]
	public GameObject explainPanel;

	[Header("Option")]
	public GameObject optionPanel;
	public GameObject BGToggle;
	public GameObject EffectToggle;
	private bool optionPanelOnoff;

	// Start is called before the first frame update
	void Start()
	{
		optionPanelOnoff = false;
		explainPanel.SetActive(false);
		rankingPanel.SetActive(false);
		DBManager.stageNum = StageNum.ONE;

		Toggle toggle = BGToggle.GetComponent<Toggle>();
		toggle.onValueChanged.AddListener(BGMSoundOnOff);
		if (BGSoundManager.instance != null)
		{
			toggle.isOn = BGSoundManager.instance.isOn;
		}
		toggle = EffectToggle.GetComponent<Toggle>();
		toggle.onValueChanged.AddListener(EffectSoundOnOff);
		if (EffectSoundManager.instance != null)
		{
			toggle.isOn = EffectSoundManager.instance.isOn;
		}
		RankManager.RankManagerAllClear();
		RankCoUnity();
		
		if (DBManager.LoggedIn)
		{
			BGSoundManager.instance.StopBGM();
			BGSoundManager.instance.PlayCardGameMainBGM();
			DBManager.score = 0;
		}
	}
	private void OnApplicationQuit()
	{
		DBManager.LogOut();
	}
	// Update is called once per frame
	void Update()
	{
	}

    public void GameStart()
	{
		BGSoundManager.instance.StopBGM();
		SceneManager.LoadScene(5);
    }
	public void Home()
	{
		BGSoundManager.instance.StopBGM();
		SceneManager.LoadScene(0);
	}
    public void RankViewBtn()
    {
		rankingPanel.SetActive(true);
    }

	public void ExplainBtn()
	{
		explainPanel.SetActive(true);
	}

	public void ShowOptionView()
	{
		optionPanelOnoff = !optionPanelOnoff;
		optionPanel.SetActive(optionPanelOnoff);
	}
	public void ExitRankView()
	{
		rankingPanel.SetActive(false);
	}
	public void ExitExplain()
	{
		explainPanel.SetActive(false);
	}
	public bool RankDBCheck()
	{
		bool check = true;
		for (int i = 0; i < 10; i++)
		{
			if (RankManager.users[i] == null)
			{
				check = false;
				break;
			}
		}
		return check;
	}

	public void BGMSoundOnOff(bool state)
	{
		BGSoundManager.instance.isOn = state;
		if (!BGSoundManager.instance.isOn)
		{
			BGSoundManager.instance.StopBGM();
		}
		else
		{
			BGSoundManager.instance.Play();
		}
	}

	public void EffectSoundOnOff(bool state)
	{
		EffectSoundManager.instance.isOn = state;
	}

	public void RankCoUnity()
	{
		string sqlConnect = "server=pickstar.co.kr;uid=pickstar;pwd=xktmvlr10;database=pickstar;charset=utf8;";

		MySqlConnection conn = new MySqlConnection(sqlConnect);
		if (conn == null)
		{
			Debug.Log("connetion Failed");
			return;
		}
		conn.Open();

		MySqlCommand command = new MySqlCommand("SELECT Ranking AS 'Rank', User, Score FROM CardRanking ORDER BY Ranking ASC LIMIT 0,10", conn);
		MySqlDataReader rdr = command.ExecuteReader();


		if (!rdr.HasRows)
		{
			Debug.Log("DB Load Failed");
			conn.Close();
			return;
		}

		while (rdr.Read())
		{
			RankManager.users[rdr.GetInt32(0) - 1] = rdr.GetString(1);
			RankManager.scores[rdr.GetInt32(0) - 1] = rdr.GetInt32(2);
		}

		rdr.Close();
		conn.Close();
		if (!RankDBCheck())
		{
			Debug.Log("Data 입력 실패");
			Debug.Log("Data Load 재실행");
			RankCoUnity();
			return;
		}
		else
		{
			for (int i = 0; i < 10; i++)
			{
				names[i].GetComponent<Text>().text = RankManager.users[i];
				scores[i].GetComponent<Text>().text = RankManager.scores[i].ToString();

			}
		}
	}
}
