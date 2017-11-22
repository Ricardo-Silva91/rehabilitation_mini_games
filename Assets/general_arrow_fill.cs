using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class general_arrow_fill : MonoBehaviour {


    public float fillPercent;
    public Image thisImage;
    public bool coolingDown;
    public float waitTime = 30.0f;
    public float fillAmount;


    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update ()
    {

        if (coolingDown == true)
        {
            //Reduce fill amount over 30 seconds
            thisImage.fillAmount -= 1.0f / waitTime * Time.deltaTime;
            fillAmount = thisImage.fillAmount;
        }
    }
}
