﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class general_free_movement : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    public void goToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
