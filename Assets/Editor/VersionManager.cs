using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using MySql.Data.MySqlClient;
using MySql.Data;

[InitializeOnLoad]
public class VersionManager : MonoBehaviour
{
	private static bool autoIncrease = true;
	private const string autoIncreaseMenuName = "Build/Auto Increase Build Version";

	static VersionManager()
	{
		autoIncrease = EditorPrefs.GetBool(autoIncreaseMenuName, true);
	}


	[MenuItem(autoIncreaseMenuName, false, 1)]
	private static void SetAutoIncrease()
	{
		autoIncrease = !autoIncrease;
		EditorPrefs.SetBool(autoIncreaseMenuName, autoIncrease);
		Debug.Log("Auto Increase : " + autoIncrease);
	}

	[MenuItem(autoIncreaseMenuName, true)]
	private static bool SetAutoIncreaseValidate()
	{
		Menu.SetChecked(autoIncreaseMenuName, autoIncrease);
		return true;
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	[MenuItem("Build/Check Current Version", false, 2)]
	private static void CheckCurrentVersion()
	{
		CheckVersionLength();
		Debug.Log("Build v" + PlayerSettings.bundleVersion + " (" + PlayerSettings.Android.bundleVersionCode + ")");
	}

	public static string ReturnCurrentVersion()
	{
		CheckVersionLength();
		return PlayerSettings.bundleVersion;
	}

	[PostProcessBuild(1)]
	public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
	{
		CheckVersionLength();
		UpdateVersion();
		if (autoIncrease)
		{
			IncreaseBuild();
		}
	}

	[MenuItem("Build/Increase Major Version", false, 51)]
	private static void IncreaseMajor()
	{
		string[] lines = PlayerSettings.bundleVersion.Split('.');
		EditVersion(1, -int.Parse(lines[1]), -int.Parse(lines[2]));
	}

	[MenuItem("Build/Increase Minor Version", false, 52)]
	private static void IncreaseMinor()
	{
		string[] lines = PlayerSettings.bundleVersion.Split('.');
		EditVersion(0, 1, -int.Parse(lines[2]));
	}

	private static void IncreaseBuild()
	{
		EditVersion(0, 0, 1);
	}

	private static void EditVersion(int majorIncr, int minorIncr, int buildIncr)
	{
		string[] lines = PlayerSettings.bundleVersion.Split('.');

		int majorVersion = int.Parse(lines[0]) + majorIncr;
		int minorVersion = int.Parse(lines[1]) + minorIncr;
		int build = int.Parse(lines[2]) + buildIncr;

		PlayerSettings.bundleVersion = majorVersion.ToString("0") + "." +
										minorVersion.ToString("0") + "." +
										build.ToString("0");
		PlayerSettings.Android.bundleVersionCode = majorVersion * 10000 + minorVersion * 1000 + build;
		CheckCurrentVersion();
	}

	[InitializeOnLoadMethod]
	static void CheckVersionLength()
	{
		string[] lines = PlayerSettings.bundleVersion.Split('.');
		if (lines.Length < 3)
		{
			int MajorVersion = 0;
			int MinorVersion = 0;
			int Build = 0;

			PlayerSettings.bundleVersion = MajorVersion.ToString("0") + "." +
										   MinorVersion.ToString("0") + "." +
										   Build.ToString("0");
			PlayerSettings.Android.bundleVersionCode =
				MajorVersion * 10000 + MinorVersion * 1000 + Build;
		}
	}

	private static void UpdateVersion()
	{
		string sqlConnect = "server=pickstar.co.kr;uid=pickstar;pwd=xktmvlr10;database=pickstar;charset=utf8;";
		MySqlConnection conn = new MySqlConnection(sqlConnect);
		if (conn.State != System.Data.ConnectionState.Closed)
		{
			Debug.Log("connetion Failed");
			return;
		}
		conn.Open();

		string quary = "UPDATE kMinigame SET version = '" + PlayerSettings.bundleVersion + "' WHERE filename = 'Minigames.exe';";
		MySqlCommand command = new MySqlCommand(quary, conn);
		MySqlDataReader rdr = command.ExecuteReader();
		rdr.Read();
		rdr.Close();
		conn.Close();
	}
}
