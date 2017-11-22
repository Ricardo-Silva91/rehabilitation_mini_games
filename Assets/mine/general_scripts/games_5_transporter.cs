using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class games_5_transporter : MonoBehaviour {


    static int gameType;
    static int patientID;
    static int selectedPatientindex;
    static bool parameters_changed;
    public string next_scene;
    static string serverUrl;
    static bool usingVR = false;

    public void serverUrlSet(string url)
    {
        serverUrl = url;
    }

    public string serverUrlGet()
    {
        return serverUrl;
    }

    public void usingVRSet(bool usi)
    {
        usingVR = usi;
    }

    public bool usingVRGet()
    {
        return usingVR;
    }

    // Use this for initialization
    void Start()
    {

    }

    public void moveToGame()
    {
        switch (gameType)
        {
            case 0:
                next_scene = "1_lifting";
                break;
            case 1:
                next_scene = "2_bowl_of_cereal";
                break;
            case 2:
                next_scene = "3_water_plants";
                break;
            case 3:
                next_scene = "4_pinch_puzzle";
                break;
            case 4:
                next_scene = "5_pinch_objects";
                break;
            default:
                break;
        }

        SceneManager.LoadScene(next_scene);
    }

    public void setGameType(int type)
    {
        gameType = type;
    }

    public void setpatientID(int id)
    {
        patientID = id;
        parameters_changed = true;
    }

    public void setselectedPatientindex(int index)
    {
        selectedPatientindex = index;
    }

    public bool getParametersChanged()
    {
        return parameters_changed;
    }

    public int getPatientID()
    {
        return patientID;
    }

    public int getselectedPatientindex()
    {
        return selectedPatientindex;
    }

    public int getGameType()
    {
        return gameType;
    }
   
	
	// Update is called once per frame
	void Update () {
	
	}
}
