using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using MySql.Data.MySqlClient;
using MySql.Data;

public class RankManager
{
    public static string[] users = new string[10];
    public static int[] scores = new int[10];

	public static string[] tempUserName = new string[10];
	public static int[] tempScore = new int[10];

    public static void RankingSet(string user, int score)
	{
		for (int i = 0; i < 10; i++)
		{
			if (users[i] == user && scores[i] >= score)
			{
				return;
			}
			else if (users[i] == user && scores[i] < score)
			{
				scores[i] = score;
				ScoreChange();
				return;
			}
		}

		UserAdded(user, score);
	}

	public static void ScoreChange()
	{
		for (int i = 0; i < 10; i++)
		{
			tempUserName[i] = users[i];
			users[i] = null;
			tempScore[i] = scores[i];

		}
		Array.Sort(scores);
		Array.Reverse(scores);

		for (int i = 0; i < 10; i++)
		{
			for (int j = 0; j < 10; j++)
			{
				if (scores[i] == tempScore[j])
				{
					bool isSame = false;
					for (int z = 0; z < 10; z++)
					{
						if (users[z] == tempUserName[j] && tempUserName[j] != "NoRank")
						{
							isSame = true;
							break;
						}
					}
					if (!isSame)
					{
						users[i] = tempUserName[j];
						break;
					}
				}
			}
				
				
			
		}
	}

	public static void UserAdded(string user, int score)
	{
		int temp = 0;
		string tempName = "";
		for (int i = 9; i >= 0; i--)
		{
			if (scores[i] < score && i < 9)
			{
				tempName = users[i];
				users[i] = user;
				users[i + 1] = tempName;
				temp = scores[i];
				scores[i] = score;
				scores[i + 1] = temp;
			}
			else if (scores[i] < score)
			{
				users[i] = user;
				scores[i] = score;
			}
			else
			{
				break;
			}
		}
	}
	public static void RankingToJson(JsonData jsonData)
	{
		for (int i = 0; i < users.Length; i++)
		{
			JsonMembers data = new JsonMembers();
			data.RANK = i + 1;
			data.USER = users[i];
			data.Score = scores[i];
			jsonData.result.Add(data);
		}
	}
	public static void RankManagerAllClear()
	{
		for (int i = 0; i < users.Length; i++)
		{
			users[i] = null;
			scores[i] = 0;
		}
	}
	public static bool CheckRankingDataEmpty()
	{
		bool check = true;
		for (int i = 0; i < 10; i++)
		{
			if (users[i] != null)
			{
				check = false;
				break;
			}
		}
		if (check)
		{
			RankCoUnity();
		}
		return check;
	}
	public static bool RankDBCheck()
	{
		bool check = true;
		for (int i = 0; i < 10; i++)
		{
			if (users[i] == null)
			{
				check = false;
				break;
			}
		}
		if (check)
		{
			RankingSet(DBManager.username, DBManager.score);
			SaveRanking();
		}
		return check;
	}
	public static void SaveRanking()
	{
		JsonData jData = new JsonData();
		RankingToJson(jData);

		string sqlConnect = "server=pickstar.co.kr;uid=pickstar;pwd=xktmvlr10;database=pickstar;charset=utf8;";

		MySqlConnection conn = new MySqlConnection(sqlConnect);
		if (conn.State != System.Data.ConnectionState.Closed)
		{
			Debug.Log("connetion Failed");
			return;
		}
		conn.Open();

		for (int i = 0; i < 10; i++)
		{
			string quary = "UPDATE CardRanking SET USER = '" + jData.result[i].USER + "', Score = '" + jData.result[i].Score + "' WHERE Ranking = '"
				+ jData.result[i].RANK + "'";
			MySqlCommand command = new MySqlCommand(quary, conn);
			MySqlDataReader rdr = command.ExecuteReader();
			rdr.Read();
			//if (rdr.RecordsAffected < 1)
			//{
			//	Debug.Log("DB Save Failed");
			//	rdr.Close();
			//	conn.Close();
			//	return;
			//}
			rdr.Close();
		}

		conn.Close();
		Debug.Log("Update Complete");

		RankManagerAllClear();
		Debug.Log("Game Saved.");


	}

	public static void RankCoUnity()
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
			users[rdr.GetInt32(0) - 1] = rdr.GetString(1);
			scores[rdr.GetInt32(0) - 1] = rdr.GetInt32(2);

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



	}

}
