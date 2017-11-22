using UnityEngine;
using System.Collections;
using System.Net;
using System.IO;

public class lift_game_rest_client : MonoBehaviour
{

    public lift_game_master masterLVL;
    public games_5_transporter transporter;

    public bool sentToServer = false;
    public JSONObject gameToDo = new JSONObject();
    public string server_url = "http://localhost:8090/";

    // Use this for initialization
    IEnumerator Start()
    {
        server_url = transporter.serverUrlGet();

        string url = server_url + "getGameToDo";
        if (transporter.getParametersChanged() == true)
        {
            Debug.Log("transporter in action");
            url = server_url + "getGameById?id=" + transporter.getPatientID() + "&type=" + transporter.getGameType();
        }
        else
        {
            Debug.Log("transporter MIA");
        }

        WWW www = new WWW(url);
        yield return www;

        if (www.text != null && !www.text.Equals(""))
        {
            //Debug.Log("REST testToDo: " + www.text);
            gameToDo = new JSONObject(www.text);

            Debug.Log("game type: " + gameToDo.GetField("type").n);

            if (gameToDo.GetField("type").n == 0)
            {
                Debug.Log("game type Correct");
                masterLVL.SetRestParameters(gameToDo);
            }
            else
            {
                Debug.Log("Wrong game type");
            }
        }
        else
        {
            Debug.Log("REST server not accessible!");
        }
        masterLVL.workFlag = true;
    }

    public void sendToServer(JSONObject results)
    {
        StartCoroutine(SendToServer_IEnumerator(results));
    }

    public IEnumerator SendToServer_IEnumerator(JSONObject results)
    {
        string url = server_url + "sendGameResults";

        JSONObject toSend = new JSONObject();

        toSend.SetField("type", transporter.getGameType());
        toSend.SetField("id", transporter.getPatientID());
        toSend.SetField("VR", transporter.usingVRGet());

        toSend.SetField("results", results);

        HttpWebRequest http = (HttpWebRequest)WebRequest.Create(url);
        http.Accept = "application/json";
        http.ContentType = "application/json";
        http.Method = "POST";

        string parsedContent = toSend.ToString();
        System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
        byte[] bytes = encoding.GetBytes(parsedContent);

        Stream newStream = http.GetRequestStream();
        newStream.Write(bytes, 0, bytes.Length);
        newStream.Close();

        var response = http.GetResponse();

        var stream = response.GetResponseStream();
        var sr = new StreamReader(stream);
        var content = sr.ReadToEnd();


        yield return response;

        sentToServer = true;

    }

    // Update is called once per frame
    void Update()
    {

    }
}
