using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnchantGame_Firework : MonoBehaviour
{
    public GameObject successPrefab;
    public GameObject failPrefab;
    public GameObject target;
    public EnchantGame_SeedPlant seedPlant;

    public GameObject boy;

    Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        EffectSoundManager.instance.EnchantGameFireWork();
        cam.GetComponent<EnchantGame_CameraMove>().SetTarget(target);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetSeedPlant(EnchantGame_SeedPlant seedPlant)
    {
        this.seedPlant = seedPlant;
    }

    public void SetBoy(GameObject boy)
	{
        this.boy = boy;
	}
    public void EnhanceCheck()
	{
		
		Destroy(target);
        Destroy(gameObject);
        EffectSoundManager.instance.Stop();
        float k = Random.Range(0.0f, 1000.0f);

        if (k < seedPlant.rate)
        {
            seedPlant.EnhanceFail();
            GameObject clone = Instantiate(failPrefab, new Vector3(0, 9.0f, 0), Quaternion.identity);
            clone.GetComponent<Enchant_EnchantFail>().SetSeedPlant(seedPlant);
            cam.GetComponent<EnchantGame_CameraMove>().SetTarget(clone);
            boy.GetComponent<Animator>().SetBool("isPlant1", false);
            boy.GetComponent<Animator>().SetBool("isPlant2", false);
            boy.GetComponent<Animator>().SetBool("isPlant3", false);
            boy.GetComponent<Animator>().SetBool("GiveWater", false);
        }
        else
        {
            seedPlant.EnhanceComplete();
            GameObject clone = Instantiate(successPrefab, new Vector3(0, 9.0f, 0), Quaternion.identity);
            clone.GetComponent<EnchantGame_EnchantSuccess>().SetSeedPlant(seedPlant);
            cam.GetComponent<EnchantGame_CameraMove>().SetTarget(clone);
            boy.GetComponent<Animator>().SetBool("GiveWater", false);
        }
    }
}
