using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MySql.Data.MySqlClient;

public class EnchantGame_PlayManager : MonoBehaviour
{
    public EnchantGame_SeedPlant seedPlant;
    public Transform seedBase;
    public EnchantGame_SeedTemplet[] seedTemplets;
    public Text textGP;
    public Text textNickname;
    public Text textGetGP;
    public Button enchantBtn;
    public Button sellBtn;
    public Animator boy;
    [Header("Option")]
    public GameObject optionPanel;
    public GameObject BGToggle;
    public GameObject EffectToggle;
    private bool optionPanelOnoff;
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
        if (DBManager.isPlant)
		{

            if (DBManager.seedTempletNum == 2)
            {
                GameObject clone = Instantiate(seedTemplets[DBManager.seedTempletNum].seedPrefab, seedBase);
                seedTemplets[2].SeedsSellSet(DBManager.randomSeedCost);

                clone.GetComponent<SpriteRenderer>().sprite = seedTemplets[DBManager.seedTempletNum].seeds[DBManager.seedEnhance].sprite;
                seedPlant.SeedLoad(clone, seedTemplets[DBManager.seedTempletNum], DBManager.seedEnhance);

				if (DBManager.seedEnhance < seedTemplets[DBManager.seedTempletNum].seeds.Length - 1)
                {
					if (DBManager.seedEnhance != 0)
                    {
                        sellBtn.interactable = true;
                    }
                    enchantBtn.interactable = true;
                    boy.SetBool("isPlant3", true);
                }
            }
			else
            {
                GameObject clone = Instantiate(seedTemplets[DBManager.seedTempletNum].seedPrefab, seedBase);
                clone.GetComponent<SpriteRenderer>().sprite = seedTemplets[DBManager.seedTempletNum].seeds[DBManager.seedEnhance].sprite;
                seedPlant.SeedLoad(clone, seedTemplets[DBManager.seedTempletNum], DBManager.seedEnhance);
                if (DBManager.seedEnhance < seedTemplets[DBManager.seedTempletNum].seeds.Length - 1)
                {
                    if (DBManager.seedEnhance != 0)
                    {
                        sellBtn.interactable = true;
                    }
                    enchantBtn.interactable = true;
                    if (DBManager.seedTempletNum < 1)
                    {
                        boy.SetBool("isPlant1", true);
                    }
                    else
                    {
                        boy.SetBool("isPlant2", true);
                    }
                }
            }
        }

        BGSoundManager.instance.StopBGM();
        BGSoundManager.instance.PlayEnchantGamePlayBGM();
    }

    // Update is called once per frame
    void Update()
    {
        textGP.text = DBManager.gp <= 0 ? "0" : string.Format("{0:#,###}", DBManager.gp);
        textNickname.text = DBManager.nickname;
		if (DBManager.isPlant && DBManager.seedEnhance > 0)
		{
            textGetGP.text = seedPlant.seedTemplet.seeds[DBManager.seedEnhance].sell <= 0 ? 
                "0" : string.Format("{0:#,###}",seedPlant.seedTemplet.seeds[DBManager.seedEnhance].sell);
		}
		else
		{
            textGetGP.text = "0";
        }
    }
    private void OnApplicationQuit()
    {
        DBManager.LogOut();
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
    public void BackHome()
    {
        DBManager.LoadGP();
        seedPlant.UpdateDataEnchant();
        SceneManager.LoadScene(13);
	}
    
}
