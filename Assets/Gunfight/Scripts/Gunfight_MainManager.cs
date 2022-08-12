using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MySql.Data;
using MySql.Data.MySqlClient;

public class Gunfight_MainManager : MonoBehaviourPunCallbacks
{
	private readonly string gameVersion = "1";

	public Button joinLobby;
	public Button howToPlayBtn;

	public GameObject explainPanel;
	public GameObject optionPanel;

	[Header("Toggle")]
	public GameObject BGToggle;
	public GameObject EffectToggle;
	private bool optionPanelOnoff;
	bool explainShow;
	float explainY;
	// Start is called before the first frame update

	private void Awake()
	{
		Screen.SetResolution(960, 600, false);
	}
	private void OnApplicationQuit()
	{
		PhotonNetwork.Disconnect();
		DBManager.LogOut();
	}
	void Start()
	{
		explainShow = false;
		explainY = 850.0f;
		PhotonNetwork.GameVersion = gameVersion;
		PhotonNetwork.SendRate = 60;
		PhotonNetwork.SerializationRate = 60;
		PhotonNetwork.PhotonServerSettings.DevRegion = "kr";
		PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "kr";
		PhotonNetwork.ConnectUsingSettings();
		
		joinLobby.interactable = false;
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

			
		if (DBManager.LoggedIn)
		{
			BGSoundManager.instance.StopBGM();
			BGSoundManager.instance.PlayGunfightMainBGM();
			DBManager.LoadGP();

			if (DBManager.gp == 0)
			{
				PhotonNetwork.Disconnect();
				Debug.Log("연결해제");
				BGSoundManager.instance.StopBGM();
				SceneManager.LoadScene("MinigameMain");
			}
		}
		
		optionPanel.SetActive(false);
	}

	public override void OnConnectedToMaster()
	{
		joinLobby.interactable = true;
		Debug.Log("연결성공");
		Debug.Log(PhotonNetwork.CloudRegion);
	}

	public override void OnDisconnected(DisconnectCause cause)
	{
		joinLobby.interactable = false;
		Debug.Log("연결실패");
		PhotonNetwork.ConnectUsingSettings();
	}

	public void Connect()
	{
		if(PhotonNetwork.IsConnected)
		{
			Debug.Log("방참가");
			PhotonNetwork.JoinRandomRoom();
			joinLobby.interactable = false;
		}
	}

	public override void OnJoinRandomFailed(short returnCode, string message)
	{
		Debug.Log("방생성");
		PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 2, CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "MasterPanel", 1 } }  });
	}

	public override void OnJoinedLobby()
	{
	}
	public override void OnJoinedRoom()
	{
		Debug.Log("방참가성공");
		PhotonNetwork.IsMessageQueueRunning = false;
		PhotonNetwork.LoadLevel("GunfightLobby");
	}
	// Update is called once per frame
	void Update()
    {
		if (explainShow)
		{
			explainY -= Time.deltaTime * 400.0f;
			if (explainY < 300.0f)
			{
				explainY = 300.0f;
			}

			explainPanel.transform.localPosition = new Vector3(0, explainY, 0);
		}
		else
		{
			explainY += Time.deltaTime * 400.0f;
			if (explainY > 850.0f)
			{
				explainY = 850.0f;
			}

			explainPanel.transform.localPosition = new Vector3(0, explainY, 0);
		}
	}
	public void ShowOptionView()
	{
		optionPanelOnoff = !optionPanelOnoff;
		optionPanel.SetActive(optionPanelOnoff);
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

	public void HomeBtn()
	{
		PhotonNetwork.Disconnect();
		Debug.Log("연결해제");
		BGSoundManager.instance.StopBGM();
		SceneManager.LoadScene("MinigameMain");
	}

	public void ShowExplainPanel()
	{
		explainShow = true;
		joinLobby.interactable = false;
		howToPlayBtn.interactable = false;
	}

	public void ExitExplainPanel()
	{
		explainShow = false;
		joinLobby.interactable = true;
		howToPlayBtn.interactable = true;
	}

}
