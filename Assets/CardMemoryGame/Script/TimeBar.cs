using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TimeBar : MonoBehaviour
{
    public Slider timeBar;
	public Image fill;
	// Start is called before the first frame update

	

	public void SetTimeBarMaxRange(float range)
	{
        timeBar.maxValue = range;
        timeBar.value = range;
	}

    public void SetTimeBarValue(float value)
	{
        timeBar.value = value;
	}
}
