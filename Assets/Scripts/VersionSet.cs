using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VersionSet : MonoBehaviour
{
    public string filename;
    public string version;
	public void VersionInit()
	{
		DBManager.gameVersion = version;
	}
}
