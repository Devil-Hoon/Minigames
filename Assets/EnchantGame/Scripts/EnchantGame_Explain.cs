using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EnchantGame_Explain : MonoBehaviour
{
    public Sprite[] sprites;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PanelOn()
	{
        Image img = GetComponent<Image>();
        img.sprite = sprites[0];
        gameObject.SetActive(true);
	}

    public void PanelOff()
	{
        gameObject.SetActive(false);
	}

    public void Next()
	{
        Image img = GetComponent<Image>();
        img.sprite = sprites[1];
	}
    public void Prev()
    {
        Image img = GetComponent<Image>();
        img.sprite = sprites[0];
    }
}
