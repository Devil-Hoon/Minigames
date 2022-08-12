using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum test
{
    one,
    two,
    three,
    four
}
public class EnchantGame_BlackOut : MonoBehaviour
{
    Image image;
    float a;
    float timeScala;
    bool up;

    public Animator boy;
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        
        image.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        a = 0;
        timeScala = 1.0f / 3.0f;
        up = true;
    }

    // Update is called once per frame
    void Update()
    {
		if (up)
		{
            a += Time.deltaTime * timeScala;
        }
		else
		{
            a -= Time.deltaTime * timeScala;
		}
		if (a >  1.0f)
		{
            a = 1.0f;
            up = false;
            boy.SetBool("isRandomSeedEnd", false);
		}
		if (a < 0.0f)
		{
            a = 0.0f;
            Destroy(gameObject);
		}
        image.color = new Color(0.0f, 0.0f, 0.0f, a);
	}
}
