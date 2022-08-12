using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class JsonData
{
    public List<JsonMembers> result = new List<JsonMembers>();
    public JsonData() { }

    public void Print()
	{
		for (int i = 0; i < result.Count; i++)
		{
            result[i].Print();
		}
	}

}

public class JsonMembers
{
    public int RANK;
    public string USER;
    public int Score;

    public void Print()
	{
        Debug.Log(RANK);
        Debug.Log(USER);
        Debug.Log(Score);
	}
}


[System.Serializable]
public class Serialization<T>
{
    public List<T> target;
    public List<T> ToList() { return target; }

    public Serialization(List<T> target)
	{
        this.target = target;
	}
}
