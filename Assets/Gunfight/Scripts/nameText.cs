using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class nameText : MonoBehaviourPunCallbacks
{
    Text t;
    // Start is called before the first frame update
    void Start()
    {
        t = gameObject.GetComponent<Text>(); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [PunRPC]
    public void SetName(string name)
	{
        t.text = name;
	}


}
