using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchant_EnchantFail : MonoBehaviour
{
    public EnchantGame_SeedPlant seedPlant;
    // Start is called before the first frame update
    void Start()
    {
        EffectSoundManager.instance.EnchantGameEnchantFail();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSeedPlant(EnchantGame_SeedPlant seedPlant)
    {
        this.seedPlant = seedPlant;
    }

    public void EnhanceFailed()
    {
        //seedPlant.EnhanceFail();
        Destroy(gameObject);
    }
}
