using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnchantGame_SeedTemplet : ScriptableObject
{
    public string seedName;
    public GameObject seedPrefab;
    public Seed[] seeds;

    public void SeedsSellSet(int sell)
	{
        int cost = sell;
        seeds[0].sell = cost;
        cost = (int)(cost * 1.95f);
        seeds[1].sell = cost;
        cost = (int)(cost * 2.9f);
        seeds[2].sell = cost;
        cost = (int)(cost * 3.8f);
        seeds[3].sell = cost;
        cost = (int)(cost * 5.8f);
        seeds[4].sell = cost;
        cost = (int)(cost * 7.6f);
        seeds[5].sell = cost;
	}


    [System.Serializable]
    public struct Seed
    {
        public Sprite sprite;
        public RuntimeAnimatorController animatorController;
        public int enchant;
        public int sell;
        public float rate;
    }

}
