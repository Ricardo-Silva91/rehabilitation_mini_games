using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class general_pinch_display : MonoBehaviour
{


    public general_pinch_state_calc pinchCalculator;

    public int lastPinchState = 0;

    // Use this for initialization
    void Start()
    {

        
    }

    void lightFinger(int fingerNumber)
    {

       
        /*
        for(int i=0; i< this.GetComponentsInChildren<RawImage>().Length; i++)
            this.GetComponentInChildren<RawImage>().color = Color.white;
        */
        foreach(RawImage j in this.GetComponentsInChildren<RawImage>())
        {
            j.color = Color.white;
        }

        switch (fingerNumber)
        {
            //none
            case 0:
                break;
            //index
            case 1:
                this.GetComponentsInChildren<RawImage>()[1].color = Color.green;
                break;
            //middle
            case 2:
                this.GetComponentsInChildren<RawImage>()[2].color = Color.green;
                break;
            //ring
            case 3:
                this.GetComponentsInChildren<RawImage>()[3].color = Color.green;
                break;
            //pinky
            case 4:
                this.GetComponentsInChildren<RawImage>()[4].color = Color.green;
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (pinchCalculator.ready == true)
        {
            if (pinchCalculator.leftHand == false)
            {
                this.transform.Rotate(new Vector3(0, 1, 0), 180);
                //this.transform.position = new Vector3(396, transform.position.y, transform.position.z);
            }
            pinchCalculator.ready = false;
        }

        if (pinchCalculator.pinchState != lastPinchState)
        {
            lastPinchState = pinchCalculator.pinchState;
            lightFinger(lastPinchState);
        }

    }
}
