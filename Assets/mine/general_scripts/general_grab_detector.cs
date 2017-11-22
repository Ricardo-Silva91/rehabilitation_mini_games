using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Leap;

public class general_grab_detector : MonoBehaviour {


    public Controller controller;
    public float margin = 0.1f;
    public Hand firstHand;
    public bool leftHand = true;
    public int CountedHands = 0;
    public int handState = 0;

    public bool ready = false;


    bool HandsAreCorrect()
    {
        bool result = true;

        Frame frame = controller.Frame();
        CountedHands = frame.Hands.Count;
        if (frame.Hands.Count == 1)
        {
            List<Hand> hands = frame.Hands;
            firstHand = hands[0];

            if ((leftHand == false && firstHand.IsLeft) || (leftHand == true && firstHand.IsLeft == false))
            {
                result = false;
            }
        }
        else
        {
            result = false;
        }

        if (result == true)
        {
        }
        return result;
    }

    void updateHandState()
    {
        if (firstHand.GrabAngle <= 0 + margin)
        {
            handState = 2;
        }
        else if (firstHand.GrabAngle > 3.14 - margin)
        {
            handState = 1;
        }
        else
        {
            handState = 0;
        }
    }

    // Use this for initialization
    void Start () {

        controller = new Controller(); //An instance must exist
        byte[] frameData = System.IO.File.ReadAllBytes("frame.data");
        Frame reconstructedFrame = new Frame();
        reconstructedFrame.Deserialize(frameData);

    }
	
	// Update is called once per frame
	void Update () {

        if(HandsAreCorrect() == true)
        {
            updateHandState();
        }
	
	}
}
