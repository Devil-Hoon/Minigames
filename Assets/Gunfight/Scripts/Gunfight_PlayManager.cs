using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MySql.Data;
using MySql.Data.MySqlClient;

public class Gunfight_PlayManager : MonoBehaviourPunCallbacks
{
    [Header("Player Info")]

    public Transform p1Pos;
    public Transform p2Pos;

    public GameObject p1NameView;
    public GameObject p2NameView;

    public GameObject p1PointView;
    public GameObject p2PointView;
    
    public GameObject p1ScoreTextView;
    public GameObject p2ScoreTextView;

    public string p1Name;
    public string p1ID;
    public string p2Name;
    public string p2ID;

    public int p1Point;
    public int p2Point;

    public int p1Score;
    public int p2Score;

    public int p1ActNum;
    public int p2ActNum;

    public int flag;

    public GameObject player;

    [Header("End Panel")]
    public GameObject winPanel;
    public GameObject losePanel;
    public GameObject drawPanel;

    [Header("Play Time")]
    public GameObject countNum;
    public GameObject countNum10;
    public Sprite[] numSprites;

    [Header("Start Game")]
    public GameObject ready;
    public GameObject start;


    Room room;
    ExitGames.Client.Photon.Hashtable cp;

    float readyTime;
    float limitCount = 0;
    float playTime;
	bool gameEnd;
    bool backMain;
    int pointResult1;
    int pointResult2;
    int score;
    bool saveOnce;
    bool once;
    bool spawnOnce;
    bool finalLoadGp;


    private static Gunfight_PlayManager instance;
    public static Gunfight_PlayManager Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType<Gunfight_PlayManager>();
            }
            return instance;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        finalLoadGp = false;
        Debug.Log("PlayManagerStart");
        spawnOnce = false;
        readyTime = 0.0f;
        saveOnce = false;
        once = false;
        flag = -1;
        if (PhotonNetwork.InRoom)
        {
            BGSoundManager.instance.StopBGM();
            BGSoundManager.instance.PlayGunfightGameBGM();
            room = PhotonNetwork.CurrentRoom;
            if (room == null)
            {
                return;
            }
            playTime = 30.0f;
            cp = room.CustomProperties;
            Button button = winPanel.GetComponentInChildren<Button>();
            button.onClick.AddListener(BackToMain);
            button = losePanel.GetComponentInChildren<Button>();
            button.onClick.AddListener(BackToMain);
            button = drawPanel.GetComponentInChildren<Button>();
            button.onClick.AddListener(BackToMain);
            ready.SetActive(true);
            start.SetActive(false);
            winPanel.SetActive(false);
            losePanel.SetActive(false);
            drawPanel.SetActive(false);
            DBManager.LoadGP();
            if (PhotonNetwork.LocalPlayer.ActorNumber == PhotonNetwork.MasterClient.ActorNumber)
            {
                photonView.RPC("SetPlayer1ActNum", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);
            }
            else
            {
                photonView.RPC("SetPlayer2ActNum", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);
            }
            Spawn();
        }
    }

    // Update is called once per frame
    void Update()
    {
		if (readyTime < 3.6f)
		{
            readyTime += Time.deltaTime;
            PlayerInfoView();
        }

		if (readyTime > 3.0f)
        {
            photonView.RPC("ReadyAndStart", RpcTarget.All, readyTime);
            if (player != null && room.PlayerCount == 2 && !gameEnd && !backMain)
            {
				if (!once)
				{
                    once = true;
                    SetGameStart();
                }
                PlayerScore();
                ScoreView();
                PlayerInfoView();

				if (PhotonNetwork.IsMasterClient)
                {
                    playTime -= Time.deltaTime;
                    photonView.RPC("SetTimeNumeric", RpcTarget.All, playTime);
                }
				
                if (playTime <= 0)
                {
                    playTime = 0.0f;
                    gameEnd = true;
                }
            }
            else if (player != null && room.PlayerCount == 2 && gameEnd && !backMain)
            {
                SetGameEnd();
                ScoreView();
                PlayerInfoView();
                DBManager.LoadGP();
                GameEnd();
                if (!saveOnce && PhotonNetwork.LocalPlayer.ActorNumber == PhotonNetwork.MasterClient.ActorNumber)
                {
                    CallSaveData();
                }
            }
			else if (room.PlayerCount == 1 && !backMain)
			{
                SetGameEnd();
                ScoreView();
                PlayerInfoView();
                if (!saveOnce)
                {
                    GameLeave();
                    CallSaveData();
                }
            }

        }

    }
    private void OnApplicationQuit()
    {
        PhotonNetwork.Disconnect();
        DBManager.LogOut();
    }
    public override void OnLeftRoom()
	{
        saveOnce = false;

        BGSoundManager.instance.StopBGM();
        SceneManager.LoadScene("GunfightMain");
    }
	public void Spawn()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == PhotonNetwork.MasterClient.ActorNumber)
        {
			if (!spawnOnce)
			{
                spawnOnce = true;
                player = PhotonNetwork.Instantiate("Prefabs/Player", p1Pos.position, new Quaternion(0, 180, 0, 0));
                player.GetComponent<PhotonView>().RPC("SetWhite", RpcTarget.All);
                string name = DBManager.nickname;
                string id = DBManager.username;
                int point = DBManager.gp;
                photonView.RPC("SetPlayer1Info", RpcTarget.All, name, id, point);
                Debug.Log("P1소환");
            }
        }
        else
        {
            if (!spawnOnce)
            {
                spawnOnce = true;
                player = PhotonNetwork.Instantiate("Prefabs/Player", p2Pos.position, Quaternion.identity);
                player.GetComponent<PhotonView>().RPC("SetBlack", RpcTarget.All);
                string name = DBManager.nickname;
                string id = DBManager.username;
                int point = DBManager.gp;
                photonView.RPC("SetPlayer2Info", RpcTarget.All, name, id, point);
                Debug.Log("P2소환");
            }
        }
    }

    public void PlayerScore()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == PhotonNetwork.MasterClient.ActorNumber)
        {
            //p2Score = player.GetComponent<PlayerScript>().EnemyScore;
            score = player.GetComponent<PlayerScript>().EnemyScore;
            photonView.RPC("SetScorePlayer2", RpcTarget.All, score);
        }
        else
        {
            //p1Score = player.GetComponent<PlayerScript>().EnemyScore;
            score = player.GetComponent<PlayerScript>().EnemyScore;
            photonView.RPC("SetScorePlayer1", RpcTarget.All, score);
        }
    }


    public void ScoreView()
	{
        p1ScoreTextView.GetComponent<Text>().text = p1Score <= 0 ? "0" : string.Format("{0:#,###}", p1Score);
        p2ScoreTextView.GetComponent<Text>().text = p2Score <= 0 ? "0" : string.Format("{0:#,###}", p2Score);
    }

    public void PlayerInfoView()
	{
        p1NameView.GetComponent<Text>().text = p1Name;
        p1PointView.GetComponent<Text>().text = p1Point <= 0 ? "0" : string.Format("{0:#,###}", p1Point);
        p2NameView.GetComponent<Text>().text = p2Name;
        p2PointView.GetComponent<Text>().text = p2Point <= 0 ? "0" : string.Format("{0:#,###}", p2Point);
    }

    public void PointResult()
    {
        if (p1Score > p2Score)

        {
            pointResult1 = p1Score - p2Score;
            pointResult2 = p2Score - p1Score;
            flag = 0;
            SetPlayerPoint(pointResult1, pointResult2);
        }
        else if (p2Score > p1Score)
        {
            pointResult1 = p1Score - p2Score;
            pointResult2 = p2Score - p1Score;
            flag = 1;
            SetPlayerPoint(pointResult1, pointResult2);
        }
        else
        {
            pointResult1 = p1Score - p2Score;
            pointResult2 = p2Score - p1Score;
            flag = 2;
            SetPlayerPoint(pointResult1, pointResult2);
        }
    }

    public void SetPlayerPoint(int p1Point, int p2Point)
	{
        if (PhotonNetwork.LocalPlayer.ActorNumber == PhotonNetwork.MasterClient.ActorNumber)
        {
            DBManager.gp += p1Point;
        }
        else
        {
            DBManager.gp += p2Point;
        }
    }
    public void SetGameStart()
	{
        player.GetComponent<PlayerScript>().isStart = true;
	}

    public void SetGameEnd()
	{
        player.GetComponent<PlayerScript>().isEnd = true;
	}

    public void GameEnd()
	{
		if (p1Score > p2Score && PhotonNetwork.LocalPlayer.ActorNumber == PhotonNetwork.MasterClient.ActorNumber)
		{
            winPanel.SetActive(true);
            PointResult();
            winPanel.GetComponent<EndPanelScript>().SetPoint(pointResult1);
		}
        else if (p1Score > p2Score && PhotonNetwork.LocalPlayer.ActorNumber != PhotonNetwork.MasterClient.ActorNumber)
        {
            losePanel.SetActive(true);
            PointResult();
            losePanel.GetComponent<EndPanelScript>().SetPoint(pointResult2);
        }
        else if (p1Score < p2Score && PhotonNetwork.LocalPlayer.ActorNumber == PhotonNetwork.MasterClient.ActorNumber)
        {
            losePanel.SetActive(true);
            PointResult();
            losePanel.GetComponent<EndPanelScript>().SetPoint(pointResult1);
        }
        else if (p1Score < p2Score && PhotonNetwork.LocalPlayer.ActorNumber != PhotonNetwork.MasterClient.ActorNumber)
        {
            winPanel.SetActive(true);
            PointResult();
            winPanel.GetComponent<EndPanelScript>().SetPoint(pointResult2);
        }
		else
		{
            drawPanel.SetActive(true);
            PointResult();
            drawPanel.GetComponent<EndPanelScript>().SetPoint(pointResult1);
		}

        backMain = true;
    }

    public void GameLeave()
	{

		if (p1Score > p2Score)
		{
            if (PhotonNetwork.LocalPlayer.ActorNumber == p1ActNum)
            {
                winPanel.SetActive(true);
                pointResult1 = p1Score - p2Score;
                pointResult2 = p2Score - p1Score;
                flag = 0;
                SetPlayerPoint(pointResult1, pointResult2);
                winPanel.GetComponent<EndPanelScript>().SetPoint(pointResult1);
            }
			else if (PhotonNetwork.LocalPlayer.ActorNumber == p2ActNum)
			{
                losePanel.SetActive(true);
                pointResult1 = p1Score - p2Score;
                pointResult2 = p2Score - p1Score;
                pointResult1 = (int)(pointResult1 * 0.5f);
                pointResult2 = (int)(pointResult2 * 0.5f);
                flag = 0;
                SetPlayerPoint(pointResult1, pointResult2);
                losePanel.GetComponent<EndPanelScript>().SetPoint(pointResult2);
            }

        }
		else if (p1Score < p2Score)
		{
            if (PhotonNetwork.LocalPlayer.ActorNumber == p1ActNum)
            {
                losePanel.SetActive(true);
                pointResult1 = p1Score - p2Score;
                pointResult2 = p2Score - p1Score;
                pointResult1 = (int)(pointResult1 * 0.5f);
                pointResult2 = (int)(pointResult2 * 0.5f);
                flag = 1;
                SetPlayerPoint(pointResult1, pointResult2);
                losePanel.GetComponent<EndPanelScript>().SetPoint(pointResult1);
            }
            else if (PhotonNetwork.LocalPlayer.ActorNumber == p2ActNum)
            {
                winPanel.SetActive(true);
                pointResult1 = p1Score - p2Score;
                pointResult2 = p2Score - p1Score;
                flag = 1;
                SetPlayerPoint(pointResult1, pointResult2);
                winPanel.GetComponent<EndPanelScript>().SetPoint(pointResult2);
            }

        }
        else
		{
            drawPanel.SetActive(true);
            PointResult();
            drawPanel.GetComponent<EndPanelScript>().SetPoint(pointResult1);
        }

    }
    public void BackToMain()
    {
        if (p1Score > p2Score && PhotonNetwork.LocalPlayer.ActorNumber == PhotonNetwork.MasterClient.ActorNumber)
        {
            winPanel.SetActive(false);
        }
        else if (p1Score > p2Score && PhotonNetwork.LocalPlayer.ActorNumber != PhotonNetwork.MasterClient.ActorNumber)
        {
            losePanel.SetActive(false);
        }
        else if (p1Score < p2Score && PhotonNetwork.LocalPlayer.ActorNumber == PhotonNetwork.MasterClient.ActorNumber)
        {
            losePanel.SetActive(false);
        }
        else if (p1Score < p2Score && PhotonNetwork.LocalPlayer.ActorNumber != PhotonNetwork.MasterClient.ActorNumber)
        {
            winPanel.SetActive(false);
        }
        else
        {
            drawPanel.SetActive(false);
        }
        PhotonNetwork.LeaveRoom();
    }
    public void CallSaveData()
    {
        SavePlayerData();
        saveOnce = true;
    }

    public void SavePlayerData()
    {
        string sqlConnect = "server=pickstar.co.kr;uid=pickstar;pwd=xktmvlr10;database=pickstar;charset=utf8;";

        int p1GP = 0, p2GP = 0;
        string winner = "";

        if (flag == 0)
        {
            //p1Final = p1Point + pointResult;
            //p2Final = p2Point - pointResult;
            winner = p1ID;
            //if (p2Final <= 0)
            //{
            //    p2Final = 0;
            //    p1Final = p1Point + p2Point;
            //}
        }
        else if (flag == 1)
        {
            //p1Final = p1Point - pointResult;
            //p2Final = p2Point + pointResult;
            winner = p2ID;
            //if (p1Final <= 0)
            //{
            //    p1Final = 0;
            //    p2Final = p1Point + p2Point;
            //}
        }
        else
        {
            //p1Final = p1Point + pointResult;
            //p2Final = p2Point + pointResult;
            winner = "None";
        }



        MySqlConnection conn = new MySqlConnection(sqlConnect);
        if (conn.State != System.Data.ConnectionState.Closed)
        {
            Debug.Log("connetion Failed");
            return;
        }
        conn.Open();

        //string quary = "INSERT INTO GunfightResult VALUES(DEFAULT, '" + p1ID + "','" + p2ID + "'," +
        //    p1Point + "," + p2Point + "," + pointResult + "," + p1Final + "," + p2Final + ",'" + winner + "',DEFAULT)";

        string p1Quary = "SELECT money From Member WHERE userid = '" + p1ID + "'";
        MySqlCommand p1Cmd = new MySqlCommand(p1Quary, conn);
        MySqlDataReader p1Rdr = p1Cmd.ExecuteReader();
		if (!p1Rdr.HasRows)
		{
            Debug.Log("P1GPLoadFailed");
            conn.Close();
            return;
        }
        p1Rdr.Read();
        p1GP = int.Parse(p1Rdr["money"].ToString());
        p1Rdr.Close();

        string p2Quary = "SELECT money From Member WHERE userid = '" + p2ID + "'";
        MySqlCommand p2Cmd = new MySqlCommand(p2Quary, conn);
        MySqlDataReader p2Rdr = p2Cmd.ExecuteReader();
        if (!p2Rdr.HasRows)
        {
            Debug.Log("P2GPLoadFailed");
            conn.Close();
            return;
        }
        p2Rdr.Read();
        p2GP = int.Parse(p2Rdr["money"].ToString());
        p2Rdr.Close();

		if (p1GP + pointResult1 < 0 && p2GP + pointResult2 >= 0)
		{
            string quary = "INSERT INTO GunfightResult VALUES(DEFAULT, '" + p1ID + "', '" + p2ID + "', " +
                        " (SELECT money FROM Member WHERE userid = '" + p1ID + "'), (SELECT money FROM Member WHERE userid = '" + p2ID + "'), " +
                        p1GP + ", 0," + "(SELECT money + (" + p1GP + ") FROM Member WHERE userid = '" + p2ID + "'), '" + winner + "', DEFAULT)";
            MySqlCommand command = new MySqlCommand(quary, conn);
            MySqlDataReader rdr = command.ExecuteReader();
            rdr.Read();
            rdr.Close();
            string updateQuary = "UPDATE member SET money = 0 WHERE userid = '" + p1ID + "';";
            command = new MySqlCommand(updateQuary, conn);
            MySqlDataReader uRdr = command.ExecuteReader();
            uRdr.Read();
            uRdr.Close();
            updateQuary = "UPDATE member SET money = money + ('" + p1GP + "') WHERE userid = '" + p2ID + "';";
            command = new MySqlCommand(updateQuary, conn);
            MySqlDataReader upRdr = command.ExecuteReader();
            upRdr.Read();
            upRdr.Close();
        }
		else if (p1GP + pointResult1 >= 0 && p2GP + pointResult2 < 0)
		{
            string quary = "INSERT INTO GunfightResult VALUES(DEFAULT, '" + p1ID + "', '" + p2ID + "', " +
                        " (SELECT money FROM Member WHERE userid = '" + p1ID + "'), (SELECT money FROM Member WHERE userid = '" + p2ID + "'), " +
                        p2GP + ", " + "(SELECT money + (" + p2GP + ") FROM Member WHERE userid = '" + p1ID + "'), 0, '" + winner + "', DEFAULT)";
            MySqlCommand command = new MySqlCommand(quary, conn);
            MySqlDataReader rdr = command.ExecuteReader();
            rdr.Read();
            rdr.Close();
            string updateQuary = "UPDATE member SET money = money + ('" + p2GP + "') WHERE userid = '" + p1ID + "';";
            command = new MySqlCommand(updateQuary, conn);
            MySqlDataReader uRdr = command.ExecuteReader();
            uRdr.Read();
            uRdr.Close();
            updateQuary = "UPDATE member SET money = 0 WHERE userid = '" + p2ID + "';";
            command = new MySqlCommand(updateQuary, conn);
            MySqlDataReader upRdr = command.ExecuteReader();
            upRdr.Read();
            upRdr.Close();
        }
		else
		{
            string quary = "INSERT INTO GunfightResult VALUES(DEFAULT, '" + p1ID + "', '" + p2ID + "', " +
                        " (SELECT money FROM Member WHERE userid = '" + p1ID + "'), (SELECT money FROM Member WHERE userid = '" + p2ID + "'), " +
                        Mathf.Abs(pointResult1) + ", " +
                        "(SELECT money + (" + pointResult1 + ") FROM Member WHERE userid = '" + p1ID + "'), " +
                        "(SELECT money + (" + pointResult2 + ") FROM Member WHERE userid = '" + p2ID + "'), '" + winner + "', DEFAULT)";
            MySqlCommand command = new MySqlCommand(quary, conn);
            MySqlDataReader rdr = command.ExecuteReader();
            rdr.Read();
            rdr.Close();
            string updateQuary = "UPDATE member SET money = money + ('" + pointResult1 + "') WHERE userid = '" + p1ID + "';";
            command = new MySqlCommand(updateQuary, conn);
            MySqlDataReader uRdr = command.ExecuteReader();
            uRdr.Read();
            uRdr.Close();
            updateQuary = "UPDATE member SET money =money + ('" + pointResult2 + "') WHERE userid = '" + p2ID + "';";
            command = new MySqlCommand(updateQuary, conn);
            MySqlDataReader upRdr = command.ExecuteReader();
            upRdr.Read();
            upRdr.Close();
        }


        

        conn.Close();

    }
    [PunRPC]
    public void SetScorePlayer1(int score)
    {
        p1Score = score;
    }

    [PunRPC]
    public void SetScorePlayer2(int score)
    {
        p2Score = score;
    }

    [PunRPC]
    public void SetPlayer1Info(string name, string id, int point)
    {
        p1Name = name;
        p1ID = id;
        p1Point = point;
    }
    [PunRPC]
    public void SetPlayer1Point(string name, string id, int point)
    {
        p1Name = name;
        p1ID = id;
        p1Point = point;
    }

    [PunRPC]
    public void SetPlayer2Info(string name, string id, int point)
    {
        p2Name = name;
        p2ID = id;
        p2Point = point;
    }
    [PunRPC]
    public void SetPlayer2Point(string name, string id, int point)
    {
        p2Name = name;
        p2ID = id;
        p2Point = point;
    }
    [PunRPC]
    public void SetPlayer1ActNum(int num)
	{
        p1ActNum = num;
	}
    [PunRPC]
    public void SetPlayer2ActNum(int num)
    {
        p2ActNum = num;
    }


    [PunRPC]
    public void SetTimeNumeric(float time)
    {
        playTime = time;
        int num = Mathf.FloorToInt(time);

        if (num < 0)
        {
            num = 0;
        }

        int n = num % 10;
        int n10 = num / 10;

        Image image = countNum.GetComponent<Image>();
        image.sprite = numSprites[n];
        image = countNum10.GetComponent<Image>();
        image.sprite = numSprites[n10];
    }
    [PunRPC]
    public void ReadyAndStart(float time)
    {
		if (time > 3.0f)
		{
            ready.SetActive(false);
            start.SetActive(true);
			if (time > 3.5f)
			{
                start.SetActive(false);
			}
        }

    }
}
