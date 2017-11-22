using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class menu_scene_master : MonoBehaviour {

    public Dropdown patientDrop;
    public Dropdown gameDrop;
    public Button goButton;
    public games_5_transporter transporter;    

    public bool workFlag = false;
    public JSONObject patientsObject;
    public List<Dropdown.OptionData> patients;
    public List<Dropdown.OptionData> games;

    public int selectedPatient=0;
    public int selectedGame;

    public void setRestParameters(JSONObject patientList)
    {
        patientsObject = patientList;
        patients = new List<Dropdown.OptionData>();

        for (int i=0; i< patientsObject.Count; i++)
        {
            patients.Add(new Dropdown.OptionData(patientsObject[i].GetField("name").str));
        }

        goButton.onClick.AddListener(goToGame);
        patientDrop.options = patients;


        if (transporter.getPatientID() != 0)
        {
            patientDrop.value = transporter.getselectedPatientindex();
        }

        populateGameDrop();

    }

    void populateGameDrop()
    {
        games = new List<Dropdown.OptionData>();

        for (int i = 0; i < patientsObject[selectedPatient].GetField("games").Count; i++)
        {
            games.Add(new Dropdown.OptionData(patientsObject[selectedPatient].GetField("games")[i].GetField("name").str));
        }

        gameDrop.options = games;
    }


    void goToGame()
    {
        transporter.setselectedPatientindex(selectedPatient);
        Debug.Log("Will move on");
        transporter.setGameType((int)patientsObject[selectedPatient].GetField("games")[selectedGame].GetField("type").n);
        transporter.setpatientID((int)patientsObject[selectedPatient].GetField("games")[selectedGame].GetField("id").n);
        transporter.moveToGame();
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
        if(workFlag==true)
        {
            if(selectedPatient != patientDrop.value)
            {
                selectedPatient = patientDrop.value;
                populateGameDrop();
                gameDrop.value = 0;
            }
            else
            {
                selectedGame = gameDrop.value;
            }
        }

	}
}
