using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using MySql.Data.MySqlClient;
using MySql.Data;
public class EnchantGame_MainManager : MonoBehaviour
{
	[Header("ETC")]
	public GameObject explainPanel;
	public Button playBtn;
	[Header("Option")]
	public GameObject optionPanel;
	public GameObject BGToggle;
	public GameObject EffectToggle;
	private bool optionPanelOnoff;

	private void Awake()
	{
		Screen.SetResolution(960, 600, false);
	}
	// Start is called before the first frame update
	void Start()
	{
		DBManager.LoadGP();
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
		playBtn.interactable = false;
		explainPanel.SetActive(false);
		if (DBManager.LoggedIn)
		{
			BGSoundManager.instance.StopBGM();
			BGSoundManager.instance.PlayEnchantGameMainBGM();
			PlayerDataLoad();
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

	public void PlayGame()
	{
		SceneManager.LoadScene(14);
	}
	public void Home()
	{
		BGSoundManager.instance.StopBGM();
		SceneManager.LoadScene(0);
	}
	public void ShowOptionView()
	{
		optionPanelOnoff = !optionPanelOnoff;
		optionPanel.SetActive(optionPanelOnoff);
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
	public void PlayerDataLoad()
	{
		string id = DBManager.username;

		string sqlConnect = "server=pickstar.co.kr;uid=pickstar;pwd=xktmvlr10;database=pickstar;charset=utf8;";

		MySqlConnection conn = new MySqlConnection(sqlConnect);
		if (conn.State != System.Data.ConnectionState.Closed)
		{
			Debug.Log("connetion Failed");
			return;
		}
		conn.Open();
		string quary = "SELECT * FROM EnhanceData WHERE USER = '" + id + "'";
		MySqlCommand command = new MySqlCommand(quary, conn);
		MySqlDataReader rdr = command.ExecuteReader();


		if (!rdr.HasRows)
		{
			rdr.Read();
			rdr.Close();
			string sql = "INSERT INTO enhancedata VALUES(DEFAULT, '" + id + "', DEFAULT, DEFAULT, DEFAULT, DEFAULT, DEFAULT)";
			MySqlCommand cmd = new MySqlCommand(sql, conn);
			MySqlDataReader datardr = cmd.ExecuteReader();
			datardr.Read();
			datardr.Close();

			DBManager.isPlant = false;
			DBManager.seedName = null;
			DBManager.seedTempletNum = 0;
			DBManager.seedEnhance = 0;
			DBManager.randomSeedCost = 0;

			Debug.Log("Create&Load Complete");
		}
		else
		{
			rdr.Read();

			DBManager.isPlant = bool.Parse(rdr["IsPlant"].ToString());
			DBManager.seedName = rdr["SeedName"].ToString();
			DBManager.seedTempletNum = int.Parse(rdr["SeedTemplet"].ToString());
			DBManager.seedEnhance = int.Parse(rdr["Enhance"].ToString());
			DBManager.randomSeedCost = int.Parse(rdr["RandomSeedCost"].ToString());

			rdr.Close();

			Debug.Log("Login&UpdateLoad Complete");
			

		}

		conn.Close();

		playBtn.interactable = true;
	}

	//public void LoadGP()
	//{
	//	string id = DBManager.username;
	//	string sqlConnect = "server=pickstar.co.kr;uid=pickstar;pwd=xktmvlr10;database=pickstar;charset=utf8;";

	//	MySqlConnection conn = new MySqlConnection(sqlConnect);
	//	if (conn.State != System.Data.ConnectionState.Closed)
	//	{
	//		Debug.Log("connetion Failed");
	//		return;
	//	}
	//	conn.Open();

	//	MySqlCommand command = new MySqlCommand("SELECT * FROM member  WHERE userid = '" + id + "' OR username='" + id + "';", conn);
	//	MySqlDataReader rdr = command.ExecuteReader();

	//	if (!rdr.HasRows)
	//	{
	//		Debug.Log("DB Load Failed");
	//		conn.Close();
	//		return;
	//	}

	//	while (rdr.Read())
	//	{
	//		DBManager.gp = int.Parse(rdr["money"].ToString());
	//	}

	//	rdr.Close();
	//	conn.Close();
	//}
}
