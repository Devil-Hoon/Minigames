using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Newtonsoft.Json;
using MySql.Data;
using MySql.Data.MySqlClient;
public class SpaceDefence_MainManager : MonoBehaviour
{
    [Header("RankingPanel")]
    public GameObject rankingPanel;
    public GameObject[] names;
    public GameObject[] scores;
    [Header("ExplainPanel")]
    public GameObject explainPanel;

    [Header("Option")]
    public GameObject optionPanel;
    public GameObject BGToggle;
    public GameObject EffectToggle;
    private bool optionPanelOnOff;

    private bool once;
    private float time;
    private bool check;
    private bool loadOnce;

	private void OnApplicationQuit()
	{
        DBManager.LogOut();
	}
	private void Awake()
    {
        time = 0.0f;
        Screen.SetResolution(960, 600, false);
    }
    // Start is called before the first frame update
    void Start()
    {
        loadOnce = false;
        optionPanelOnOff = false;
        rankingPanel.SetActive(false);

        Toggle toggle = BGToggle.GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(BGSoundOnOff);
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
            BGSoundManager.instance.PlaySpaceDefenceMainBGM();
            DBManager.score = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ShowOptionView()
    {
        optionPanelOnOff = !optionPanelOnOff;
        optionPanel.SetActive(optionPanelOnOff);
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
    public void RankViewBtn()
    {
        rankingPanel.SetActive(true);
    }
    public void RankViewExit()
	{
        rankingPanel.SetActive(false);
	}
    public void ExplainBtn()
    {
        explainPanel.SetActive(true);
    }
    public void ExplainExit()
    {
        explainPanel.SetActive(false);
    }
    public void GameStart()
	{
        SceneManager.LoadScene(16);
	}
    public void BGSoundOnOff(bool state)
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
   
	public void Home()
    {
        BGSoundManager.instance.StopBGM();
        SceneManager.LoadScene(0);
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

        MySqlCommand command = new MySqlCommand("SELECT Ranking AS 'Rank', User, Score FROM TowerDefenseRanking ORDER BY Ranking ASC LIMIT 0,10", conn);
        MySqlDataReader rdr = command.ExecuteReader();

        string temp = string.Empty;

        if (rdr == null)
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
