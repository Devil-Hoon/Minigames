using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Minigame_MainManager : MonoBehaviour
{
	private float time;
    [Header("Loading Panel")]
    public GameObject loadingPanel;

    [Header("LoginPanel")]
    public GameObject loginPanel;
    public InputField idInputField;
    public InputField passwordInputField;
    public Button login;

	[Header("AlarmPanel")]
	public GameObject alarmPanel;
	public GameObject accessAlarmPanel;
	public Text alarmText;
	public Text accessAlarmText;
	[Header("UserInfo")]
	public Text nameText;
	public Text gpText;

	[Header("VersionCheck")]
	public GameObject vCheckPanel;
	public Button vCheckButton;
	public VersionSet vSet;

	private void Awake()
	{
		Screen.SetResolution(960, 600, false);
	}
	private void OnApplicationQuit()
	{
		DBManager.LogOut();
	}
	void Start()
    {
		vSet.VersionInit();
		if (!DBManager.GameVersionCheck())
		{
			vCheckPanel.SetActive(true);
		}
		else
		{
			vCheckPanel.SetActive(false);
		}
		alarmPanel.SetActive(false);
		if (LoadingOnce.loadingOnce)
		{
			time = 3.6f;
		}
		else
		{
			time = 0.0f;
		}
        if (DBManager.LoggedIn)
        {
            loginPanel.SetActive(false);
            DBManager.LoadGP();

			nameText.text = DBManager.nickname;
			gpText.text = DBManager.gp <= 0 ? "0" : string.Format("{0:#,###}", DBManager.gp);
		}
        else
        {
            loginPanel.SetActive(true);
        }
    }

    void Update()
    {
		time += Time.deltaTime;
		if (time < 3.5f)
		{
			loadingPanel.SetActive(true);
		}
		else
		{
			loadingPanel.SetActive(false);
		}
	}

	public void LoginBtn()
	{
		string alarmMessage;
		if (DBManager.Login(idInputField.text, passwordInputField.text,out alarmMessage))
		{
			loginPanel.SetActive(false);
			LoadingOnce.loadingOnce = true;

			nameText.text = DBManager.nickname;
			gpText.text = DBManager.gp <= 0 ? "0" : string.Format("{0:#,###}", DBManager.gp);

			idInputField.text = "";
			passwordInputField.text = "";
		}
		else
		{
			alarmText.text = alarmMessage;
			alarmPanel.SetActive(true);
			Debug.Log("LoginFailed");
		}
	}
	public void AlarmOff()
	{
		if (alarmPanel.activeSelf)
		{
			alarmPanel.SetActive(false);
		}
		if (accessAlarmPanel.activeSelf)
		{
			accessAlarmPanel.SetActive(false);
		}
	}
	public void LogOutBtn()
	{
		if (DBManager.LogOut())
		{
			loginPanel.SetActive(true);
		}
	}
	public void ExitBtn()
	{
		Application.Quit();
	}

	public void Game1Load()
	{
		if (DBManager.gp <= 0)
		{
			accessAlarmText.text = "보유 GP가 부족합니다.";
			accessAlarmPanel.SetActive(true);
		}
		else
		{
			SceneManager.LoadScene("GunfightMain");
		}
	}
	public void Game2Load()
	{
		SceneManager.LoadScene("CardMemoryGameMain");
	}
	public void Game3Load()
	{
		if (DBManager.gp <= 0)
		{
			accessAlarmText.text = "보유 GP가 부족합니다.";
			accessAlarmPanel.SetActive(true);
		}
		else
		{
			SceneManager.LoadScene("EnchantGameMain");
		}
	}
	public void Game4Load()
	{
		SceneManager.LoadScene("SpaceDefenceMain");
	}
	
	public void VerifyInputs()
	{
		login.interactable = (idInputField.text.Length >= 1 && passwordInputField.text.Length >= 4);
	}
}
