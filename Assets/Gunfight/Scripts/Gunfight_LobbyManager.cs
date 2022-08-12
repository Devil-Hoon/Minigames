using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Photon.Pun.Demo.PunBasics;
using UnityEngine.SceneManagement;
using Photon.Pun.Demo.Cockpit.Forms;
using MySql.Data.MySqlClient;

public class Gunfight_LobbyManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
            }
            return instance;
        }
    }
    private static GameManager instance;

    private float limitCount = 0;
    private float count = 0;
    private bool once = false;
    public Transform[] pPoses;
    // Start is called before the first frame update

    [Header("PlayerInfo")]
    public Animator P1Animator1;
    public Animator P1Animator2;
    public Animator P2Animator1;
    public Animator P2Animator2;
    public GameObject p1Wait1;
    public GameObject p1Wait2;
    public GameObject p1Panel;
    public GameObject p1Text;
    public GameObject p1GP;
    public GameObject p2Wait1;
    public GameObject p2Wait2;
    public GameObject p2Panel;
    public GameObject p2Text;
    public GameObject p2GP;
    public GameObject TimeLimit;

    public Button p1ExitBtn;
    public Button p2ExitBtn;
    Room room;
    ExitGames.Client.Photon.Hashtable cp;

    [Header("LobbyOption")]
    public int masterPanelNum;
    public int[] playerNum;
    public bool[] panelActive;
    public bool[] readyCheck;
    public int masterClientIndex = 1;
    private bool bothOut = false;
    private string uName;
    private int uGP;

    public Sprite[] numSprites;
    void Start()
    {
        DBManager.LoadGP();
        photonView.RPC("P1WaitImgActiveUpdate", RpcTarget.All, false,false);
        photonView.RPC("P2WaitImgActiveUpdate", RpcTarget.All, false,false);
        once = false;
        if (PhotonNetwork.InRoom)
        {
            BGSoundManager.instance.StopBGM();
            BGSoundManager.instance.PlayGunfightLobbyBGM();
            room = PhotonNetwork.CurrentRoom;
            if (room == null)
            {
                return;
            }
            cp = room.CustomProperties;
        }
        masterClientIndex = PhotonNetwork.MasterClient.ActorNumber;
        masterPanelNum = cp["MasterPanel"].GetHashCode();
        Debug.Log(masterPanelNum);
        panelActive = new bool[] { false, false };
        readyCheck = new bool[] { false, false };
        p1Panel.SetActive(false);
        p2Panel.SetActive(false);
        PhotonNetwork.IsMessageQueueRunning = true;
        TimeLimit.SetActive(false);
        SpawnPanel();
    }
    private void OnApplicationQuit()
    {
        PhotonNetwork.Disconnect();
        DBManager.LogOut();
    }
    // Update is called once per frame
    void Update()
    {
        if (!readyCheck[0] || !readyCheck[1])
		{
            if (PhotonNetwork.InRoom && room.PlayerCount != 2)
            {
                ToggleIsOn(false);
                ToggleInteractable(false);
            }
            else if (PhotonNetwork.InRoom && room.PlayerCount == 2)
            {
                ToggleInteractable(true);
            }
            LimitReady();

            if (PhotonNetwork.InRoom)
            {
                if (masterPanelNum == 1)
                {
                    if (PhotonNetwork.LocalPlayer.ActorNumber == masterClientIndex)
                    {
                        photonView.RPC("SetP1Name", RpcTarget.All, uName);
                        photonView.RPC("SetP1GP", RpcTarget.All, uGP);
                    }
                    else
                    {
                        photonView.RPC("SetP2Name", RpcTarget.All, uName);
                        photonView.RPC("SetP2GP", RpcTarget.All, uGP);
                    }
                }
                else
                {
                    if (PhotonNetwork.LocalPlayer.ActorNumber == masterClientIndex)
                    {
                        photonView.RPC("SetP2Name", RpcTarget.All, uName);
                        photonView.RPC("SetP2GP", RpcTarget.All, uGP);
                    }
                    else
                    {
                        photonView.RPC("SetP1Name", RpcTarget.All, uName);
                        photonView.RPC("SetP1GP", RpcTarget.All, uGP);
                    }
                }
            }
        }
        if (bothOut && PhotonNetwork.InRoom)
        {
            bothOut = false;
            photonView.RPC("ViewTimeLimit", RpcTarget.All, false);
            photonView.RPC("LimitSet", RpcTarget.All, 0.0f);
            PhotonNetwork.LeaveRoom();
        }
        
		

    }

	private void FixedUpdate()
	{
        CheckReady();
	}
	public bool[] GetActives()
    {
        return panelActive;
    }

    private void SpawnPanel()
    {
        Debug.Log(masterClientIndex);
        var localPannelIndex = PhotonNetwork.LocalPlayer.ActorNumber;
        Debug.Log(PhotonNetwork.LocalPlayer.ActorNumber);
        if (masterPanelNum == 1)
        {
            if (localPannelIndex == masterClientIndex)
            {
                PanelActive(1, true);
                ButtonSet();
                Player1ToggleSet();

                photonView.RPC("P1WaitImgActiveUpdate", RpcTarget.All, true, false);


                uName = DBManager.nickname;
                uGP = DBManager.gp;
                photonView.RPC("SetP1Name", RpcTarget.All, uName);
                photonView.RPC("SetP1GP", RpcTarget.All, uGP);
            }
            else
            {
                PanelActive(1, true);
                PanelActive(0, true);
                ButtonSet();
                Player2ToggleSet();

                photonView.RPC("P1WaitImgActiveUpdate", RpcTarget.All, true, false);
                photonView.RPC("P2WaitImgActiveUpdate", RpcTarget.All, false, true);
                uName = DBManager.nickname;
                uGP = DBManager.gp;
                photonView.RPC("SetP2Name", RpcTarget.All, uName);
                photonView.RPC("SetP2GP", RpcTarget.All, uGP);
            }
        }
        else
        {
            if (localPannelIndex == masterClientIndex)
            {
                PanelActive(0, true);
                ButtonSet();
                Player2ToggleSet();
                photonView.RPC("P2WaitImgActiveUpdate", RpcTarget.All, true, false);
                uName = DBManager.nickname;
                uGP = DBManager.gp;
                photonView.RPC("SetP2Name", RpcTarget.All, uName);
                photonView.RPC("SetP2GP", RpcTarget.All, uGP);
            }
            else
            {
                PanelActive(0, true);
                PanelActive(1, true);
                photonView.RPC("P1WaitImgActiveUpdate", RpcTarget.All, false, true);
                photonView.RPC("P2WaitImgActiveUpdate", RpcTarget.All, true, false);
                ButtonSet();
                Player1ToggleSet();

                uName = DBManager.nickname;
                uGP = DBManager.gp;
                photonView.RPC("SetP1Name", RpcTarget.All, uName);
                photonView.RPC("SetP1GP", RpcTarget.All, uGP);
            }
        }


    }

    public override void OnLeftRoom()
    {
        Debug.Log("방퇴장");

        limitCount = 0.0f;
        bothOut = false;
        SceneManager.LoadScene("GunfightMain");
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (masterPanelNum == 1)
        {
            cp["MasterPanel"] = 2;
            masterPanelNum = 2;
            PhotonNetwork.CurrentRoom.SetCustomProperties(cp);
            photonView.RPC("SetMasterClientIndex", RpcTarget.All, newMasterClient.ActorNumber);
            PanelActive(0, true);
            PanelActive(1, false);
            photonView.RPC("P1WaitImgActiveUpdate", RpcTarget.All, false, false);

            photonView.RPC("P2WaitImgActiveUpdate", RpcTarget.All, true, false);
        }
        else
        {
            cp["MasterPanel"] = 1;
            masterPanelNum = 1;
            PhotonNetwork.CurrentRoom.SetCustomProperties(cp);
            photonView.RPC("SetMasterClientIndex", RpcTarget.All, newMasterClient.ActorNumber);
            PanelActive(1, true);
            PanelActive(0, false);
            photonView.RPC("P1WaitImgActiveUpdate", RpcTarget.All, true, false);

            photonView.RPC("P2WaitImgActiveUpdate", RpcTarget.All, false, false);
        }
        Debug.Log("Change");
    }
    public void PanelActive(int pNum, bool state)
    {
        panelActive[pNum] = state;

        photonView.RPC("PanelActiveUpdate", RpcTarget.All, panelActive[1], panelActive[0]);
    }

    

    public void BackToMain1()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber > masterClientIndex && masterPanelNum != 1)
        {
            PanelActive(1, false);
            Toggle toggle = p1Panel.GetComponentInChildren<Toggle>();
            toggle.isOn = false;
            SetReady1(false);
            p1ExitBtn.interactable = false;
            PhotonNetwork.LeaveRoom();
        }
        else if (PhotonNetwork.LocalPlayer.ActorNumber == masterClientIndex && masterPanelNum == 1)
        {
            PanelActive(1, false);
            Toggle toggle = p1Panel.GetComponentInChildren<Toggle>();
            toggle.isOn = false;
            SetReady1(false);
            p1ExitBtn.interactable = false;
            PhotonNetwork.LeaveRoom();
        }

    }

    public void BackToMain2()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber > masterClientIndex && masterPanelNum != 2)
        {
            PanelActive(0, false);
            Toggle toggle = p2Panel.GetComponentInChildren<Toggle>();
            toggle.isOn = false;
            SetReady2(false);
            p2ExitBtn.interactable = false;
            PhotonNetwork.LeaveRoom();
        }
        else if (PhotonNetwork.LocalPlayer.ActorNumber == masterClientIndex && masterPanelNum == 2)
        {
            PanelActive(0, false);
            Toggle toggle = p2Panel.GetComponentInChildren<Toggle>();
            toggle.isOn = false;
            SetReady2(false);
            p2ExitBtn.interactable = false;
            PhotonNetwork.LeaveRoom();
        }
    }


    public void SetReady1(bool ready)
    {
        if(PhotonNetwork.LocalPlayer.ActorNumber > masterClientIndex && masterPanelNum != 1)
		{
            readyCheck[0] = ready;
            SetP1ReadyAnim(ready);
            photonView.RPC("SetReadyUpdate", RpcTarget.All, readyCheck[0], readyCheck[1]);
        }
        else if(PhotonNetwork.LocalPlayer.ActorNumber == masterClientIndex && masterPanelNum == 1)
		{
            readyCheck[0] = ready;
            SetP1ReadyAnim(ready);
            photonView.RPC("SetReadyUpdate", RpcTarget.All, readyCheck[0], readyCheck[1]);
        }
    }
    public void SetReady2(bool ready)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber > masterClientIndex && masterPanelNum != 2)
        {
            readyCheck[1] = ready;
            SetP2ReadyAnim(ready);
            photonView.RPC("SetReadyUpdate", RpcTarget.All, readyCheck[0], readyCheck[1]);
        }
        else if (PhotonNetwork.LocalPlayer.ActorNumber == masterClientIndex && masterPanelNum == 2)
        {
            readyCheck[1] = ready;
            SetP2ReadyAnim(ready);
            photonView.RPC("SetReadyUpdate", RpcTarget.All, readyCheck[0], readyCheck[1]);
        }
    }
   
    private void LimitReady()
	{

        if (room.PlayerCount == 2)
        {
			if (PhotonNetwork.IsMasterClient)
            {
                limitCount += Time.deltaTime;
                photonView.RPC("SetTimeNumeric", RpcTarget.All, limitCount);
            }

            
			if (!once)
			{
                photonView.RPC("ViewTimeLimit", RpcTarget.All, true);
                once = true;
            }
        }
        else
        {
            once = false;
            limitCount = 0.0f;
            photonView.RPC("ViewTimeLimit", RpcTarget.All, false);
        }

        if (limitCount > 10.0f)
		{
            if (!readyCheck[0] && !readyCheck[1])
            {
                photonView.RPC("BothOut", RpcTarget.All, true);
                limitCount = 0.0f;
            }
			else
			{
                if (PhotonNetwork.LocalPlayer.ActorNumber == masterClientIndex && masterPanelNum == 1 && !readyCheck[0])
                {
                    photonView.RPC("BothOut", RpcTarget.All, false);
                    PanelActive(1, false);
                    photonView.RPC("ReadyReset2", RpcTarget.All, false);
                    photonView.RPC("SetReadyUpdate", RpcTarget.All, readyCheck[0], false);
                    photonView.RPC("ViewTimeLimit", RpcTarget.All, false);
                    photonView.RPC("LimitSet", RpcTarget.All, 0.0f);
                    PhotonNetwork.LeaveRoom();
                }
                if (PhotonNetwork.LocalPlayer.ActorNumber == masterClientIndex && masterPanelNum == 2 && !readyCheck[1])
                {
                    photonView.RPC("BothOut", RpcTarget.All, false);
                    PanelActive(0, false);
                    photonView.RPC("ReadyReset1", RpcTarget.All, false);
                    photonView.RPC("SetReadyUpdate", RpcTarget.All, false, readyCheck[1]);
                    photonView.RPC("ViewTimeLimit", RpcTarget.All, false);
                    photonView.RPC("LimitSet", RpcTarget.All, 0.0f);
                    PhotonNetwork.LeaveRoom();
                }
                if (PhotonNetwork.LocalPlayer.ActorNumber > masterClientIndex && masterPanelNum == 1 && !readyCheck[1])
                {
                    photonView.RPC("BothOut", RpcTarget.All, false);
                    PanelActive(0, false);
                    photonView.RPC("ReadyReset1", RpcTarget.All, false);
                    photonView.RPC("SetReadyUpdate", RpcTarget.All, false, readyCheck[1]);
                    photonView.RPC("ViewTimeLimit", RpcTarget.All, false);
                    photonView.RPC("LimitSet", RpcTarget.All, 0.0f);
                    PhotonNetwork.LeaveRoom();
                }
                if (PhotonNetwork.LocalPlayer.ActorNumber > masterClientIndex && masterPanelNum == 2 && !readyCheck[0])
                {
                    photonView.RPC("BothOut", RpcTarget.All, false);
                    PanelActive(1, false);
                    photonView.RPC("ReadyReset2", RpcTarget.All, false);
                    photonView.RPC("SetReadyUpdate", RpcTarget.All, readyCheck[0], false);
                    photonView.RPC("ViewTimeLimit", RpcTarget.All, false);
                    photonView.RPC("LimitSet", RpcTarget.All, 0.0f);
                    PhotonNetwork.LeaveRoom();
                }
            }
            
            
		}



    }

    public void ButtonSet()
	{
        Button button = p1Panel.GetComponentInChildren<Button>();
        button.onClick.AddListener(BackToMain1);
        button = p2Panel.GetComponentInChildren<Button>();
        button.onClick.AddListener(BackToMain2);
    }

    public void Player1ToggleSet()
	{
        Toggle toggle = p1Panel.GetComponentInChildren<Toggle>();
        toggle.onValueChanged.AddListener(SetReady1);
        toggle.isOn = false;
        toggle = p2Panel.GetComponentInChildren<Toggle>();
        toggle.onValueChanged.AddListener(SetReady2);
        toggle.interactable = false;
        toggle.isOn = false;
    }

    public void Player2ToggleSet()
	{

        Toggle toggle = p1Panel.GetComponentInChildren<Toggle>();
        toggle.onValueChanged.AddListener(SetReady1);
        toggle.isOn = false;
        toggle.interactable = false;
        toggle = p2Panel.GetComponentInChildren<Toggle>();
        toggle.onValueChanged.AddListener(SetReady2);
        toggle.isOn = false;
    }


    public void ToggleInteractable(bool state)
	{
        photonView.RPC("SetToggleInteractable", RpcTarget.All, state);
	}

    public void ToggleIsOn(bool state)
	{
        photonView.RPC("SetToggleIsOn", RpcTarget.All, state);
	}
    

    public void SetP1ReadyAnim(bool state)
	{
        photonView.RPC("SetP1AnimatorReady", RpcTarget.All, state);
	}
    public void SetP2ReadyAnim(bool state)
    {
        photonView.RPC("SetP2AnimatorReady", RpcTarget.All, state);
    }
    private void CheckReady()
    {
        if (readyCheck[0] && readyCheck[1])
        {
            count += Time.deltaTime;

            Toggle toggle = p1Panel.GetComponentInChildren<Toggle>();
            toggle.interactable = false;
            toggle = p2Panel.GetComponentInChildren<Toggle>();
            toggle.interactable = false;
        }

		if (count > 1.0f)
		{
            if (PhotonNetwork.LocalPlayer.ActorNumber == PhotonNetwork.MasterClient.ActorNumber)
            {
                room.IsOpen = false;
            }
            SceneManager.LoadScene("GunfightPlay");
            //PhotonNetwork.LoadLevel("GunfightPlay");
        }
    }

    
    [PunRPC]
    private void PanelActiveUpdate(bool p1State, bool p2State)
    {
        p1Panel.SetActive(p1State);
        p2Panel.SetActive(p2State);
    }

    [PunRPC]
    private void P1WaitImgActiveUpdate(bool p1State1, bool p1State2)
    {
        p1Wait1.SetActive(p1State1);
        p1Wait2.SetActive(p1State2);
    }
    [PunRPC]
    private void P2WaitImgActiveUpdate(bool p2State1, bool p2State2)
    {
        p2Wait1.SetActive(p2State1);
        p2Wait2.SetActive(p2State2);
    }
    [PunRPC]
    private void SetMasterClientIndex(int index)
    {
        masterClientIndex = index;
    }

    [PunRPC]
    public void SetReadyUpdate(bool p1Ready, bool p2Ready)
    {
        readyCheck[0] = p1Ready;
        readyCheck[1] = p2Ready;
        Toggle toggle = p1Panel.GetComponentInChildren<Toggle>();
        toggle.isOn = p1Ready;
        toggle = p2Panel.GetComponentInChildren<Toggle>();
        toggle.isOn = p2Ready;
    }
    [PunRPC]
    public void BothOut(bool check)
    {
        bothOut = check;
    }

    [PunRPC]
    public void LimitSet(float count)
    {
        limitCount = count;
    }

    [PunRPC]
    public void ReadyReset1(bool ready)
    {
        Toggle toggle = p1Panel.GetComponentInChildren<Toggle>();
        toggle.isOn = ready;
    }
    [PunRPC]
    public void ReadyReset2(bool ready)
    {
        Toggle toggle = p2Panel.GetComponentInChildren<Toggle>();
        toggle.isOn = ready;
    }
    [PunRPC]
    public void ViewTimeLimit(bool active)
	{
        TimeLimit.SetActive(active);
	}

    [PunRPC]
    public void SetTimeNumeric(float time)
	{
        limitCount = time;
        int num =  9 - Mathf.FloorToInt(time);
        
        if(num < 0)
		{
            num = 0;
		}
        Image image = TimeLimit.GetComponent<Image>();
        image.sprite = numSprites[num];
	}

    [PunRPC]
    public void SetP1Name(string name)
	{
        p1Text.GetComponent<Text>().text = name;
	}
    [PunRPC]
    public void SetP1GP(int point)
    {
        p1GP.GetComponent<Text>().text = point <= 0 ? "0" : string.Format("{0:#,###}",point);
    }

    [PunRPC]
    public void SetP2Name(string name)
    {
        p2Text.GetComponent<Text>().text = name;
    }
    [PunRPC]
    public void SetP2GP(int point)
    {
        p2GP.GetComponent<Text>().text = point <= 0 ? "0" : string.Format("{0:#,###}", point);
    }
    [PunRPC]
    public void SetToggleInteractable(bool state)
	{

        if (masterPanelNum == 1)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == masterClientIndex)
            {
                Toggle toggle = p1Panel.GetComponentInChildren<Toggle>();
                toggle.interactable = state;
            }
            else
            {
                Toggle toggle = p2Panel.GetComponentInChildren<Toggle>();
                toggle.interactable = state;
            }
        }
        else
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == masterClientIndex)
            {
                Toggle toggle = p2Panel.GetComponentInChildren<Toggle>();
                toggle.interactable = state;
            }
            else
            {
                Toggle toggle = p1Panel.GetComponentInChildren<Toggle>();
                toggle.interactable = state;
            }
        }
    }
    [PunRPC]
    public void SetP1AnimatorReady(bool state)
	{
		if (p1Wait1.activeSelf)
		{
            P1Animator1.SetBool("Ready", state);
		}
		if (p1Wait2.activeSelf)
		{
            P1Animator2.SetBool("Ready", state);
		}
	}
    [PunRPC]
    public void SetP2AnimatorReady(bool state)
    {
        if (p2Wait1.activeSelf)
        {
            P2Animator1.SetBool("Ready", state);
        }
		if (p2Wait2.activeSelf)
        {
            P2Animator2.SetBool("Ready", state);
        }
    }
    [PunRPC]
    public void SetToggleIsOn(bool state)
	{
        if (masterPanelNum == 1)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == masterClientIndex)
            {
                Toggle toggle = p1Panel.GetComponentInChildren<Toggle>();
                readyCheck[1] = state;
                toggle.isOn = state;
            }
            else
            {
                Toggle toggle = p2Panel.GetComponentInChildren<Toggle>();
                readyCheck[0] = state;
                toggle.isOn = state;
            }
        }
        else
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == masterClientIndex)
            {
                Toggle toggle = p2Panel.GetComponentInChildren<Toggle>();
                readyCheck[0] = state;
                toggle.isOn = state;
            }
            else
            {
                Toggle toggle = p1Panel.GetComponentInChildren<Toggle>();
                readyCheck[1] = state;
                toggle.isOn = state;
            }
        }
    }
}
