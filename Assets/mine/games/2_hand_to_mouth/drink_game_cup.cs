using UnityEngine;
using System.Collections;

public class drink_game_cup : MonoBehaviour {

    public Vector3 startPos;
    public Quaternion startRot;

    // Use this for initialization
    void Start () {
        startPos = transform.position;
        startRot = transform.rotation;
	}
	
    public void moveToMe(Vector3 pos, Quaternion rotation)
    {
        transform.position = Vector3.MoveTowards(transform.position, pos, 1);
        transform.rotation = rotation;
    }

    public void putBack()
    {
        transform.position = startPos;
        transform.rotation = startRot;
    }
    public void ThrowAway()
    {
        transform.position = new Vector3(99, 99, 99) ;
    }

    // Update is called once per frame
    void Update () {

        if (Input.GetKeyDown("z"))
        {
            transform.position = startPos;
        }
    }
}
