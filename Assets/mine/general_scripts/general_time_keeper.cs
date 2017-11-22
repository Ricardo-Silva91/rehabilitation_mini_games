using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class general_time_keeper : MonoBehaviour {

    public bool workFlag;

    public float completeStartTime;

    public bool timeOverTotal = false;
    public int total_time = 60;
    public float startTime = 0;
    public int secsPassed;

	// Use this for initialization
	void Start () {
        startTime = Time.time;
        workFlag = false;
    }
	
    public void completeReset()
    {
        completeStartTime = Time.time;
    }
    public int completeSecsPassed()
    {
        float currentTime = Time.time;
        int completeSecsPassed = (int)(currentTime - completeStartTime);
        return completeSecsPassed;
    }

    public void reset()
    {
        startTime = Time.time;
        workFlag = true;
    }

    public void stop()
    {
        workFlag = false;
    }

	// Update is called once per frame
	void Update () {
		
        if(workFlag == true)
        {
            float currentTime = Time.time;
            secsPassed = (int)(currentTime - startTime);

            if(secsPassed >= total_time)
            {
                timeOverTotal = true;
                workFlag = false;
            }
        }

	}
}
