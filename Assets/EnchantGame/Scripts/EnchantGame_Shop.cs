using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MySql.Data.MySqlClient;
public class EnchantGame_Shop : MonoBehaviour
{
    [SerializeField]
    private GameObject canvas;
    [SerializeField]
    public GameObject blackOut;
    [SerializeField]
    private GameObject shopPanel;
    [SerializeField]
    private Button enchantBtn;
    [SerializeField]
    private Button sellBtn;

    [SerializeField]
    private EnchantGame_SeedTemplet[] seeds;
    [SerializeField]
    private EnchantGame_SeedPlant seedPlant;
    [SerializeField]
    private Transform seedBase;
    
    public TMP_InputField buyout;

    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShopPanelOn()
    {
        shopPanel.SetActive(true);
    }

    public void ShopPanelOff()
	{
        shopPanel.SetActive(false);
	}

    public void FirstSeedBuy()
	{
        DBManager.LoadGP();
		if (DBManager.isPlant)
		{
            return;
		}
		if (DBManager.gp < seeds[0].seeds[0].sell)
		{
            return;
		}

        animator.SetBool("isPlant1", true);
        shopPanel.SetActive(false);
    }
    public void FirstSeedInstantiate()
	{
        if (!DBManager.isPlant)
        {
            DBManager.adminData.Set(DBManager.username, "씨앗구매", "A", -seeds[0].seeds[0].sell, 0);
            GameObject clone = Instantiate(seeds[0].seedPrefab, seedBase);
            seedPlant.Initialize(clone, seeds[0]);
            DBManager.isPlant = true;
            DBManager.seedName = seeds[0].seedName;
            DBManager.seedTempletNum = 0;
            DBManager.seedEnhance = seeds[0].seeds[0].enchant;

            seedPlant.UpdateDataTrade(-seeds[0].seeds[0].sell);
            DBManager.UpdateAdminDataTrade();
            DBManager.LoadGP();
            enchantBtn.interactable = true;
        }

    }
    public void SecondSeedBuy()
    {
        DBManager.LoadGP();
        if (DBManager.isPlant)
        {
            return;
        }
        if (DBManager.gp < seeds[1].seeds[0].sell)
        {
            return;
        }


        animator.SetBool("isPlant2", true);
        shopPanel.SetActive(false);
    }

    public void SecondSeedInstantiate()
    {
        if (!DBManager.isPlant)
        {
            DBManager.adminData.Set(DBManager.username, "씨앗구매", "B", -seeds[1].seeds[0].sell, 0);
            GameObject clone = Instantiate(seeds[1].seedPrefab, seedBase);
            seedPlant.Initialize(clone, seeds[1]);

            DBManager.isPlant = true;
            DBManager.seedName = seeds[1].seedName;
            DBManager.seedTempletNum = 1;
            DBManager.seedEnhance = seeds[1].seeds[0].enchant;

            seedPlant.UpdateDataTrade(-seeds[1].seeds[0].sell);
            DBManager.UpdateAdminDataTrade();
            DBManager.LoadGP();
            enchantBtn.interactable = true;
        }

    }
    public void ThirdSeedBuy()
    {
        DBManager.LoadGP();
        if (buyout.text == null)
		{
            return;
		}
        if (DBManager.isPlant)
        {
            return;
        }
        if (DBManager.gp < Mathf.Abs(int.Parse(buyout.text)))
        {
            return;
        }
		if (Mathf.Abs(int.Parse(buyout.text)) < 100)
		{
            return;
		}
        if (Mathf.Abs(int.Parse(buyout.text)) > 1000000)
        {
            return;
        }

        animator.SetBool("isPlant3", true);
        shopPanel.SetActive(false);
    }

    public void ThirdSeedInstantiate()
    {
		if (!DBManager.isPlant)
		{
            seeds[2].SeedsSellSet(Mathf.Abs(int.Parse(buyout.text)));
            DBManager.adminData.Set(DBManager.username, "씨앗구매", "C", -int.Parse(buyout.text), 0);
            GameObject clone = Instantiate(seeds[2].seedPrefab, seedBase);
            seedPlant.Initialize(clone, seeds[2]);

            DBManager.gp -= seeds[2].seeds[0].sell;
            DBManager.isPlant = true;
            DBManager.seedName = seeds[2].seedName;
            DBManager.seedTempletNum = 2;
            DBManager.seedEnhance = seeds[2].seeds[0].enchant;
            DBManager.randomSeedCost = Mathf.Abs(int.Parse(buyout.text));
            seedPlant.UpdateDataTrade(-int.Parse(buyout.text));
            DBManager.UpdateAdminDataTrade();
            DBManager.LoadGP();
            buyout.text = "";

            enchantBtn.interactable = true;
        }
    }

    public void RandomSeedComplete()
	{
        animator.SetBool("isRandomSeedEnd", true);
	}

    public void BlackOut()
	{
        sellBtn.interactable = true;
        animator.SetBool("isPlant1", false);
        animator.SetBool("isPlant2", false);
        animator.SetBool("isPlant3", false);
        GameObject clone = Instantiate(blackOut, canvas.transform);
        clone.GetComponent<EnchantGame_BlackOut>().boy = animator;
    }

    public void SeedSell()
    {
        animator.SetBool("isPlant1", false);
        animator.SetBool("isPlant2", false);
        animator.SetBool("isPlant3", false);
        seedPlant.SeedSell();
        enchantBtn.interactable = false;
	}

    public void SetIdle()
	{
        animator.SetBool("isPlant1", false);
        animator.SetBool("isPlant2", false);
        animator.SetBool("isPlant3", false);
    }

    public void Enchant()
	{
        animator.SetBool("GiveWater", true);
	}

    public void EnchantStart()
	{
        seedPlant.Enhance();
	}
}
