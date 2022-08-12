using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class StageLoad : MonoBehaviour
{
	private void OnApplicationQuit()
	{
		DBManager.LogOut();
	}
	public void Stage2Load()
	{
		SceneManager.LoadScene(8);
	}
	public void Stage3Load()
	{
		SceneManager.LoadScene(10);
	}
	public void Stage4Load()
	{
		SceneManager.LoadScene(12);
	}
}
