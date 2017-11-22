using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Leap;

public class general_pinch_state_calc : MonoBehaviour {

    public Controller controller;

    public bool leftHand = true;
    public int pinchMargin = 20;
    public int CountedHands = 0;
    public Hand firstHand;
    public int pinchState = 0;
    public bool ready = false;



    // Use this for initialization
    void Start () {

        controller = new Controller(); //An instance must exist
        byte[] frameData = System.IO.File.ReadAllBytes("frame.data");
        Frame reconstructedFrame = new Frame();
        reconstructedFrame.Deserialize(frameData);

    }

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
            //logText.setCurrentText("");
        }
        return result;
    }

    void updatePinchState()
    {
        Finger thumb = firstHand.Fingers[0];
        Finger index = firstHand.Fingers[1];
        Finger middle = firstHand.Fingers[2];
        Finger ring = firstHand.Fingers[3];
        Finger pinky = firstHand.Fingers[4];

        float index_distance = thumb.TipPosition.DistanceTo(index.TipPosition);
        float middle_distance = thumb.TipPosition.DistanceTo(middle.TipPosition);
        float ring_distance = thumb.TipPosition.DistanceTo(ring.TipPosition);
        float pinky_distance = thumb.TipPosition.DistanceTo(pinky.TipPosition);


        if (!thumb.IsExtended)
        {
            //index pinch
            if (!index.IsExtended && pinky.IsExtended && ring.IsExtended && middle.IsExtended)
            {
                //Debug.Log("index dist: " + index_distance);

                if (index_distance < pinchMargin)
                {
                    pinchState = 1;
                }
            }
            //middle pinch
            else if (index.IsExtended && pinky.IsExtended && ring.IsExtended && !middle.IsExtended)
            {
                //Debug.Log("middle dist: " + middle_distance);

                if (middle_distance < pinchMargin)
                {
                    pinchState = 2;
                }
            }
            //ring pinch
            else if (index.IsExtended && pinky.IsExtended && !ring.IsExtended && middle.IsExtended)
            {
                //Debug.Log("ring dist: " + ring_distance);

                if (ring_distance < pinchMargin)
                {
                    pinchState = 3;
                }
            }
            //pinky pinch
            else if (index.IsExtended && !pinky.IsExtended && ring.IsExtended && middle.IsExtended)
            {
                //Debug.Log("pinky dist: " + pinky_distance);

                if (pinky_distance < pinchMargin)
                {
                    pinchState = 4;
                }

            }
            else
            {
                //no pinch
                pinchState = 0;
            }
        }
        else
        {
            //can't be pinch (thumb is extended)
            pinchState = 0;
        }

    }

    // Update is called once per frame
    void Update () {
	
        if(HandsAreCorrect() == true)
        {
            updatePinchState();
        }

	}
}
