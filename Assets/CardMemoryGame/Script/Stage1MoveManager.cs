using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Stage1MoveManager : MonoBehaviour
{
    public float time;
	public Camera cam;
    public bool isReady;
    public bool isMoveEnd;
    private bool once;
	// Start is called before the first frame update
	private void OnApplicationQuit()
	{
        DBManager.LogOut();
    }
	void Start()
    {
        time = 0.0f;
        once = false;
        isReady = false;
        isMoveEnd = false;

    }

    // Update is called once per frame
    void Update()
    {
		if (!isReady && !isMoveEnd)
        {
            time += Time.deltaTime;
        }

		if (time > 1.0f)
		{
            isReady = true;
            time = 0.0f;
		}

		if (isReady && !isMoveEnd)
		{
            cam.transform.position = Vector3.Lerp(cam.transform.position, new Vector3(-9.0f, 1.0f, -10), Time.deltaTime);
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, 5.0f, Time.deltaTime);


            if (cam.orthographicSize < 5.099f)
			{
                isMoveEnd = true;
			}
			
        }


		if (isMoveEnd && isReady)
		{
			if (!once)
			{
                once = true;
                SceneManager.LoadScene(6);
			}
		}


    }
}
