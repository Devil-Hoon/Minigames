using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class EndPanelScript : MonoBehaviourPunCallbacks
{
    public GameObject point;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPoint(int point)
	{
        this.point.GetComponent<Text>().text = point <= 0 ? "0" : string.Format("{0:#,###}",point.ToString());
	}
}
