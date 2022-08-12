using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MySql.Data;
using MySql.Data.MySqlClient;
using TMPro;
public class EnchantGame_SeedPlant : MonoBehaviour
{
	public GameObject seed;
	public EnchantGame_SeedTemplet seedTemplet;
	public bool isPlant;

	public Button enhanceBtn;
	public Button sellBtn;
	public Button shopBtn;

	public int maxEnchant;
	public int enchant;
	public float rate;

	public bool updateCheck;

	public void Initialize(GameObject seed, EnchantGame_SeedTemplet seedTemplet)
	{
		isPlant = true;
		this.seed = seed;
		this.seedTemplet = seedTemplet;
		enchant = seedTemplet.seeds[0].enchant;
		maxEnchant = seedTemplet.seeds[seedTemplet.seeds.Length - 1].enchant;
		rate = seedTemplet.seeds[0].rate;
		updateCheck = true;
	}

	public void SeedLoad(GameObject seed, EnchantGame_SeedTemplet seedTemplet, int enchant)
	{
		isPlant = true;
		this.seed = seed;
		this.seedTemplet = seedTemplet;
		this.enchant = enchant;
		this.rate = seedTemplet.seeds[enchant].rate; 
		maxEnchant = seedTemplet.seeds[seedTemplet.seeds.Length - 1].enchant;
		updateCheck = true;
	}

	public void Enhance()
	{
		if (seed == null)
		{
			return;
		}

		shopBtn.interactable = false;
		enhanceBtn.interactable = false;
		sellBtn.interactable = false;
		gameObject.GetComponent<EnchantGame_Earthquake>().EarthquakePlay();
	}

	public void EnhanceComplete()
	{
		if (enchant < seedTemplet.seeds.Length - 1)
		{
			enchant = seedTemplet.seeds[enchant + 1].enchant;
			rate = seedTemplet.seeds[enchant].rate;
			seed.GetComponent<SpriteRenderer>().sprite = seedTemplet.seeds[enchant].sprite;
			DBManager.adminData.Set(DBManager.username, "강화성공", seedTemplet.seedName , 0, seedTemplet.seeds[enchant].sell);
			DBManager.isPlant = isPlant;
			DBManager.seedName = seedTemplet.seedName;
			DBManager.seedEnhance = seedTemplet.seeds[enchant].enchant;

			UpdateDataEnchant();
			DBManager.UpdateAdminDataEnchant();
		}
	}

	public void EnhanceFail()
	{

		DBManager.adminData.Set(DBManager.username, "강화실패", seedTemplet.seedName, 0, 0);
		Destroy(seed);
		seed = null;
		seedTemplet = null;
		isPlant = false;
		enchant = 0;

		DBManager.isPlant = isPlant;
		DBManager.seedName = "";
		DBManager.seedTempletNum = 0;
		DBManager.seedEnhance = 0;
		DBManager.randomSeedCost = 0;

		UpdateDataEnchant();
		DBManager.UpdateAdminDataEnchant();
	}

	public void SeedSell()
	{
		if (seed == null)
		{
			return;
		}


		int price = seedTemplet.seeds[enchant].sell;
		DBManager.adminData.Set(DBManager.username, "씨앗판매", seedTemplet.seedName, price, 0);
		Destroy(seed);
		seed = null;
		seedTemplet = null;
		isPlant = false;
		enchant = 0;

		DBManager.isPlant = isPlant;
		DBManager.seedName =  "";
		DBManager.seedTempletNum = 0;
		DBManager.seedEnhance = 0;
		DBManager.randomSeedCost = 0;

		UpdateDataTrade(price);
		DBManager.UpdateAdminDataTrade();
		DBManager.LoadGP();
	}

	public void UpdateDataEnchant()
	{
		string sqlConnect = "server=pickstar.co.kr;uid=pickstar;pwd=xktmvlr10;database=pickstar;charset=utf8;";

		MySqlConnection conn = new MySqlConnection(sqlConnect);
		if (conn.State != System.Data.ConnectionState.Closed)
		{
			Debug.Log("connetion Failed");
			return;
		}
		conn.Open();
		string sql = "UPDATE EnhanceData SET IsPlant = '"+ DBManager.isPlant +"', SeedName = '"+ DBManager.seedName
			+ "', SeedTemplet = '"+ DBManager.seedTempletNum +"', Enhance = '"+ DBManager.seedEnhance +"', RandomSeedCost = '"+ DBManager.randomSeedCost 
			+"' WHERE User = '"+ DBManager.username +"';";
		MySqlCommand command = new MySqlCommand(sql, conn);
		MySqlDataReader rdr = command.ExecuteReader();
		rdr.Read();
		rdr.Close();
		conn.Close();
	}

	public void UpdateDataTrade(int price)
	{
		string sqlConnect = "server=pickstar.co.kr;uid=pickstar;pwd=xktmvlr10;database=pickstar;charset=utf8;";

		MySqlConnection conn = new MySqlConnection(sqlConnect);
		if (conn.State != System.Data.ConnectionState.Closed)
		{
			Debug.Log("connetion Failed");
			return;
		}
		conn.Open();
		string sql = "UPDATE EnhanceData SET IsPlant = '" + DBManager.isPlant + "', SeedName = '" + DBManager.seedName
			+ "', SeedTemplet = '" + DBManager.seedTempletNum + "', Enhance = '" + DBManager.seedEnhance + "', RandomSeedCost = '" + DBManager.randomSeedCost
			+ "' WHERE User = '" + DBManager.username + "';";
		MySqlCommand command = new MySqlCommand(sql, conn);
		MySqlDataReader rdr = command.ExecuteReader();
		rdr.Read();
		rdr.Close();
		sql = "UPDATE member SET money = money + ('" + price + "') WHERE userid = '" + DBManager.username + "';";
		command = new MySqlCommand(sql, conn);
		MySqlDataReader updateRdr = command.ExecuteReader();
		updateRdr.Read();
		updateRdr.Close();
		conn.Close();
	}
}
