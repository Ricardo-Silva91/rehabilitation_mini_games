using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class general_arrow_direction : MonoBehaviour {

    public Quaternion filteredRotation;

    public Quaternion rotationUp;
    public Quaternion rotationDown;

    public bool rotating;
    public bool goingUp;
    public float rotationSpeed;
    public float angleMargin;
    public float angleDistance;

    public float zAngle;

    // Use this for initialization
    void Start () {
        filteredRotation = new Quaternion(0, 0, this.transform.rotation.z, 0);
    }
	
    public void flipUp()
    {
        if (filteredRotation != rotationUp)
        {
            goingUp = true;
            rotating = true;
        }
    }

    public void flipDown()
    {
        if (filteredRotation != rotationDown)
        {
            goingUp = false;
            rotating = true;
        }
    }

    public void flip()
    {
        if(goingUp == true)
        {
            goingUp = false;
        }
        else
        {
            goingUp = true;
        }
        rotating = true;
    }

	// Update is called once per frame
	void Update () {
        
        if (rotating == true)
        {
            if(goingUp==true)
            {
                //Debug.Log("zAngle: " + zAngle);

                if ((-1*zAngle) < angleMargin)
                {
                    this.transform.rotation = rotationUp;
                    zAngle = 100;
                    rotating = false;
                }
                else
                {
                    zAngle = this.transform.rotation.z;
                    //angleDistance = Quaternion.Angle(this.transform.rotation, rotationUp);
                    this.transform.Rotate(0, 0, rotationSpeed);
                }
                

            }
            else
            {
                if ( (zAngle + 1) < angleMargin)
                {
                    this.transform.rotation = rotationDown;
                    zAngle = -100;
                    rotating = false;
                }
                else
                {
                    zAngle = this.transform.rotation.z;
                    this.transform.Rotate(0, 0, -rotationSpeed);
                }
            }
        }

	}
}
