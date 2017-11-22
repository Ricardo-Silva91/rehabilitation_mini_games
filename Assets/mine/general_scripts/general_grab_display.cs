using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class general_grab_display : MonoBehaviour {


    public Texture handOpen;
    public Texture handClosed;


    public general_grab_detector grabDetector;

    public int lastGrabState = 0;

	// Use this for initialization
	void Start () {
	
	}

    void showHand (int handState)
    {
        if(handState == 2)
        {
            this.GetComponent<RawImage>().texture = handOpen;
        }
        else
        {
            this.GetComponent<RawImage>().texture = handClosed;
        }
    }
	
	// Update is called once per frame
	void Update () {
        
        if (grabDetector.ready == true)
        {
            if (grabDetector.leftHand == false)
            {
                this.transform.Rotate(new Vector3(0, 1, 0), 180);
            }
            grabDetector.ready = false;
        }

        if (grabDetector.handState != lastGrabState)
        {
            lastGrabState = grabDetector.handState;
            showHand(lastGrabState);
        }

    }
}
