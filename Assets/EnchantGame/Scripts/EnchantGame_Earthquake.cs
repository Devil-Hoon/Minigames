using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnchantGame_Earthquake : MonoBehaviour
{
    public EnchantGame_SeedPlant seedPlant;
    public GameObject boy;

    public Vector3 origin;
    public Vector3 rightEnd;
    public Vector3 LeftEnd;

    public GameObject fireworkPrefab;

    Camera cam;

    bool posEnd;
    bool camEnd;
    
    float time;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        time = 0;
        posEnd = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EarthquakePlay()
	{
        time = 0;
        posEnd = false;
        EffectSoundManager.instance.EnchantGameEarthquake();
        cam.GetComponent<EnchantGame_CameraMove>().earthquake = true;
        StartCoroutine(Earthquaking());
	}

    IEnumerator Earthquaking()
	{
		while (time < 3.0f)
		{
            time += Time.deltaTime;

			if (!camEnd)
            {
                cam.transform.Translate(Vector3.up * 0.015f);
                if (cam.transform.position.y > 0.15f)
				{
                    cam.transform.position = new Vector3(0, 0.1f, -10);
                    camEnd = true;
				}
            }
			else
			{
                cam.transform.Translate(Vector3.down * 0.015f);
                if (cam.transform.position.y < 0.0f)
                {
                    cam.transform.position = new Vector3(0, 0, -10);
                    camEnd = false;
                }
            }


			if (!posEnd)
            {
                transform.Translate(Vector3.right * 0.04f);
                if (transform.position.x > rightEnd.x)
				{
                    transform.position = rightEnd;
                    posEnd = true;
				}
			}
			else
			{
                transform.Translate(Vector3.left * 0.04f);
                if (transform.position.x < LeftEnd.x)
                {
                    transform.position = LeftEnd;
                    posEnd = false;
                }
            }

			if (time > 3.0f)
			{
                transform.position = origin;
                cam.transform.position = new Vector3(0, 0, -10);
                cam.GetComponent<EnchantGame_CameraMove>().earthquake = false;

                EffectSoundManager.instance.Stop();
                GameObject clone = Instantiate(fireworkPrefab, new Vector3(0.4f, 4.0f, 0.0f), Quaternion.identity);
                clone.GetComponent<EnchantGame_Firework>().SetSeedPlant(seedPlant);
                clone.GetComponent<EnchantGame_Firework>().SetBoy(boy);
            }
            

            yield return 0;
		}
	}
}
