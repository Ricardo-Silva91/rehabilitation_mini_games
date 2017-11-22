﻿using UnityEngine;
using System.Collections;

public class games_landing_scene_rest_client : MonoBehaviour
{

    public games_landing_scene_master masterLVL;

    public bool sentToServer = false;
    public JSONObject gameToDo = new JSONObject();
    string server_url = "http://localhost:8090/";

    // Use this for initialization
    IEnumerator Start()
    {
        string url = server_url + "getGameToDo";

        WWW www = new WWW(url);
        yield return www;

        if (www.text != null && !www.text.Equals(""))
        {
            //Debug.Log("REST testToDo: " + www.text);
            gameToDo = new JSONObject(www.text);

            Debug.Log("game type: " + gameToDo.GetField("type").n);

            Debug.Log("game type Correct");
            masterLVL.SetRestParameters(gameToDo);

        }
        else
        {
            Debug.Log("REST server not accessible!");
        }
        masterLVL.workFlag = true;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
