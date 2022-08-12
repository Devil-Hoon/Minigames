using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
	[SerializeField]
    public bool isBuildTower { set; get; }
	// Start is called before the first frame update
	private void Awake()
	{
		isBuildTower = false;
	}
}
