using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using MySql.Data.MySqlClient;
public enum StageNum
{
	ONE,
	TWO,
	THREE,
	FOUR
}

public struct Enchant_AdminData
{
	public string userId;
	public string action;
	public string seedName;
	public int price;
	public int hopePrice;

	public void Reset()
	{
		userId = "";
		action = "";
		seedName = "";
		price = 0;
		hopePrice = 0;
	}

	//EnchantGame SeedInfoSet
	public void Set(string id, string action, string seedName, int price, int hopePrice)
	{
		userId = id;
		this.action = action;
		switch (seedName)
		{
			case "FlowerSeed":
				this.seedName = "A";
				break;
			case "AppleSeed":
				this.seedName = "B";
				break;
			case "RandomSeed":
				this.seedName = "C";
				break;
			default:
				this.seedName = seedName;
				break;
		}
		this.price = price;
		this.hopePrice = hopePrice;
	}
}
public static class DBManager
{
	public static string username;
	public static string nickname;
	public static int gp;
	public static int score;
	public static string data;
	public static StageNum stageNum = StageNum.ONE;
	public static bool isPlant;
	public static string seedName;
	public static int seedTempletNum;
	public static int seedEnhance;
	public static int randomSeedCost;
	public static Enchant_AdminData adminData;
	public static string gameVersion;

	public static bool LoggedIn { get { return username != null; } }
	public static bool Login(in string id_input,in string password_input, out string alarmMessage)
	{
		string id = id_input;
		string password = password_input;
		//string passwordHash = passwordEncryption(id + password);
		string sqlConnect = "server=servername;uid=id;pwd=password;database=database;charset=utf8;";


		MySqlConnection conn = new MySqlConnection(sqlConnect);
		if (conn.State != System.Data.ConnectionState.Closed)
		{
			Debug.Log("connetion Failed");
			alarmMessage = "DataBase에 접근할 수 없습니다.";
			return false;
		}
		conn.Open();

		string quary = "SELECT * FROM member  WHERE(userid = '" + id + "' OR username='" + id + "') AND pw='" + password + "'";
		MySqlCommand command = new MySqlCommand(quary, conn);
		MySqlDataReader rdr = command.ExecuteReader();

		if (!rdr.HasRows)
		{
			rdr.Read();
			rdr.Close();
			conn.Close();
			alarmMessage = "아이디 또는 비밀번호가 일치하지 않습니다.";
			return false;
		}
		else
		{
			rdr.Read();
			string tempid = rdr["userid"].ToString();
			string tempnickname = rdr["username"].ToString();
			int tempgp = int.Parse(rdr["money"].ToString());

			if (int.Parse(rdr["out"].ToString()) == 1)
			{
				alarmMessage = "탈퇴 신청 중인 아이디입니다.";
				rdr.Close();
				conn.Close();
				return false;
			}
			else if (int.Parse(rdr["out"].ToString()) == 2)
			{
				alarmMessage = "회원 탈퇴된 아이디입니다.";
				rdr.Close();
				conn.Close();
				return false;
			}
			else if (int.Parse(rdr["out"].ToString()) == 3)
			{
				alarmMessage = "정지 된 아이디입니다.";
				rdr.Close();
				conn.Close();
				return false;
			}
			rdr.Close();
			string loginCheckQuary = "SELECT * FROM clientCheck WHERE userid = '" + tempid + "'";
			MySqlCommand loginCmd = new MySqlCommand(loginCheckQuary, conn);
			MySqlDataReader loginRdr = loginCmd.ExecuteReader();
			if (!loginRdr.HasRows)
			{
				loginRdr.Read();
				loginRdr.Close();
				string sql = "INSERT INTO clientCheck VALUES('" + tempid + "', 1)";
				MySqlCommand cmd = new MySqlCommand(sql, conn);
				MySqlDataReader datardr = cmd.ExecuteReader();
				datardr.Read();
				datardr.Close();

				username = tempid;
				nickname = tempnickname;
				gp = tempgp;

				conn.Close();

				alarmMessage = "";
				return true;
			}
			else
			{
				int flag = int.MinValue;

				loginRdr.Read();
				flag = loginRdr.GetInt32(1);
				loginRdr.Close();

				if (flag == 1)
				{
					alarmMessage = "동일한 아이디가 이미 로그인 되어있습니다.";
					return false;
				}

				string sql = "UPDATE clientCheck SET lcheck = 1 WHERE userid = '" + tempid + "'";
				MySqlCommand cmd = new MySqlCommand(sql, conn);
				MySqlDataReader datardr = cmd.ExecuteReader();
				datardr.Read();
				datardr.Close();

				username = tempid;
				nickname = tempnickname;
				gp = tempgp;
				
				conn.Close();

				alarmMessage = "";
				return true;
			}
		}

	}
	public static bool LogOut()
	{
		if (!LoggedIn)
		{
			return true;
		}

		string sqlConnect = "server=servername;uid=id;pwd=password;database=database;charset=utf8;";

		MySqlConnection conn = new MySqlConnection(sqlConnect);
		if (conn.State != System.Data.ConnectionState.Closed)
		{
			Debug.Log("connetion Failed");
			return false;
		}
		conn.Open();

		string sql = "UPDATE clientCheck SET lcheck = 0 WHERE userid = '" + username + "'";
		MySqlCommand cmd = new MySqlCommand(sql, conn);
		MySqlDataReader datardr = cmd.ExecuteReader();
		datardr.Read();
		datardr.Close();

		conn.Close();

		username = null;
		nickname = null;
		gp = 0;
		return true;
	}
	public static void LoadGP()
	{
		string id = username;
		string sqlConnect = "server=servername;uid=id;pwd=password;database=database;charset=utf8;";

		MySqlConnection conn = new MySqlConnection(sqlConnect);
		if (conn.State != System.Data.ConnectionState.Closed)
		{
			Debug.Log("connetion Failed");
			return;
		}
		conn.Open();

		MySqlCommand command = new MySqlCommand("SELECT * FROM member  WHERE userid = '" + id + "' OR username='" + id + "';", conn);
		MySqlDataReader rdr = command.ExecuteReader();

		if (!rdr.HasRows)
		{
			Debug.Log("DB Load Failed");
			conn.Close();
			return;
		}

		while (rdr.Read())
		{
			gp = int.Parse(rdr["money"].ToString());
		}

		rdr.Close();
		conn.Close();
	}
	//public static string passwordEncryption(string passwordString)
	//{
	//	UTF8Encoding ue = new UTF8Encoding();
	//	byte[] bytes = ue.GetBytes(passwordString);


	//	SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider();
	//	byte[] hashBytes = sha256.ComputeHash(bytes);

	//	string hashString = System.Convert.ToBase64String(hashBytes);


	//	return hashString;
	//}

	public static void UpdateAdminDataTrade()
	{
		string sqlConnect = "server=servername;uid=id;pwd=password;database=database;charset=utf8;";

		MySqlConnection conn = new MySqlConnection(sqlConnect);
		if (conn.State != System.Data.ConnectionState.Closed)
		{
			Debug.Log("connetion Failed");
			return;
		}
		conn.Open();

		string quary = "INSERT INTO ENCHANT_ADMIN VALUES(DEFAULT, '" + adminData.userId + "','" + adminData.action + "' , '" + adminData.seedName +
					"', '" + adminData.price + "','" + adminData.hopePrice + "', (SELECT money FROM Member WHERE userid = '" + adminData.userId + "'), DEFAULT)";
		MySqlCommand command = new MySqlCommand(quary, conn);
		MySqlDataReader rdr = command.ExecuteReader();
		rdr.Read();
		rdr.Close();
		conn.Close();

		adminData.Reset();
	}
	public static void UpdateAdminDataEnchant()
	{
		string sqlConnect = "server=servername;uid=id;pwd=password;database=database;charset=utf8;";

		MySqlConnection conn = new MySqlConnection(sqlConnect);
		if (conn.State != System.Data.ConnectionState.Closed)
		{
			Debug.Log("connetion Failed");
			return;
		}
		conn.Open();

		string quary = "INSERT INTO ENCHANT_ADMIN VALUES(DEFAULT, '" + adminData.userId + "','" + adminData.action + "' , '" + adminData.seedName +
					"', 0 ,'" + adminData.hopePrice + "', (SELECT money FROM Member WHERE userid = '" + adminData.userId + "'), DEFAULT)";
		MySqlCommand command = new MySqlCommand(quary, conn);
		MySqlDataReader rdr = command.ExecuteReader();
		rdr.Read();
		rdr.Close();
		conn.Close();

		adminData.Reset();
	}

	public static bool GameVersionCheck()
	{
		string sqlConnect = "server=servername;uid=id;pwd=password;database=database;charset=utf8;";
		string tempVersion;
		MySqlConnection conn = new MySqlConnection(sqlConnect);
		if (conn.State != System.Data.ConnectionState.Closed)
		{
			Debug.Log("connetion Failed");
			return false;
		}
		conn.Open();

		string quary = "SELECT version FROM kMinigame WHERE filename = 'Minigames.exe';";
		MySqlCommand command = new MySqlCommand(quary, conn);
		MySqlDataReader rdr = command.ExecuteReader();
		if (!rdr.HasRows)
		{
			conn.Close();
			return false;
		}
		else
		{
			rdr.Read();
			tempVersion = rdr["version"].ToString();
			rdr.Close();
		}
		conn.Close();

		if (gameVersion != tempVersion)
		{
			Debug.Log("일치하지않음");
			return false;
		}
		else
		{
			return true;
		}
	}

}
