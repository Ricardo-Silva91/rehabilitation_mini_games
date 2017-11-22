using UnityEngine;
using UnityEngine.VR;
using UnityEngine.UI;
using System.Collections;

using System.Text;
using System.IO;
using System;





public class menu_scene_rest_client : MonoBehaviour {

    public menu_scene_master masterLVL;
    public games_5_transporter transporter;

    public ChangeColor vrPic;
    public bool sentToServer = false;
    public JSONObject patientList = new JSONObject();
    public string server_url = "http://localhost:8090/";
    public bool usingHMD = false;

    // Use this for initialization
    IEnumerator Start()
    {
        usingHMD = VRDevice.isPresent;
        if(usingHMD == true)
        {
            vrPic.GetComponent<RawImage>().color = Color.green;
            transporter.usingVRSet(true);
        }

        Load("server_url.txt");
        transporter.serverUrlSet(server_url);

        string url = server_url + "getPatients";

        WWW www = new WWW(url);
        yield return www;

        if (www.text != null && !www.text.Equals(""))
        {
            //Debug.Log("REST testToDo: " + www.text);
            patientList = new JSONObject(www.text);

            masterLVL.setRestParameters(patientList);
            masterLVL.workFlag = true;
        }
        else
        {
            Debug.Log("REST server not accessible!");
        }
    }
    

    private bool Load(string fileName)
    {
        // Handle any problems that might arise when reading the text
        try
        {
            string line;
            // Create a new StreamReader, tell it which file to read and what encoding the file
            // was saved as
            StreamReader theReader = new StreamReader(fileName, Encoding.Default);
            // Immediately clean up the reader after this block of code is done.
            // You generally use the "using" statement for potentially memory-intensive objects
            // instead of relying on garbage collection.
            // (Do not confuse this with the using directive for namespace at the 
            // beginning of a class!)
            using (theReader)
            {
                // While there's lines left in the text file, do this:
                do
                {
                    line = theReader.ReadLine();

                    if (line != null)
                    {
                        // Do whatever you need to do with the text line, it's a string now
                        // In this example, I split it into arguments based on comma
                        // deliniators, then send that array to DoStuff()
                        string[] entries = line.Split(',');
                        if (entries.Length > 0)
                            server_url = entries[0];
                    }
                }
                while (line != null);
                // Done reading, close the reader and return true to broadcast success    
                theReader.Close();
                return true;
            }
        }
        // If anything broke in the try block, we throw an exception with information
        // on what didn't work
        catch (Exception e)
        {
            Debug.Log(e.Message);
            return false;
        }
        
    }







// Update is called once per frame
void Update()
    {

    }
}
